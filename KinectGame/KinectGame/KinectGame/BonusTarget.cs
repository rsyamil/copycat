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
    class BonusTarget
    {
        private Texture2D itemTexture;
        private Texture2D resultTexture;
        private Rectangle rectangleArea;
        //private Point position;
        private Point size;
        private Point screenResolution;
        private bool active;
        private bool caught;
        private Random randomGenerator;

        private int skeletonWidth;
        private int skeletonHeight;
        private int speedMultiplier;

        float shiftFactorX = 100;
        float shiftFactorY = 170;

        const int SPEED_INCREMENT = 50;

        public BonusTarget(Texture2D inItemTexture, Texture2D inResultTexture, Point inSize, Point inScreenResolution,
                           int inSpeedMultiplier)
        {
            this.itemTexture = inItemTexture;
            this.resultTexture = inResultTexture;
            this.size = inSize;
            this.screenResolution = inScreenResolution;
            this.skeletonWidth = this.screenResolution.X;
            this.skeletonHeight = this.screenResolution.Y;
            this.speedMultiplier = inSpeedMultiplier;
            //this.screenResolution.X = screenResolution.X * 2;
            
            this.randomGenerator = new Random(Environment.TickCount);
            randomGenerator.NextDouble();
            randomGenerator.NextDouble();
            randomGenerator.NextDouble();
            Point position = new Point(Convert.ToInt32(randomGenerator.NextDouble()*inScreenResolution.X), 0);
            this.rectangleArea = new Rectangle(position.X, position.Y, inSize.X, inSize.Y);
            this.active = true;
            this.caught = false;
        }

        public bool updateTarget(Skeleton skeleton)
        {
            // update position of rectangle - top to bottom straight line first 
            if (active)
            {
                rectangleArea.Y = rectangleArea.Y + (screenResolution.Y) / (speedMultiplier * SPEED_INCREMENT);
                if (rectangleArea.Y < 0 && !caught)
                {
                    active = false;
                    return active;
                }
                else if ((rectangleArea.Y > (screenResolution.Y*1.5 - size.Y)) && caught)
                {
                    rectangleArea.Y = Convert.ToInt32(screenResolution.Y*1.5 - size.Y);
                }

                if (skeleton != null)
                {
                    foreach (Joint joint in skeleton.Joints)
                    {
                        Vector2 currentPosition = new Vector2((((0.5f * joint.Position.X) + 0.5f) * (skeletonWidth/2)) + shiftFactorX,
                                                 (((-0.5f * joint.Position.Y) + 0.5f) * (skeletonHeight)) + shiftFactorY);
                        Point currentPoint = new Point(Convert.ToInt32(currentPosition.X), Convert.ToInt32(currentPosition.Y));
                        if (rectangleArea.Contains(currentPoint))
                        {
                            //active = false;
                            caught = true;
                            break;
                        }
                    }
                }
            }
            return active;
        }

        public void printTarget(SpriteBatch spriteBatch)
        {
            if (active)
            {
                if (caught)
                {
                    spriteBatch.Draw(resultTexture, rectangleArea, Color.White);
                }
                else
                {
                    spriteBatch.Draw(itemTexture, rectangleArea, Color.White);
                }
            }
        }

        public bool isCaught()
        {
            return caught;
        }

    }
}
