/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * TRBotBingo
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TRBotBingo
{
    public class BingoBoard
    {
        public Grid<GridItem> BingoGrid { get; private set; }= null;
        private Texture2D BoardTex = null;
        private Texture2D MarkedTex = null;
        
        private Texture2D DebugSquare = null;

        private MouseState PrevMouseState = default;

        private bool ShowGridBounds = false;

        public BingoBoard(in int columns, in int rows, in Vector2 cellSize, Texture2D boardTex, Texture2D markedTex)
        {
            BingoGrid = new Grid<GridItem>(columns, rows, cellSize);
            BoardTex = boardTex;
            MarkedTex = markedTex;
        }

        public void Initialize()
        {
            //Add all the grid items
            for (int i = 0; i < BingoGrid.MaxElementsInGrid; i++)
            {
                BingoGrid.AddGridElement(new GridItem(Vector2.Zero, false));
            }
        }

        public void SetDebugSquare(Texture2D debugSquare)
        {
            DebugSquare = debugSquare;
        }

        /// <summary>
        /// Marks a bingo tile by a code (Ex. "a1" is the upper-left tile)
        /// </summary>
        public void ToggleMarkBingoTile(string code)
        {
            //Invalid code
            if (string.IsNullOrEmpty(code) == true || code.Length != 2)
            {
                return;
            }

            //Extract the row and column from the code
            string codeLower = code.ToLowerInvariant();

            int column = GetCodeFromCharVal((int)code[0]);
            int row = GetCodeFromCharVal((int)code[1]);

            int index = BingoGrid.GetIndex(column, row);

            //Invalid
            if (index < 0)
            {
                return;
            }

            //Mark bingo tile
            ToggleMarkBingoTile(index);
        }

        private int GetCodeFromCharVal(in int val)
        {
            if (val >= ((int)'a') && val <= ((int)'z'))
            {
                return (val - ((int)'a'));
            }
            else if (val >= ((int)'0') && val <= ((int)'9'))
            {
                //Subtract 1 here so that "1" marks the 0th index
                return (val - ((int)'0') - 1);
            }

            //Invalid
            return int.MaxValue;
        }

        public void ToggleMarkBingoTile(in int index)
        {
            //Toggle marked
            GridItem gridItem = BingoGrid.GetGridElement(index);
            if (gridItem != null)
            {
                gridItem.Marked = !gridItem.Marked;
            }
        }

        public void Update(GameTime gameTime)
        {
            //Check for mouse input to click on each tile
            MouseState mouseState = Mouse.GetState();

            //Console.WriteLine($"MouseLeft: {mouseState.LeftButton} | Prev: {PrevMouseState.LeftButton}");

            //Check for clicking
            if (mouseState.LeftButton == ButtonState.Pressed && PrevMouseState.LeftButton == ButtonState.Released)
            {
                //Console.WriteLine($"Entered: {BingoGrid.NumElementsInGrid}");
                Point mousePos = mouseState.Position;

                for (int i = 0; i < BingoGrid.NumElementsInGrid; i++)
                {
                    //Check for a click inside any of the grid cells
                    if (BingoGrid.IsPointInGridElement(i, mousePos) == true)
                    {
                        //Toggle marked
                        ToggleMarkBingoTile(i);
                        break;
                    }
                }
            }

            //Check for showing grid bounds with right-click
            if (mouseState.RightButton == ButtonState.Pressed && PrevMouseState.RightButton == ButtonState.Released)
            {
                ShowGridBounds = !ShowGridBounds;
            }

            PrevMouseState = Mouse.GetState();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BoardTex, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.1f);
            
            for (int i = 0; i < BingoGrid.NumElementsInGrid; i++)
            {
                GridItem gridItem = BingoGrid.GetGridElement(i);
                if (gridItem.Marked == false)
                {
                    continue;
                }

                Vector2 markPos = BingoGrid.GetPositionAtIndex(i);
                spriteBatch.Draw(MarkedTex, markPos, null, Color.Red, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, .2f);
            }
            
            //Draw the grid bounds if we should
            if (ShowGridBounds == true)
            {
                BingoGrid.DrawGridBounds(spriteBatch, DebugSquare, new Rectangle(0, 0, 1, 1));
            }
        }
    }
}