using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace LearningModule2
{
    class ImageCropper
    {
        /* Number of folders of images to be trained */
        private int numDayFolders;
        private int numNightFolders;

        /* List of data to be trained (folder name, image name, upper left x, upper left y, lower right x, lower right y, label) */
        private List<string> folderList;
        private List<string> nameList;
        private List<int> upperLeftXList;
        private List<int> upperLeftYList;
        private List<int> lowerRightXList;
        private List<int> lowerRightYList;
        private List<int> label;

        /* Number of data per class */
        int numCircle = 0;
        int numLeftArrow = 0;

        /*              *
         *  Constructor *
         *              */
        public ImageCropper()
        {
            numDayFolders = Directory.GetDirectories("../../../images/dayTrain/").Length;
            numNightFolders = Directory.GetDirectories("../../../images/nightTrain/").Length;

            folderList = new List<string>();
            nameList = new List<string>();
            upperLeftXList = new List<int>();
            upperLeftYList = new List<int>();
            lowerRightXList = new List<int>();
            lowerRightYList = new List<int>();
            label = new List<int>();
        }

        public int getNumDayFolders()
        {
            return numDayFolders;
        }

        public int getNightDayFolders()
        {
            return numNightFolders;
        }

        private void addToList(string fileName, string[] values)
        {
            if (fileName.Contains("day"))
            {
                string folder = "dayClip" + fileName[fileName.IndexOf("dayClip") + 7] + fileName[fileName.IndexOf("dayClip") + 8];
                folder = folder.Replace("/", "");
                folderList.Add(folder);
            }
            else
            {
                string folder = "nightClip" + fileName[fileName.IndexOf("nightClip") + 9] + fileName[fileName.IndexOf("nightClip") + 10];
                folder = folder.Replace("/", "");
                folderList.Add(folder);
            }

            nameList.Add(values[0]);
            upperLeftXList.Add(Int32.Parse(values[2]));
            upperLeftYList.Add(Int32.Parse(values[3]));
            lowerRightXList.Add(Int32.Parse(values[4]));
            lowerRightYList.Add(Int32.Parse(values[5]));
            switch (values[1])
            {
                case "stop": label.Add(0); ++numCircle; break;
                case "go": label.Add(0); ++numCircle; break;
                case "warning": label.Add(0); ++numCircle; break;
                case "stopLeft": label.Add(1); ++numLeftArrow; break;
                case "goLeft": label.Add(1); ++numLeftArrow; break;
                case "warningLeft": label.Add(1); ++numLeftArrow; break;
            }
        }

        /*                                                                  *
         *  Read annotated CSV files and store the information in the lists *
         *                                                                  */
        public void readLabeledCSV(string fileName)
        {
            var reader = new StreamReader(File.OpenRead(fileName));
            var header = reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');

                if (values[1] == "stopLeft" || values[1] == "goLeft" || values[1] == "warningLeft")
                {
                    addToList(fileName, values);
                }
                else if (values[1] == "go" || values[1] == "stop" || values[1] == "warning")
                {
                    addToList(fileName, values);
                }
            }

            Console.WriteLine("Circle : " + numCircle + ", Left Arrow : " + numLeftArrow);
        }

        public static List<Rectangle> FindBoundingBoxes(Image<Gray, byte> image)
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

        public void cropTrafficLightImages()
        {
            var ext = new List<string> { ".jpg", ".gif", ".png" };
            var fileArray = Directory.GetFiles("../../images/raw/", "*.*").Where(s => ext.Any(e => s.EndsWith(e)));
            foreach (string file in fileArray)
            {
                cropImageFromRaw(new Image<Bgr, byte>(file));
            }
        }

        public void cropTrafficLightVideos(int Time_millisecounds)
        {
            var ext = new List<string> { ".mp4", ".avi", ".wmv" };
            var fileArray = Directory.GetFiles("../../images/", "*.*").Where(s => ext.Any(e => s.EndsWith(e)));
            foreach (string file in fileArray)
            {
                List<Image<Bgr, Byte>> image_array = new List<Image<Bgr, Byte>>();
                System.Diagnostics.Stopwatch SW = new System.Diagnostics.Stopwatch();

                bool Reading = true;
                Capture cap = new Capture(file);
                SW.Start();

                while (Reading)
                {
                    Image<Bgr, Byte> frame = cap.QueryFrame().ToImage<Bgr, Byte>();
                    if (frame != null)
                    {
                        cropImageFromRaw(frame.Copy());
                        if (SW.ElapsedMilliseconds >= Time_millisecounds) Reading = false;
                    }
                    else
                    {
                        Reading = false;
                    }
                }
            }
        }

        private void cropImageFromRaw(Image<Bgr, byte> image)
        {
            if (image.Width >= 15 && image.Height >= 15)
            {
                Image<Gray, Byte> imgR, imgY, imgG, imgEdit;
                List<Rectangle> boundingBoxes;

                imgR = image.InRange(new Bgr(0, 0, 200), new Bgr(100, 100, 255));
                imgY = image.InRange(new Bgr(0, 100, 200), new Bgr(80, 255, 255));
                imgG = image.InRange(new Bgr(150, 150, 0), new Bgr(255, 255, 80));
                imgEdit = imgR + imgY + imgG;
                imgEdit = imgEdit.SmoothGaussian(7);

                // Find bounding boxes for each object captured
                boundingBoxes = FindBoundingBoxes(imgEdit);
                int init_size;
                do
                {
                    init_size = boundingBoxes.Count;
                    bool intersect = false;
                    for (int j = 0; j < boundingBoxes.Count - 1; ++j)
                    {
                        for (int k = j + 1; k < boundingBoxes.Count; ++k)
                        {
                            if (boundingBoxes[j].IntersectsWith(boundingBoxes[k]))
                            {
                                Rectangle r1 = boundingBoxes[k];
                                Rectangle r2 = boundingBoxes[j];
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

                foreach (Rectangle rect in boundingBoxes)
                {
                    image.ROI = rect;
                    image.Save("../../images/" + Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 8) + ".jpg");
                    image.ROI = Rectangle.Empty;
                }
            }
        }

        /*                                        *
         *  Iterate through all list, crop image  *
         *                                        */
        public void cropImagesFromDataset()
        {
            string[] class_values = new string[] { "circle", "left_arrow", "right_arrow" };
            Image<Bgr, Byte> image;

            Directory.CreateDirectory("../../../images/day/circle");
            Directory.CreateDirectory("../../../images/day/left_arrow");
            Directory.CreateDirectory("../../../images/night/circle");
            Directory.CreateDirectory("../../../images/night/left_arrow");

            for (int i = 0; i < nameList.Count; ++i)
            {
                string imagePath;
                if (folderList[i].Contains("day"))
                {
                    imagePath = "../../../images/dayTrain/" + folderList[i] + "/frames/" + nameList[i].Replace("dayTraining/", "");
                }
                else
                {
                    imagePath = "../../../images/nightTrain/" + folderList[i] + "/frames/" + nameList[i].Replace("nightTraining/", "");
                }

                image = new Image<Bgr, Byte>(imagePath);

                int boundingBoxWidth = lowerRightXList[i] - upperLeftXList[i];
                int boundingBoxHeight = lowerRightYList[i] - upperLeftYList[i];
                Rectangle r = new Rectangle(upperLeftXList[i], upperLeftYList[i], boundingBoxWidth, boundingBoxHeight);

                if (r.Width >= 12 && r.Height >= 12)
                {
                    image.ROI = r;
                    if (label[i] == 0)
                    {
                        image.Save("../../../images/night/circle/" + Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 8) + ".jpg");
                    }
                    else if (label[i] == 1)
                    {
                        image.Save("../../../images/night/left_arrow/" + Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 8) + ".jpg");
                    }

                    // If the image is left_arrow, flip it and save as right arrow
                    if (label[i] == 1)
                    {
                        image = image.Flip(FlipType.Horizontal);
                        image.Save("../../../images/night/right_arrow/" + Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 8) + ".jpg");
                    }

                    image.ROI = Rectangle.Empty;
                }

                Console.WriteLine(i);
            }
        }
    }
}
