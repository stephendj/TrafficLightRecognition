using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TrafficLightRecognition
{
    public partial class Form1 : Form
    {
        private Capture cap = null;
        Timer timer = new Timer();
        int FPS = 40;

        private bool cam = false;        // Set to true to record using webcam           
        private string video = "../../video/drive.mp4";
        Image<Bgr, Byte> imgOri;
        Image<Gray, Byte> imgR, imgY, imgG, imgEdit;

        public Form1()
        {
            InitializeComponent();

            /*                              *
             *  Initialize Camera or Video  *
             *                              */
            if (cam == true)
            {
                try { cap = new Capture(); }
                catch (NullReferenceException e)
                {
                    Console.WriteLine(e.InnerException);
                    return;
                }
            }
            else
            {
                try { cap = new Capture(video); }
                catch (NullReferenceException e)
                {
                    Console.WriteLine(e.InnerException);
                    return;
                }
            }

            timer.Interval = 1000 / FPS;
            timer.Tick += ProcessFrame;
            timer.Start();
        }
        
        private void ProcessFrame(object sender, EventArgs e)
        {
            // Color Segmentation and Gaussian Smooth
            imgOri = cap.QueryFrame().ToImage<Bgr, Byte>();
            imgR = imgOri.InRange(new Bgr(0, 0, 200), new Bgr(100, 100, 255));
            imgY = imgOri.InRange(new Bgr(0, 100, 200), new Bgr(80, 255, 255));
            imgG = imgOri.InRange(new Bgr(150, 150, 0), new Bgr(255, 255, 80));
            imgEdit = imgR + imgY + imgG;
            imgEdit = imgEdit.SmoothGaussian(7);

            if (cam == false)
            {
                //Show time stamp
                double time_index = cap.GetCaptureProperty(CapProp.PosMsec);
                Time_Label.Text = "Time: " + TimeSpan.FromMilliseconds(time_index).ToString();

                //show frame number
                double framenumber = cap.GetCaptureProperty(CapProp.PosFrames);
                Frame_lbl.Text = "Frame: " + framenumber.ToString();
            }

            Video_Image_1.Image = imgOri.Bitmap;
            Video_Image_2.Image = imgEdit.Bitmap;
        }
    }
}
