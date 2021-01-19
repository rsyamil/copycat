/********************************************************************
 * Name         : TargetCircle.cs
 * Author       : Jason Lee
 * Description  : This class implements a target "circle," serving as 
 *                an abstraction for the Button class.
 * 
 * Notes        : n/a
 * 4/8/2012 (Rebecca) - Added size to ctr for accuracy threshold 
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

	class TargetCircle
	{
        private Button targetButton;
        private Button hoverButton;
        private Button regularButton;
        private Rectangle targetArea;

        private Circle hoveredCircle;
        private Circle normalCircle;
        private Circle displayCircle;

        private Vector2 position;
        private Vector2 center;

        private double hoverTime;
        private const int TARGET_DIM = 24;
        private int[] targetOffsets = { 36, 48, 60 }; 
        private const int TRIGGER_TIME = 5;

        public TargetCircle()
        {
        }

		/*public TargetCircle(GraphicsDevice graphicsDevice, Vector2 position)
		{
            this.targetArea = new Rectangle(Convert.ToInt32(position.X), Convert.ToInt32(position.Y), 
                                            TARGET_DIM, TARGET_DIM);
            this.targetButton = new Button("Yellow", "Green", inContent, targetArea);
            this.hoveredCircle = new Circle(graphicsDevice, position, TARGET_DIM, Color.Green);

            this.normalCircle = new Circle(graphicsDevice, position, TARGET_DIM, Color.Yellow);
             
            center = new Vector2(position.X + (TARGET_DIM / 2), position.Y + (TARGET_DIM / 2));
            displayCircle = normalCircle;
        }*/

        public TargetCircle(Texture2D backgroundBackground, Texture2D hoverBackground,
                            Vector2 position, int size)
        {
            this.targetArea = new Rectangle(Convert.ToInt32(position.X-targetOffsets[size-1]), Convert.ToInt32(position.Y-targetOffsets[size-1]),
                                            TARGET_DIM*(size+2), TARGET_DIM*(size+2));
            //this.targetButton = new Button(backgroundBackground, hoverBackground, targetArea);
            this.regularButton = new Button(backgroundBackground, null, targetArea);
            targetButton = regularButton;
            this.hoverButton = new Button(hoverBackground, null, targetArea);
        } 

        public void drawCircle(SpriteBatch spriteBatch)
        {
            
            if (hoverCircle())
            {
                targetButton = hoverButton;
            }
            else
            {
                targetButton = regularButton;
            }
            targetButton.drawButton(spriteBatch);
            //displayCircle.DrawCircle(spriteBatch);
        }

        public bool hoverCircle()
        {
            /*double[] differenceVector = new double[2];
            differenceVector[0] = Math.Abs(jointPosition.X - center.X);
            differenceVector[1] = Math.Abs(jointPosition.Y - center.Y);
            double distance = Math.Sqrt(Math.Pow(differenceVector[0], 2) + Math.Pow(differenceVector[1], 2));
            
            return (distance <= TARGET_DIM);*/
            return targetArea.Contains(new Point((int)Math.Round(position.X),(int)Math.Round(position.Y))); 
        }

        public void updateCircle(Vector2 jointPosition, double inTimeElapsed)
        {
            this.position = jointPosition; 
            //targetButton.updateButton(position, inTimeElapsed);
            /*if (hoverCircle(jointPosition))
            {
                hoverTime = hoverTime + inTimeElapsed;
                if (displayCircle.Equals(normalCircle) && hoverTime < TRIGGER_TIME)
                {
                    displayCircle = this.hoveredCircle;
                }
            }
            else
            {
                hoverTime = 0;
                displayCircle = normalCircle;
            }*/
        }

	}

}