/********************************************************************
 * Name         : DrawingClass.cs
 * Author       : Syamil Razak
 * Description  : This class handles all drawing methods pertaining to
 * user interface, graphics and animations. 
 * 
 * Notes        : Aspect Ratio 16:9. Keep background image to this ratio 
 * if possible
 * 
 * *****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
    public class DrawingClass : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphicsManager;
        SpriteBatch spriteBatch;
        ContentManager contentManager;

        SpriteFont font;
        string connectedStatus = "Status: Not connected";

        Texture2D cursor;
        Texture2D alert;
        Texture2D background;
        Texture2D avatar;
        Texture2D shadow;
        Texture2D kinectRGBVideo;

        Vector2 avatarPosition;
        Vector2 cursorPosition;

        Clipped clipped;
    
        public DrawingClass(GraphicsDeviceManager graphicsManager, ContentManager content)
        {         
            this.graphicsManager = graphicsManager;
            this.contentManager = content;
            this.contentManager.RootDirectory = "Content";
        }

        public void SetFrameSize(int width, int height)
        {
            this.graphicsManager.PreferredBackBufferWidth = width;
            this.graphicsManager.PreferredBackBufferHeight = height;            
        }

        public void MakeFullScreen()
        {
            // Make screen full screen, TODO - figure out scaling factor 
            if (!this.graphicsManager.IsFullScreen)
            {
                this.graphicsManager.ToggleFullScreen();
            }           
        }

        public void LoadLevel(int level)
        {
            this.graphicsManager.GraphicsDevice.Clear(Color.WhiteSmoke);
            spriteBatch = new SpriteBatch(this.graphicsManager.GraphicsDevice);

            spriteBatch.Begin();
            this.font = this.contentManager.Load<SpriteFont>("Score");
            this.cursor = this.contentManager.Load<Texture2D>("Level-0/pinkbird");
            this.alert = this.contentManager.Load<Texture2D>("Alert");

            switch (level)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        this.avatarPosition = new Vector2(0, 0);
                        this.background = this.contentManager.Load<Texture2D>("Level-1/Background-1");
                        this.avatar = this.contentManager.Load<Texture2D>("Level-1/James");
                        break;
                    }
                case 2:
                    {
                        break;
                    }
                case 3:
                    {
                        break;
                    }
                case 4:
                    {
                        break;
                    }
                case 5:
                    {
                        break;
                    }
                case 6:
                    {
                        break;
                    }
                case 7:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            spriteBatch.Draw(this.background, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(this.cursor, cursorPosition, Color.White);
            spriteBatch.Draw(this.avatar, avatarPosition, Color.White);
            
            spriteBatch.DrawString(font, "Coordinate : " + cursorPosition.ToString(), 
                new Vector2(250, 445), Color.White);
            spriteBatch.DrawString(font, this.connectedStatus, new Vector2(10, 445), Color.White);

            //spriteBatch.Draw(this.kinectRGBVideo, new Rectangle(0, 0,
            //   graphicsManager.PreferredBackBufferWidth,
            //   graphicsManager.PreferredBackBufferHeight), Color.White);

            RenderClippedEdges(this.clipped);
            spriteBatch.End();
        }

        public void RenderClippedEdges(Clipped clipped)
        {
            if (clipped == Clipped.top)
            {
                spriteBatch.Draw(this.alert, new Vector2(200, 10), Color.White);
            }
            if (clipped == Clipped.bottom)
            {
                spriteBatch.Draw(this.alert, new Vector2(200, 300) , Color.White);
            }
            if (clipped == Clipped.right)
            {
                spriteBatch.Draw(this.alert, new Vector2(400, 100), Color.White);
            }
            if (clipped == Clipped.left)
            {     
                spriteBatch.Draw(this.alert, new Vector2(10, 100), Color.White);
            }
        
        }

        public void SetVideo(GraphicsDeviceManager graphicsManager, int width, int height)
        {
            if ((width != 0) && (height != 0))
            {
                this.kinectRGBVideo = new Texture2D(this.graphicsManager.GraphicsDevice, width, height);
            }
        }

        public void SetData(Color[] color)
        {
            if (color != null)
            {
                this.kinectRGBVideo.SetData(color);
            }
        }

        public void SetCursorPosition(Vector2 cursorPosition) 
        {
            this.cursorPosition = cursorPosition;
        }

        public void SetKinectStatus(string connectedStatus)
        {
            this.connectedStatus = connectedStatus;
        }

        public void SetClippedStatus(Clipped clipped)
        {
            this.clipped = clipped;
        }       


    }
}
