using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using weka.core;

namespace LearningModule
{
    class Program
    {
        public static Image<Bgr, Byte> Resize(Image<Bgr, Byte> im)
        {
            return im.Resize(40, 40, INTER.CV_INTER_LINEAR);
        }

        public static float[] GetVector(Image<Bgr, Byte> im)
        {
            HOGDescriptor hog = new HOGDescriptor(new Size(40, 40), new Size(8, 8), new Size(4, 4), new Size(4, 4),
                9, 1, -1, 0.2, true);
            Image<Bgr, Byte> imageOfInterest = Resize(im);
            Point[] p = new Point[imageOfInterest.Width * imageOfInterest.Height];
            int k = 0;
            for (int i = 0; i < imageOfInterest.Width; i++)
            {
                for (int j = 0; j < imageOfInterest.Height; j++)
                {
                    Point p1 = new Point(i, j);
                    p[k++] = p1;
                }
            }
            float[] result = hog.Compute(imageOfInterest, new Size(4, 4), new Size(0, 0), p);
            return result;
        }

        static void Main(string[] args)
        {
            int numFolders = 13;
            for (int i = 1; i <= numFolders; ++i)
            {
                var reader = new StreamReader(File.OpenRead("../../images/dayTrain/dayClip" + i + "/frameAnnotationsBULB.csv"));
                List<string> nameList = new List<string>();
                List<int> upperLeftXList = new List<int>();
                List<int> upperLeftYList = new List<int>();
                List<int> lowerRightXList = new List<int>();
                List<int> lowerRightYList = new List<int>();
                List<string> label = new List<string>();

                var header = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    nameList.Add(values[0]);
                    upperLeftXList.Add(Int32.Parse(values[2]));
                    upperLeftYList.Add(Int32.Parse(values[3]));
                    lowerRightXList.Add(Int32.Parse(values[4]));
                    lowerRightYList.Add(Int32.Parse(values[5]));
                    label.Add(values[1]);
                }

                

                // Iterate through all list
                for (int j = 0; j < nameList.Count; ++j)
                {
                    string imagePath = "../../images/dayTrain/dayClip" + i + "/frames/" + nameList[j].Replace("dayTraining/", "");
                    Image<Bgr, Byte> image = new Image<Bgr, Byte>(imagePath);

                    int boundingBoxWidth = lowerRightXList[j] - upperLeftXList[j];
                    int boundingBoxHeight = lowerRightYList[j] - upperLeftYList[j];
                    Rectangle r = new Rectangle(upperLeftXList[j], upperLeftYList[j], boundingBoxWidth, boundingBoxHeight);
                    image.ROI = r;
                    float[] HOGDescriptor = GetVector(image);
                    

                    CvInvoke.cvResetImageROI(image);
                    
                }
            }
        }
    }
}
