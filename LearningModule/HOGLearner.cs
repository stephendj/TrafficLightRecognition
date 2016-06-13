using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.ML;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Emgu.CV.ML.MlEnum;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

namespace LearningModule
{
    class HOGLearner
    {
        public static HOGDescriptor hog;
        public static List<string> class_values = new List<string>() { "circle", "left_arrow", "right_arrow", "unknown" };
        int imageCount = 0;
        int numFeatures;

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

            numFeatures = Convert.ToInt32(hog.DescriptorSize);
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
            float[] result = hog.Compute(imageOfInterest);
            return result;
        }

        public static void Serialize(object t, string path)
        {
            using (Stream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, t);
            }
        }

        public static object Deserialize(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                return bformatter.Deserialize(stream);
            }
        }

        public void extractFeatures(string folderPath, bool day, bool night)
        {
            Image<Bgr, Byte> image;

            if(day)
            {
                imageCount += Directory.GetFiles("../../images/day/circle/", "*.jpg").Length;
                imageCount += (Directory.GetFiles("../../images/day/left_arrow/", "*.jpg").Length * 2);
                imageCount += Directory.GetFiles("../../images/day/unknown/", "*.jpg").Length;
            }

            if(night)
            {
                imageCount += Directory.GetFiles("../../images/night/circle/", "*.jpg").Length;
                imageCount += (Directory.GetFiles("../../images/night/left_arrow/", "*.jpg").Length * 2);
                imageCount += Directory.GetFiles("../../images/night/unknown/", "*.jpg").Length;
            }

            Matrix<float> instances = new Matrix<float>(imageCount, numFeatures);
            Matrix<int> labels = new Matrix<int>(imageCount, 1);

            // Randomize index to put in instances
            int[] randomIdx = new int[imageCount];
            for(int idx = 0; idx < imageCount; ++idx)
            {
                randomIdx[idx] = idx;
            }
            Random rnd = new Random();
            randomIdx = randomIdx.OrderBy(x => rnd.Next()).ToArray();

            int i = 0;

            if(day)
            {
                Console.WriteLine("Creating circles instances");
                foreach (string file in Directory.GetFiles("../../images/day/circle/", "*.jpg"))
                {
                    image = new Image<Bgr, byte>(file);
                    float[] hogVector = GetVector(image);
                    for (int j = 0; j < hogVector.Length; ++j)
                    {
                        instances[randomIdx[i], j] = hogVector[j];
                    }
                    labels[randomIdx[i], 0] = 0;
                    ++i;
                }

                Console.WriteLine("Creating left arrow instances");
                foreach (string file in Directory.GetFiles("../../images/day/left_arrow/", "*.jpg"))
                {
                    image = new Image<Bgr, byte>(file);
                    float[] hogVector = GetVector(image);
                    for (int j = 0; j < hogVector.Length; ++j)
                    {
                        instances[randomIdx[i], j] = hogVector[j];
                    }
                    labels[randomIdx[i], 0] = 1;
                    ++i;
                }

                Console.WriteLine("Creating right arrow instances");
                foreach (string file in Directory.GetFiles("../../images/day/left_arrow/", "*.jpg"))
                {
                    image = new Image<Bgr, byte>(file);
                    image = image.Flip(FlipType.Horizontal);
                    float[] hogVector = GetVector(image);
                    for (int j = 0; j < hogVector.Length; ++j)
                    {
                        instances[randomIdx[i], j] = hogVector[j];
                    }
                    labels[randomIdx[i], 0] = 2;
                    ++i;
                }

                Console.WriteLine("Creating unknown instances");
                foreach (string file in Directory.GetFiles("../../images/day/unknown/", "*.jpg"))
                {
                    image = new Image<Bgr, byte>(file);
                    float[] hogVector = GetVector(image);
                    for (int j = 0; j < hogVector.Length; ++j)
                    {
                        instances[randomIdx[i], j] = hogVector[j];
                    }
                    labels[randomIdx[i], 0] = 3;
                    ++i;
                }
            }
            
            if(night)
            {
                Console.WriteLine("Creating circles instances");
                foreach (string file in Directory.GetFiles("../../images/night/circle/", "*.jpg"))
                {
                    image = new Image<Bgr, byte>(file);
                    float[] hogVector = GetVector(image);
                    for (int j = 0; j < hogVector.Length; ++j)
                    {
                        instances[randomIdx[i], j] = hogVector[j];
                    }
                    labels[randomIdx[i], 0] = 0;
                    ++i;
                }

                Console.WriteLine("Creating left arrow instances");
                foreach (string file in Directory.GetFiles("../../images/night/left_arrow/", "*.jpg"))
                {
                    image = new Image<Bgr, byte>(file);
                    float[] hogVector = GetVector(image);
                    for (int j = 0; j < hogVector.Length; ++j)
                    {
                        instances[randomIdx[i], j] = hogVector[j];
                    }
                    labels[randomIdx[i], 0] = 1;
                    ++i;
                }

                Console.WriteLine("Creating right arrow instances");
                foreach (string file in Directory.GetFiles("../../images/night/left_arrow/", "*.jpg"))
                {
                    image = new Image<Bgr, byte>(file);
                    image = image.Flip(FlipType.Horizontal);
                    float[] hogVector = GetVector(image);
                    for (int j = 0; j < hogVector.Length; ++j)
                    {
                        instances[randomIdx[i], j] = hogVector[j];
                    }
                    labels[randomIdx[i], 0] = 2;
                    ++i;
                }

                Console.WriteLine("Creating unknown instances");
                foreach (string file in Directory.GetFiles("../../images/night/unknown/", "*.jpg"))
                {
                    image = new Image<Bgr, byte>(file);
                    float[] hogVector = GetVector(image);
                    for (int j = 0; j < hogVector.Length; ++j)
                    {
                        instances[randomIdx[i], j] = hogVector[j];
                    }
                    labels[randomIdx[i], 0] = 3;
                    ++i;
                }
            }

            if(day && night)
            {
                Serialize(instances, folderPath + "instances");
                Serialize(labels, folderPath + "labels");
            }
            else if(day && !night)
            {
                Serialize(instances, folderPath + "dayInstances");
                Serialize(labels, folderPath + "dayLabels");
            }
            else if(night && !day)
            {
                Serialize(instances, folderPath + "nightInstances");
                Serialize(labels, folderPath + "nightLabels");
            }
            
        }

        public void testSetEvaluationSVM(SVM svm, Matrix<float> testInstances, Matrix<int> testLabels, string[] classes)
        {
            ConfusionMatrix cm = new ConfusionMatrix(classes.Length);
            cm.setLabel(classes);

            for (int j = 0; j < testInstances.Rows; ++j)
            {
                Matrix<float> test = new Matrix<float>(1, numFeatures);
                for (int k = 0; k < numFeatures; ++k)
                {
                    test[0, k] = testInstances[j, k];
                }
                int prediction = (int) svm.Predict(test);
                cm.incrementCM(testLabels[j, 0], prediction);
            }

            cm.printConfusionMatrix();
            cm.printEvaluation();
        }

        public void testSetEvaluationANN(ANN_MLP ann, Matrix<float> testInstances, Matrix<float> testLabels, string[] classes)
        {
            ConfusionMatrix cm = new ConfusionMatrix(classes.Length);
            cm.setLabel(classes);

            for (int j = 0; j < testInstances.Rows; ++j)
            {
                Matrix<float> test = new Matrix<float>(1, numFeatures);
                for (int k = 0; k < numFeatures; ++k)
                {
                    test[0, k] = testInstances[j, k];
                }
                float result = ann.Predict(test);
                int prediction = Convert.ToInt32(result);

                int actualValue = 0;
                for(int k = 0; k < 4; ++k)
                {
                    if((int) testLabels[j, k] == 1)
                    {
                        actualValue = k;
                    }
                } 
                cm.incrementCM(actualValue, prediction);
            }

            cm.printConfusionMatrix();
            cm.printEvaluation();
        }

        public SVM trainSVM(Matrix<float> instances, Matrix<int> labels)
        {
            SVM svm = new SVM();
            svm.SetKernel(SVM.SvmKernelType.Rbf);
            TrainData td = new TrainData(instances, DataLayoutType.RowSample, labels);
            bool trained = svm.Train(td);
            
            return svm;
        }

        private void ActivationFunctionHardFix(ANN_MLP network)
        {
            string tmpFile = Path.GetTempPath() + "TempAnnForActivationParametersFix.tmp";
            network.Save(tmpFile);
            StreamReader reader = new StreamReader(tmpFile);
            string configContent = reader.ReadToEnd();
            reader.Close();

            configContent = configContent.Replace("min_val: 0.", "min_val: -0.95");
            configContent = configContent.Replace("max_val: 0.", "max_val: 0.95");
            configContent = configContent.Replace("min_val1: 0.", "min_val1: -0.98");
            configContent = configContent.Replace("max_val1: 0.", "max_val1: 0.98");

            StreamWriter writer = new StreamWriter(tmpFile, false);
            writer.Write(configContent);
            writer.Close();

            network.Read(new FileStorage(tmpFile, FileStorage.Mode.Read).GetFirstTopLevelNode());
            File.Delete(tmpFile);
        }

        public ANN_MLP trainANN(Matrix<float> instances, Matrix<float> labels)
        {
            Matrix<int> layerSize = new Matrix<int>(new int[] { instances.Cols, 20, 4 });
            ANN_MLP ann = new ANN_MLP();
            ann.SetActivationFunction(ANN_MLP.AnnMlpActivationFunction.SigmoidSym);
            ann.TermCriteria = new MCvTermCriteria(3000, 1.0e-6);
            ann.SetLayerSizes(layerSize);
            ActivationFunctionHardFix(ann);
            TrainData td = new TrainData(instances, DataLayoutType.RowSample, labels);
            bool trained = ann.Train(td);

            return ann;
        }

        public RTrees trainRTrees(Matrix<float> instances, Matrix<int> labels, int folds)
        {
            RTrees rTrees = new RTrees();
            rTrees.CVFolds = folds;
            TrainData td = new TrainData(instances, DataLayoutType.RowSample, labels);
            bool trained = rTrees.Train(td);

            return rTrees;
        }

        public LogisticRegression trainLR(Matrix<float> instances, Matrix<int> labels)
        {
            LogisticRegression LR = new LogisticRegression();
            return LR;
        }

        public void crossValidate(string algorithm, Matrix<float> instances, Matrix<int> labels, int fold, string[] classes)
        {
            int numInstances = instances.Rows;

            // Randomize data
            CvInvoke.RandShuffle(instances, 1, 0);
            int interval = numInstances / fold;
            ConfusionMatrix[] cmArray = new ConfusionMatrix[fold];

            for(int i = 0; i < fold; ++i)
            {
                Console.WriteLine("Processing fold " + i + "...");
                int min = i * interval;
                int max;
                if(i < fold - 1)
                {
                    max = (i + 1) * interval - 1;
                }
                else
                {
                    max = numInstances - 1;
                }

                // Matrix for small data set
                Matrix<float> trainInstances = new Matrix<float>(numInstances - (max - min + 1), numFeatures);
                Matrix<int> trainLabels = new Matrix<int>(numInstances - (max - min + 1), 1);

                // Matrix for small test set
                Matrix<float> testInstances = new Matrix<float>(max - min + 1, numFeatures);
                Matrix<int> testLabels = new Matrix<int>(max - min + 1, 1);
                
                int incTrain = 0;
                int incTest = 0;

                Console.WriteLine("Dividing data...");
                // Divide the data
                for(int j = 0; j < instances.Rows; ++j)
                {
                    if(j >= min && j <= max)
                    {
                        for(int k = 0; k < numFeatures; ++k)
                        {
                            testInstances[incTest, k] = instances[j, k];
                        }
                        testLabels[incTest, 0] = labels[j, 0];
                        ++incTest;
                    }
                    else
                    {
                        for (int k = 0; k < numFeatures; ++k)
                        {
                            trainInstances[incTrain, k] = instances[j, k];
                        }
                        trainLabels[incTrain, 0] = labels[j, 0];
                        ++incTrain;
                    }
                }
                Console.WriteLine("Dividing data finished!");

                // Begin training and make predictions on test set
                ConfusionMatrix cm = new ConfusionMatrix(classes.Length);
                cm.setLabel(classes);
                Console.WriteLine("Begin Training...");
                SVM svm = trainSVM(instances, labels);
                Console.WriteLine("Training Finished!");

                Console.WriteLine("Begin Predicting");
                for (int j = 0; j < testInstances.Rows; ++j)
                {
                    Matrix<float> test = new Matrix<float>(1, numFeatures);
                    for(int k = 0; k < numFeatures; ++k)
                    {
                        test[0,k] = testInstances[j, k];
                    }
                    int prediction = (int) svm.Predict(test);
                    cm.incrementCM(testLabels[j, 0], prediction);
                }

                cmArray[i] = cm;
                cm.printConfusionMatrix();
            }

            ConfusionMatrix cmGlobal = ConfusionMatrix.getCrossValidationCM(cmArray);
            cmGlobal.printConfusionMatrix();
            cmGlobal.printEvaluation();
        }
    }
}
