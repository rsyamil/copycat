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

namespace KinectGame
{
    class TimeBar
    {
        Bar timeBar;
        Bar[] timeBars;
        int totalTime;
        Texture2D clockIcon;

        //Point clockPosition;
        Rectangle clockArea;

        const int CLOCK_DIM = 60;

        public TimeBar(Texture2D[] inTexture, Texture2D inClockIcon, Point inScreenResolution,
                       Point inPosition, int inTotalTime)
        {
            this.timeBars = new Bar[inTexture.Length];
            for (int i = 0; i < inTexture.Length; i++)
            {
                timeBars[i] = new Bar(inTexture[i], inScreenResolution, inPosition);
            }
            totalTime = inTotalTime;

            clockIcon = inClockIcon;
            clockArea = new Rectangle(inPosition.X - CLOCK_DIM, inPosition.Y, CLOCK_DIM, CLOCK_DIM);
        }

        public void drawBar(SpriteBatch spriteBatch, double timeElapsed)
        {
            double proportion = Math.Abs((totalTime-Math.Abs(timeElapsed))/totalTime);
            if (proportion >= 1) 
            {
                proportion = 0.9999; 
            } else if (proportion < 0) 
            { 
                proportion = 0; 
            }
            spriteBatch.Draw(clockIcon, clockArea, Color.White);
            int timeBarID = Convert.ToInt32(Math.Floor(proportion * timeBars.Length));
            timeBars[timeBarID].drawBar(spriteBatch, proportion);  
        }

    }
}
