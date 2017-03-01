using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Flann;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.VideoSurveillance;

namespace SLAM_Kinect
{
    public partial class Form1 : Form
    {

        private VideoCapture _capture;

        /// <summary>
        /// SLAM
        /// Variables definition required for SLAM
        /// </summary>
        Mat previous_frame , current_frame = new Mat();
        long matchTime;
        VectorOfKeyPoint previousKeyPoints;
        VectorOfKeyPoint currentKeyPoints;
        Mat previousDescriptors = new Mat();
        Mat currentDescriptors = new Mat();
        VectorOfDMatch good_matches;

        //static KAZE featureDetector = new KAZE();
        static AKAZE featureDetector = new AKAZE();
        static Stopwatch watch;
        Mat homography;

        public Form1()
        {
            InitializeComponent();
            //try to create the capture
            if (_capture == null)
            {
                try
                {
                    _capture = new VideoCapture();
                }
                catch (NullReferenceException excpt)
                {   //show errors if there is any
                    //MessageBox.Show(excpt.Message);
                    Console.WriteLine("Error : " + excpt.Message);
                }
            }
            if (_capture != null) //if camera capture has been successfully created
            {
                _capture.ImageGrabbed += ProcessFrame;
                _capture.Start();
            }
            
        }
        
        private void ProcessFrame(object sender, EventArgs e)
        {
            Mat image = new Mat();

            _capture.Retrieve(image);
            current_frame = image;
           
            if (this.Disposing || this.IsDisposed)
                return;

            if(previous_frame == null)
            {
                //CaptureFirst();
                Console.WriteLine("First Frame Captured !!");
                previous_frame = current_frame;
                FindPoints(current_frame, out matchTime, out previousKeyPoints, out previousDescriptors);
                return;
            }

            // Detect and compute points in current frame
            FindPoints(current_frame, out matchTime, out currentKeyPoints, out currentDescriptors);

            ////////////////////////////////////////////////////////////////////////////////////
            //      Check for matches between current and previous frame
            ////////////////////////////////////////////////////////////////////////////////////

            //using (VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch())
            VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch();
            //{
                Mat mask;
                
                FindMatch2(previousDescriptors, currentDescriptors, out matchTime, previousKeyPoints, currentKeyPoints, matches,
                    out mask, out homography);

                Console.WriteLine("Total matches : " + matches.Size);

            //}

            //////////////////////////////////
            //          To-do
            //////////////////////////////////
            //IntrinsicCameraParameters intrin = new IntrinsicCameraParameters(4);
            ExtrinsicCameraParameters p = new ExtrinsicCameraParameters();
            
            VectorOfMat rotationVectors = new VectorOfMat();
            VectorOfMat translationVectors = new VectorOfMat();
            //Matrix<VectorOfDMatch> inliers = new Matrix<VectorOfDMatch>(99) ;

            //Matrix<double> intrinc = new Matrix<double>(3, 3);
            //Matrix<double> distorc = new Matrix<double>(8, 1);
            //Mat intrinc = new Mat(3, 3, DepthType.Cv64F, 1);
            //Mat distorc = new Mat(8, 1, DepthType.Cv64F, 1);
            //[6.5475340581882324e+002, 0.0F, 2.2678108877714533e+002, 0.0F, 6.5475340581882324e+002, 1.4283035250823869e+002, 0.0F, 0.0F, 1.0F ];
            //intrinc = new Mat(3, 3, DepthType.Cv64F, 1, intrinc1, 4);

            //Double[] tdat = new double[] { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F };
            Double[,] tdat = new double[3,3] { { 0.0F, 0.0F, 0.0F }, { 0.0F, 0.0F, 0.0F }, { 0.0F, 0.0F, 0.0F }};
            Double[,] tdat2 = new double[1,4] { { 0.0F, 0.0F, 0.0F, 0.0F } };
            MCvPoint3D32f[] objpts = new MCvPoint3D32f[matches.Size];
            for(int i = 0; i < matches.Size; i++)
            {
                int r = matches[i].;
                //objpts[i].X = matches[i].trainIdx;
            }

            Matrix<double> intrinc = new Matrix<double>(tdat);
            Matrix <double>distorc = new Matrix<double>(tdat2);

            //Emgu.CV.CvEnum.SolvePnpMethod method = Emgu.CV.CvEnum.SolvePnpMethod.Iterative;
            //IOutputArray inliers1 = null;
            CvInvoke.SolvePnP(currentKeyPoints, matches, tdat, tdat2, p.RotationVector, p.TranslationVector, false, SolvePnpMethod.Iterative);
            //CvInvoke.SolvePnPRansac(currentKeyPoints, current_frame, intrin.IntrinsicMatrix, intrin.DistortionCoeffs, p.RotationVector, p.TranslationVector, false, 100, 0.0F, 80, inliers1, method);
            //CvInvoke.SolvePnPRansac(currentKeyPoints, current_frame, intrinc, distorc, p.RotationVector, p.TranslationVector, false, 100, 0.0F, 99, inliers1, method);
            
            previousDescriptors = currentDescriptors;
            previousKeyPoints = currentKeyPoints;


            capturedImageBox.Image = current_frame.ToImage<Bgr, Byte>().ToBitmap();
            forgroundImageBox.Image = previous_frame.ToImage<Bgr, Byte>().ToBitmap();
            previous_frame = current_frame;

            //Display the amount of motions found on the current image
            //UpdateText(String.Format("Total Motions found: {0}; Motion Pixel count: {1}", rects.Length, overallMotionPixelCount));

            //Display the image of the motion
            //motionImageBox.Image = motionImage;

        }

        private void CaptureFirst()
        {
            Console.WriteLine("First Frame Captured !!");
        }
        
        public void FindPoints(Mat mImage, out long matchTime, out VectorOfKeyPoint modelKeyPoints, out Mat mDescriptors)
        {
            modelKeyPoints = new VectorOfKeyPoint();
            mDescriptors = new Mat();

            using (UMat uModelImage = mImage.GetUMat(AccessType.Read))
            {
                watch = Stopwatch.StartNew();

                //extract features from the object image
                featureDetector.DetectAndCompute(uModelImage, null, modelKeyPoints, mDescriptors, false);                
            }
            matchTime = watch.ElapsedMilliseconds;
            Console.WriteLine("Feature : " + matchTime + "ms");
        }

        public void FindMatch2(Mat mDescriptors, Mat oDescriptors, out long matchTime, VectorOfKeyPoint modelKeyPoints, VectorOfKeyPoint observedKeyPoints, VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography)
        {

            homography = null;
            //  Assign and compute in main

                watch = Stopwatch.StartNew();

                // Bruteforce, slower but more accurate
                // You can use KDTree for faster matching with slight loss in accuracy
                //using (Emgu.CV.Flann.LinearIndexParams ip = new Emgu.CV.Flann.LinearIndexParams())
                //using (Emgu.CV.Flann.SearchParams sp = new SearchParams())
                //using (DescriptorMatcher matcher = new FlannBasedMatcher(ip, sp))
                using (DescriptorMatcher matcher = new BFMatcher(DistanceType.Hamming))
                {
                    matcher.Add(mDescriptors);

                    matcher.KnnMatch(oDescriptors, matches, 4, null); // k-nearst match , k = 4
                    mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                    mask.SetTo(new MCvScalar(255));
                    Features2DToolbox.VoteForUniqueness(matches, 0.80, mask);        // uniquenessThreshold = 0.80

                    int nonZeroCount = CvInvoke.CountNonZero(mask);
                    if (nonZeroCount >= 4)
                    {
                        nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints,
                            matches, mask, 1.5, 20);
                        if (nonZeroCount >= 4)
                            homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints,
                                observedKeyPoints, matches, mask, 2);
                    }

                watch.Stop();

            }
           // C++ code ...iteeration...and copying to VectorofDmatch
           // for(int ts =0; ts < matches.Size; ts++){
           //     good_matches.Push(matches[ts][0]);
           //}

            matchTime = watch.ElapsedMilliseconds;
            Console.WriteLine("ElapsedMilliseconds : " + matchTime);
        }
    }
}
