using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.ML;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrafficLightRecognition
{
    public partial class Form1 : Form
    {
        private Capture cap = null;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        int FPS = 40;

        string fileToOpen = "";
        bool cam = false;
        public Image<Bgr, Byte> imgOri, imgShow;
        private Image<Gray, Byte> imgR, imgY, imgG, imgEdit;

        /* HOG Descriptor for feature extraction */
        private HOGDescriptor hog;

        SVM svm;
        
        public string[] class_values;

        public Form1()
        {
            InitializeComponent();
            
            hog = new HOGDescriptor(
                new Size(40, 40),   // win_size
                new Size(8, 8),     // block_size
                new Size(4, 4),     // block_stride
                new Size(4, 4),     // cell_size
                    9,              // nbins
                    1,              // deriv_aperture
                    -1,             // win_sigma
                    0.2,            // L2HysThreshold
                    true            // gamma correction
            );

            // Load the model
            svm = new SVM();
            FileStorage fsr = new FileStorage("../../../data/model.xml", FileStorage.Mode.Read);
            svm.Read(fsr.GetFirstTopLevelNode());
            
            // Initialize Instances
            class_values = new string[] { "circle", "left_arrow", "right_arrow", "unknown" };

            // Initialize Timer
            timer.Interval = 1000 / FPS;
            timer.Tick += ProcessFrame;
        }

        // Browse Button
        private void browse_image_Click(object sender, EventArgs e)
        {
            var FD = new OpenFileDialog();
            FD.Title = "Choose Image";
            FD.InitialDirectory = @"C:\Users\Stephen Djohan\Documents\Visual Studio 2015\Projects\TrafficLightRecognition\TrafficLightRecognition\video";
            FD.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            FD.FilterIndex = 2;
            FD.RestoreDirectory = true;
            if (FD.ShowDialog() == DialogResult.OK)
            {
                fileToOpen = FD.FileName;
                cam = false;
                Application.Idle += ProcessFrame;
            }
        }

        private void browse_video_Click(object sender, EventArgs e)
        {
            var FD = new OpenFileDialog();
            FD.Title = "Choose Image";
            FD.InitialDirectory = @"C:\Users\Stephen Djohan\Documents\Visual Studio 2015\Projects\TrafficLightRecognition\TrafficLightRecognition\video";
            FD.Filter = "All Videos Files |*.dat; *.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; " +
                        " *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm";
            FD.FilterIndex = 2;
            FD.RestoreDirectory = true;
            if (FD.ShowDialog() == DialogResult.OK)
            {
                fileToOpen = FD.FileName;
                try { cap = new Capture(fileToOpen); }
                catch (NullReferenceException ex)
                {
                    Console.WriteLine(ex.InnerException);
                    return;
                }

                cam = true;
                //Application.Idle += ProcessFrame;
                timer.Start();
            }
        }

        private void camera_Click(object sender, EventArgs e)
        {
            try { cap = new Capture(); }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.InnerException);
                return;
            }

            cam = true;
            Application.Idle += ProcessFrame;
        }

        public List<Rectangle> FindBoundingBoxes(Image<Gray, byte> image)
        {
            List<Rectangle> boundingBoxes = new List<Rectangle>();
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            
            CvInvoke.FindContourTree(image, contours, ChainApproxMethod.ChainApproxSimple);
            for (int i = 0; i < contours.Size; ++i)
            {
                using (VectorOfPoint contour = contours[i])
                {
                    boundingBoxes.Add(CvInvoke.BoundingRectangle(contour));
                }
            }

            return boundingBoxes;
        }
        
        private Dictionary<Rectangle, string> Evaluate(Dictionary<Rectangle, string> boundingBoxesLabels)
        {
            Rectangle[] keyArray = new Rectangle[boundingBoxesLabels.Keys.Count];
            boundingBoxesLabels.Keys.CopyTo(keyArray, 0);
           
            Matrix<float> test = new Matrix<float>(1, Convert.ToInt32(hog.DescriptorSize));
            foreach (var r in keyArray)
            {
                imgOri.ROI = r;
                float[] HOGDescriptor = GetVector(imgOri);
                for (int j = 0; j < test.Cols; ++j)
                {
                    test[0, j] = HOGDescriptor[j];
                }
                imgOri.ROI = Rectangle.Empty;
                
                float prediction = svm.Predict(test);

                boundingBoxesLabels[r] = class_values[(int)prediction];
            }

            return boundingBoxesLabels;
        }

        /*                              *
         *  Get HOG descriptor vectors  *
         *                              */
        private float[] GetVector(Image<Bgr, Byte> im)
        {
            Image<Bgr, Byte> imageOfInterest = im.Resize(40, 40, Inter.Linear);
            float[] result = hog.Compute(imageOfInterest);
            return result;
        }
        
        private void ProcessFrame(object sender, EventArgs e)
        {
            if(cam == true)
            {
                imgOri = cap.QueryFrame().ToImage<Bgr, Byte>();
            }
            else
            {
                imgOri = new Image<Bgr, byte>(fileToOpen);
            }
            
            imgShow = imgOri;
            imgR = imgOri.InRange(new Bgr(0, 0, 200), new Bgr(100, 100, 255));
            imgY = imgOri.InRange(new Bgr(0, 100, 200), new Bgr(80, 255, 255));
            imgG = imgOri.InRange(new Bgr(150, 150, 0), new Bgr(255, 255, 80));
            imgEdit = imgR + imgY + imgG;
            imgEdit = imgEdit.SmoothGaussian(7);

            // Find bounding boxes for each object captured
            List<Rectangle> boundingBoxes = FindBoundingBoxes(imgEdit);
            int init_size;
            do
            {
                init_size = boundingBoxes.Count;
                bool intersect = false;
                for (int i = 0; i < boundingBoxes.Count - 1; ++i)
                {
                    for (int j = i + 1; j < boundingBoxes.Count; ++j)
                    {
                        if (boundingBoxes[i].IntersectsWith(boundingBoxes[j]))
                        {
                            Rectangle r1 = boundingBoxes[j];
                            Rectangle r2 = boundingBoxes[i];
                            boundingBoxes.Remove(r1);
                            boundingBoxes.Remove(r2);
                            boundingBoxes.Add(Rectangle.Union(r1, r2));
                            intersect = true;
                            break;
                        }
                    }
                    if (intersect)
                    {
                        break;
                    }
                }
            } while (boundingBoxes.Count != init_size);

            // Create a Dictionary to store bounding box and its label
            Dictionary<Rectangle, string> boundingBoxesLabels = new Dictionary<Rectangle, string>();
            for(int i = 0; i < boundingBoxes.Count; ++i) {
                boundingBoxesLabels.Add(boundingBoxes[i], "");
            }

            // Classify all objects captured
            boundingBoxesLabels = Evaluate(boundingBoxesLabels);

            Parallel.ForEach(boundingBoxesLabels, KeyValuePair =>
            {
                if (KeyValuePair.Value != "unknown")
                {
                    imgShow.Draw(KeyValuePair.Key, new Bgr(0, 0, 255), 3);
                    imgShow.Draw(
                        KeyValuePair.Value,
                        new Point(KeyValuePair.Key.Left - 30, KeyValuePair.Key.Bottom + 20),
                        FontFace.HersheyPlain,
                        2,
                        new Bgr(255, 255, 255),
                        2
                    );
                }
            });

            Video_Image_1.Image = imgShow.Bitmap;
         }
    }
}
