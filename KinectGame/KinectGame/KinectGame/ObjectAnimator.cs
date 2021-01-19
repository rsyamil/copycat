/********************************************************************
 * Name         : ObjectAnimator.cs
 * Author       : Syamil Razak
 * Description  : This class stores attributes for each animated objects 
 * 
 * *****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

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
    public enum AnimationOption { SplashScreen, LoadingScreen, Random };

    public class ObjectAnimator
    {
        private Random random;
        public Vector2 SourceAnimation { get; set; }
        private List<Object> objects;
        private List<Texture2D> textures;
        private AnimationOption option;

        public ObjectAnimator(List<Texture2D> textures, Vector2 location, AnimationOption opt)
        {
            SourceAnimation = location;
            this.textures = textures;
            this.objects = new List<Object>();
            this.option = opt;
            random = new Random();
            
        }

        /// <summary>
        /// Update is called in DrawingClass main. This must be expanded to fit animation of other things. 
        /// Currently only works for SplashScreen
        /// </summary>
        public void Update()
        {           
            int total = 1;              // only generate 1 object per update for now

            for (int i = 0; i < total; i++)
            {
                if (objects.Count == 1) break; // only 1 object to take care of for now 
                objects.Add(GenerateNewObject(this.option));
            }

            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Update();
                if (objects[i].ExistenceTime <= 0)
                {
                    objects.RemoveAt(i);
                    i--;
                }
            }
        }

        private Object GenerateNewObject(AnimationOption opt)
        {
            Texture2D texture;
            Vector2 position;  
            Vector2 velocity; 
            float angle;   
            float angularVelocity; 
            Color color;  
            float size;  
            int existenceTime;    

            if (opt == AnimationOption.Random)
            {
                texture = textures[random.Next(textures.Count)];
                position = SourceAnimation;
                velocity = new Vector2(
                                        1f * (float)(.9),
                                        1f * (float)(.9));
                angle = 0;
                angularVelocity = 0.1f ;
                color = new Color(
                        (float)random.NextDouble(),
                        (float)random.NextDouble(),
                        (float)random.NextDouble());
                size = 1;
                existenceTime = 12 ;
                return new Object(texture, position, velocity, angle, angularVelocity, color, size, existenceTime);
            }


            if (opt == AnimationOption.SplashScreen)
            {
                texture = textures[random.Next(textures.Count)];
                position = new Vector2(0, 0);
                velocity = new Vector2(0, 0);
                angle = 0;
                angularVelocity = 0;
                // TODO: fading out new Color(255, 255, 255, (byte)MathHelper.Clamp(AlphaValue, 0, 255)
                color = Color.White;
                size = 1.649f;
                existenceTime = 35;
                return new Object(texture, position, velocity, angle, angularVelocity, color, size, existenceTime);
            }

            texture = textures[random.Next(textures.Count)];
            position = SourceAnimation;
            velocity = new Vector2(
                                    1f * (float)(random.NextDouble() * 2 - 1),
                                    1f * (float)(random.NextDouble() * 2 - 1));
            angle = 0;
            angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
            color = new Color(
                        (float)random.NextDouble(),
                        (float)random.NextDouble(),
                        (float)random.NextDouble());
            size = (float)random.NextDouble();
            existenceTime = 20 + random.Next(40);
            return new Object(texture, position, velocity, angle, angularVelocity, color, size, existenceTime);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int index = 0; index < objects.Count; index++)
            {
                objects[index].Draw(spriteBatch);
            }
        }
    }
}
