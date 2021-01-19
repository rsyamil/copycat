/******************************************************************************
 * Name:        SkeletalTracker.cs 
 * Author:      Jason Lee
 * Description: This class will provide abstractions for obtaining skeletal data and interacting with 
 * Microsoft Kinect.
 * 
 * Log:
 * 2/21/2013 (Syamil) - Added ResetAngleKinect(), removed declaration of DrawingClass 
 * and NavigationClass. 
 * 3/10/2013 (Syamil) - Smoothens cursor by only updating cursor 15 times per second
 * vs 30 times. Scaled Y coord of hand (skeleton is scaled to the screen, but we want to ignore
 * the lower half of the body if dealing with the cursor. 
 * 3/26/2013 (Rebecca) - Depth and color stream tracking, implimented green screen effect,
 * added mapMethod
 * 
 * ****************************************************************************/

﻿using System;
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
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Drawing;
using System.Windows;


namespace KinectGame
{
    public class SkeletonTracker
    {
        KinectSensor kinect;
        int skeletonWidthScale;
        int skeletonHeightScale;

        Skeleton[] skeletonData = new Skeleton[0];
        //Dictionary<int, Skeleton> playerSkeletons = new Dictionary<int, Skeleton>();
        Skeleton[] playerSkeletons = new Skeleton[MAX_PLAYERS];
        //List<Skeleton> playerSkeletons = new List<Skeleton>();

        const int MAX_PLAYERS = 2;
        int numActivePlayers = 0;

        string connectedStatus = "Status: Not Connected";
        Vector2 handCursorRight = new Vector2(0, 0);
        Vector2 handCursorLeft;

        List<Vector2> cursorValtoAverage;

        Microsoft.Xna.Framework.Color[] color = null;
        int videoHeight = 0;
        int videoWidth = 0;

        private const DepthImageFormat DepthFormat = DepthImageFormat.Resolution320x240Fps30;
        private const ColorImageFormat ColorFormat = ColorImageFormat.RgbResolution640x480Fps30;
        private WriteableBitmap colorBitmap;
        private WriteableBitmap playerOpacityMaskImage = null;
        public DepthImagePixel[] depthPixels;
        private byte[] colorPixels;
        public int[] greenScreenPixelData;
        private ColorImagePoint[] colorCoordinates;
        private int colorToDepthDivisor;
        private int depthWidth;
        private int depthHeight;
        private int opaquePixelValue = -1;
        public bool depthReceived = false;
        public bool colorReceived = false;

        public int frameHeight;
        public int frameWidth;

        /// <summary>
        /// REQUIRES: height and width 
        /// EFFECTS: Sets up the SkeletonTracker class 
        /// </summary>
        /// <param name="sHeight"></param>
        /// <param name="sWidth"></param>
        public SkeletonTracker(int sHeight, int sWidth)
        {

            this.skeletonHeightScale = sHeight * 2;
            this.skeletonWidthScale = sWidth;
            KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            this.DiscoverKinectSensor();

            this.ResetAngleKinect();


            cursorValtoAverage = new List<Vector2>();
        }

        /// <summary>
        /// EFFECTS: Event handler that detects if the kinect sensor has changed status.
        /// Check what's actually happened, If a kinect has been attached
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
   
                if (e.Status == KinectStatus.Disconnected ||
                    e.Status == KinectStatus.NotPowered)
                {
                    this.kinect = null;
                    this.DiscoverKinectSensor();
                }
                if (e.Status == KinectStatus.Connected)
                {
                    this.DiscoverKinectSensor();
                }


        } 

        private void DiscoverKinectSensor()
        {
            foreach (KinectSensor sensor in KinectSensor.KinectSensors)
            {
                if (sensor.Status == KinectStatus.Connected)
                {
                    // Find the first one then set our sensor to this
                    this.kinect = sensor; break; 
                }
            }
            if (this.kinect == null)
            {
                connectedStatus = "Status: Not connected"; return;         
            }

            // You can use the kinectSensor.Status to check for status
            // and give the user some kind of feedback
            switch (kinect.Status)
            {
                case KinectStatus.Connected:
                    {
                        connectedStatus = "Status: Connected"; break;    
                    }
                case KinectStatus.Disconnected:
                    {
                        connectedStatus = "Status: Disconnected"; break;
                    }
                case KinectStatus.NotPowered:
                    {
                        connectedStatus = "Status: Connect the power"; break;
                    }
                default:
                    {
                        connectedStatus = "Status: Error"; break;
                    }
            }
            if (kinect.Status == KinectStatus.Connected) InitializeKinect();
        } 

        /// <summary>
        /// EFFECTS: Initiailizes the Kinect and sets some parameteres
        /// </summary>
        private void InitializeKinect()
        {
           // kinect.DepthStream.Range = DepthRange.Near; 
            kinect.SkeletonStream.EnableTrackingInNearRange = true;

            // TODO values require adjustment
            this.kinect.SkeletonStream.Enable(new TransformSmoothParameters()
            {
                Smoothing = 0.9f,
                Correction = 0.9f,
                Prediction = 0.05f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.05f
            });

            // Attach an event handler to the event that fires when the skeleton frame is ready.
            this.kinect.SkeletonFrameReady += 
                new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);

            //set up for green screen
            // Turn on the depth stream to receive depth frames

            //DEPTH STREAM ***************
            this.kinect.DepthStream.Enable(DepthFormat);

            this.depthWidth = this.kinect.DepthStream.FrameWidth;
            this.depthHeight = this.kinect.DepthStream.FrameHeight;

            // Allocate space to put the depth pixels we'll receive
            this.depthPixels = new DepthImagePixel[this.kinect.DepthStream.FramePixelDataLength];

            //COLOR STREAM **************
            kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

            int colorWidth = this.kinect.ColorStream.FrameWidth;
            int colorHeight = this.kinect.ColorStream.FrameHeight;

            // Allocate space to put the color pixels we'll create
            this.colorPixels = new byte[this.kinect.ColorStream.FramePixelDataLength];


            this.colorToDepthDivisor = colorWidth / this.skeletonWidthScale;
            this.colorToDepthDivisor = colorWidth / this.depthWidth;

            this.greenScreenPixelData = new int[this.kinect.DepthStream.FramePixelDataLength];

            this.colorCoordinates = new ColorImagePoint[this.kinect.DepthStream.FramePixelDataLength];

            // This is the bitmap we'll display on-screen
            this.colorBitmap = new WriteableBitmap(colorWidth, colorHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

            // Add an event handler to be called whenever there is new depth frame data
            this.kinect.AllFramesReady += this.SensorAllFramesReady;

            try
            {
                this.kinect.Start();
            }
            catch
            {
                connectedStatus = "Unable to start Kinect Sensor";
            }
        }


        /// <summary>
        /// EFFECTS: This function triggers whenever a skeleton frame is ready to be read.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            List<int> activeSkeletonIDs = new List<int>();
            Dictionary<int, Skeleton> activeSkeletons = new Dictionary<int, Skeleton>();
            // Using the new frame that has been sent (we use a using block to automatically dipose of it when we're done)
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    // Initialize a Skeleton array and copy the data from the frame to the array
                    skeletonData = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletonData);
                 
                    // Extract a player's skeleton from the data (in this case, we're extracting the 
                    // first skeleton that is "tracked" which means it has a TrackingState of SkeletonTrackingState.Tracked
                    // playerSkeletons[0] = (from s in skeletonData where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();

                    for (int i = 0; i < skeletonData.Length; i++)
                    {
                        if (skeletonData[i].TrackingState == SkeletonTrackingState.Tracked)
                        {
                            activeSkeletonIDs.Add(skeletonData[i].TrackingId);
                            activeSkeletons.Add(skeletonData[i].TrackingId, skeletonData[i]);
                        }
                    }

                    int nullCount = 0;
                    numActivePlayers = 0;
                    for (int i = 0; i < MAX_PLAYERS; i++)
                    {
                        if (playerSkeletons[i] != null)
                        {
                            if (activeSkeletonIDs.Contains(playerSkeletons[i].TrackingId))
                            {
                                playerSkeletons[i] = activeSkeletons[playerSkeletons[i].TrackingId];
                                activeSkeletons.Remove(playerSkeletons[i].TrackingId);
                                activeSkeletonIDs.Remove(playerSkeletons[i].TrackingId);
                                numActivePlayers++;
                            }
                            else
                            {
                                playerSkeletons[i] = null;
                                nullCount++;
                            }
                        }
                        else
                        {
                            nullCount++;
                        }
                    }

                    if (activeSkeletons.Count > 0 && nullCount > 0)
                    {
                        for (int i = 0; i < MAX_PLAYERS; i++)
                        {
                            if (playerSkeletons[i] == null && activeSkeletonIDs.Count > 0) 
                            {
                                int newSkeletonID = activeSkeletonIDs.First();
                                activeSkeletonIDs.Remove(newSkeletonID);
                                playerSkeletons[i] = activeSkeletons[newSkeletonID];
                                activeSkeletons.Remove(newSkeletonID);
                                numActivePlayers++;
                            }
                        }
                    }

                    if (playerSkeletons[1] != null & playerSkeletons[0] == null)
                    {
                        playerSkeletons[0] = playerSkeletons[1];
                        playerSkeletons[1] = null;
                    }
                 
                    if (playerSkeletons[0] != null)
                    {
                        Joint rightHand = playerSkeletons[0].Joints[JointType.HandRight];
                        Vector2 temp = new Vector2((((0.5f * rightHand.Position.X) + 0.5f)
                            * (this.skeletonWidthScale)), (((-0.5f * rightHand.Position.Y) + 0.5f)
                            * (this.skeletonHeightScale)));

                        cursorValtoAverage.Add(temp);

                        Joint leftHand = playerSkeletons[0].Joints[JointType.HandLeft];
                        this.handCursorLeft = new Vector2((((0.5f * leftHand.Position.X) + 0.5f)
                            * (this.skeletonWidthScale)), (((-0.5f * leftHand.Position.Y) + 0.5f)
                            * (this.skeletonHeightScale)));
                    }
                }
            }

            if (skeletonData.Length != 0)
            {
                foreach (Skeleton skel in skeletonData)
                {
                    break;
                }
            }
        }

        //  TODO implement skeleton identification for each player 
        //  using nearest neighbor analysis 
        //  NOTE! SkeletonPoints have X, Y, and Z values. X and Y values are in horizontal and vertical 
        //  meters away from the center of the sensor (range -3 to +3)
        //  whereas Z is in meters (depth) away from the sensor.
        //  You can convert these values to usable screenspace coordinates on your own, or use Microsoft's 
        //  Coding4Fun scaleTo function which you can get by importing Coding4Fun.

        /// <summary>
        /// Event handler for Kinect sensor's DepthFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            // in the middle of shutting down, so nothing to do
            if (null == this.kinect)
            {
                return;
            }

            this.depthReceived = false;
            this.colorReceived = false;
            //GET ALL THE DEPTH DATA
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (null != depthFrame)
                {
                    this.frameWidth = depthFrame.Width;
                    this.frameHeight = depthFrame.Height;
                    // Copy the pixel data from the image to a temporary array
                    depthFrame.CopyDepthImagePixelDataTo(this.depthPixels);
                    depthReceived = true;
                }
            }
            //GET ALL THE COLOR DATA
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (null != colorFrame)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    colorReceived = true;
                }
            }

            // do our processing outside of the using block
            // so that we return resources to the kinect as soon as possible
            if (true == depthReceived)
            {
                this.kinect.CoordinateMapper.MapDepthFrameToColorFrame(
                    DepthFormat,
                    this.depthPixels,
                    ColorFormat,
                    this.colorCoordinates);

                Array.Clear(this.greenScreenPixelData, 0, this.greenScreenPixelData.Length);

                // loop over each row and column of the depth
                for (int y = 0; y < this.depthHeight; ++y)
                {
                    for (int x = 0; x < this.depthWidth; ++x)
                    {
                        // calculate index into depth array
                        int depthIndex = x + (y * this.depthWidth);

                        DepthImagePixel depthPixel = this.depthPixels[depthIndex];

                        int player = depthPixel.PlayerIndex;

                        // if we're tracking a player for the current pixel, do green screen
                        if (player > 0)
                        {
                            // retrieve the depth to color mapping for the current depth pixel
                            ColorImagePoint colorImagePoint = this.colorCoordinates[depthIndex];

                            // scale color coordinates to depth resolution
                            int colorInDepthX = colorImagePoint.X / this.colorToDepthDivisor;
                            int colorInDepthY = colorImagePoint.Y / this.colorToDepthDivisor;

                            // make sure the depth pixel maps to a valid point in color space
                            // check y > 0 and y < depthHeight to make sure we don't write outside of the array
                            // check x > 0 instead of >= 0 since to fill gaps we set opaque current pixel plus the one to the left
                            // because of how the sensor works it is more correct to do it this way than to set to the right
                            if (colorInDepthX > 0 && colorInDepthX < this.depthWidth && colorInDepthY >= 0 && colorInDepthY < this.depthHeight)
                            {
                                // calculate index into the green screen pixel array
                                int greenScreenIndex = colorInDepthX + (colorInDepthY * this.depthWidth);

                                // set opaque
                                this.greenScreenPixelData[greenScreenIndex] = opaquePixelValue;

                                // compensate for depth/color not corresponding exactly by setting the pixel 
                                // to the left to opaque as well
                                this.greenScreenPixelData[greenScreenIndex - 1] = opaquePixelValue;
                            }
                        }
                    }
                }
            }

            // do our processing outside of the using block
            // so that we return resources to the kinect as soon as possible
            if (true == colorReceived)
            {
                // Write the pixel data into our bitmap
                this.colorBitmap.WritePixels(
                    new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                    this.colorPixels,
                    this.colorBitmap.PixelWidth * sizeof(int),
                    0);

                if (this.playerOpacityMaskImage == null)
                {
                    this.playerOpacityMaskImage = new WriteableBitmap(
                        this.depthWidth,
                        this.depthHeight,
                        96,
                        96,
                        PixelFormats.Bgra32,
                        null);

                    // -> MaskedColor.OpacityMask = new ImageBrush { ImageSource = this.playerOpacityMaskImage }; WINDOWS FORM WAY OF DISPLAYING MASK
                }

                this.playerOpacityMaskImage.WritePixels(
                    new Int32Rect(0, 0, this.depthWidth, this.depthHeight),
                    this.greenScreenPixelData,
                    this.depthWidth * ((this.playerOpacityMaskImage.Format.BitsPerPixel + 7) / 8),
                    0);
            }


        }

        /// <summary>
        /// gets the player's shadow representation
        /// </summary>
        public WriteableBitmap getPlayerMask()
        {
            return this.playerOpacityMaskImage;
        }

        /// <summary>
        /// gets the depth stream size
        /// </summary>
        public int getSize_d()
        {
            return this.kinect.DepthStream.FramePixelDataLength;
        }

        public string getKinectStatus()
        {
            return this.connectedStatus;
        }

        public int getNumActivePlayers()
        {
            return numActivePlayers;
        }

        public Skeleton[] getPlayerSkeletons()
        {
            return playerSkeletons;
        }

        /// <summary>
        /// REQUIRES: 0 indicates the player who is logged in
        ///           or 1 indicates the guest player
        /// EFFECTS:  Returns the most current skeleton of the
        ///           requested player and a NULL if the player
        ///           is currently inactive 
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public Skeleton getSkeleton(int playerID)
        {
            if (playerID <= 1)
            {
                return playerSkeletons[playerID];
            }
            return null; // throw not implemented exception 
        }

        /// <summary>
        /// REQUIRES: false indicates that the player’s right 
        ///           hand joint is being requested and true 
        ///           indicates that the player’s left hand
        /// EFFECTS:  Returns the current joint of the player’s
        ///           requested hand
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        public Vector2 getHandCursor(int playerID, bool left)
        {
            if (left)
            {
                return this.handCursorLeft;
            }
            else
            {
                // Frames are updated 30times per second, we will update the cursor 15times each second
                // Once you have added all 2, average it and then clear the vector
                Vector2 curPosition = new Vector2(0, 0);
                Vector2 scalarDivision = new Vector2(2, 2);
                if (cursorValtoAverage.Count > 2)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        curPosition = Vector2.Add(curPosition, cursorValtoAverage.ElementAt(i));
                    }                    
                    this.handCursorRight = Vector2.Divide(curPosition, scalarDivision);
                    cursorValtoAverage.Clear();
                }
                return this.handCursorRight;
            }
        }

        /// <summary>
        /// REQUIRES: n/a 
        /// EFFECTS:  Stops and disposes of the current Kinect   
        ///		   instance
        /// </summary>
        public void UnloadKinect()
        {
            if (kinect != null)
            {
                kinect.Stop();
                kinect.Dispose();
            }
        }

        /// <summary>
        /// REQUIRES: n/a 
        /// EFFECTS:  Resets the angle of the Kinect
        /// </summary>
        public void ResetAngleKinect()
        {
            if (kinect != null && kinect.Status == KinectStatus.Connected)
            {
                kinect.ElevationAngle = 0;
            }
        }

        public int getVideoWidth()
        {
            return this.videoWidth;
        }

        public int getVideoHeight()
        {
            return this.videoHeight;
        }

        public Microsoft.Xna.Framework.Color[] getVideoColor()
        {
            return this.color;
        }

        /// <summary>
        /// This method maps a SkeletonPoint to the depth frame.
        /// </summary>
        /// <param name="point">The SkeletonPoint to map.</param>
        /// <returns>A Vector2 of the location on the depth frame.</returns>
        public Vector2 mapMethod(SkeletonPoint point)
        {
            if ((null != this.kinect) && (null != this.kinect.DepthStream))
            {
                // This is used to map a skeleton point to the depth image location
                var depthPt = this.kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(point, this.kinect.DepthStream.Format);

                //change values to manipulate shadow
                return new Vector2(((1.8f * depthPt.X)) + 250, ((1.8f * (depthPt.Y) + 1.0f)) + 250);
            }

            return Vector2.Zero;
        }

    }

}
