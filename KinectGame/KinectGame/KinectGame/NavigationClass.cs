/********************************************************************
 * Name         : NavigationClass.cs
 * Author       : Syamil Razak
 * Description  : This class handles all navigation methods pertaining to
 * moving between levels, start and end of game, for easier debugging.
 * 
 * <space>        : exit game
 * <numPad0>      : spashscreen
 * <numPadn>      : move to level n, 1<=n<=5
 * <numPad6>      : move to profilePage
 * 
 * *****************************************************************/

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
    /// <summary>
    /// Enum for Clipped Edges. To be used in zooming in/out and also
    /// to guide player to stay in the range of Kinect
    /// </summary>
    public enum Clipped { top, bottom, right, left, none };

    /// <summary>
    /// Enum for moving between levels and navigation in game. 
    /// </summary>
    public enum Command { exitGame, pauseGame, resumeGame, toLevel0,
                                                           toLevel1,
                                                           toLevel2,
                                                           toLevel3,
                                                           toLevel4,
                                                           toLevel5,
                                                           toProfilePage,
                                                           none };

    public class NavigationClass : Microsoft.Xna.Framework.Game
    {
        KeyboardState oldState;
 
        public NavigationClass()
        {
        }

        public Clipped CheckClippedEdges(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
                {
                    return Clipped.bottom;
                }
                if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
                {
                    return Clipped.top;
                }
                if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
                {
                    return Clipped.left;
                }
                if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
                {
                    return Clipped.right;
                }
            }
            return Clipped.none;
        }

        public void SetStateKeyboard()
        {
            this.oldState = Keyboard.GetState();
        }

        public Command UpdateInput()
        {
            KeyboardState newState = Keyboard.GetState();

            if (newState.IsKeyDown(Keys.Space))
            {
                if (!oldState.IsKeyDown(Keys.Space))
                {
                    return Command.exitGame;                
                }
            }
            else if (oldState.IsKeyDown(Keys.Space))
            {
            }

            Keys[] key = new Keys[8];
            key = newState.GetPressedKeys();

            if (key.Length != 0)
            {
                if (key[0].Equals(Keys.NumPad0))
                {
                    return Command.toLevel0;
                }
                else if (key[0].Equals(Keys.NumPad1))
                {
                    return Command.toLevel1;
                }
                else if (key[0].Equals(Keys.NumPad2))
                {
                    return Command.toLevel2;
                }
                else if (key[0].Equals(Keys.NumPad3))
                {
                    return Command.toLevel3;
                }
                else if (key[0].Equals(Keys.NumPad4))
                {
                    return Command.toLevel4;
                }
                else if (key[0].Equals(Keys.NumPad5))
                {
                    return Command.toLevel5;
                }
                else if (key[0].Equals(Keys.NumPad6))
                {
                    return Command.toProfilePage;
                }
                else { }
            }
            oldState = newState;
            return Command.none;
        }
    }
}
