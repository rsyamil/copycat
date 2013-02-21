/********************************************************************
 * Name         : NavigationClass.cs
 * Author       : Syamil Razak
 * Description  : This class handles all navigation methods pertaining to
 * moving between levels, start and end of game.   
 * 
 * *****************************************************************/

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
    public enum Clipped { top, bottom, right, left, none };
    public enum Command { exitGame, pauseGame, resumeGame, none };

    public class NavigationClass : Microsoft.Xna.Framework.Game
    {
        KeyboardState oldState;
        Command currentCommand;

        List<ButtonsInfo> buttonsInfoList = new List<ButtonsInfo>(); 

        public NavigationClass()
        {
            buttonsInfoList.Add(new Button
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

            // Check if space key is down
            if (newState.IsKeyDown(Keys.Space))
            {
                // If not down on the last update, then it was just pressed
                if (!oldState.IsKeyDown(Keys.Space))
                {
                    return Command.exitGame;                
                }
            }
            else if (oldState.IsKeyDown(Keys.Space))
            {
                // Key was down last update, but not down now, so
                // it has just been released.
            }

            Keys[] key = new Keys[8];

            key = newState.GetPressedKeys();

            if (key.Length != 0)
            {
                if (key[0].Equals(Keys.B))
                {
                    // listens to other keys etc
                }
            }

            // Update saved state.
            oldState = newState;
            return Command.none;
        }
    }

    public class ButtonsInfo : Microsoft.Xna.Framework.Game
    {
        int level;
        int x;
        int y;
        int width;
        int height;

        public ButtonsInfo(int level, int x, int y, int width, int height)
        {
            this.level = level;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }
}
