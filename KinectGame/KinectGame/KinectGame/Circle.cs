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
    class Circle
    {

        private Texture2D texture;

        private Vector2 position;
        private Color color;
        private int radius;

        public Circle(GraphicsDevice graphicsDevice, 
                      Vector2 inPosition, int inRadius, Color inColor)
        {
            this.position = inPosition;
            this.color = inColor;
            this.radius = inRadius;

            CreateCircle(graphicsDevice);
        }

        // function fetched from http://stackoverflow.com/questions/2983809/how-to-draw-circle-with-specific-color-in-xna
        public void CreateCircle(GraphicsDevice graphicsDevice)
        {
            int outerRadius = radius * 2 + 2;
            texture = new Texture2D(graphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];
            // Colo ur the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            //for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            for (int i = 0; i <= radius; i++)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                //for (int i = 0; i <= radius; i++)
                for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
                {
                    int x = (int)Math.Round(i + i * Math.Cos(angle));
                    int y = (int)Math.Round(i + i * Math.Sin(angle));

                    data[y * outerRadius + x + 1] = Color.White;
                }
            }

            texture.SetData(data);
        }

        public void DrawCircle(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, color);
        }

    }
}
