/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * TRBotBingo
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace TRBotBingo
{
    /// <summary>
    /// A class for defining Extension Methods
    /// </summary>
    public static class Extensions
    {
        #region Texture2D Extensions
        
        /// <summary>
        /// Gets the origin of a Texture2D by ratio instead of specifying width and height.
        /// </summary>
        /// <param name="texture2D">The texture to get the origin for.</param>
        /// <param name="x">The X ratio of the origin, between 0 and 1.</param>
        /// <param name="y">The Y ratio of the origin, between 0 and 1.</param>
        /// <returns>A Vector2 with the origin.</returns>
        public static Vector2 GetOrigin(this Texture2D texture2D, in float x, in float y)
        {
            int xVal = (int)(texture2D.Width * Math.Clamp(x, 0f, 1f));
            int yVal = (int)(texture2D.Height * Math.Clamp(y, 0f, 1f));

            return new Vector2(xVal, yVal);
        }

        /// <summary>
        /// Gets the center origin of a Texture2D.
        /// </summary>
        /// <param name="texture2D">The texture to get the origin for.</param>
        /// <returns>A Vector2 with the center origin.</returns>
        public static Vector2 GetCenterOrigin(this Texture2D texture2D)
        {
            return texture2D.GetOrigin(.5f, .5f);
        }

        /// <summary>
        /// Gets the texture coordinates at specified X and Y values of a Texture2D in a Vector2. The returned X and Y values will be from 0 to 1.
        /// </summary>
        /// <param name="texture2D">The Texture2D to get the texture coordinates from.</param>
        /// <param name="sourceRect">The Rectangle to get the coordinates from.</param>
        /// <returns>A Vector2 with the Rectangle's X and Y values divided by the texture's width and height, respectively.</returns>
        public static Vector2 GetTexCoordsAt(this Texture2D texture2D, Rectangle? sourceRect)
        {
            Vector2 texCoords = Vector2.Zero;

            if (sourceRect != null)
            {
                return GetTexCoordsAt(texture2D, sourceRect.Value.X, sourceRect.Value.Y);
            }

            return texCoords;
        }

        /// <summary>
        /// Gets the texture coordinates at specified X and Y values of a Texture2D in a Vector2. The returned X and Y values will be from 0 to 1.
        /// </summary>
        /// <param name="texture2D">The Texture2D to get the texture coordinates from.</param>
        /// <param name="x">The X position on the texture.</param>
        /// <param name="y">The Y position on the texture.</param>
        /// <returns>A Vector2 with the X and Y values divided by the texture's width and height, respectively.</returns>
        public static Vector2 GetTexCoordsAt(this Texture2D texture2D, in int x, in int y)
        {
            Vector2 texCoords = Vector2.Zero;

            //Get the ratio of the X and Y values from the Width and Height of the texture
            if (texture2D.Width > 0)
                texCoords.X = x / (float)texture2D.Width;
            if (texture2D.Height > 0)
                texCoords.Y = y / (float)texture2D.Height;

            return texCoords;
        }

        #endregion

        #region SpriteFont Extensions

        /// <summary>
        /// Gets the origin of a SpriteFont by ratio instead of specifying width and height.
        /// </summary>
        /// <param name="spriteFont">The font to get the origin for.</param>
        /// <param name="text">The text to be displayed.</param>
        /// <param name="x">The X ratio of the origin, between 0 and 1.</param>
        /// <param name="y">The Y ratio of the origin, between 0 and 1.</param>
        /// <returns>A Vector2 with the origin.</returns>
        public static Vector2 GetOrigin(this SpriteFont spriteFont, in string text, in float x, in float y)
        {
            if (string.IsNullOrEmpty(text) == true) return Vector2.Zero;

            Vector2 size = spriteFont.MeasureString(text);
            size.X *= Math.Clamp(x, 0f, 1f);
            size.Y *= Math.Clamp(y, 0f, 1f);

            return size;
        }

        /// <summary>
        /// Gets the center origin of a SpriteFont.
        /// </summary>
        /// <param name="spriteFont">The font to get the origin for.</param>
        /// <param name="text">The text to be displayed.</param>
        /// <returns>A Vector2 with the center origin.</returns>
        public static Vector2 GetCenterOrigin(this SpriteFont spriteFont, in string text)
        {
            return spriteFont.GetOrigin(text, .5f, .5f);
        }

        #endregion

        #region Rectangle Extensions

        /// <summary>
        /// Gets the top-left point of the Rectangle.
        /// </summary>
        /// <param name="rectangle">The Rectangle.</param>
        /// <returns>A Vector2 containing the top-left point of the rectangle.</returns>
        public static Vector2 TopLeft(this Rectangle rectangle) => new Vector2(rectangle.Left, rectangle.Top);

        /// <summary>
        /// Gets the top-right point of the Rectangle.
        /// </summary>
        /// <param name="rectangle">The Rectangle.</param>
        /// <returns>A Vector2 containing the top-right point of the rectangle.</returns>
        public static Vector2 TopRight(this Rectangle rectangle) => new Vector2(rectangle.Right, rectangle.Top);

        /// <summary>
        /// Gets the bottom-left point of the Rectangle.
        /// </summary>
        /// <param name="rectangle">The Rectangle.</param>
        /// <returns>A Vector2 containing the bottom-left point of the rectangle.</returns>
        public static Vector2 BottomLeft(this Rectangle rectangle) => new Vector2(rectangle.Left, rectangle.Bottom);

        /// <summary>
        /// Gets the bottom-right point of the Rectangle.
        /// </summary>
        /// <param name="rectangle">The Rectangle.</param>
        /// <returns>A Vector2 containing the bottom-right point of the rectangle.</returns>
        public static Vector2 BottomRight(this Rectangle rectangle) => new Vector2(rectangle.Right, rectangle.Bottom);

        /// <summary>
        /// Gets the origin of a Rectangle.
        /// </summary>
        /// <param name="rectangle">The Rectangle to get the origin for.</param>
        /// <param name="x">The X ratio of the origin, from 0 to 1.</param>
        /// <param name="y">The Y ratio of the origin, from 0 to 1.</param>
        /// <returns>A Vector2 with the origin.</returns>
        public static Vector2 GetOrigin(this Rectangle rectangle, in float x, in float y)
        {
            int xVal = (int)(rectangle.Width * Math.Clamp(x, 0f, 1f));
            int yVal = (int)(rectangle.Height * Math.Clamp(y, 0f, 1f));

            return new Vector2(xVal, yVal);
        }

        /// <summary>
        /// Gets the origin of a Rectangle without truncating the result.
        /// </summary>
        /// <param name="rectangle">The Rectangle to get the origin for.</param>
        /// <param name="x">The X ratio of the origin, from 0 to 1.</param>
        /// <param name="y">The Y ratio of the origin, from 0 to 1.</param>
        /// <returns>A Vector2 with the origin.</returns>
        public static Vector2 GetOriginFloat(this Rectangle rectangle, in float x, in float y)
        {
            float xVal = (float)(rectangle.Width * Math.Clamp(x, 0f, 1f));
            float yVal = (float)(rectangle.Height * Math.Clamp(y, 0f, 1f));

            return new Vector2(xVal, yVal);
        }

        /// <summary>
        /// Gets the center origin of a Rectangle.
        /// </summary>
        /// <param name="rectangle">The Rectangle to get the origin for.</param>
        /// <returns>A Vector2 with the center origin.</returns>
        public static Vector2 GetCenterOrigin(this Rectangle rectangle)
        {
            return rectangle.GetOrigin(.5f, .5f);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
        /// <param name="rectTex">The texture for the rectangle.</param>
        /// <param name="rect">The Rectangle to draw.</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <param name="layer">The layer of the rectangle.</param>
        public static void DrawRect(this SpriteBatch spriteBatch, in Texture2D rectTex, in Rectangle rect, in Color color, in float layer)
        {
            spriteBatch.Draw(rectTex, rect, null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
        /// <param name="rectTex">The texture for the rectangle.</param>
        /// <param name="sourceRect">The source rectangle for the texture.</param>
        /// <param name="rect">The Rectangle to draw.</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <param name="layer">The layer of the rectangle.</param>
        public static void DrawRect(this SpriteBatch spriteBatch, in Texture2D rectTex, in Rectangle? sourceRect, in Rectangle rect, in Color color, in float layer)
        {
            spriteBatch.Draw(rectTex, rect, sourceRect, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        }

        /// <summary>
        /// Draws a hollow rectangle.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
        /// <param name="rectTex">The texture for the rectangle.</param>
        /// <param name="rect">The Rectangle to draw.</param>
        /// <param name="color">The color of the hollow rectangle.</param>
        /// <param name="layer">The layer of the hollow rectangle.</param>
        /// <param name="thickness">The thickness of the hollow rectangle.</param>
        public static void DrawHollowRect(this SpriteBatch spriteBatch, in Texture2D rectTex, Rectangle rect, in Color color, in float layer, in int thickness)
        {
            DrawHollowRect(spriteBatch, rectTex, null, rect, color, layer, thickness);
        }

        /// <summary>
        /// Draws a hollow rectangle.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
        /// <param name="rectTex">The texture for the rectangle.</param>
        /// <param name="sourceRect">The source rectangle for the texture.</param>
        /// <param name="rect">The Rectangle to draw.</param>
        /// <param name="color">The color of the hollow rectangle.</param>
        /// <param name="layer">The layer of the hollow rectangle.</param>
        /// <param name="thickness">The thickness of the hollow rectangle.</param>
        public static void DrawHollowRect(this SpriteBatch spriteBatch, in Texture2D rectTex, in Rectangle? sourceRect, Rectangle rect, in Color color, in float layer, in int thickness)
        {
            Rectangle topLine = new Rectangle(rect.X, rect.Y, rect.Width, thickness);
            Rectangle rightLine = new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height);
            Rectangle leftLine = new Rectangle(rect.X, rect.Y, thickness, rect.Height);
            Rectangle bottomLine = new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness);

            spriteBatch.Draw(rectTex, topLine, sourceRect, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
            spriteBatch.Draw(rectTex, rightLine, sourceRect, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
            spriteBatch.Draw(rectTex, leftLine, sourceRect, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
            spriteBatch.Draw(rectTex, bottomLine, sourceRect, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        }

        #endregion
    }
}
