using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using java.util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using weka.core;
using weka.core.converters;

namespace LearningModule
{
    class HOGLearner
    {
        /* HOG Descriptor for learning */
        private static HOGDescriptor hog = new HOGDescriptor(
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

        /* Number of folders of images to be trained */
        private static int numDayFolders = 1;
        private static int numNightFolders = 5;
        //private static int numDayFolders = Directory.GetDirectories("../../images/dayTrain/").Length;
        //private static int numNightFolders = Directory.GetDirectories("../../images/nightTrain/").Length;

        /* List of data to be trained (folder name, image name, upper left x, upper left y, lower right x, lower right y, label) */
        private static List<string> folderList = new List<string>();
        private static List<string> nameList = new List<string>();
        private static List<int> upperLeftXList = new List<int>();
        private static List<int> upperLeftYList = new List<int>();
        private static List<int> lowerRightXList = new List<int>();
        private static List<int> lowerRightYList = new List<int>();
        private static List<string> label = new List<string>();

        /*                                                                  *
         *  Read annotated CSV files and store the information in the lists *
         *                                                                  */
        public static void readLabeledCSV(string fileName)
        {
            var reader = new StreamReader(File.OpenRead(fileName));
            var header = reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');

                if (fileName.Contains("day"))
                {
                    folderList.Add("dayClip" + fileName[fileName.IndexOf("dayClip") + 7]);
                }
                else
                {
                    folderList.Add("nightClip" + fileName[fileName.IndexOf("nightClip") + 9]);
                    Console.WriteLine("nightClip" + fileName[fileName.IndexOf("nightClip") + 9]);
                }
                
                nameList.Add(values[0]);
                upperLeftXList.Add(Int32.Parse(values[2]));
                upperLeftYList.Add(Int32.Parse(values[3]));
                lowerRightXList.Add(Int32.Parse(values[4]));
                lowerRightYList.Add(Int32.Parse(values[5]));
                switch (values[1])
                {
                    case "stop": label.Add("circle"); break;
                    case "go": label.Add("circle"); break;
                    case "warning": label.Add("circle"); break;
                    case "stopLeft": label.Add("left_arrow"); break;
                    case "goLeft": label.Add("left_arrow"); break;
                    case "warningLeft": label.Add("left_arrow"); break;
                }
            }
        }

        /*                                                                                           *
         *  Iterate through all list, crop image, extract features, and store data as training data  *
         *                                                                                           */
        public static Instances extractFeatures()
        {
            List<string> class_values = new List<string>() { "circle", "left_arrow", "right_arrow" };
            Instances HOGInstances = createHOGInstances(Convert.ToInt32(hog.DescriptorSize), class_values, 60000);

            int numData = 1;
            for (int i = 0; i < nameList.Count; ++i)
            {
                string imagePath;
                if (folderList[i].Contains("day"))
                {
                    imagePath = "../../images/dayTrain/" + folderList[i] + "/frames/" + nameList[i].Replace("dayTraining/", "");
                }
                else
                {
                    imagePath = "../../images/nightTrain/" + folderList[i] + "/frames/" + nameList[i].Replace("nightTraining/", "");
                }
                
                Image<Bgr, Byte> image = new Image<Bgr, Byte>(imagePath);

                int boundingBoxWidth = lowerRightXList[i] - upperLeftXList[i];
                int boundingBoxHeight = lowerRightYList[i] - upperLeftYList[i];
                Rectangle r = new Rectangle(upperLeftXList[i], upperLeftYList[i], boundingBoxWidth, boundingBoxHeight);
                image.ROI = r;
                float[] HOGDescriptor = GetVector(image);

                Instance instance = new Instance(HOGInstances.numAttributes());
                instance.setDataset(HOGInstances);
                for (int j = 0; j < instance.numAttributes() - 1; ++j)
                {
                    instance.setValue(j, HOGDescriptor[j]);
                }
                instance.setClassValue(label[i]);
                HOGInstances.add(instance);
                Console.WriteLine("Done creating training data #" + numData);
                ++numData;

                // If the image is a left arrow, flip it and store it in training data as right arrow
                if (label[i] == "left_arrow")
                {
                    Image<Bgr, Byte> flippedImage = image.Flip(FLIP.HORIZONTAL);
                    HOGDescriptor = GetVector(flippedImage);
                    instance = new Instance(HOGInstances.numAttributes());
                    instance.setDataset(HOGInstances);
                    for (int j = 0; j < instance.numAttributes() - 1; ++j)
                    {
                        instance.setValue(j, HOGDescriptor[j]);
                    }
                    instance.setClassValue("right_arrow");
                    HOGInstances.add(instance);
                    Console.WriteLine("Done creating training data #" + numData);
                    ++numData;
                }

                CvInvoke.cvResetImageROI(image);
            }

            return HOGInstances;
        }

        /*                                                               *
         *  Create new HOG Instances to store all of the training data   *
         *                                                               */
        private static Instances createHOGInstances(int descriptorSize, List<string> class_values, int maxInstances)
        {
            FastVector attributes = new FastVector(descriptorSize + 1);
            for (int i = 1; i <= Convert.ToInt32(hog.DescriptorSize); ++i)
            {
                weka.core.Attribute att = new weka.core.Attribute("v" + i);
                attributes.addElement(att);
            }

            FastVector values = new FastVector(class_values.Count);
            foreach (string class_value in class_values)
            {
                values.addElement(class_value);
            }
            attributes.addElement(new weka.core.Attribute("label", values));

            Instances dataTrain = new Instances("HOG Vector", attributes, maxInstances);
            dataTrain.setClassIndex(dataTrain.numAttributes() - 1);

            return dataTrain;
        }

        /*                                          *
         *  Write the instances data to arff file   *
         *                                          */
        public static void writeToARFF(Instances instances, string fileName)
        {
            ArffSaver saver = new ArffSaver();
            saver.setInstances(instances);
            saver.setFile(new java.io.File(fileName));
            saver.writeBatch();
        }

        /*                                  *
         *  Resize the image to 40x40 size  *
         *                                  */
        public static Image<Bgr, Byte> Resize(Image<Bgr, Byte> im)
        {
            return im.Resize(40, 40, INTER.CV_INTER_LINEAR);
        }

        /*                              *
         *  Get HOG descriptor vectors  *
         *                              */
        public static float[] GetVector(Image<Bgr, Byte> im)
        {
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
            /* Read the annotated CSV and store the informations in the lists */
            for (int i = 1; i <= numDayFolders; ++i)
            {
                readLabeledCSV("../../images/dayTrain/dayClip" + i + "/frameAnnotationsBULB.csv");
            }
            //for (int i = 1; i <= numNightFolders; ++i)
            //{
            //    readLabeledCSV("../../images/nightTrain/nightClip" + i + "/frameAnnotationsBULB.csv");
            //}

            /* Train the data and write to ARFF file */
            Instances HOGInstances = extractFeatures();
            Console.WriteLine(HOGInstances);
            //writeToARFF(HOGInstances, "../../data/test.arff");
        }
    }
}
