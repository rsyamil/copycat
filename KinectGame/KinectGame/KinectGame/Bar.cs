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
    class Bar
    {
        Texture2D barColor;
        Rectangle barArea;
        Rectangle colorArea;
        Point screenResolution;

        int BAR_LENGTH;
        //const int BAR_LENGTH = 1200;
        const int BAR_HEIGHT = 60;

        public Bar(Texture2D inBarColor, Point inScreenResolution,
                   Point position)
        {
            this.barColor = inBarColor;
            this.screenResolution = inScreenResolution;
            this.BAR_LENGTH = (inScreenResolution.X) -(4 * position.X);
            this.barArea = new Rectangle(position.X, position.Y, BAR_LENGTH, BAR_HEIGHT);
        }

        public void drawBar(SpriteBatch spriteBatch, double proportion) 
        {
            this.colorArea = new Rectangle(barArea.X, barArea.Y, Convert.ToInt32(Math.Round(barArea.Width * proportion)),
                                           barArea.Height);
                                           //Convert.ToInt32(Math.Round(barArea.Height * proportion)));
            spriteBatch.Draw(barColor, colorArea, Color.White);
        }
    }
}
