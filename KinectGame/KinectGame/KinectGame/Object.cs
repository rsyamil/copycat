/********************************************************************
 * Name         : Object.cs
 * Author       : Syamil Razak
 * Description  : This class stores attributes for each animated objects * 
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
    public class Object
    {
        public Texture2D Texture { get; set; }            // The texture that will be drawn to represent object
        public Vector2 Position { get; set; }             // The current position of the object, may be updated, or not        
        public Vector2 Velocity { get; set; }             // The speed at the current instance
        public float Angle { get; set; }                  // The current angle of rotation with a pivot 
        public float AngularVelocity { get; set; }        // The speed that the angle is changing
        public Color Color { get; set; }                  // This color is the tinting 
        public float Size { get; set; }                   // The scale of the object
        public int ExistenceTime { get; set; }            // The duration of time that object will exist for 
            
        public Object(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, Color color, float size, int existenceTime)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Color = color;
            Size = size;
            ExistenceTime = existenceTime;
        }

        public void Update()
        {
            ExistenceTime--;
            Position += Velocity;
            Angle += AngularVelocity;
        }
 
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(0, 0);             // Always from the upper left corner
                                                            // null for sourceRectangle : draw the entire texture
            spriteBatch.Draw(Texture, Position, null, Color, 
                Angle, origin, Size, SpriteEffects.None, 0f);
        } 
    }
}
