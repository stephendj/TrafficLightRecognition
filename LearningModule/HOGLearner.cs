using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.ML;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Emgu.CV.ML.MlEnum;

namespace LearningModule2
{
    class HOGLearner
    {
        public static HOGDescriptor hog;
        public static List<string> class_values = new List<string>() { "circle", "left_arrow", "right_arrow", "unknown" };
        public static Matrix<float> instances;
        public static Matrix<int> labels;
        public static SVM svm;

        int imageCount = 0;

        public HOGLearner()
        {
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

            imageCount += Directory.GetFiles("../../images/day/circle/", "*.jpg").Length;
            imageCount += (Directory.GetFiles("../../images/day/left_arrow/", "*.jpg").Length * 2);
            imageCount += Directory.GetFiles("../../images/day/unknown/", "*.jpg").Length;
        }

        /*                                  *
         *  Resize the image to 40x40 size  *
         *                                  */
        private Image<Bgr, Byte> Resize(Image<Bgr, Byte> im)
        {
            return im.Resize(40, 40, Inter.Linear);
        }

        /*                              *
         *  Get HOG descriptor vectors  *
         *                              */
        private float[] GetVector(Image<Bgr, Byte> im)
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
            float[] result = hog.Compute(imageOfInterest);
            return result;
        }

        public void extractFeatures()
        {
            Image<Bgr, Byte> image;

            instances = new Matrix<float>(imageCount, Convert.ToInt32(hog.DescriptorSize));
            labels = new Matrix<int>(imageCount, 1);

            svm = new SVM();

            int i = 0;
            foreach(string file in Directory.GetFiles("../../images/day/circle/", "*.jpg"))
            {
                image = new Image<Bgr, byte>(file);
                float[] hogVector = GetVector(image);
                int j = 0;
                foreach (float v in hogVector)
                {
                    instances[i, j] = v;
                    ++j;
                    
                }
                labels[i, 0] = 0;
                ++i;
            }

            foreach (string file in Directory.GetFiles("../../images/day/left_arrow/", "*.jpg"))
            {
                image = new Image<Bgr, byte>(file);
                float[] hogVector = GetVector(image);
                int j = 0;
                foreach (float v in hogVector)
                {
                    instances[i, j] = v;
                    ++j;
                }
                labels[i, 0] = 1;
                ++i;
            }

            foreach (string file in Directory.GetFiles("../../images/day/left_arrow/", "*.jpg"))
            {
                image = new Image<Bgr, byte>(file);
                image = image.Flip(FlipType.Horizontal);
                float[] hogVector = GetVector(image);
                int j = 0;
                foreach (float v in hogVector)
                {
                    instances[i, j] = v;
                    ++j;
                }
                labels[i, 0] = 2;
                ++i;
            }

            foreach (string file in Directory.GetFiles("../../images/day/unknown/", "*.jpg"))
            {
                image = new Image<Bgr, byte>(file);
                float[] hogVector = GetVector(image);
                int j = 0;
                foreach (float v in hogVector)
                {
                    instances[i, j] = v;
                    ++j;
                }
                labels[i, 0] = 3;
                ++i;
            }
        }

        public void train(string filepath)
        {
            svm.SetKernel(SVM.SvmKernelType.Linear);
            TrainData td = new TrainData(instances, DataLayoutType.RowSample, labels);

            // Begin training
            bool trained = svm.Train(td);

            // Save model to file;
            svm.Save(filepath);
        }
    }
}
