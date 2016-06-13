using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningModule
{
    class ConfusionMatrix
    {
        private string[] labels;
        private int[,] cm;
        private int n;

        /**
         *
         * @param n number of labels
         */
        public ConfusionMatrix(int n)
        {
            this.n = n;
            labels = new string[n];
            cm = new int[n,n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    cm[i,j] = 0;
                }
            }
        }

        /**
         *
         * @param labels labels for confusion matrix
         * @param cm confusion matrix
         */
        public ConfusionMatrix(string[] labels, int[,] cm)
        {
            this.labels = labels;
            this.cm = cm;
        }

        /**
         *
         * @param labels labels
         */
        public void setLabel(string[] labels)
        {
            this.labels = labels;
        }

        /**
         *
         * @return labels
         */
        public string[] getLabel()
        {
            return this.labels;
        }

        /**
         *
         * @param actualLabel index of actual label
         * @param predictedLabel index of predicted label
         * @param value value
         */
        public void setCM(int actualLabel, int predictedLabel, int value)
        {
            this.cm[actualLabel, predictedLabel] = value;
        }

        public void incrementCM(int actualLabel, int predictedLabel)
        {
            this.cm[actualLabel, predictedLabel]++;
        }

        /**
         *
         * @param actualLabel index of actual label
         * @param predictedLabel index of predicted label
         * @return value
         */
        public int getCM(int actualLabel, int predictedLabel)
        {
            return this.cm[actualLabel, predictedLabel];
        }

        /**
         * Print the confusion matrix
         */
        public void printConfusionMatrix()
        {
            Console.WriteLine("=========CONFUSION MATRIX=========");
            for (int i = 0; i < n; i++)
            {
                Console.Write(labels[i] + "\t");
                for (int j = 0; j < n; j++)
                {
                    Console.Write(cm[i,j] + "\t");
                }
                Console.WriteLine();
            }
        }

        private int sumMatrix()
        {
            int sum = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    sum += cm[i, j];
                }
            }
            return sum;
        }

        private int sumColumn(int idx)
        {
            int sum = 0;
            for (int i = 0; i < n; i++)
            {
                sum += cm[i, idx];
            }
            return sum;
        }

        private int sumRow(int idx)
        {
            int sum = 0;
            for (int i = 0; i < n; i++)
            {
                sum += cm[idx, i];
            }
            return sum;
        }

        /**
         *
         * @return accuracy for all label
         */
        public double getAccuracy()
        {
            int trueLabel = 0;
            for (int i = 0; i < n; i++)
            {
                trueLabel += cm[i, i];
            }
            return (double)trueLabel / sumMatrix();
        }

        /**
         * Print Precision, Recall, and F1 for all label
         */
        public void printEvaluation()
        {
            Console.WriteLine("============CONFUSION MATRIX EVALUATION============");
            Console.WriteLine("Accuracy: " + getAccuracy());


            for (int i = 0; i < n; i++)
            {
                double precision, recall, f1;

                if (sumColumn(i) == 0)
                {
                    precision = double.NaN;
                }
                else {
                    precision = (double) cm[i,i] / sumColumn(i);
                }

                if (sumRow(i) == 0)
                {
                    recall = double.NaN;
                }
                else {
                    recall = (double) cm[i,i] / sumRow(i);
                }

                if (precision + recall == double.NaN)
                {
                    f1 = double.NaN;
                }
                else {
                    f1 = (2 * precision * recall) / (precision + recall);
                }

                Console.WriteLine("label: " + labels[i]);
                Console.WriteLine("prec: " + precision + " rec: " + recall + " f1: " + f1);
            }

        }

        /**
         * Combine the confusion matrix
         * @param cm array of confusion matrix
         * @return confusion matrix
         */
        public static ConfusionMatrix getCrossValidationCM(ConfusionMatrix[] cm)
        {
            int numLabel = cm[0].getLabel().Length;

            ConfusionMatrix cmCV = new ConfusionMatrix(numLabel);
            cmCV.setLabel(cm[0].getLabel());

            for (int i = 0; i < numLabel; i++)
            {
                for (int j = 0; j < numLabel; j++)
                {
                    int sum = 0;
                    for (int k = 0; k < cmCV.n; k++)
                    {
                        sum += cm[k].getCM(i, j);
                    }
                    cmCV.setCM(i, j, sum);
                }
            }
            return cmCV;
        }
    }
}
