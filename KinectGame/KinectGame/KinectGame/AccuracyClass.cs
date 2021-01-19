/********************************************************************
 * Name         : AccuracyClass.cs
 * Author       : Cliff Lee
 * Description  : This class handle the algorithm of pose comparison and derive the accuracy and scores.   
 * 
 * 4/8/2013 (Rebecca) - Added getDir so that sound can access current dir loc
 * 4/9/2013 (Rebecca) - Remove hard coded path directory
 * 
 * *****************************************************************/
using System;
using System.IO;
using Microsoft.Kinect;
using System.Diagnostics;

namespace KinectGame
{
    public class AccuracyClass
    {
        private double threshold;
        private double normalizationDistance;
        private double maxScores;
        private double accuracy;
        private double totalScores;
        private double scores;
        private int currentLevel;
        private int currentPoseIndex;
        private int recordedPoseIndex;
        private String dir;
        private String[] expectedPoseTextfile;
        private String recordedPoseTextfile;
        private Boolean readyToCompare;

        private double[] possibleThreshold = { 0.4, 0.7, 1.0 }; 

        public AccuracyClass(int thresholdIndex)
        {
            SetThreshold(thresholdIndex);
            totalScores = 0;
            currentLevel = -1;
            currentPoseIndex = 0;
            maxScores = 0;
            readyToCompare = false;
            recordedPoseIndex = 0; //
        }

        public void SetNormalizationDistance(Skeleton skel)
        {
            //scale version of normalization
            //set normalizationDistance
            double rx, ry, rz;
            double lx, ly, lz;
            rx = skel.Joints[JointType.ShoulderRight].Position.X;
            ry = skel.Joints[JointType.ShoulderRight].Position.Y;
            rz = skel.Joints[JointType.ShoulderRight].Position.Z;
            lx = skel.Joints[JointType.ShoulderLeft].Position.X;
            ly = skel.Joints[JointType.ShoulderLeft].Position.Y;
            lz = skel.Joints[JointType.ShoulderLeft].Position.Z;
            normalizationDistance = Math.Sqrt(Math.Pow((rx - lx), 2) + Math.Pow((ry - ly), 2) + Math.Pow((rz - lz), 2));
            return;
        }

        public void SetThreshold(int thresholdIndex)
        {
            this.threshold = possibleThreshold[thresholdIndex-1];
        }

        public double GetScores()  //will be called every 5 seconds
        {
            return this.scores;
        }

        public double GetTotalScores() //will be called every 5 seconds
        {
            scores = maxScores;
            //totalScores = totalScores + scores;
            totalScores += scores;
            scores = 0;
            maxScores = 0;
            if (currentPoseIndex < 4)
                currentPoseIndex++;
            else
                currentPoseIndex = 0;
            return totalScores;
        }

        public void ResetTotalScore()
        {
            maxScores = 0;
            currentPoseIndex = 0;
            scores = 0;
            totalScores = 0;
            readyToCompare = false;
        }

        public double GetAccuracy()
        {
            return this.accuracy;
        }

        private void CountScores(double max) //will be called every 100 ms. by CountAccuracy method
        {
            //use threshold to limit the worst performance, for example, the geometric distance between two vectors should not larger than the length of normalization vector
            //according to the geometric distance, compute the scores
            scores = 0;
            if (max < threshold)
            {
                scores = ((1.5 * threshold - max) * 100 / threshold);
            }
            //scores = Math.Abs(((threshold - distance) * 100) / threshold);
            //totalScores = totalScores + scores;
            if (scores > this.maxScores)
            {
                if (scores > 100)
                    scores = 100;
                maxScores = scores;
            }
        }

        public void CountAccuracy(Skeleton skel) //will be called every 100 ms.
        {
            //get expectedVector from text file
            String textFile = expectedPoseTextfile[currentPoseIndex];
            double max = 0;
               StreamReader sr = new StreamReader(@textFile);
            double[,] expectedVector = new double[skel.Joints.Count, 3];
            for (int i = 0; i < skel.Joints.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    String line = sr.ReadLine();
                    expectedVector[i, j] = Convert.ToDouble(line);
                }
            }
            sr.Close();
            //normalize observedVectors
            double[,] observedVector = new double[skel.Joints.Count, 3];
            for (int i = 0; i < skel.Joints.Count; i++) //subtract with spine joint to get observedVectors
            {
                observedVector[i, 0] = (skel.Joints[JointType.HipCenter + i].Position.X - skel.Joints[JointType.Spine].Position.X) / normalizationDistance;
                observedVector[i, 1] = (skel.Joints[JointType.HipCenter + i].Position.Y - skel.Joints[JointType.Spine].Position.Y) / normalizationDistance;
                observedVector[i, 2] = (skel.Joints[JointType.HipCenter + i].Position.Z - skel.Joints[JointType.Spine].Position.Z) / normalizationDistance;
            }
            //vector subtraction from user’s to the expected one
            double[,] differenceVector = new double[skel.Joints.Count, 3];
            for (int i = 0; i < skel.Joints.Count; i++)
            {
                differenceVector[i, 0] = observedVector[i, 0] - expectedVector[i, 0];
                differenceVector[i, 1] = observedVector[i, 1] - expectedVector[i, 1];
                differenceVector[i, 2] = observedVector[i, 2] - expectedVector[i, 2];
            }
            double distance = 0;
            for (int i = 0; i < skel.Joints.Count; i++)
            {
                double tmp = Math.Sqrt(Math.Pow(differenceVector[i, 0], 2) + Math.Pow(differenceVector[i, 1], 2) + Math.Pow(differenceVector[i, 2], 2)) /
                             Math.Sqrt(Math.Pow(expectedVector[i, 0], 2) + Math.Pow(expectedVector[i, 1], 2) + Math.Pow(expectedVector[i, 2], 2));
                if (tmp > max)
                    max = tmp;
                distance = distance + tmp;
            }
            CountScores(max);
            //accuracy = max;
            //accuracy = scores / 100; //need to consider about what's the meaning...
        }

        public void RecordExpetedVector(Skeleton skel)
        {
            //
            String textFile = recordedPoseTextfile + Convert.ToString(recordedPoseIndex) + ".txt";
            double expectedNormalizationDistance;
            double rx, ry, rz;
            double lx, ly, lz;
            rx = skel.Joints[JointType.ShoulderRight].Position.X;
            ry = skel.Joints[JointType.ShoulderRight].Position.Y;
            rz = skel.Joints[JointType.ShoulderRight].Position.Z;
            lx = skel.Joints[JointType.ShoulderLeft].Position.X;
            ly = skel.Joints[JointType.ShoulderLeft].Position.Y;
            lz = skel.Joints[JointType.ShoulderLeft].Position.Z;
            expectedNormalizationDistance = Math.Sqrt(Math.Pow((rx - lx), 2) + Math.Pow((ry - ly), 2) + Math.Pow((rz - lz), 2));

            double[,] expectedVector = new double[skel.Joints.Count, 3];
            for (int i = 0; i < skel.Joints.Count; i++) //subtract with spine joint to get observedVectors
            {
                expectedVector[i, 0] = (skel.Joints[JointType.HipCenter + i].Position.X - skel.Joints[JointType.Spine].Position.X) / expectedNormalizationDistance;
                expectedVector[i, 1] = (skel.Joints[JointType.HipCenter + i].Position.Y - skel.Joints[JointType.Spine].Position.Y) / expectedNormalizationDistance;
                expectedVector[i, 2] = (skel.Joints[JointType.HipCenter + i].Position.Z - skel.Joints[JointType.Spine].Position.Z) / expectedNormalizationDistance;
            }
            StreamWriter sw = new StreamWriter(@textFile);
            for (int i = 0; i < skel.Joints.Count; i++)
            {
                sw.WriteLine(expectedVector[i, 0]);
                sw.WriteLine(expectedVector[i, 1]);
                sw.WriteLine(expectedVector[i, 2]);
            }
            sw.Close();
            recordedPoseIndex++;
        }

        public double[, ,] ReturnExpectedCoordinate(String fileName)
        {
            double[, ,] expectedCoordinate = new double[5, 20, 2];
            StreamReader sr = new StreamReader(@dir+fileName);
            for (int i = 0; i < 5; i++)
            {
                sr.ReadLine();
                for (int j = 0; j < 20; j++)
                {
                    String s = sr.ReadLine();
                    expectedCoordinate[i, j, 0] = 130 * Convert.ToDouble(s);
                    s = sr.ReadLine();
                    expectedCoordinate[i, j, 1] = -90 * Convert.ToDouble(s);
                    sr.ReadLine();
                }
            }
            sr.Close();
            return expectedCoordinate;
        }

        public void SetCurrentLevel(int level)
        {
            if (level != currentLevel && level >= 1 && level <= 5)
            {
                this.currentLevel = level;

                //NO NEED TO UPDATE PATH DIR ANYMORE :)

                string curdir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                curdir = curdir.Replace("\\", "/");
                //int end = curdir.IndexOf("KinectGame/bin/x86");
                //curdir = curdir.Substring(0, end);
                curdir = curdir + "/LevelData/";
                         
                this.dir = curdir + "Level-" + this.currentLevel.ToString() + "/";
                this.expectedPoseTextfile = new String[] { dir + "3d_pose0.txt",  
                                                           dir + "3d_pose1.txt",
                                                           dir + "3d_pose2.txt", 
                                                           dir + "3d_pose3.txt",
                                                           dir + "3d_pose4.txt" };

                this.recordedPoseTextfile = dir + "recordedPose";
                readyToCompare = true;
            }
        }

        public Boolean GetReady()
        {
            return readyToCompare;
        }

        public string getDir()
        {
            return this.dir;
        }
    }
}
