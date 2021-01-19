/********************************************************************
 * Name         : TargetCircles.cs
 * Author       : Jason Lee
 * Description  : Provides an abstraction for all target circles 
 *                supplied to a player for a single pose.
 * 
 * Notes        : n/a
 *  4/8/2012 (Rebecca) - Added size to ctr for accuracy threshold 
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

namespace KinectGame
{

	class TargetCircles
	{
        TargetCircle[] targetCircles;
        int numTargets;
        int bestScore;

        public TargetCircles()
        {
        }

		/*public TargetCircles(GraphicsDevice graphicsDevice, Vector2[] position)
		{
            targetCircles = new TargetCircle[position.Length];
            for (int target = 0; target < position.Length; target++)
            {
                targetCircles[target] = new TargetCircle(graphicsDevice, position[target]);
            }
		}*/

        public TargetCircles(Texture2D normalBackground, Texture2D hoverBackground,
                             Vector2[] position, int size)
        {
            targetCircles = new TargetCircle[position.Length];
            for (int target = 0; target < position.Length; target++)
            {
                targetCircles[target] = new TargetCircle(normalBackground, hoverBackground,
                                                         position[target], size);
            }
        }

        public void drawCircle(SpriteBatch spriteBatch)
        {
            foreach (TargetCircle targetCircle in targetCircles)
            {
                targetCircle.drawCircle(spriteBatch);
            }
        }

        public void updateCircle(Vector2 []position, double inTimeElapsed)
        {
            //foreach (TargetCircle targetCircle in targetCircles)
            int thisScore = 0;
            for (int i = 0; i < position.Length; i++)
            {
                targetCircles[i].updateCircle(position[i], inTimeElapsed);
                if (targetCircles[i].hoverCircle())
                {
                    thisScore++;
                }
            }
            if (thisScore > this.bestScore)
            {
                this.bestScore = thisScore;
            }
        }

        public int getBestScore()
        {
            return this.bestScore;
        }

	}

}