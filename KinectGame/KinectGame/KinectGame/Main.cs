using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

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
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        DrawingClass draw;
        NavigationClass navigate;
        AccuracyClass accuracy;
        SkeletonTracker skeletonTracker;
        Timer timer5Seconds;
        Timer timer1Seconds;
        Timer timer100MilliSeconds;
        Stopwatch stopwatch;

        const int MAIN_PLAYER = 0;
        const int SECOND_PLAYER = 1;

        //elapsed time records total running time of the game
        //only goes from 0 to 1000 (1 second)
        double elapsedTime = 0;

        //starts with 30, then this will be decremented every 1 second
        int displayedTime = 30;

        bool coordPulled = false;

        int prevDisplayTime = 0;

        int poseCount = 0;

        const float BACKGROUND_SCALE = 1.649f;

        Avatar avatarStatus;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            draw = new DrawingClass(this.graphics, Content);
            draw.SetFrameSize();
            navigate = new NavigationClass();
            accuracy = new AccuracyClass(2);
            draw.SetAccuracyClass(accuracy);
            timer5Seconds = new Timer(5000);  /////add with timer
            timer1Seconds = new Timer(1000);
            timer100MilliSeconds = new Timer(100);
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

            this.graphics.PreferredBackBufferWidth = 1366;
            this.graphics.PreferredBackBufferHeight = 768;

            //this.graphics.PreferMultiSampling = true;

            //accuracy = new AccuracyClass(this.graphics.PreferredBackBufferHeight, this.graphics.PreferredBackBufferWidth);//
            navigate.SetStateKeyboard();
            timer100MilliSeconds.Start();//timer start at this right now, but should be modified to start after loading to level
            timer1Seconds.Start();
            timer5Seconds.Start();
            skeletonTracker = new SkeletonTracker
                (this.graphics.PreferredBackBufferHeight, this.graphics.PreferredBackBufferWidth);
            this.draw.skeletonTracker = skeletonTracker;

            navigate.SetStateKeyboard();

            if (stopwatch == null)
            {
                stopwatch = new Stopwatch();
                stopwatch.Start();
            }

            base.Initialize();
        }
        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            draw.LoadDrawingClass();
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
            accuracy = draw.GetAccuracyClass();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (draw.GetGameStatus())
            {
                this.Exit();
            }

            // Check for keyboard input, if exitGame, then exit Main
            // otherwise it will be handled by DrawingClass
            Command command = navigate.UpdateInput();
            if (command == Command.exitGame) this.Exit();
            else
            {
                draw.navigateLevels(command);
                accuracy.SetCurrentLevel(draw.getLevel());
            }

            //Skeleton skel = skeletonTracker.getSkeleton(MAIN_PLAYER);
            Skeleton[] skel = skeletonTracker.getPlayerSkeletons();
            MouseState ms = Mouse.GetState();
            /* float horScale = (float)graphics.GraphicsDevice.PresentationParameters.BackBufferWidth /
                (float)graphics.PreferredBackBufferWidth;
            float vertScale = (float)graphics.GraphicsDevice.PresentationParameters.BackBufferHeight /
                            (float)graphics.PreferredBackBufferHeight;

            Vector2 tempMouse = new Vector2(ms.X, ms.Y);*/
            Vector2 mouse = new Vector2(ms.X * BACKGROUND_SCALE, ms.Y * BACKGROUND_SCALE);
            draw.SetMousePosition(mouse);
            draw.SetCursorPosition(skeletonTracker.getHandCursor(MAIN_PLAYER, false));
            draw.SetKinectStatus(skeletonTracker.getKinectStatus());
            draw.SetVideo(this.graphics, skeletonTracker.getVideoWidth(), skeletonTracker.getVideoHeight());
            draw.SetData(skeletonTracker.getVideoColor());

            draw.SetClippedStatus(navigate.CheckClippedEdges(skel[0]));

            //100ms event implemented here
            if (/*(timer100MilliSeconds.CheckEventInvoke() &&*/skel[0] != null && accuracy.GetReady() == true)
            {
                accuracy.SetNormalizationDistance(skel[0]);
                accuracy.CountAccuracy(skel[0]);
                //accuracy.RecordExpetedVector(skel);//this would be turned on when we design new pose and need to record them.
            }

            //draw.SetDisplayedTime(this.displayedTime);//

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// ALL Drawing codes go here! 
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            TimeSpan ts = stopwatch.Elapsed;
            elapsedTime = ts.TotalSeconds;
            //elapsedTime = elapsedTime + gameTime.ElapsedGameTime.Milliseconds;
            draw.SetElapsedTime(elapsedTime);
            draw.SetDisplayedTime(displayedTime);

            Skeleton[] skels = skeletonTracker.getPlayerSkeletons();

            avatarStatus = CheckRunningTime(draw.getLevel());
            
            //5 second event implemented here
            if (((prevDisplayTime % 5) != 0) && (displayedTime % 5 == 0)/* && skels[MAIN_PLAYER] != null*/)// && accuracy.GetReady() == true)
            {
                if (skels[MAIN_PLAYER] != null)
                {
                    accuracy.SetNormalizationDistance(skels[MAIN_PLAYER]);
                }
                draw.SetTotalScore(accuracy.GetTotalScores());
                Debug.Print("Score: "+accuracy.GetTotalScores());
                //draw.LoadLevel(skeletonTracker.getSkeleton(MAIN_PLAYER), Avatar.Change); // this is what I did now, but syamil may modify it with the Avatar

                //draw.LoadLevel(skels, Avatar.Change); // this is what I did now, but syamil may modify it with the Avatar
            }
            //else
            
            draw.LoadLevel(skels, avatarStatus);// this is what I did now, but syamil may modify it with the Avatar

            //draw.LoadLevel(skeletonTracker.getSkeleton(MAIN_PLAYER), CheckRunningTime(draw.getLevel()));

            base.Draw(gameTime);
        }

        public Avatar CheckRunningTime(int currentLevel)
        {
            avatarStatus = Avatar.None;
            //if (elapsedTime >= 1000 && (currentLevel != 0 && currentLevel != 6))
            
            //1 second event implemented here
            //if (timer1Seconds.CheckEventInvoke())
            //{
            this.prevDisplayTime = displayedTime;
            this.displayedTime = Convert.ToInt32(Math.Round(25 - (elapsedTime-draw.getStartTime())));
            if (this.displayedTime < 0)
            {
                this.displayedTime = 25;
                elapsedTime = 0;
            } 
            else if ((prevDisplayTime % 5) != 0 && (displayedTime % 5 == 0))
            {
                avatarStatus = Avatar.Change;
            }
               
            //elapsedTime = 0;
            //}

            //UPDATE THIS WITH YOUR OWN PATH 
            /*String dir = "C:/Users/Becca/Desktop/eecs481/eecs481/eecs481/KinectGame/KinectGame/KinectGameContent/"
                         + "Level-" + currentLevel.ToString() + "/";

            string[] poseCoor = new string[] { dir + "3d_pose0.txt", 
                                               dir + "3d_pose1.txt",
                                               dir + "3d_pose2.txt",
                                               dir + "3d_pose3.txt",
                                               dir + "3d_pose4.txt" };
             
            //array of file names for observed poses? 
            String[] observedPose = new String[] { dir + "observed_pose0_1.txt",
                                                   dir + "observed_pose1_1.txt",
                                                   dir + "observed_pose2_1.txt",
                                                   dir + "observed_pose3_1.txt",
                                                   dir + "observed_pose4_1.txt" };*/

            if (currentLevel >= 1 && currentLevel <= 5)
            {
                if (displayedTime == 0)
                {
                    //accuracy.ResetTotalScore();
                    avatarStatus = Avatar.Done;
                }

                //if (currentLevel == 0 || currentLevel == 6)
                //{
                  //  this.displayedTime = 25;
                    //accuracy.ResetTotalScore();
                    //draw.SetDisplayedTime(this.displayedTime);//
                    //draw.SetTotalScore(accuracy.GetTotalScores());

                //}
            }
            return avatarStatus;
        }
    
    
    }
    
    
}

