using System;
using System.Collections.Generic;
using System.IO;

using System.Linq;
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
    /// <summary>
    /// Main for game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        DrawingClass draw;
        NavigationClass navigate;
        SkeletonTracker skeletonTracker;

        public int screenWidth = 864;
        public int screenHeight = 486;

        const int MAIN_PLAYER = 0;
        const int SECOND_PLAYER = 1;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            draw = new DrawingClass(this.graphics, Content);
            draw.SetFrameSize(screenWidth, screenHeight);
            navigate = new NavigationClass();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.  
        /// </summary>      
        protected override void Initialize()
        {
            // Removes the frame provided by Windows.
            IntPtr hWnd = this.Window.Handle;
            var control = System.Windows.Forms.Control.FromHandle(hWnd);
            var form = control.FindForm();
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            skeletonTracker = new SkeletonTracker(screenHeight, screenWidth);

            navigate.SetStateKeyboard();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            skeletonTracker.UnloadKinect();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (navigate.UpdateInput() == Command.exitGame) this.Exit();

            draw.SetCursorPosition(skeletonTracker.getHandCursor(MAIN_PLAYER, false));
            draw.SetKinectStatus(skeletonTracker.getKinectStatus());
            draw.SetVideo(this.graphics, skeletonTracker.getVideoWidth(), skeletonTracker.getVideoHeight());
            draw.SetData(skeletonTracker.getVideoColor());
            draw.SetClippedStatus(navigate.CheckClippedEdges(skeletonTracker.getSkeleton(MAIN_PLAYER)));

            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// ALL Drawing codes go here! 
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            int lvl = 1;       
            draw.LoadLevel(lvl);

            base.Draw(gameTime);
        }
    }
}
