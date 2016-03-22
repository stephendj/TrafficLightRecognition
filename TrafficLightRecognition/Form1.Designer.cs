namespace TrafficLightRecognition
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
            this.Video_Image_1 = new System.Windows.Forms.PictureBox();
            this.Video_Image_2 = new System.Windows.Forms.PictureBox();
            this.Time_Label = new System.Windows.Forms.Label();
            this.Frame_lbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Video_Image_1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Video_Image_2)).BeginInit();
            this.SuspendLayout();
            // 
            // Video_Image_1
            // 
            this.Video_Image_1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Video_Image_1.Location = new System.Drawing.Point(15, 25);
            this.Video_Image_1.Name = "Video_Image_1";
            this.Video_Image_1.Size = new System.Drawing.Size(570, 420);
            this.Video_Image_1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Video_Image_1.TabIndex = 1;
            this.Video_Image_1.TabStop = false;
            // 
            // Video_Image_2
            // 
            this.Video_Image_2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Video_Image_2.Location = new System.Drawing.Point(610, 25);
            this.Video_Image_2.Name = "Video_Image_2";
            this.Video_Image_2.Size = new System.Drawing.Size(570, 420);
            this.Video_Image_2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Video_Image_2.TabIndex = 1;
            this.Video_Image_2.TabStop = false;
            // 
            // Time_Label
            // 
            this.Time_Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Time_Label.AutoSize = true;
            this.Time_Label.Location = new System.Drawing.Point(12, 463);
            this.Time_Label.Name = "Time_Label";
            this.Time_Label.Size = new System.Drawing.Size(33, 13);
            this.Time_Label.TabIndex = 4;
            this.Time_Label.Text = "Time:";
            // 
            // Frame_lbl
            // 
            this.Frame_lbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Frame_lbl.AutoSize = true;
            this.Frame_lbl.Location = new System.Drawing.Point(12, 489);
            this.Frame_lbl.Name = "Frame_lbl";
            this.Frame_lbl.Size = new System.Drawing.Size(39, 13);
            this.Frame_lbl.TabIndex = 5;
            this.Frame_lbl.Text = "Frame:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 520);
            this.Controls.Add(this.Frame_lbl);
            this.Controls.Add(this.Time_Label);
            this.Controls.Add(this.Video_Image_1);
            this.Controls.Add(this.Video_Image_2);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.Video_Image_1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Video_Image_2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox Video_Image_1;
        private System.Windows.Forms.PictureBox Video_Image_2;
        private System.Windows.Forms.Label Time_Label;
        private System.Windows.Forms.Label Frame_lbl;
    }
}