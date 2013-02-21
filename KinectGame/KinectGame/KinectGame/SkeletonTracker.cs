/******************************************************************************
 * Name:        SkeletalTracker.cs 
 * Author:      Jason Lee
 * Description: This class will provide abstractions for obtaining skeletal data and interacting with 
 * Microsoft Kinect.
 * 
 * Log:
 * 2/21/2013 (Syamil) - Added ResetAngleKinect(), removed declaration of DrawingClass 
 * and NavigationClass. 
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

namespace KinectGame
{
    public class SkeletonTracker
    {
        KinectSensor kinect;
        int screenWidth;
        int screenHeight;

        Skeleton[] skeletonData = new Skeleton[0];
        Skeleton[] playerSkeletons = new Skeleton[MAX_PLAYERS];

        const int MAX_PLAYERS = 2;
        const int baseHeight = 480;
        const int baseWidth = 640;

        string connectedStatus = "Status: Not Connected";
        Vector2 handCursorRight;
        Vector2 handCursorLeft;

        Color[] color = null;
        int videoHeight = 0;
        int videoWidth = 0;

        /// <summary>
        /// REQUIRES: height and width 
        /// EFFECTS: Sets up the SkeletonTracker class 
        /// </summary>
        /// <param name="sHeight"></param>
        /// <param name="sWidth"></param>
        public SkeletonTracker(int sHeight, int sWidth)
        {
            KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            this.DiscoverKinectSensor();

            this.screenHeight = sHeight;
            this.screenWidth = sWidth;
        }

        /// <summary>
        /// EFFECTS: Event handler that detects if the kinect sensor has changed status.
        /// Check what's actually happened, If a kinect has been attached
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (this.kinect == e.Sensor)
            {
                if (e.Status == KinectStatus.Disconnected ||
                    e.Status == KinectStatus.NotPowered)
                {
                    this.kinect = null;
                    this.DiscoverKinectSensor();
                }
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
            kinect.DepthStream.Range = DepthRange.Near;
            kinect.SkeletonStream.EnableTrackingInNearRange = true;

            kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            kinect.ColorFrameReady +=
                new EventHandler<ColorImageFrameReadyEventArgs>(kinectSensor_ColorFrameReady);

            // TODO values require adjustment
            this.kinect.SkeletonStream.Enable(new TransformSmoothParameters()
            {
                Smoothing = 0.5f,
                Correction = 0.5f,
                Prediction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            });

            // Attach an event handler to the event that fires when the skeleton frame is ready.
            this.kinect.SkeletonFrameReady += 
                new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);

            try
            {
                this.kinect.Start();
            }
            catch
            {
                connectedStatus = "Unable to start Kinect Sensor";
            }
        }

        void kinectSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorImageFrame = e.OpenColorImageFrame())
            {
                if (colorImageFrame != null)
                {

                    byte[] pixelsFromFrame = new byte[colorImageFrame.PixelDataLength];
                    colorImageFrame.CopyPixelDataTo(pixelsFromFrame);
                    color = new Color[colorImageFrame.Height * colorImageFrame.Width];
                    this.videoHeight = colorImageFrame.Height;
                    this.videoWidth = colorImageFrame.Width;

                    // Go through each pixel and set the RGB bytes correctly
                    int index = 0;
                    for (int y = 0; y < colorImageFrame.Height; y++)
                    {
                        for (int x = 0; x < colorImageFrame.Width; x++, index += 4)
                        {
                            color[y * colorImageFrame.Width + x] = new Color(pixelsFromFrame[index + 2], pixelsFromFrame[index + 1], pixelsFromFrame[index + 0]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// EFFECTS: This function triggers whenever a skeleton frame is ready to be read.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
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
                    playerSkeletons[0] = (from s in skeletonData where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();

                    if (playerSkeletons[0] != null)
                    {
                        Joint rightHand = playerSkeletons[0].Joints[JointType.HandRight];
                        this.handCursorRight = new Vector2((((0.5f * rightHand.Position.X) + 0.5f)
                            * (this.screenWidth)), (((-0.5f * rightHand.Position.Y) + 0.5f)
                            * (this.screenHeight)));
                        Joint leftHand = playerSkeletons[0].Joints[JointType.HandLeft];
                        this.handCursorLeft = new Vector2((((0.5f * leftHand.Position.X) + 0.5f)
                            * (this.screenWidth)), (((-0.5f * leftHand.Position.Y) + 0.5f)
                            * (this.screenHeight)));
                    }
                }
            }

            if (skeletonData.Length != 0)
            {
                foreach (Skeleton skel in skeletonData)
                {
                    // draw.SetClippedStatus(navigate.CheckClippedEdges(skel));
                    // break because there is only one skeleton for Alpha Release
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

        public string getKinectStatus()
        {
            return this.connectedStatus;
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

        public Color[] getVideoColor()
        {
            return this.color;
        }


    }

}
