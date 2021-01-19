using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;

namespace KinectGame
{
    class Level
    {
        SpriteFont hudFont;
        Texture2D background;
        Texture2D[] posePictures;
        Texture2D[] timeBarColors;
        Texture2D clockIcon;

        TargetCircles[] targetCircles;
        Dictionary<int, JointType> targetJoints;
        BonusTargets bonusTargets;
        TimeBar timeBar;

        string[] TIMEBAR_COLOR_NAMES = { "Red", "Yellow", "DarkGreen"};

        int poseID;
        int skeletonWidth;
        int skeletonHeight;
        int speedMultiplier;

        int currentScore;
        double bonusScore;

        const int POSE_INTERVAL = 5;
        const int RESOLUTION_FACTOR = 1000;

        double timeElapsed;
        double prevTimeElapsed;

        float backgroundScale = 1.649f;
        float avatarScale = 1.8f;
        float shiftFactorX = 100;
        float shiftFactorY = 170;

        bool isGameLevel;

        Vector2 avatarPosition = new Vector2(900, 370);
        Vector2 timerPosition = new Vector2(220, 560);
        Vector2 scorePosition = new Vector2(1000, 560);
        Point timeBarPosition = new Point(75, 50);

        public Level(string inBackground, string[] inPosePictures, 
                     TargetCircles[] inTargetCircles, Point inScreenResolution, SpriteFont inHudFont,
                     ContentManager inContentManager)
        {
            bootstrapLevel(inBackground, inPosePictures, inHudFont, inTargetCircles, inScreenResolution, inContentManager);
            bonusTargets = null;
        }

        public Level(string inBackground, string[] inPosePictures,
                     TargetCircles[] inTargetCircles, SpriteFont inHudFont, 
                     ContentManager inContentManager,
                     String[] inTargetTextureNames, Point[] inSizes,
                     Point inScreenResolution, double inFrequencyLevel, int inSpeedMultiplier)
        {
            bootstrapLevel(inBackground, inPosePictures, inHudFont, inTargetCircles, inScreenResolution, inContentManager);
            bonusTargets = new BonusTargets(inTargetTextureNames, inSizes, inScreenResolution, inFrequencyLevel, inSpeedMultiplier,
                                            inContentManager);
        }

        private void bootstrapLevel(string inBackground, string[] inPosePictures, SpriteFont inHudFont,
                                    TargetCircles[] inTargetCircles, Point inScreenResolution, ContentManager inContentManager)
        {
            this.hudFont = inHudFont;
            this.background = inContentManager.Load<Texture2D>(inBackground);
            this.posePictures = new Texture2D[inPosePictures.Length];
            this.skeletonWidth = inScreenResolution.X;
            this.skeletonHeight = inScreenResolution.Y;
            this.timeElapsed = 0;
            for (int i = 0; i < inPosePictures.Length; i++)
            {
                posePictures[i] = inContentManager.Load<Texture2D>(inPosePictures[i]);
            }

            this.timeBarColors = new Texture2D[TIMEBAR_COLOR_NAMES.Length];
            for (int i = 0; i < timeBarColors.Length; i++)
            {
                timeBarColors[i] = inContentManager.Load<Texture2D>(TIMEBAR_COLOR_NAMES[i]);
            }
            this.clockIcon = inContentManager.Load<Texture2D>("hourglass");
            this.timeBar = new TimeBar(timeBarColors, clockIcon, inScreenResolution, timeBarPosition, POSE_INTERVAL);

            targetCircles = inTargetCircles;
            poseID = 0;

            targetJoints = new Dictionary<int, JointType>();
            targetJoints.Add(0, JointType.HandLeft);
            targetJoints.Add(1, JointType.HandRight);
            targetJoints.Add(2, JointType.FootLeft);
            targetJoints.Add(3, JointType.FootRight);

            this.isGameLevel = (targetCircles != null && targetCircles.Length > 0);
        }

        private void updateObjects(Skeleton skele, double inTimeElapsed)
        {
            timeElapsed = inTimeElapsed;

            Vector2[] currentPosition = new Vector2[targetJoints.Count()];
            for (int i = 0; i < targetJoints.Count(); i++)
            {
                if (skele != null)
                {
                    currentPosition[i] = new Vector2((((0.5f * skele.Joints[targetJoints[i]].Position.X) + 0.5f) * (skeletonWidth / 2)) + shiftFactorX,
                                                 (((-0.5f * skele.Joints[targetJoints[i]].Position.Y) + 0.5f) * (skeletonHeight)) + shiftFactorY);
                }
                else
                {
                    currentPosition[i] = new Vector2(0, 0);
                }
            }
            if (isGameLevel)
            {
                targetCircles[poseID % targetCircles.Length].updateCircle(currentPosition, inTimeElapsed);
                currentScore = 0;
                for (int i = 0; i < targetCircles.Length; i++)
                {
                    currentScore = currentScore + targetCircles[i].getBestScore();
                }
            }
            if (bonusTargets != null)
            {
                bonusTargets.updateTargets(skele);
            }
        }

        public void updateLevel(Skeleton skele, double inTimeElapsed, Avatar avatarStat)
        {
            //timeElapsed = inTimeElapsed;
            //poseID = Convert.ToInt32(Math.Floor((30-inTimeElapsed)/POSE_INTERVAL));
            //poseID = poseID % targetCircles.Length;

            if (Convert.ToInt32(Math.Floor(inTimeElapsed)) % 5 == 0 && 
                Convert.ToInt32(Math.Floor(prevTimeElapsed)) % 5 != 0) // avatarStat == Avatar.Change)
            {
                poseID = poseID + 1;
                //poseID = poseID % targetCircles.Length;
            }
            updateObjects(skele, inTimeElapsed);
            prevTimeElapsed = inTimeElapsed;
        }

        public void updateLevel(Skeleton[] skeletons, double inTimeElapsed, Avatar avatarStat)
        {
            //timeElapsed = inTimeElapsed;
            //poseID = Convert.ToInt32(Math.Floor((30 - inTimeElapsed) / POSE_INTERVAL));
            //poseID = poseID % targetCircles.Length;
            if (avatarStat == Avatar.Change && isGameLevel)
            {
                poseID = poseID + 1;
                //poseID = poseID % targetCircles.Length;
            }
            foreach (Skeleton skele in skeletons)
            {
                updateObjects(skele, inTimeElapsed);
            }
        }

        public void drawLevel(SpriteBatch spriteBatch, double currentTime, double currentScore)
        {
            spriteBatch.Draw(this.background, Vector2.Zero, null, Color.White, 0f, Vector2.Zero,
                backgroundScale, SpriteEffects.None, 0f);
            if (isGameLevel)
            {
                spriteBatch.Draw(this.posePictures[poseID % targetCircles.Length], avatarPosition, null, Color.White, 0f, Vector2.Zero,
                    avatarScale, SpriteEffects.None, 0f);
            }

            timeElapsed = currentTime; 
            double decimalDisplayTime = timeElapsed - (poseID * POSE_INTERVAL);
            Debug.WriteLine("Pose: " + poseID + " currentTime: " + currentTime +
                            " timeElapsed :"+timeElapsed+" decimalDisplayTime: " + decimalDisplayTime);

            if (isGameLevel)
            {
                timeBar.drawBar(spriteBatch, decimalDisplayTime);
                targetCircles[poseID % targetCircles.Length].drawCircle(spriteBatch);
            }

            if (bonusTargets != null)
            {
                bonusTargets.printTargets(spriteBatch);
            }
            
        }

        public int getCurrentScore()
        {
            return currentScore;
        }

        public double getBonusScore()
        {
            if (bonusTargets != null)
            {
                bonusScore = (double)bonusTargets.getBonusScore() / (double)bonusTargets.getTotalTokens();
            }
            return bonusScore;
        }

    }
}
