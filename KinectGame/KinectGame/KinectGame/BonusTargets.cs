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
using Microsoft.Kinect;

namespace KinectGame
{
    class BonusTargets
    {
        Texture2D[] targetTextures;
        Texture2D resultTexture;
        Point[] sizes;
        Point screenResolution;

        int bonusScore;

        List<BonusTarget> bonusTargets;
        double frequencyLevel;
        int speedMultiplier;

        public BonusTargets(Texture2D[] inTargetTextures, Texture2D inResultTexture, Point[] inSizes,
                            int inSpeedMultiplier, Point inScreenResolution, double inFrequencyLevel)
        {
            bootstrapMembers(inSizes, inScreenResolution, inFrequencyLevel, inSpeedMultiplier);
            targetTextures = inTargetTextures;
        }

        public BonusTargets(string[] inTargetTextureNames, Point[] inSizes,
                            Point inScreenResolution, double inFrequencyLevel,
                            int inSpeedMultiplier, ContentManager inContentManager)
        {
            bootstrapMembers(inSizes, inScreenResolution, inFrequencyLevel, inSpeedMultiplier);

            targetTextures = new Texture2D[inTargetTextureNames.Length];
            for (int i = 0; i < inTargetTextureNames.Length; i++)
            {
                targetTextures[i] = inContentManager.Load<Texture2D>(inTargetTextureNames[i]);
            }
            resultTexture = inContentManager.Load<Texture2D>("Level-2/Italian_Level3_1Up");
        }

        private void bootstrapMembers(Point[] inSizes,
                            Point inScreenResolution, double inFrequencyLevel, int inSpeedMultiplier)
        {
            bonusTargets = new List<BonusTarget>();
            sizes = inSizes;
            screenResolution = inScreenResolution;
            frequencyLevel = inFrequencyLevel;
            speedMultiplier = inSpeedMultiplier;
        }

        public void updateTargets(Skeleton skeleton)
        {
            for (int i = 0; i < bonusTargets.Count; i++)
            {
                if (!bonusTargets[i].updateTarget(skeleton))
                {
                    //bonusTargets.RemoveAt(i);
                    //i--;
                }
            }
            Random randomGenerator = new Random(Environment.TickCount);
            double spin = randomGenerator.NextDouble();
            if (spin < frequencyLevel)
            {
                int selectedTextureID = randomGenerator.Next() % targetTextures.Length;
                Texture2D selectedTexture = targetTextures[selectedTextureID];
                bonusTargets.Add(new BonusTarget(selectedTexture, resultTexture, sizes[selectedTextureID], 
                                                 screenResolution, speedMultiplier));
            }
        }

        public void printTargets(SpriteBatch spriteBatch)
        {
            foreach (BonusTarget bonusTarget in bonusTargets)
            {
                bonusTarget.printTarget(spriteBatch); 
            }
        }

        public int getBonusScore()
        {
            bonusScore = 0;
            foreach (BonusTarget bonusTarget in bonusTargets)
            {
                if (bonusTarget.isCaught())
                {
                    bonusScore = bonusScore + 1;
                }
            }
            return bonusScore;
        }

        public int getTotalTokens()
        {
            return bonusTargets.Count;
        }
        
    }
}
