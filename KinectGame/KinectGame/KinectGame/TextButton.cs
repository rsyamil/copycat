/********************************************************************
 * Name         : TextButton.cs
 * Author       : Jason Lee
 * Description  : This class implements a text button with a 
 *                hover effect.
 * 
 * Notes        : To disable hover, pass NULL as hoverState
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

namespace KinectGame
{
    class TextButton : Button
    {
        private String buttonText;
        private Vector2 textOffset;
        private SpriteFont buttonFont;

        public TextButton(Texture2D normalBackground, Texture2D hoverBackground,
                          String inText, Rectangle inArea, Vector2 inTextOffset,
                          SpriteFont inButtonFont)
        {
            normalState = normalBackground;
            hoverState = hoverBackground;
            //buttonFont = content.Load<SpriteFont>("ButtonText");
            currentState = normalState;

            this.buttonFont = inButtonFont;
            rectangle = inArea;
            buttonText = inText;
            textOffset = inTextOffset;
        }

        public TextButton(String normalBackground, String hoverBackground,
               String inText, ContentManager inContent, Rectangle inArea,
               Vector2 inTextOffset)
        {
            content = inContent;
           
            normalState = content.Load<Texture2D>(normalBackground);
            if (hoverBackground != null)
            {
                hoverState = content.Load<Texture2D>(hoverBackground);
            }
            else
            {
                hoverState = null;
            }
            buttonFont = content.Load<SpriteFont>("ButtonText");
            currentState = normalState;

            rectangle = inArea;
            buttonText = inText;
            textOffset = inTextOffset;
        }

        public override void drawButton(SpriteBatch spriteBatch)
        // EFFECT: This will draw the button on the screen.
        //         It should be called each time the XNA 
        //         framework calls Draw()
        {
            spriteBatch.Draw(currentState, rectangle, Color.White);
            spriteBatch.DrawString(buttonFont, buttonText,
                new Vector2(rectangle.X + textOffset.X, rectangle.Y + textOffset.Y), Color.Black);
        }

        public void updateText(String inText)
        // EFFECT: Updates the text displayed inside a text
        //		   button.
        {
            buttonText = inText;
        }

    }
}
