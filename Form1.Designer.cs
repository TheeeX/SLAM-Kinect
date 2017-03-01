namespace SLAM_Kinect
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.capturedImageBox = new System.Windows.Forms.PictureBox();
            this.forgroundImageBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.capturedImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.forgroundImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // capturedImageBox
            // 
            this.capturedImageBox.Location = new System.Drawing.Point(11, 9);
            this.capturedImageBox.Name = "capturedImageBox";
            this.capturedImageBox.Size = new System.Drawing.Size(640, 480);
            this.capturedImageBox.TabIndex = 0;
            this.capturedImageBox.TabStop = false;
            // 
            // forgroundImageBox
            // 
            this.forgroundImageBox.Location = new System.Drawing.Point(662, 9);
            this.forgroundImageBox.Name = "forgroundImageBox";
            this.forgroundImageBox.Size = new System.Drawing.Size(640, 480);
            this.forgroundImageBox.TabIndex = 1;
            this.forgroundImageBox.TabStop = false;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1311, 496);
            this.Controls.Add(this.forgroundImageBox);
            this.Controls.Add(this.capturedImageBox);
            this.Name = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.capturedImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.forgroundImageBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        
        private System.Windows.Forms.PictureBox capturedImageBox;
        private System.Windows.Forms.PictureBox forgroundImageBox;
    }
}

