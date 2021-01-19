/********************************************************************
 * Name         : Button.cs
 * Author       : Jason Lee
 * Description  : This class implements a rectangle button with a 
 *                hover effect.
 * 
 * Notes        : To disable hover, pass NULL as hoverState
 * if possible
 *   4/8/2012 (Rebecca) - Added mouse & click support. Added object 
 *   animator for user feedback on hover.  Added colorgreen,
 *   colorwhite and isClicked methods for click buttons
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
    class Button
    {
        protected ContentManager content;

        protected Texture2D normalState;
        protected Texture2D hoverState;
        protected Texture2D currentState;

        protected Rectangle rectangle; 
        protected double timeElapsed;
        protected Point cursorPosition;

        protected bool switched;
        protected bool clicked;

        MouseState lastMouseState;
        MouseState currentMouseState;

        const int TRIGGER_INCREMENT = 3;

        Texture2D paw;
        Texture2D dot;
        Texture2D dot2;
        Texture2D dot3;
        ObjectAnimator objectAnimator;
        Stopwatch stopwatch;

        public Button()
        {
        }

        public Button(Texture2D normalBackground, Texture2D hoverBackground,
                      Rectangle inArea)
        {
            this.normalState = normalBackground;
            if (hoverBackground != null)
            {
                this.hoverState = hoverBackground;
            }
            this.rectangle = inArea;
            switched = false;
            
              
        }
        
        public Button(String normalBackground, String hoverBackground,
                      ContentManager inContent, Rectangle inArea)
        {
            content = inContent;
            paw = content.Load<Texture2D>("Level-0/paw");
            dot = content.Load<Texture2D>("Level-0/loadingcircle");
            dot2 = content.Load<Texture2D>("Level-0/mediumJoint");
            dot3 = content.Load<Texture2D>("Level-0/largeJoint");
            normalState = content.Load<Texture2D>(normalBackground);
            if (hoverBackground != null)
            {
                hoverState = content.Load<Texture2D>(hoverBackground);
            }
            else
            {
                hoverState = null;
            }
            this.currentState = normalState;
            this.rectangle = inArea;

            this.switched = false;

            
        }

        public void updateButton(Vector2 newPosition, double inTimeElapsed)
        // EFFECT: This function will update the hand cursor 
        //         and time for the button. Time elapsed
        // 		   should be the time elapsed provided by
        //         GameTime divided by 1000. This allows 
        // 		   the button change colors, for example.
        {
            cursorPosition = new Point((int)newPosition.X, (int)newPosition.Y);

            if (rectangle.Contains(cursorPosition) && hoverState != null)
            {  
                timeElapsed = timeElapsed + inTimeElapsed;
                currentState = hoverState;
                Debug.Write(inTimeElapsed+ " ");
                if (timeElapsed >= TRIGGER_INCREMENT && !switched)
                {
                       switched = true;
                }
               
                List<Texture2D> loading = new List<Texture2D>();
                loading.Add(dot);
                Vector2 loadpos = new Vector2(newPosition.X-20, newPosition.Y - 20);
                objectAnimator = new ObjectAnimator(loading, loadpos, AnimationOption.Random);
                this.objectAnimator.Update();
            }
            else
            {
                timeElapsed = 0;
                currentState = normalState;
            }

            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed && rectangle.Contains(cursorPosition) && hoverState != null)
            {
                clicked = true;
            }

        }

        public virtual void resetButton()
        // EFFECT: This will reset the button’s settings. 
        //         It should be called each time the button
        //         has been triggered.
        {
            switched = false;
            clicked = false;
            timeElapsed = 0;
            objectAnimator = null;
            
        }
        
        public virtual void drawButton(SpriteBatch spriteBatch)
        // EFFECT: This will draw the button on the screen.
        //         It should be called each time the XNA 
        //         framework calls Draw()
        {
            if (currentState == null)
            {
                currentState = normalState;
            }
            spriteBatch.Draw(this.currentState, rectangle, Color.White);

            
            if (this.objectAnimator != null && isButtonHovered())
            {
                this.objectAnimator.Draw(spriteBatch);
                
            }
         }

        public virtual bool isButtonTriggered()
        // EFFECT: If the button has been triggered (or 
        //         user has hovered over button for more 
        //         than 5 seconds), the function returns 
        //         TRUE. Otherwise, it returns FALSE.
        {
            return switched;
        }

        public virtual bool isButtonClicked()
        {
            return clicked;

        }

        public virtual bool isButtonHovered()
        {
            return rectangle.Contains(cursorPosition);
        }

        public void colorGreen()
        {
            normalState = content.Load<Texture2D>("Green");
            hoverState = content.Load<Texture2D>("Green");
        }

        public void colorWhite()
        {
            normalState = content.Load<Texture2D>("White");
            hoverState = content.Load<Texture2D>("White");

        }

    }
}
