using Emgu.CV;
using Emgu.CV.ML;
using Emgu.CV.Structure;
using System;
using System.IO;

namespace LearningModule
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            /*                          *
             *  Crop Image From Dataset *
             *                          */
            ImageCropper cropper = new ImageCropper();

            /* Read the annotated CSV and store the informations in the lists */
            //for (int i = 1; i <= cropper.getNumDayFolders(); ++i)
            //{
            //    Console.WriteLine("Opening day folder " + i);
            //    cropper.readLabeledCSV("../../../images/dayTrain/dayClip" + i + "/frameAnnotationsBULB.csv");
            //}
            //for (int i = 1; i <= cropper.getNightDayFolders(); ++i)
            //{
            //    Console.WriteLine("Opening night folder " + i);
            //    cropper.readLabeledCSV("../../../images/nightTrain/nightClip" + i + "/frameAnnotationsBULB.csv");
            //}

            ///* Train the data and write the model to xml file */
            //Console.WriteLine("Cropping images..");
            //cropper.cropTrafficLightVideos();
            //Console.WriteLine("Cropping image finished!");

            string[] classes = new string[] { "circle", "left_arrow", "right_arrow", "unknown" };
            HOGLearner learner = new HOGLearner();

            //Console.WriteLine("Extracting features...");
            //try
            //{
            //    learner.extractFeatures("../../data/", false, true);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}

            //Console.WriteLine("Loading dataset...");
            //Matrix<float> instances = (Matrix<float>)HOGLearner.Deserialize("../../data/nightInstances");
            //Matrix<int> labels = (Matrix<int>)HOGLearner.Deserialize("../../data/nightLabels");

            //Console.WriteLine("Dividing data into test and train");
            //int numTestInstances = (int)Math.Ceiling(0.3 * instances.Rows);
            //Console.WriteLine(numTestInstances);
            //Matrix<float> trainInstances = new Matrix<float>(instances.Rows - numTestInstances, instances.Cols);
            //Matrix<int> trainLabels = new Matrix<int>(labels.Rows - numTestInstances, 1);
            //Matrix<float> testInstances = new Matrix<float>(numTestInstances, instances.Cols);
            //Matrix<int> testLabels = new Matrix<int>(numTestInstances, 1);
            //for (int i = 0; i < numTestInstances; ++i)
            //{
            //    for (int j = 0; j < instances.Cols; ++j)
            //    {
            //        testInstances[i, j] = instances[i, j];
            //    }
            //    testLabels[i, 0] = labels[i, 0];
            //}
            //for (int i = numTestInstances; i < instances.Rows; ++i)
            //{
            //    for (int j = 0; j < instances.Cols; ++j)
            //    {
            //        trainInstances[i - numTestInstances, j] = instances[i, j];
            //    }
            //    trainLabels[i - numTestInstances, 0] = labels[i, 0];
            //}
            //Console.WriteLine("Number of train data: " + trainInstances.Rows);
            //Console.WriteLine("Number of test data: " + testInstances.Rows);
            //Console.WriteLine("Total number of data: " + instances.Rows);

            //HOGLearner.Serialize(trainInstances, "../../data/nightTrainInstances");
            //HOGLearner.Serialize(trainLabels, "../../data/nightTrainLabels");
            //HOGLearner.Serialize(testInstances, "../../data/nightTestInstances");
            //HOGLearner.Serialize(testLabels, "../../data/nightTestLabels");

            Matrix<float> trainInstances = (Matrix<float>)HOGLearner.Deserialize("../../data/nightTrainInstances");
            Matrix<int> trainLabels = (Matrix<int>)HOGLearner.Deserialize("../../data/nightTrainLabels");
            Matrix<float> testInstances = (Matrix<float>)HOGLearner.Deserialize("../../data/nightTestInstances");
            Matrix<int> testLabels = (Matrix<int>)HOGLearner.Deserialize("../../data/nightTestLabels");

            Matrix<float> annTrainLabels = new Matrix<float>(trainLabels.Rows, 4);
            for (int i = 0; i < annTrainLabels.Rows; ++i)
            {
                switch (trainLabels[i, 0])
                {
                    case 0: annTrainLabels[i, 0] = 1; annTrainLabels[i, 1] = -1; annTrainLabels[i, 2] = -1; annTrainLabels[i, 3] = -1; break;
                    case 1: annTrainLabels[i, 0] = -1; annTrainLabels[i, 1] = 1; annTrainLabels[i, 2] = -1; annTrainLabels[i, 3] = -1; break;
                    case 2: annTrainLabels[i, 0] = -1; annTrainLabels[i, 1] = -1; annTrainLabels[i, 2] = 1; annTrainLabels[i, 3] = -1; break;
                    case 3: annTrainLabels[i, 0] = -1; annTrainLabels[i, 1] = -1; annTrainLabels[i, 2] = -1; annTrainLabels[i, 3] = 1; break;
                }
            }
            Matrix<float> annTestLabels = new Matrix<float>(testLabels.Rows, 4);
            for (int i = 0; i < annTestLabels.Rows; ++i)
            {
                switch (testLabels[i, 0])
                {
                    case 0: annTestLabels[i, 0] = 1; annTestLabels[i, 1] = -1; annTestLabels[i, 2] = -1; annTestLabels[i, 3] = -1; break;
                    case 1: annTestLabels[i, 0] = -1; annTestLabels[i, 1] = 1; annTestLabels[i, 2] = -1; annTestLabels[i, 3] = -1; break;
                    case 2: annTestLabels[i, 0] = -1; annTestLabels[i, 1] = -1; annTestLabels[i, 2] = 1; annTestLabels[i, 3] = -1; break;
                    case 3: annTestLabels[i, 0] = -1; annTestLabels[i, 1] = -1; annTestLabels[i, 2] = -1; annTestLabels[i, 3] = 1; break;
                }
            }

            Console.WriteLine("Begin Learning..");
            ANN_MLP ann = null;
            try
            {
                ann = learner.trainANN(trainInstances, annTrainLabels);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Begin predicting...");
            try
            {
                learner.testSetEvaluationANN(ann, testInstances, annTestLabels, classes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            var a = Console.ReadLine();
        }
    }
}
