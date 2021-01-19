/********************************************************************
 * Name         : PlayerRecord.cs
 * Author       : Rebecca Gebhard
 * Description  : This class stores player's level scores in a text file
 * 
 * Notes        : 
 * if possible
 * 
 * *****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace KinectGame
{
    class PlayerRecord
    {
        private string playerName;
        private DateTime startTime;
        private string fileName;
        private string pathString;

        public PlayerRecord(int playerNumber)
        {
            startTime = DateTime.Now;
            if (playerNumber == 0) playerName = "Felix";
            else if (playerNumber == 1) playerName = "Garfield";
            else playerName = "Spotty";

            //WHERE TO STORE?
            string folderName = @"C:\CopyCatScoreHistory";
            folderName = System.IO.Path.Combine(folderName, playerName);

            System.IO.Directory.CreateDirectory(folderName);
            fileName = "Score History.txt";
            pathString = System.IO.Path.Combine(folderName, fileName); 
          //  using (System.IO.FileStream fs = System.IO.File.Create(pathString))
            {
            }
         }

        public void recordScore(int level, double score, int targetsCaptured, int grade, double bonusScore)
        {
            int percentBonus = Convert.ToInt32(Math.Round(bonusScore*100));
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(pathString, true))
            {
                file.WriteLine("Level " + level + ": Score " + score + "; " + targetsCaptured +
                    " / 20 targets captured; "+percentBonus+"% tokens captured; \r\n\t Grade: "+(grade+1)+
                    " / 5  @ " + Convert.ToString(DateTime.Now));
            } 

        }


    }
}
