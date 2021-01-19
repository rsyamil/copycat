/********************************************************************
 * Name         : DrawingClass.cs
 * Author       : Syamil Razak
 * Description  : This class handles all drawing methods pertaining to
 * user interface, graphics and animations. 
 * 
 * Notes        : Aspect Ratio 16:9. Keep background image to this ratio 
 * if possible
 * 2/21/2012 (Rebecca) - Added shadow, skeleton draw method. 
 * 2/23/2012 (Syamil) - Added scaling method to fit screen of all sizes
 * 3/10/2012 (Rebecca) - Added sound fix, added settings and all menu 
 * pages
 * 
 * *****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Media;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace KinectGame
{
    /// <summary>
    /// Avatar enum for Drawing method that will draw the Avatars
    /// Avatar.Change   : change avatar
    /// Avatar.Done     : level has ended
    /// Avatar.None     : do nothing, don't change yet
    /// </summary>
    public enum Avatar { Change, Done, None };

    public class DrawingClass : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphicsManager;
        //Resolution resolution; 
        SpriteBatch spriteBatch;
        ContentManager contentManager;

        SpriteFont font;


        SpriteFont hudFont;
        SpriteFont buttonFont;

        string connectedStatus = "Status: Not connected";
        double elapsedTime;
        double prevElapsedTime;
        double startElapsedTime;
        double totalScore;
        int displayTime;
        int level = 0;

        int loadingPageIndex = -1;

        // score page 
        String[] scoreGradeStr = { null,
                                   "Level-6/Score-Page-2star",
                                   "Level-6/Score-Page-3star",
                                   "Level-6/Score-Page-4star",
                                   "Level-6/Score-Page-5star" };

        Texture2D[] scoreGrades;
        int scoreGradeIndex;
        int targetsCaptured;
        double bonusScore;

        int prevLevel = 0;

        bool isMultiplayer;

        Texture2D jointTexture;
        Texture2D boneTexture;
        private Vector2 jointOrigin;
        private Vector2 boneOrigin;
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public SkeletonTracker skeletonTracker;
        int width_d; 
        int height_d;

        Texture2D normalTarget;
        Texture2D hoverTarget;
        Texture2D normalTarget_small; //accuracy threshold
        Texture2D hoverTarget_small;
        Texture2D normalTarget_large;
        Texture2D hoverTarget_large;


        Texture2D cursor;
        Texture2D alert;
        Texture2D background;
        Texture2D avatar;
        Texture2D kinectRGBVideo;
        Texture2D splashScreen;
        Texture2D settingsScreen;

        Texture2D greenBackground;
        Texture2D yellowBackground;
        Level currentLevel;

        List<Texture2D> loadingPage;

        // Buttons for navigation

        Rectangle playArea;
        Button playButton;

        /**************************/
        Rectangle quitArea;
        Button quitButton;

        Rectangle quitArea1;
        Button quitButton1;

        Video video;
        VideoPlayer player;
        Texture2D videoTexture;

        /**************************/

        List<Texture2D> peekyCats;// Holds SplashScreen animation
        
        //level screen
        Texture2D levelScreen;
        Rectangle level1Area;
        Button level1Button;
        Rectangle level2Area;
        Button level2Button;
        Rectangle level3Area;
        Button level3Button;
        Rectangle level4Area;
        Button level4Button;
        Rectangle level5Area;
        Button level5Button; 

        //player screen
        Texture2D playerScreen;
        Rectangle felixArea;
        Button felixButton;
        Rectangle garfieldArea;
        Button garfieldButton;
        Rectangle spottyArea;
        Button spottyButton;
        Rectangle howToArea;
        Button howToButton;
        Rectangle infoArea;
        Button infoButton;
        Rectangle settingsArea;
        Button settingsButton;
        PlayerRecord playerRecord;

        //tutorial 
        VideoPlayer videoPlayer;
        Video howTo;
        int prevLevelSong = 0;

        //score screen
        Texture2D scoreScreen;
        SoundEffect applause;
        //SoundPlayer applause;
        SoundPlayer menuSong;
        Texture2D stars2;
        Texture2D stars3;
        Texture2D stars4;
        Texture2D stars5;
        Rectangle okButtonArea;
        Button okButton;

        Rectangle replayArea;
        Button replayButton;
                
        //info screen
        Texture2D infoScreen;

        //buttons for settings
        Rectangle accuracyEasyArea;
        Button accuracyEasy;
        Rectangle accuracyMediumArea;
        Button accuracyMedium;
        Rectangle accuracyHardArea;
        Button accuracyHard;
        Rectangle bonusOnArea;
        Button bonusItemsOn;
        Rectangle bonusOffArea;
        Button bonusItemsOff;
        Rectangle soundOnArea;
        Button soundOn;
        Rectangle soundOffArea;
        Button soundOff;
        Rectangle backButtonArea;
        Button backButton;
        Rectangle playerCnt1Area;
        Button playerCnt1;
        Rectangle playerCnt2Area;
        Button playerCnt2;

        bool bonusItems;
        bool sound;
        int accuracyThreshold;
        int numPlayers;
        int playerSelected;
        string catColor;
        string[] catColors = { "_P", "_O", "_G" }; 

        //Rectangle speechArea;
        //TextButton speechBox;

        int avatarCount = 0;
        bool changedAlready = true;

        Vector2 cursorPositionLeft;
        Vector2 cursorPositionRight;
        Vector2 mousePosition;

        int skeletonWidth;
        int skeletonHeight;

        public int screenWidth;
        public int screenHeight;

        AccuracyClass accuracyClass;
        Clipped clipped;
        ObjectAnimator animatorSplashScreen;

        TargetCircles[] poses;
        Dictionary<int, JointType> targetJoints;

        bool loadedLevel;

        bool quitGame = false;
        bool playingAlready = false;
        double lastPlayed = -34;
        Vector2 pawTracker;
        //Vector2 pawTrackerLeft;

        // Scale for 1366x768
        float backgroundScale = 1.649f;
        float avatarScale = 1.8f;
        Vector2 statusPosition = new Vector2(10, 700);
        Vector2 coordinatePosition = new Vector2(300, 700);
        Vector2 avatarPosition = new Vector2(1000, 370);
        Vector2 timerPosition = new Vector2(220, 560);
        Vector2 scorePosition = new Vector2(1000, 560);
        float shiftFactorX = 100;
        float shiftFactorY = 170;
        Vector2 topEdge = new Vector2(650, 10);
        Vector2 bottomEdge = new Vector2(650, 700);
        Vector2 rightEdge = new Vector2(1300, 350);
        Vector2 leftEdge = new Vector2(10, 350);

        Timer timer500Millisecond = new Timer(500); //for loadingPage
        bool loadingPageCheck;                           // for loadingPage

        public DrawingClass(GraphicsDeviceManager graphicsManager, ContentManager content)
        {
            this.videoPlayer = new VideoPlayer();

            this.graphicsManager = graphicsManager;
            this.contentManager = content;
            this.contentManager.RootDirectory = "Content";
            this.elapsedTime = 0;
            this.prevElapsedTime = 0;
            this.loadedLevel = false;

            this.currentLevel = null;

            targetJoints = new Dictionary<int, JointType>();
            targetJoints.Add(0, JointType.HandLeft);
            targetJoints.Add(1, JointType.HandRight);
            targetJoints.Add(2, JointType.FootLeft);
            targetJoints.Add(3, JointType.FootRight);

            this.accuracyThreshold = 2;
            this.accuracyClass = new AccuracyClass(accuracyThreshold);

            this.peekyCats = new List<Texture2D>();

            this.scoreGrades = new Texture2D[scoreGradeStr.Length];

            this.applause = this.contentManager.Load<SoundEffect>("applause-4");

            //default settings
            this.sound = true;
            this.bonusItems = true;
            
            this.numPlayers = 1;
            this.playerSelected = 0;
            this.catColor = catColors[playerSelected];
            this.loadingPageCheck = false;
        }

        public void SetFrameSize()
        {
            this.screenWidth = this.graphicsManager.PreferredBackBufferWidth;
            this.screenHeight = this.graphicsManager.PreferredBackBufferHeight;
            //MakeFullScreen();
            this.skeletonHeight = this.graphicsManager.PreferredBackBufferHeight;
            this.skeletonWidth = this.graphicsManager.PreferredBackBufferWidth;
        
        }

        public void MakeFullScreen()
        {
            if (!this.graphicsManager.IsFullScreen)
            {
               this.graphicsManager.ToggleFullScreen();
            }
            //this.graphicsManager.ToggleFullScreen();
            
        }

        public void LoadDrawingClass()
        {
            // for LoadingScreen animation
            List<Texture2D> textures = new List<Texture2D>();
            loadingPage = new List<Texture2D>();
            textures.Add(this.contentManager.Load<Texture2D>("SplashScreen/paw"));
            textures.Add(this.contentManager.Load<Texture2D>("SplashScreen/mediumJoint"));
            textures.Add(this.contentManager.Load<Texture2D>("SplashScreen/largeJoint"));
            //this.animatorSplashScreen = new ObjectAnimator(textures, new Vector2(400, 240), AnimationOption.Random);

            // for SplashScreen animation
            this.cursor = this.contentManager.Load<Texture2D>("SplashScreen/paw");
            this.splashScreen = this.contentManager.Load<Texture2D>("SplashScreen/SplashScreen");
            this.settingsScreen = this.contentManager.Load<Texture2D>("SettingsScreen/SettingsBackground");
            this.peekyCats.Add(this.contentManager.Load<Texture2D>("SplashScreen/peeky1"));
            this.peekyCats.Add(this.contentManager.Load<Texture2D>("SplashScreen/peeky2"));
            this.peekyCats.Add(this.contentManager.Load<Texture2D>("SplashScreen/peeky3"));
            this.peekyCats.Add(this.contentManager.Load<Texture2D>("SplashScreen/peeky4"));
            this.peekyCats.Add(this.contentManager.Load<Texture2D>("SplashScreen/PeekyCover1"));
            this.peekyCats.Add(this.contentManager.Load<Texture2D>("SplashScreen/PeekyCover2"));
            this.peekyCats.Add(this.contentManager.Load<Texture2D>("SplashScreen/PeekyCover3"));
            this.peekyCats.Add(this.contentManager.Load<Texture2D>("SplashScreen/PeekyCover4"));
            this.animatorSplashScreen = new ObjectAnimator(this.peekyCats, new Vector2(0, 0), AnimationOption.SplashScreen);

            this.loadingPage.Add(this.contentManager.Load<Texture2D>("Loading-1"));
            this.loadingPage.Add(this.contentManager.Load<Texture2D>("Loading-2"));
            this.loadingPage.Add(this.contentManager.Load<Texture2D>("Loading-3"));
            this.loadingPage.Add(this.contentManager.Load<Texture2D>("Loading-4"));
            this.loadingPage.Add(this.contentManager.Load<Texture2D>("Loading-5"));
            this.loadingPage.Add(this.contentManager.Load<Texture2D>("Loading-6"));
            this.loadingPage.Add(this.contentManager.Load<Texture2D>("Loading-7"));
            this.loadingPage.Add(this.contentManager.Load<Texture2D>("Loading-8"));

            /******************** FOR VIDEO ************************/

            String cd;
            cd = "C:/Users/rsyamil/Desktop";
            cd = cd + "/LevelData/";   
 
            //video = Content.Load<Video>(cd + "xblcg");
            player = new VideoPlayer(); 

            /******************************************************/

            //general
            this.cursor = this.contentManager.Load<Texture2D>("SplashScreen/paw");
            this.hudFont = this.contentManager.Load<SpriteFont>("displayFont");
            this.buttonFont = this.contentManager.Load<SpriteFont>("ButtonText");
            this.font = this.contentManager.Load<SpriteFont>("Score");
            this.alert = this.contentManager.Load<Texture2D>("Alert");

            this.greenBackground = this.contentManager.Load<Texture2D>("Green");
            this.yellowBackground = this.contentManager.Load<Texture2D>("Yellow");
            
            this.jointTexture = this.contentManager.Load<Texture2D>("SplashScreen/Joint");
            this.jointOrigin =  new Vector2(this.jointTexture.Width / 2, this.jointTexture.Height / 2);
            this.boneTexture = this.contentManager.Load<Texture2D>("SplashScreen/Bone");
            this.boneOrigin = new Vector2(0.7f, 0.0f);

            for (int i = 0; i < scoreGradeStr.Length; i++)
            {
                if (scoreGradeStr[i] != null) 
                { 
                    this.scoreGrades[i] = this.contentManager.Load<Texture2D>(scoreGradeStr[i]);
                } 
            }

            //this.smallJoint = this.contentManager.Load<Texture2D>("SplashScreen/smallJoint");
            //this.mediumJoint = this.contentManager.Load<Texture2D>("SplashScreen/mediumJoint");
            //this.largeJoint = this.contentManager.Load<Texture2D>("SplashScreen/largeJoint");

            this.normalTarget = this.contentManager.Load<Texture2D>("TargetCircle_normal");
            this.hoverTarget = this.contentManager.Load<Texture2D>("TargetCircle_hover");
            this.normalTarget_small = this.contentManager.Load<Texture2D>("TargetCircle_normal_small");
            this.hoverTarget_small = this.contentManager.Load<Texture2D>("TargetCircle_hover_small");
            this.normalTarget_large = this.contentManager.Load<Texture2D>("TargetCircle_normal_large");
            this.hoverTarget_large = this.contentManager.Load<Texture2D>("TargetCircle_hover_large");
            this.backButtonArea = new Rectangle(5, 580, 188, 82);
            this.backButton = new Button("Level-0/Level-Page-Back-Hover", "Level-0/Level-Page-Back", this.contentManager, backButtonArea);
            
            this.playArea = new Rectangle(480, 650, 188, 82);
            this.playButton = new Button("SplashScreen/start1", "SplashScreen/start2", this.contentManager, playArea);

            /******* QUIT BUTTONS **********/
            this.quitArea = new Rectangle(680, 650 , 188, 82);
            this.quitButton = new Button("SplashScreen/exit1", "SplashScreen/exit2", this.contentManager, quitArea);

            this.quitArea1 = new Rectangle(1130, 650, 188, 82);
            this.quitButton1 = new Button("SplashScreen/exit1", "SplashScreen/exit2", this.contentManager, quitArea1);
            /*****************/

            this.replayArea = new Rectangle(screenWidth*2 - 500, screenHeight*2 - 350, 157, 100);
            this.replayButton = new Button("Level-6/Score-Page-OKButton", "Level-6/Score-Page-OKButton-Hover", this.contentManager, replayArea);

            this.soundOffArea = new Rectangle(750, 120, 188, 82);
            this.soundOff = new TextButton("White", "White", "Off", this.contentManager, this.soundOffArea, new Vector2(25, 25));
            this.soundOnArea = new Rectangle(550, 120, 188, 82);
            this.soundOn = new TextButton("White", "White", "On", this.contentManager, this.soundOnArea, new Vector2(25, 25));

            this.bonusOnArea = new Rectangle(550, 230, 188, 82);
            this.bonusItemsOn = new TextButton("White", "White", "On", this.contentManager, this.bonusOnArea, new Vector2(25, 25));
            this.bonusOffArea = new Rectangle(750, 230, 188, 82);
            this.bonusItemsOff = new TextButton("White", "White", "Off", this.contentManager, this.bonusOffArea, new Vector2(25, 25));

            this.playerCnt1Area = new Rectangle(550, 340, 188, 82);
            this.playerCnt1 = new TextButton("White", "White", "One", this.contentManager, this.playerCnt1Area, new Vector2(25, 25));
            this.playerCnt2Area = new Rectangle(750, 340, 188, 82);
            this.playerCnt2 = new TextButton("White", "White", "Two", this.contentManager, this.playerCnt2Area, new Vector2(25, 25));

            this.accuracyEasyArea = new Rectangle(550, 450, 188, 82);
            this.accuracyEasy = new TextButton("White", "White", "Easy", this.contentManager, this.accuracyEasyArea, new Vector2(25, 25));
            this.accuracyMediumArea = new Rectangle(750, 450, 188, 82);
            this.accuracyMedium = new TextButton("White", "White", "Medium", this.contentManager, this.accuracyMediumArea, new Vector2(25, 25));

            this.accuracyHardArea= new Rectangle(950, 450, 188, 82);
            this.accuracyHard = new TextButton("White", "White", "Hard", this.contentManager, this.accuracyHardArea, new Vector2(25, 25));

            //player screen
            this.playerScreen = this.contentManager.Load<Texture2D>("PlayerScreen/PlayerBackground");
            this.felixArea = new Rectangle(110, 155, 298, 374);
            this.felixButton = new Button("PlayerScreen/felix", "PlayerScreen/felix-hover", this.contentManager, felixArea);
            this.garfieldArea = new Rectangle(530, 155, 298, 374);
            this.garfieldButton = new Button("PlayerScreen/garfield", "PlayerScreen/garfield-hover", this.contentManager, garfieldArea);
            this.spottyArea = new Rectangle(960, 155, 298, 374);
            this.spottyButton = new Button("PlayerScreen/spotty", "PlayerScreen/spotty-hover", this.contentManager, spottyArea);
            this.howToArea = new Rectangle(100, 675, 65, 60);
            this.howToButton = new Button("PlayerScreen/Player-Page-Howto-Hover", "PlayerScreen/Player-Page-Howto", this.contentManager, howToArea);
            this.infoArea = new Rectangle(200, 675, 65, 60);
            this.infoButton = new Button("PlayerScreen/Player-Page-Info", "PlayerScreen/Player-Page-Info-Hover", this.contentManager, infoArea);
            this.settingsArea = new Rectangle(300, 675, 65, 60);
            this.settingsButton = new Button("PlayerScreen/Player-Page-Settings", "PlayerScreen/Player-Page-Settings-Hover", this.contentManager, settingsArea);

            //level screen
            this.levelScreen = this.contentManager.Load<Texture2D>("LevelScreen/LevelBackground");
            this.level1Area = new Rectangle(43, 155, Convert.ToInt32(146 * 1.5), Convert.ToInt32(215 * 1.5));
            this.level1Button = new Button("LevelScreen/Level-Page-Halloween", "LevelScreen/Level-Page-Halloween-Hover", this.contentManager, level1Area);
            this.level2Area = new Rectangle(313, 155, Convert.ToInt32(146 * 1.5), Convert.ToInt32(215 * 1.5));
            this.level2Button = new Button("LevelScreen/Level-Page-Chef", "LevelScreen/Level-Page-Chef-Hover", this.contentManager, level2Area);
            this.level3Area = new Rectangle(573, 155, Convert.ToInt32(146 * 1.5), Convert.ToInt32(215 * 1.5));
            this.level3Button = new Button("LevelScreen/Level-Page-BasketBall", "LevelScreen/Level-Page-BasketBall-Hover", this.contentManager, level3Area);
            this.level4Area = new Rectangle(843, 155, Convert.ToInt32(146 * 1.5), Convert.ToInt32(215 * 1.5));
            this.level4Button = new Button("LevelScreen/Level-Page-Level3", "LevelScreen/Level-Page-Level3-Hover", this.contentManager, level4Area);
            this.level5Area = new Rectangle(1103, 155, Convert.ToInt32(146 * 1.5), Convert.ToInt32(215 * 1.5));
            this.level5Button = new Button("LevelScreen/Level-Page-Level5", "LevelScreen/Level-Page-Level5-Hover", this.contentManager, level5Area);

            //score screen
            this.scoreScreen = this.contentManager.Load<Texture2D>("ScoreScreen/Score-Page-Background");
      
            this.stars2 = this.contentManager.Load<Texture2D>("ScoreScreen/Score-Page-2star");
            this.stars3 = this.contentManager.Load<Texture2D>("ScoreScreen/Score-Page-3star");
            this.stars4 = this.contentManager.Load<Texture2D>("ScoreScreen/Score-Page-4star");
            this.stars5 = this.contentManager.Load<Texture2D>("ScoreScreen/Score-Page-5star");
            this.okButtonArea = new Rectangle(1090, 650, 188, 82);
            this.okButton = new Button("ScoreScreen/Score-Page-OKButton", "ScoreScreen/Score-Page-OKButton-Hover", this.contentManager, okButtonArea);

            this.backButtonArea = new Rectangle(5, 650, 188, 82);
            this.backButton = new Button("Level-0/Level-Page-Back-Hover", "Level-0/Level-Page-Back", this.contentManager, backButtonArea);

            //info screen
            this.infoScreen = this.contentManager.Load<Texture2D>("InfoScreen/Info-Page");


            string curdir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            curdir = curdir.Replace("\\", "/");

            curdir = "C:/Users/rsyamil/Desktop/eecs481/LevelData";
            curdir = curdir + "/LevelData/";   

            this.menuSong = new SoundPlayer(curdir+"MenuSong.wav");
            this.howTo = this.contentManager.Load<Video>("HowToScreen/CopyCat_HowTo");

            //this.speechArea = new Rectangle(0, 0, 400, 300);
            //this.speechBox = new TextButton(speechBubble, null, "Welcome!", speechArea, new Vector2(75, 75), buttonFont);
        }

        private void RunLevel(Skeleton[] skele, Avatar avatarStat)
        {
            this.menuSong.Stop();
            if (isMultiplayer)
            {
                currentLevel.updateLevel(skele, elapsedTime-startElapsedTime, avatarStat);
            }
            else if (!isMultiplayer)
            {
                currentLevel.updateLevel(skele[0], elapsedTime-startElapsedTime, avatarStat);
            }
            currentLevel.drawLevel(spriteBatch, elapsedTime-startElapsedTime, Math.Round(totalScore));

            if (skele[0] != null && this.level >= 1 && this.level <= 5)
            {
                //accuracyClass.CountAccuracy(skele[0]);
                drawShadow();
                for (int i = 0; i < skele.Length; i++)
                {
                    if (i == 0 || (i > 0 && isMultiplayer))
                    {
                        if (skele[i] != null)
                        {
                            drawSkeleton(skele[i], spriteBatch);
                        }
                    }
                }
                RenderClippedEdges(this.clipped);
            }

            if (level == 6)
            {
                prevLevel = 6;

                if (this.sound && prevLevel != 6)
                {
                    string dir = accuracyClass.getDir();
                    int end = dir.IndexOf("Level-");
                    dir = dir.Substring(0, end);
                    //this.applause = new SoundPlayer(dir + "applause-4.wav");
                    //this.applause.Play();
                }
                
                //prevLevel = 6;
                //this.graphicsManager.GraphicsDevice.Clear(Color.White);
                this.displayTime = 25;
                /*replayButton.updateButton(cursorPosition, ((elapsedTime - prevElapsedTime)));
                if (replayButton.isButtonTriggered())
                {
                    this.level = 0;
                    this.displayTime = 30;
                    replayButton.resetButton();
                }*/

                // draw score stars
                if (scoreGradeIndex > 0)
                {
                    spriteBatch.Draw(this.scoreGrades[scoreGradeIndex], Vector2.Zero, null, Color.White, 0f, Vector2.Zero,
                                     backgroundScale, SpriteEffects.None, 0f);

                }

                okButton.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                if (okButton.isButtonTriggered())
                {
                    accuracyClass = null; //.ResetTotalScore();
                    accuracyClass = new AccuracyClass(this.accuracyThreshold);
                    this.level = -3;//level selection screen
                    prevLevel = 6;
                    okButton.resetButton();
                    currentLevel = null;
                    
                }
                this.okButton.drawButton(spriteBatch);

                //quitButton.drawButton(spriteBatch);
                spriteBatch.Draw(this.cursor, pawTracker, Color.White);
            }

            if (displayTime == 0)
            {
                targetsCaptured = currentLevel.getCurrentScore();
                totalScore = accuracyClass.GetTotalScores();
                bonusScore = currentLevel.getBonusScore();
                scoreGradeIndex = Convert.ToInt32(Math.Floor((totalScore - 300) / 60));
                if (scoreGradeIndex < 0)
                {
                    scoreGradeIndex = 0;
                }
                else if (scoreGradeIndex >= 5)
                {
                    scoreGradeIndex = 4;
                }
                this.currentLevel = null;
                this.level = 6;
                loadedLevel = false;
            }
        }

        public void LoadLevel(Skeleton[] skele, Avatar avatarStat)
        {
            float horScale = (float)graphicsManager.GraphicsDevice.PresentationParameters.BackBufferWidth / 
                            (float)graphicsManager.PreferredBackBufferWidth;
            float vertScale = (float)graphicsManager.GraphicsDevice.PresentationParameters.BackBufferHeight / 
                            (float)graphicsManager.PreferredBackBufferHeight;
            Matrix SpriteScale = Matrix.CreateScale(horScale, vertScale, 1);

            //Resolution.BeginDraw();
            spriteBatch = new SpriteBatch(this.graphicsManager.GraphicsDevice);
            //spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, null
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null,
                null, null, null, SpriteScale);
            
            this.graphicsManager.GraphicsDevice.Clear(Color.White);
            //this.level = 5;
            
            //isMultiplayer = true;
            if (skele[0] == null)
            {
                this.pawTracker = this.mousePosition;
                //this.pawTrackerLeft = new Vector2(0,0);
            }
            else
            {
                 //this.pawTrackerLeft = this.cursorPositionLeft;
                 this.pawTracker = this.cursorPositionRight;
            }

            if (this.level == 6 && prevLevel != 6 && this.sound)
            {
                this.applause.Play();
            } 
            if ((this.level < 1 || this.level > 6) && this.level != -4)
            {
                if ((!this.playingAlready || prevLevelSong == -4) && this.sound)
                {
                          this.menuSong.Play();
                    this.playingAlready = true;
                    this.lastPlayed = elapsedTime;
                    this.prevLevelSong = 0;
                }
                if (playingAlready && !this.sound)
                {
                    this.menuSong.Stop();
                    this.playingAlready = false;
                }
                if (elapsedTime - this.lastPlayed > 33)
                {
                    this.playingAlready = false;
                }
            }

            if (this.level == -4)
            {
                this.prevLevelSong = -4;
                this.menuSong.Stop();
                videoPlayer.Play(howTo);
            }

            if (this.level != -4)
            {
                videoPlayer.Stop();
            }

            switch (this.level)
            {
                case -1: //settings screen
                    {
                        spriteBatch.Draw(this.settingsScreen, Vector2.Zero, null, Color.White, 0f, Vector2.Zero,
                            backgroundScale, SpriteEffects.None, 0f);

                        //load initiall settings 
                        if(this.bonusItems)                 bonusItemsOn.colorGreen();
                        else                                bonusItemsOff.colorGreen();
                        if(this.sound)                      soundOn.colorGreen();
                        else                                soundOff.colorGreen();
                        if(accuracyThreshold == 3)          accuracyEasy.colorGreen();
                        else if (accuracyThreshold == 2)    accuracyMedium.colorGreen();
                        else                                accuracyHard.colorGreen();
                        if (numPlayers == 1)                playerCnt1.colorGreen();
                        else                                playerCnt2.colorGreen();

                        bonusItemsOff.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        bonusItemsOn.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));

                        if (bonusItemsOn.isButtonClicked())
                        {
                            this.bonusItems = true;
                            bonusItemsOn.resetButton();

                            bonusItemsOff.colorWhite();
                            bonusItemsOn.colorGreen();
                        }
                        if (bonusItemsOff.isButtonClicked())
                        {
                            this.bonusItems = false;
                            bonusItemsOff.resetButton();

                            bonusItemsOff.colorGreen();
                            bonusItemsOn.colorWhite();
                        }
                        soundOff.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        soundOn.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (soundOff.isButtonClicked())
                        {
                            this.sound = false;
                            soundOff.resetButton();

                            soundOff.colorGreen();
                            soundOn.colorWhite();
                        }
                        if (soundOn.isButtonClicked())
                        {
                            this.sound = true;
                            soundOn.resetButton();

                            soundOff.colorWhite();
                            soundOn.colorGreen();
                        }
                        accuracyEasy.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        accuracyMedium.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        accuracyHard.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (accuracyEasy.isButtonClicked())
                        {
                            this.accuracyThreshold = 3;
                            accuracyClass.SetThreshold(accuracyThreshold);
                            accuracyEasy.resetButton();

                            accuracyMedium.colorWhite();
                            accuracyHard.colorWhite();
                            accuracyEasy.colorGreen();
                        }
                        if (accuracyMedium.isButtonClicked())
                        {
                            this.accuracyThreshold = 2;
                            accuracyClass.SetThreshold(accuracyThreshold);
                            accuracyMedium.resetButton();

                            accuracyMedium.colorGreen();
                            accuracyHard.colorWhite();
                            accuracyEasy.colorWhite();
                        }
                        if (accuracyHard.isButtonClicked())
                        {
                            this.accuracyThreshold = 1;
                            accuracyClass.SetThreshold(accuracyThreshold);
                            accuracyHard.resetButton();

                            accuracyMedium.colorWhite();
                            accuracyHard.colorGreen();
                            accuracyEasy.colorWhite();
                        }
                        backButton.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (backButton.isButtonClicked() || backButton.isButtonTriggered())
                        {
                            this.level = -2;//player selection screen
                            backButton.resetButton();
                        }
                        playerCnt1.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        playerCnt2.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (playerCnt1.isButtonClicked())
                        {
                            this.numPlayers = 1;
                            isMultiplayer = false;
                            playerCnt1.resetButton();

                            playerCnt1.colorGreen();
                            playerCnt2.colorWhite();
                        }
                        if (playerCnt2.isButtonClicked())
                        {
                            this.numPlayers = 2;
                            isMultiplayer = true; 
                            playerCnt2.resetButton();

                            playerCnt1.colorWhite();
                            playerCnt2.colorGreen();
                        }

                        this.backButton.drawButton(spriteBatch);
                        this.bonusItemsOn.drawButton(spriteBatch);
                        this.accuracyEasy.drawButton(spriteBatch);
                        this.accuracyMedium.drawButton(spriteBatch);
                        this.accuracyHard.drawButton(spriteBatch);
                        this.soundOff.drawButton(spriteBatch);
                        this.soundOn.drawButton(spriteBatch);
                        this.bonusItemsOff.drawButton(spriteBatch);
                        this.playerCnt1.drawButton(spriteBatch);
                        this.playerCnt2.drawButton(spriteBatch);

                        spriteBatch.Draw(this.cursor, pawTracker, Color.White);

                        break;

                    }
                case -2: //player selection page
                    {
                        spriteBatch.Draw(this.playerScreen, Vector2.Zero, null, Color.White, 0f, Vector2.Zero,
                            backgroundScale, SpriteEffects.None, 0f);
                       
                        felixButton.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (felixButton.isButtonTriggered())
                        {
                            this.level = -3;//select level
                            this.playerSelected = 0;
                            this.playerRecord = new PlayerRecord(playerSelected);
                            felixButton.resetButton();
                        }
                        garfieldButton.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (garfieldButton.isButtonTriggered())
                        {
                            this.level = -3;//select level
                            this.playerSelected = 1;
                            this.playerRecord = new PlayerRecord(playerSelected);
                            garfieldButton.resetButton();
                        }
                        spottyButton.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (spottyButton.isButtonTriggered())
                        {
                            this.level = -3;//select level
                            this.playerSelected = 2;
                            this.playerRecord = new PlayerRecord(playerSelected);
                            spottyButton.resetButton();
                        }
                        this.catColor = catColors[playerSelected];

                        howToButton.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (howToButton.isButtonTriggered())
                        {
                            this.level = -4;//howto
                            howToButton.resetButton();
                        }
                        infoButton.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (infoButton.isButtonTriggered())
                        {
                            this.level = -5;//info
                            infoButton.resetButton();
                        }
                        settingsButton.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (settingsButton.isButtonTriggered())
                        {
                            this.level = -1;//settings
                            settingsButton.resetButton();
                        }

                        /*******************************/
                        quitButton1.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (quitButton1.isButtonTriggered())
                        {
                            //exit;
                            quitGame = true;
                            quitButton1.resetButton();
                        }
                        quitButton1.drawButton(spriteBatch);
                        /*******************************/

                        felixButton.drawButton(spriteBatch);
                        garfieldButton.drawButton(spriteBatch);
                        spottyButton.drawButton(spriteBatch);
                        howToButton.drawButton(spriteBatch);
                        infoButton.drawButton(spriteBatch);
                        settingsButton.drawButton(spriteBatch);

                        spriteBatch.Draw(this.cursor, pawTracker, Color.White);
                        break;
                    }
                case -3: //Level selection page
                    {
                        spriteBatch.Draw(this.levelScreen, Vector2.Zero, null, Color.White, 0f, Vector2.Zero,
                            backgroundScale, SpriteEffects.None, 0f);

                        level1Button.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (level1Button.isButtonTriggered())
                        {
                            this.level = 1;
                            level1Button.resetButton();
                        }
                        level2Button.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (level2Button.isButtonTriggered())
                        {
                            this.level = 2;
                            level2Button.resetButton();
                        }
                        level3Button.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (level3Button.isButtonTriggered())
                        {
                            this.level = 3;
                            level3Button.resetButton();
                        }

                        level4Button.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (level4Button.isButtonTriggered())
                        {
                            this.level = 4;
                            level4Button.resetButton();
                        }
                        level5Button.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (level5Button.isButtonTriggered())
                        {
                            this.level = 5;
                            level5Button.resetButton();
                        }
                        backButton.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (backButton.isButtonTriggered())
                        {
                            this.level = -2;//player selection page
                            backButton.resetButton();
                        }

                        level1Button.drawButton(spriteBatch);
                        level2Button.drawButton(spriteBatch);
                        level3Button.drawButton(spriteBatch);
                        level4Button.drawButton(spriteBatch);
                        level5Button.drawButton(spriteBatch);
                        backButton.drawButton(spriteBatch);

                        spriteBatch.Draw(this.cursor, pawTracker, Color.White);
                        break;
                    }
                case -4://how-to Screen
                    {
                        if (videoPlayer.State == MediaState.Playing)
                        {
                            spriteBatch.Draw(videoPlayer.GetTexture(), new Rectangle(170, 92, howTo.Width, howTo.Height), Color.White);
                        }

                        backButton.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (backButton.isButtonTriggered())
                        {
                            this.level = -2;//player selection page
                            backButton.resetButton();
                        }
                        backButton.drawButton(spriteBatch);

                        /******************** VIDEO ************/
                        if (video != null)
                        {
                            player.Play(video);

                            if (player.State != MediaState.Stopped)             // Only call GetTexture if a video is playing or paused
                                videoTexture = player.GetTexture();

                            Rectangle screen = new Rectangle(GraphicsDevice.Viewport.X, // Drawing to the rectangle will stretch the video to fill the screen
                                GraphicsDevice.Viewport.Y,
                                GraphicsDevice.Viewport.Width,
                                GraphicsDevice.Viewport.Height);

                            if (videoTexture != null)       // Draw the video, if we have a texture to draw.
                            {
                                spriteBatch.Draw(videoTexture, screen, Color.White);
                            }
                        }
                        

                        /**************************************/

                        spriteBatch.Draw(this.cursor, pawTracker, Color.White);

                        break;
                    }
                case -5://info Screen
                    {
                        spriteBatch.Draw(this.infoScreen, Vector2.Zero, null, Color.White, 0f, Vector2.Zero,
                          backgroundScale, SpriteEffects.None, 0f);

                        backButton.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (backButton.isButtonTriggered())
                        {
                            this.level = -2;//player selection page
                            backButton.resetButton();
                        }

                        backButton.drawButton(spriteBatch);

                        spriteBatch.Draw(this.cursor, pawTracker, Color.White);
                        break;
                    }

                case 0: // Splash Screen
                    {
                        spriteBatch.Draw(this.splashScreen, Vector2.Zero, null, Color.White, 0f, Vector2.Zero,
                            backgroundScale, SpriteEffects.None, 0f);

                        playButton.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (playButton.isButtonTriggered())
                        {
                            this.level = -2;  //player selection page
                            playButton.resetButton();
                        }
                        playButton.drawButton(spriteBatch);

                        /*******************************/
                        quitButton.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));
                        if (quitButton.isButtonTriggered())
                        {
                            //exit;
                            quitGame = true;
                            quitButton.resetButton();
                        }
                        quitButton.drawButton(spriteBatch);

                        //spriteBatch.DrawString(hudFont, skeletonTracker.getKinectStatus(), Vector2.Zero, Color.Black);
                        /*******************************/


                        spriteBatch.Draw(this.splashScreen, Vector2.Zero, null, Color.White, 0f, Vector2.Zero,
                            backgroundScale, SpriteEffects.None, 0f);

                        this.animatorSplashScreen.Update();
                        this.animatorSplashScreen.Draw(spriteBatch);

                        spriteBatch.Draw(this.cursor, pawTracker             , Color.White);
                        break;
                    }
                case 1: 
                    {
                        this.prevLevel = 1;

                        ///////////////////////////////////////////////////////////
                        if (loadingPageCheck == false)
                        {
                            this.level = 7;
                            this.loadingPageCheck = true;
                            break;
                        }
                        /////////////////////////////////////////////////////////

                        if (currentLevel == null)
                        {
                            this.displayTime = 25;
                            //accuracyClass.ResetTotalScore();

                            string[] poseStr = new string[] { "Poses/pose1"+catColor,
                                                           "Poses/pose8"+catColor,
                                                           "Poses/pose9"+catColor,
                                                           "Poses/pose2"+catColor,
                                                           "Poses/pose13"+catColor };

                            String[] bonusTargets = new string[] { "Level-1/pumpkin1",
                                                                   "Level-1/pumpkin2",
                                                                   "Level-1/pumpkin3",
                                                                   "Level-1/wizardhat" };
                            Point[] bonusTargetSizes = new Point[] { new Point(51,52),
                                                                     new Point(51,51),
                                                                     new Point(51,51),
                                                                     new Point(51,51) };
                            accuracyClass.SetCurrentLevel(this.level);
                            generateTargetCircles("expectedCoordinates.txt");

                            /*currentLevel = new Level("Level-1/halloween", poseStr,
                            poses, hudFont, contentManager, bonusTargets, bonusTargetSizes,
                            new Point(screenWidth * 2, screenHeight), 0.25, 2); */

                            currentLevel = new Level("Level-1/halloween", poseStr, poses,
                                                     new Point(screenWidth * 2, screenHeight), hudFont,
                                                     contentManager);

                            startElapsedTime = elapsedTime;

                            loadedLevel = true;
                        }

                        break;
                    }
                case 2: 
                    {
                        this.prevLevel = 2;

                        /////////////////////////////////////////
                        if (loadingPageCheck == false)
                        {
                            this.level = 7;
                            this.loadingPageCheck = true;
                            break;
                        }
                        /////////////////////////////////////////


                        if (currentLevel == null)
                        {
                            this.displayTime = 25;
                            //accuracyClass.ResetTotalScore();
                            string[] poseStr = new string[] { "Poses/pose3"+catColor,
                                                              "Poses/pose8"+catColor,
                                                              "Poses/pose11"+catColor,
                                                              "Poses/pose9"+catColor,
                                                              "Poses/pose2"+catColor};
                            String[] bonusTargets = new string[] { "Level-2/Italian_Level3_GreenOlive",
                                                                   "Level-2/Italian_Level3_GreenPepper",
                                                                   "Level-2/Italian_Level3_Ham",
                                                                   "Level-2/Italian_Level3_Mushroom",
                                                                   "Level-2/Italian_Level3_RedOnion" };
                            Point[] bonusTargetSizes = new Point[] { new Point(50,50),
                                                                     new Point(50,50),
                                                                     new Point(50,50),
                                                                     new Point(50,50),
                                                                     new Point(50,50) };

                            accuracyClass.SetCurrentLevel(this.level);
                            generateTargetCircles("expectedCoordinates.txt");

                            if (this.bonusItems)
                            {
                                currentLevel = new Level("Level-2/Italian_Level3", poseStr,
                                poses, hudFont, contentManager, bonusTargets, bonusTargetSizes,
                                new Point(screenWidth * 2, screenHeight), 0.05, 2);
                            }
                            else
                            {
                                currentLevel = new Level("Level-2/Italian_Level3", poseStr, poses,
                                                    new Point(screenWidth * 2, screenHeight), hudFont,
                                                    contentManager);

                            }

                            startElapsedTime = elapsedTime;

                            loadedLevel = true;
                        }

                        break;
                    }
                case 3: 
                    {
                        this.prevLevel = 3;

                        /////////////////////////////////////////
                        if (loadingPageCheck == false)
                        {
                            this.level = 7;
                            this.loadingPageCheck = true;
                            break;
                        }
                        /////////////////////////////////////////

                        if (currentLevel == null)
                        {
                            this.displayTime = 25;
                            //accuracyClass.ResetTotalScore();
                            string[] poseStr = new string[] { "Poses/pose9"+catColor,
                                                           "Poses/pose6"+catColor,
                                                           "Poses/pose2"+catColor,
                                                           "Poses/pose1"+catColor,
                                                           "Poses/pose10"+catColor };
                            String[] bonusTargets = new string[] { "Level-3/basketball" };
                                                                   
                            Point[] bonusTargetSizes = new Point[] { new Point(50,50) };
                            accuracyClass.SetCurrentLevel(this.level);
                            generateTargetCircles("expectedCoordinates.txt");

                            if (this.bonusItems)
                            {
                                currentLevel = new Level("Level-3/background_basketball", poseStr,
                                poses, hudFont, contentManager, bonusTargets, bonusTargetSizes,
                            new Point(screenWidth * 2, screenHeight), 0.05, 2);
                            }
                            else
                            {
                                currentLevel = new Level("Level-3/background_basketball", poseStr, poses,
                                                   new Point(screenWidth * 2, screenHeight), hudFont,
                                                   contentManager);
                            }
                            startElapsedTime = elapsedTime;

                            loadedLevel = true;
                        }

                        break;
                    }
                case 4: 
                    {
                        this.prevLevel = 4;

                        /////////////////////////////////////////
                        if (loadingPageCheck == false)
                        {
                            this.level = 7;
                            this.loadingPageCheck = true;
                            break;
                        }
                        /////////////////////////////////////////

                        if (currentLevel == null)
                        {
                            this.displayTime = 25;
                            //accuracyClass.ResetTotalScore();
                            string[] poseStr = new string[] { "Poses/pose10"+catColor,
                                                           "Poses/pose5"+catColor,
                                                           "Poses/pose7"+catColor,
                                                           "Poses/pose3"+catColor,
                                                           "Poses/pose4"+catColor };
                            String[] bonusTargets = new string[] { "Level-4/flower_face",
                                                                   "Level-4/moon_face",
                                                                   "Level-4/sun_face"};

                            Point[] bonusTargetSizes = new Point[] { new Point(50,50),
                                                                     new Point(50,50),
                                                                     new Point(50,50) };
                            accuracyClass.SetCurrentLevel(this.level);
                            generateTargetCircles("expectedCoordinates.txt");
                            if (this.bonusItems)
                            {
                                currentLevel = new Level("Level-4/space_background", poseStr,
                                poses, hudFont, contentManager, bonusTargets, bonusTargetSizes,
                                new Point(screenWidth * 2, screenHeight), 0.05, 2);
                            }
                            else
                            {
                                currentLevel = new Level("Level-4/space_background", poseStr, poses,
                                                 new Point(screenWidth * 2, screenHeight), hudFont,
                                                 contentManager);
                            }
                            startElapsedTime = elapsedTime;

                            loadedLevel = true;
                        }

                        break;
                    }
                case 5:
                    {
                        this.prevLevel = 5;

                        /////////////////////////////////////////
                        if (loadingPageCheck == false)
                        {
                            this.level = 7;
                            this.loadingPageCheck = true;
                            break;
                        }
                        /////////////////////////////////////////

                        if (currentLevel == null)
                        {
                            this.displayTime = 25;
                            //accuracyClass.ResetTotalScore();
                            string[] poseStr = new string[] { "Poses/pose8"+catColor,
                                                           "Poses/pose12"+catColor,
                                                           "Poses/pose7"+catColor,
                                                           "Poses/pose13"+catColor,
                                                           "Poses/pose5"+catColor };
                            String[] bonusTargets = new string[] { "Level-5/falling_note1",
                                                                   "Level-5/falling_note2",
                                                                   "Level-5/falling_note3",
                                                                   "Level-5/falling_note4",
                                                                   "Level-5/falling_note5",
                                                                   "Level-5/falling_note6"};

                            Point[] bonusTargetSizes = new Point[] { new Point(44,26),
                                                                     new Point(44,44),
                                                                     new Point(32,49), 
                                                                     new Point(45,31),
                                                                     new Point(42,35),
                                                                     new Point(33,48)};
                            accuracyClass.SetCurrentLevel(this.level);
                            generateTargetCircles("expectedCoordinates.txt");
                            if (this.bonusItems)
                            {
                                currentLevel = new Level("Level-5/dancebackground", poseStr,
                                poses, hudFont, contentManager, bonusTargets, bonusTargetSizes,
                                new Point(screenWidth * 2, screenHeight), 0.05, 2);
                            }
                            else
                            {
                                currentLevel = new Level("Level-t/dancebackground", poseStr, poses,
                                                 new Point(screenWidth * 2, screenHeight), hudFont,
                                                 contentManager);
                            }
                            startElapsedTime = elapsedTime;

                            loadedLevel = true;
                        }

                        break;
                    }
                case 6: // profilePage
                    {
                        //playButton.updateButton(pawTracker, ((elapsedTime - prevElapsedTime)));

                        double fallingStarsRatio = (totalScore > 300) ? (totalScore - 300) / 600 : 0.05;

                        if (currentLevel == null)
                        {
                            //accuracyClass.ResetTotalScore();
                            string[] poseStr = new string[] {  };
                            String[] bonusTargets = new string[] { "Level-2/Italian_Level3_1Up" };

                            Point[] bonusTargetSizes = new Point[] { new Point(50,50) };
                            generateTargetCircles("expectedCoordinates.txt");
                            currentLevel = new Level("Level-6/Score-Page-Background", poseStr,
                                                     null, hudFont, contentManager, bonusTargets, bonusTargetSizes,
                            new Point(screenWidth * 2, screenHeight), fallingStarsRatio, 2);

                            //totalScore = accuracyClass.GetTotalScores();

                            Debug.Write("Score: " + totalScore);

                            //record the score
                            this.playerRecord.recordScore(this.prevLevel, totalScore, targetsCaptured, scoreGradeIndex, bonusScore);

                            //startElapsedTime = elapsedTime;

                            //accuracyClass.ResetTotalScore();

                            loadedLevel = true;
                        }

                        this.loadingPageCheck = false;          // for loading page
                                                                /////////////////////////////////////////////////////////////
                        spriteBatch.Draw(this.cursor, pawTracker, Color.White);
                        
                        break;
                    }
                case 7:
                    {
                        if (loadingPageIndex == -1)
                        {
                            timer500Millisecond.Start();
                            loadingPageIndex++;
                        }
                        spriteBatch.Draw(loadingPage[loadingPageIndex], Vector2.Zero, null, Color.White, 0f, Vector2.Zero, backgroundScale, SpriteEffects.None, 0f);
                        if (timer500Millisecond.CheckEventInvoke())
                            loadingPageIndex++;
                        if (loadingPageIndex == 8)
                        {
                            loadingPageIndex = -1;
                            timer500Millisecond.Stop();
                            this.level = this.prevLevel;    // once loading page is done, reset to prev Level 
                                                            //////////////////////////////////////////////////
    
                            break;
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            if (level >= 1 && level <= 6 && currentLevel != null)
            {
                RunLevel(skele, avatarStat);
            } 
            //spriteBatch.Draw(this.kinectRGBVideo, new Rectangle(0, 0,
            //   graphicsManager.PreferredBackBufferWidth,
            //   graphicsManager.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();
        }

        public void DrawSplashScreenAnimation(SpriteBatch spriteBatch)
        {



        }

        public void RenderClippedEdges(Clipped clipped)
        {
            if (clipped == Clipped.top)
            {
                spriteBatch.Draw(this.alert, topEdge, Color.White);
            }
            if (clipped == Clipped.bottom)
            {
                spriteBatch.Draw(this.alert, bottomEdge, Color.White);
            }
            if (clipped == Clipped.right)
            {
                spriteBatch.Draw(this.alert, rightEdge, Color.White);
            }
            if (clipped == Clipped.left)
            {
                spriteBatch.Draw(this.alert, leftEdge, Color.White);
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

        /// <summary>
        /// EFFECTS: Sets cursor position
        /// </summary>
        /// <param name="cursorPosition"></param>
        public void SetCursorPosition(Vector2 cursorPositionRight)
        {
            //this.cursorPositionLeft = cursorPositionLeft;
            this.cursorPositionRight = cursorPositionRight;
        }

        /// <summary>
        /// EFFECTS: Sets mouse position
        /// </summary>
        /// <param name="cursorPosition"></param>
        public void SetMousePosition(Vector2 spot)
        {
            this.mousePosition = spot;
        }

        /// <summary>
        /// EFFECTS: Lets drawing class know if the Kinect is connected 
        /// </summary>
        /// <param name="connectedStatus"></param>
        public void SetKinectStatus(string connectedStatus)
        {
            this.connectedStatus = connectedStatus;
        }

        public void SetClippedStatus(Clipped clipped)
        {
            this.clipped = clipped;
        }

        /// <summary>
        /// EFFECTS: Sets the elapsed time 
        /// </summary>
        /// <param name="inTime"></param>
        public void SetElapsedTime(double inTime)
        {
            prevElapsedTime = elapsedTime;
            elapsedTime = inTime;
        }

        public double getStartTime()
        {
            return startElapsedTime;
        }

        /// <summary>
        /// EFFECTS: Sets the time for display on the screen
        /// </summary>
        /// <param name="inDisplayTime"></param>
        public void SetDisplayedTime(int inDisplayTime)
        {
            displayTime = inDisplayTime;
        }

        /// <summary>
        /// EFFECTS: Sets the total score for display on the screen 
        /// </summary>
        /// <param name="inTotalScore"></param>
        public void SetTotalScore(double inTotalScore)
        {
            totalScore = inTotalScore;
        }

        public bool GetGameStatus()
        {
            return quitGame;
        }

        public void SetAccuracyClass(AccuracyClass accuracy)
        {
            accuracyClass = accuracy;
        }

        public AccuracyClass GetAccuracyClass()
        {
            return accuracyClass;
        }

        public void generateTargetCircles(String fileName)
        {
            double[, ,] expectedCoordinates = this.accuracyClass.ReturnExpectedCoordinate(fileName);
            int numPoses = 5;
            poses = new TargetCircles[numPoses];
            int numTargets = 4;
            for (int i = 0; i < numPoses; i++)
            {
                Vector2[] targetCoordinates = new Vector2[numTargets];
                for (int j = 0; j < numTargets; j++)
                {
                    targetCoordinates[j].X = 600 + (float)expectedCoordinates[i, (int)targetJoints[j], 0];
                    targetCoordinates[j].Y = 400 + (float)expectedCoordinates[i, (int)targetJoints[j], 1];
                }
                if (this.accuracyThreshold == 2)
                {
                    poses[i] = new TargetCircles(this.normalTarget, this.hoverTarget, targetCoordinates, this.accuracyThreshold);
                }
                else if (this.accuracyThreshold == 1)
                {
                    poses[i] = new TargetCircles(this.normalTarget_small, this.hoverTarget_small, targetCoordinates, this.accuracyThreshold);
                }
                else
                {
                    poses[i] = new TargetCircles(this.normalTarget_large, this.hoverTarget_large, targetCoordinates, this.accuracyThreshold);
                 }
                    //totalScore = inTotalScore;
            }
            //return poses;
        }

        public int getLevel()
        {
            return this.level;
        }

        /// <summary>
        /// Check keyboard input and then handle navigation in game
        /// </summary>
        public void navigateLevels(Command command)
        {
            if (command == Command.toLevel0) this.level = 0;
            else if (command == Command.toLevel1) this.level = 1;
            else if (command == Command.toLevel2) this.level = 2;
            else if (command == Command.toLevel3) this.level = 3;
            else if (command == Command.toLevel4) this.level = 4;
            else if (command == Command.toLevel5) this.level = 5;
            else if (command == Command.toProfilePage) this.level = 6;
            else { }
        }

        /// <summary>
        /// Draws the shadow representation of the player via green screen effect
        /// </summary>
        public void drawShadow()
        {
            if (this.skeletonTracker != null && skeletonTracker.colorReceived && skeletonTracker.depthReceived)
            {
                WriteableBitmap playerImg = this.skeletonTracker.getPlayerMask();
                Texture2D canvas_d = new Texture2D(this.graphicsManager.GraphicsDevice, playerImg.PixelWidth, playerImg.PixelHeight, false, SurfaceFormat.Color);
                int[] pixels_d = new int[skeletonTracker.getSize_d()];
                playerImg.CopyPixels(pixels_d, playerImg.PixelWidth * sizeof(int), 0);

                canvas_d.SetData<int>(skeletonTracker.greenScreenPixelData, 0, skeletonTracker.getSize_d());
                this.width_d = Convert.ToInt32(Math.Ceiling(playerImg.Width)) * 2;
                this.height_d = Convert.ToInt32(Math.Ceiling(playerImg.Height) * 2);
                
                //this.width_d = Convert.ToInt32(Math.Ceiling(playerImg.Width));
                //this.height_d = Convert.ToInt32(Math.Ceiling(playerImg.Height));

                spriteBatch.Draw(canvas_d, new Rectangle(220, 220, this.width_d, this.height_d), Color.Gray);
            }
        }

        /// <summary>
        /// Draws a line in the with the spriteBatch from startJoint to endJoint
        /// </summary>
        /// <param name="joints"></param>
        /// <param name="stratJoint"></param>
        /// <param name="endJoint"></param>
        private void drawBone(JointCollection joints, JointType startJoint, JointType endJoint)
        {

            Vector2 start = this.skeletonTracker.mapMethod(joints[startJoint].Position);
            Vector2 end = this.skeletonTracker.mapMethod(joints[endJoint].Position);
        

            Vector2 diff = end - start;
            Vector2 scale = new Vector2(1.0f, diff.Length() / this.boneTexture.Height);

            float angle = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;

            Color color = Color.Orange;
            if (joints[startJoint].TrackingState != JointTrackingState.Tracked ||
                joints[endJoint].TrackingState != JointTrackingState.Tracked)
            {
                color = Color.Gray;
            }

            this.spriteBatch.Draw(this.boneTexture, start, null, color, angle, this.boneOrigin, scale, SpriteEffects.None, 1.0f);
        }

        /// <summary>
        /// Draws the skeleton representation of the player
        /// </summary>
        /// <param name="skeleton"></param>
        /// <param name="spriteBatch"></param>
        public void drawSkeleton(Skeleton skeleton, SpriteBatch spriteBatch)
        {
            if (skeleton != null)
            {
                
                switch (skeleton.TrackingState)
                {
                    case SkeletonTrackingState.Tracked:
                        // Draw Bones
                        drawBone(skeleton.Joints, JointType.Head, JointType.ShoulderCenter);
                        drawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderLeft);
                        drawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderRight);
                        drawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.Spine);
                        drawBone(skeleton.Joints, JointType.Spine, JointType.HipCenter);
                        drawBone(skeleton.Joints, JointType.HipCenter, JointType.HipLeft);
                        drawBone(skeleton.Joints, JointType.HipCenter, JointType.HipRight);

                        drawBone(skeleton.Joints, JointType.ShoulderLeft, JointType.ElbowLeft);
                        drawBone(skeleton.Joints, JointType.ElbowLeft, JointType.WristLeft);
                        drawBone(skeleton.Joints, JointType.WristLeft, JointType.HandLeft);

                        drawBone(skeleton.Joints, JointType.ShoulderRight, JointType.ElbowRight);
                        drawBone(skeleton.Joints, JointType.ElbowRight, JointType.WristRight);
                        drawBone(skeleton.Joints, JointType.WristRight, JointType.HandRight);

                        drawBone(skeleton.Joints, JointType.HipLeft, JointType.KneeLeft);
                        drawBone(skeleton.Joints, JointType.KneeLeft, JointType.AnkleLeft);
                        drawBone(skeleton.Joints, JointType.AnkleLeft, JointType.FootLeft);

                        drawBone(skeleton.Joints, JointType.HipRight, JointType.KneeRight);
                        drawBone(skeleton.Joints, JointType.KneeRight, JointType.AnkleRight);
                        drawBone(skeleton.Joints, JointType.AnkleRight, JointType.FootRight);

                        // Now draw the joints
                        foreach (Joint j in skeleton.Joints)
                        {
                            Color jointColor = Color.Orange;
                            if (j.TrackingState != JointTrackingState.Tracked)
                            {
                                jointColor = Color.AntiqueWhite;
                            }

                            this.spriteBatch.Draw(
                                this.jointTexture,
                                this.skeletonTracker.mapMethod(j.Position),
                                 new Rectangle(200, 150, 500, 500),
                                jointColor,
                                0.0f,
                                this.jointOrigin,
                                1.0f,
                                SpriteEffects.None,
                                0.0f);
                        }

                        break;
                    case SkeletonTrackingState.PositionOnly:
                        // If we are only tracking position, draw a blue dot
                        this.spriteBatch.Draw(
                                this.jointTexture,
                                this.skeletonTracker.mapMethod(skeleton.Position),
                                null,
                                Color.Blue,
                                0.0f,
                                this.jointOrigin,
                                1.0f,
                                SpriteEffects.None,
                                0.0f);
                        break;
                }
            }
        }
       
    
    
    
    }
        
}
