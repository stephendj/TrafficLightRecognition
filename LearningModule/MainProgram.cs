using System;

namespace LearningModule2
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            ///*                          *
            // *  Crop Image From Dataset *
            // *                          */
            //ImageCropper cropper = new ImageCropper();

            ///* Read the annotated CSV and store the informations in the lists */
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
            //cropper.cropImagesFromDataset();
            //Console.WriteLine("Cropping image finished!");

            HOGLearner learner = new HOGLearner();

            Console.WriteLine("Extracting features...");
            learner.extractFeatures();
            Console.WriteLine("Training SVM...");
            learner.train("../../../data/model.xml");
            Console.WriteLine("Training Finished!");

            var a = Console.ReadLine();
        }
    }
}
