﻿/* This Source Code Form is subject to the terms of the Mozilla Public
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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TRBotBingo
{
    /// <summary>
    /// A grid holding elements.
    /// <para>By default the grid repositions the elements when it is modified in some way (Ex. Rows, Columns, CellSize).
    /// To change this behavior, set <see cref="AutomaticReposition"/> to false.
    /// In this case, the grid will need to be manually repositioned with <see cref="RepositionGridElements"/> after changes have been made.</para>
    /// </summary>
    public class Grid<T> where T : GridItem
    {
        /// <summary>
        /// The types of pivots for the grid.
        /// </summary>
        public enum GridPivots
        {
            UpperLeft,
            UpperCenter,
            UpperRight,
            CenterLeft,
            Center,
            CenterRight,
            BottomLeft,
            BottomCenter,
            BottomRight
        }

        /// <summary>
        /// Whether to automatically reposition the elements after any changes or not
        /// </summary>
        public bool AutomaticReposition = true;

        public Vector2 Position
        {
            get => GridPosition;
            set
            {
                Vector2 prevPos = Position;
                //Set position
                GridPosition = value;

                //Reposition the grid after changing the position
                if (AutomaticReposition == true && prevPos != Position)
                    RepositionGridElements();
            }
        }

        /// <summary>
        /// A property for the size of each cell in the grid.
        /// </summary>
        public Vector2 CellSize
        {
            get => GridCellSize;
            set
            {
                Vector2 prevCellSize = GridCellSize;
                GridCellSize = value;

                //Reposition the grid if the value is different
                if (AutomaticReposition == true && prevCellSize != GridCellSize)
                {
                    RepositionGridElements();
                }
            }
        }

        /// <summary>
        /// A property for the number of columns in the grid.
        /// </summary>
        public int Columns
        {
            get => GridColumns;
            set
            {
                int prevCols = GridColumns;
                GridColumns = value;

                //Reposition the grid if the value is different
                if (AutomaticReposition == true && prevCols != GridColumns)
                {
                    RepositionGridElements();
                }
            }
        }

        /// <summary>
        /// A property for the number of rows in the grid.
        /// </summary>
        public int Rows
        {
            get => GridRows;
            set
            {
                int prevRows = GridRows;
                GridRows = value;

                //Reposition the grid if the value is different
                if (AutomaticReposition == true && prevRows != GridRows)
                {
                    RepositionGridElements();
                }
            }
        }

        /// <summary>
        /// The spacing of the grid elements.
        /// </summary>
        public Vector2 Spacing
        {
            get => GridElementSpacing;
            set
            {
                Vector2 prevSpacing = GridElementSpacing;
                GridElementSpacing = value;

                if (AutomaticReposition == true && prevSpacing != GridElementSpacing)
                {
                    RepositionGridElements();
                }
            }
        }

        /// <summary>
        /// Tells whether to constrain the grid by column or row.
        /// <para>A value of true constrains by column and false constrains by row.</para>
        /// </summary>
        public bool ConstrainByColumn = true;

        /// <summary>
        /// The max number of elements that can be in the grid based on its size.
        /// </summary>
        public int MaxElementsInGrid => (Columns * Rows);

        /// <summary>
        /// The number of elements in the grid.
        /// </summary>
        public int NumElementsInGrid => (GridElements == null) ? 0 : GridElements.Count;

        /// <summary>
        /// The total size of the grid. This does not take into account the pivot.
        /// </summary>
        public Vector2 GridSize => new Vector2(Columns * CellSize.X, Rows * CellSize.Y);

        /// <summary>
        /// The bounds of the grid. This does not take into account the pivot.
        /// <para>This is primarily for debugging.</para>
        /// </summary>
        protected Rectangle GridBounds
        {
            get
            {
                Vector2 gridSize = GridSize;

                return new Rectangle((int)Position.X, (int)Position.Y, (int)gridSize.X, (int)gridSize.Y);
            }
        }

        /// <summary>
        /// The size of each cell in the grid.
        /// </summary>
        protected Vector2 GridCellSize = new Vector2(32, 32);

        /// <summary>
        /// The number of columns in the grid.
        /// </summary>
        protected int GridColumns = 2;

        /// <summary>
        /// The number of rows in the grid.
        /// </summary>
        protected int GridRows = 2;

        /// <summary>
        /// The grid pivot.
        /// </summary>
        public GridPivots GridPivot { get; protected set; } = GridPivots.UpperLeft;

        /// <summary>
        /// The pivot for the grid elements.
        /// </summary>
        public GridPivots ElementPivot { get; protected set; } = GridPivots.UpperLeft;

        protected Vector2 GridPosition = Vector2.Zero;

        /// <summary>
        /// The amount to space the grid elements away from each other.
        /// </summary>
        protected Vector2 GridElementSpacing = Vector2.Zero;

        /// <summary>
        /// The padding of the grid.
        /// </summary>
        protected Padding GridPadding = new Padding();

        /// <summary>
        /// The elements in the grid.
        /// This is a list for performance/usability reasons, as we can easily position a list in a grid-like manner.
        /// </summary>
        protected List<T> GridElements = null;

        public Grid(in int columns, in int rows, in Vector2 cellSize)
        {
            //Set the values directly instead of going through the properties
            //At this point there cannot be any elements in the grid, so bypass repositioning since it's unnecessary
            GridColumns = columns;
            GridRows = rows;
            GridCellSize = cellSize;

            GridElements = new List<T>(Columns * Rows);
        }

        /// <summary>
        /// Adds an element to the grid.
        /// </summary>
        /// <param name="element">The element to add to the grid.</param>
        public void AddGridElement(T element)
        {
            if (element == null)
            {
                Console.WriteLine($"Attempting to add null {nameof(element)} to the {nameof(Grid<T>)}!");
                return;
            }

            GridElements.Add(element);

            //Issue a warning saying to expand the grid if the number of elements is going over
            if (NumElementsInGrid > MaxElementsInGrid)
            {
                Console.WriteLine($"The {nameof(Grid<T>)} has {NumElementsInGrid} elements which exceeds the max of {MaxElementsInGrid}. "
                    + $"Please adjust the number of {nameof(Columns)} and {nameof(Rows)} when expanding the grid.");
            }

            if (AutomaticReposition == true)
            {
                RepositionGridElements();
            }
        }

        /// <summary>
        /// Removes an element from the grid.
        /// </summary>
        /// <param name="element">The element to remove from the grid.</param>
        public void RemoveGridElement(T element)
        {
            bool removed = GridElements.Remove(element);
            if (AutomaticReposition == true && removed == true)
            {
                RepositionGridElements();
            }
        }

        /// <summary>
        /// Removes an element from the grid.
        /// </summary>
        /// <param name="index">The index of the element to remove from the grid.</param>
        public void RemoveGridElement(in int index)
        {
            RemoveGridElement(GetGridElement(index));
        }

        /// <summary>
        /// Removes an element from the grid.
        /// </summary>
        /// <param name="column">The zero-based column number of the element.</param>
        /// <param name="row">The zero-based row number of the element.</param>
        public void RemoveGridElement(in int column, in int row)
        {
            RemoveGridElement(GetGridElement(column, row));
        }

        /// <summary>
        /// Clears the grid by removing all elements from it.
        /// </summary>
        public void ClearGrid()
        {
            GridElements.Clear();
        }

        /// <summary>
        /// Repositions the elements in the grid.
        /// </summary>
        public void RepositionGridElements()
        {
            //Check for null - this should only be possible in the constructor
            if (GridElements == null)
                return;

            for (int i = 0; i < GridElements.Count; i++)
            {
                GridElements[i].Position = GetPositionAtIndex(i);
            }
        }

        /// <summary>
        /// Returns an index in the grid from column and row numbers.
        /// </summary>
        /// <param name="column">The zero-based column of the grid.</param>
        /// <param name="row">The zero-based row of the grid.</param>
        /// <returns>-1 if the column or row is out of the grid's range, otherwise an index</returns>
        public int GetIndex(in int column, in int row)
        {
            if (column < 0 || column >= Columns || row < 0 || row >= Rows)
            {
                Console.WriteLine($"Column {column} or Row {row} is out of the grid's range!");
                return -1;
            }

            //Return the row times the total number of Columns and offset by the supplied column
            //Do vice versa if constraining by row
            int index = (row * Columns) + column;
            if (ConstrainByColumn == false)
                index = (column * Rows) + row;

            return index;
        }

        /// <summary>
        /// Returns zero-based column and row numbers from an index in the grid.
        /// </summary>
        /// <param name="index">The index to retrieve the zero-based column and row numbers for.</param>
        /// <param name="column">An out integer that will be the zero-based column number. -1 if the grid has 0 or fewer Columns.</param>
        /// <param name="row">An out integer that will be the zero-based row number. -1 if the grid has 0 or fewer Columns.</param>
        public void GetColumnRowFromIndex(in int index, out int column, out int row)
        {
            //Set initial default values
            column = -1;
            row = -1;

            if (Columns <= 0 && ConstrainByColumn == true)
            {
                Console.WriteLine($"Max grid columns is {Columns} which is less than or equal to 0!");
                return;
            }

            if (Rows <= 0 && ConstrainByColumn == false)
            {
                Console.WriteLine($"Max grid rows is {Rows} which is less than or equal to 0!");
                return;
            }

            //Perform Modulo to obtain the column number and division to obtain the row number
            column = index % Columns;
            row = index / Columns;

            if (ConstrainByColumn == false)
            {
                column = index / Rows;
                row = index % Rows;
            }
        }

        /// <summary>
        /// Returns the grid element at an index.
        /// </summary>
        /// <param name="index">The index to retrieve the element for.</param>
        /// <returns>null if the index is out of the grid's range, otherwise the element at the index.</returns>
        public T GetGridElement(in int index)
        {
            if (index < 0 || index >= GridElements.Count)
            {
                Console.WriteLine($"index {index} is out of the grid's range!");
                return default(T);
            }

            return GridElements[index];
        }

        /// <summary>
        /// Returns the grid element at a particular column and row number.
        /// </summary>
        /// <param name="column">The zero-based column number.</param>
        /// <param name="row">The zero-based row number.</param>
        /// <returns>null if the column or index are out of the grid's range, otherwise the element at the index.</returns>
        public T GetGridElement(in int column, in int row)
        {
            return GetGridElement(GetIndex(column, row));
        }

        /// <summary>
        /// Gets the position of a grid element at a particular index relative to the grid's position.
        /// This overload is used for convenience.
        /// </summary>
        /// <param name="index">The index of the grid element. This can be outside of the grid's range.</param>
        /// <returns>The relative position of the grid element.</returns>
        protected Vector2 GetRelativePositionAtIndex(in int index)
        {
            return GetRelativePositionAtIndex(index, GridPivots.UpperLeft);
        }

        /// <summary>
        /// Gets the position of a grid element at a particular index relative to the grid's position.
        /// </summary>
        /// <param name="index">The index of the grid element. This can be outside of the grid's range.</param>
        /// <param name="pivot">The pivot used to offset the elements in the grid.</param>
        /// <returns>The relative position of the grid element.</returns>
        protected Vector2 GetRelativePositionAtIndex(in int index, in GridPivots pivot)
        {
            GetColumnRowFromIndex(index, out int xIndex, out int yIndex);

            Vector2 elementPivotPos = GetElementOffsetForPivot(pivot);

            //Use the GridPivot for the Spacing since it applies to the entire grid
            Vector2 spacingOffset = GetSpacingAtColumnRow(xIndex, yIndex, GridPivot);

            //Apply the padding
            Vector2 paddingOffset = GridPadding.TotalPadding;

            Vector2 relativePos = new Vector2(xIndex * CellSize.X, yIndex * CellSize.Y) - elementPivotPos + spacingOffset + paddingOffset;
            return relativePos;
        }

        /// <summary>
        /// Gets the position a grid element would be at a particular index.
        /// </summary>
        /// <param name="index">The index of the grid element. This can be outside of the grid's range.</param>
        /// <returns>The position of the grid element.</returns>
        public Vector2 GetPositionAtIndex(in int index)
        {
            return GetPositionAtIndex(index, GridPivot, ElementPivot);
        }

        /// <summary>
        /// Gets the position a grid element would be at a particular index with a particular element and grid pivot.
        /// </summary>
        /// <param name="index">The index of the grid element. This can be outside of the grid's range.</param>
        /// <param name="elementPivot">The pivot used to offset the elements in the grid.</param>
        /// <param name="gridPivot">The pivot used to offset the grid.</param>
        /// <returns>The position of the grid element with the designated element and grid pivots.</returns>
        public Vector2 GetPositionAtIndex(in int index, in GridPivots gridPivot, in GridPivots elementPivot)
        {
            //Add the grid's Position with the relative position of the element at the index
            //Then subtract from the pivot offset
            Vector2 relativePos = GetRelativePositionAtIndex(index, elementPivot);
            Vector2 offsetPos = GetOffsetFromPivot(gridPivot);
            Vector2 posToDraw = (Position + relativePos) - offsetPos;

            return posToDraw;
        }

        /// <summary>
        /// Changes the GridPivot of the grid.
        /// </summary>
        /// <param name="pivot">The GridPivot to change to.</param>
        public void ChangeGridPivot(in GridPivots pivot)
        {
            GridPivots prevGridPivot = GridPivot;
            GridPivot = pivot;

            if (AutomaticReposition == true && prevGridPivot != GridPivot)
            {
                RepositionGridElements();
            }
        }

        /// <summary>
        /// Changes the ElementPivot of the grid.
        /// </summary>
        /// <param name="pivot">The GridPivot to change to.</param>
        public void ChangeElementPivot(in GridPivots pivot)
        {
            GridPivots prevElementPivot = ElementPivot;
            ElementPivot = pivot;

            if (AutomaticReposition == true && prevElementPivot != ElementPivot)
            {
                RepositionGridElements();
            }
        }

        /// <summary>
        /// Changes the padding of the grid.
        /// </summary>
        /// <param name="left">The left padding.</param>
        /// <param name="right">The right padding.</param>
        /// <param name="top">The top padding.</param>
        /// <param name="bottom">The bottom padding.</param>
        public void ChangeGridPadding(in int left, in int right, in int top, in int bottom)
        {
            GridPadding.Left = left;
            GridPadding.Right = right;
            GridPadding.Top = top;
            GridPadding.Bottom = bottom;

            if (AutomaticReposition == true)
            {
                RepositionGridElements();
            }
        }

        /// <summary>
        /// Changes the padding of the grid relative to its original padding.
        /// </summary>
        /// <param name="left">The left padding.</param>
        /// <param name="right">The right padding.</param>
        /// <param name="top">The top padding.</param>
        /// <param name="bottom">The bottom padding.</param>
        public void ChangeGridPaddingRelative(in int left, in int right, in int top, in int bottom)
        {
            ChangeGridPadding(GridPadding.Left + left, GridPadding.Right + right, GridPadding.Top + top, GridPadding.Bottom + bottom);
        }

        /// <summary>
        /// Gets the offset of the grid at a particular pivot.
        /// </summary>
        /// <param name="pivot">The pivot to get the grid offset for.</param>
        /// <returns>A Vector2 of the grid offset for the pivot.</returns>
        protected Vector2 GetOffsetFromPivot(in GridPivots pivot)
        {
            //Store these quick-access values, as this makes it more readable and easier to modify
            //The indices are converted to be zero-based
            float colRight = GridSize.X;
            float colCenter = (colRight / 2f);
            float rowBottom = GridSize.Y;
            float rowCenter = (rowBottom / 2f);

            switch (pivot)
            {
                case GridPivots.UpperCenter: return new Vector2(colCenter, 0f);
                case GridPivots.UpperRight: return new Vector2(colRight, 0f);
                case GridPivots.CenterLeft: return new Vector2(0f, rowCenter);
                case GridPivots.Center: return new Vector2(colCenter, rowCenter);
                case GridPivots.CenterRight: return new Vector2(colRight, rowCenter);
                case GridPivots.BottomLeft: return new Vector2(0f, rowBottom);
                case GridPivots.BottomCenter: return new Vector2(colCenter, rowBottom);
                case GridPivots.BottomRight: return new Vector2(colRight, rowBottom);
                case GridPivots.UpperLeft:
                default: return Vector2.Zero;
            }
        }

        /// <summary>
        /// Gets the offset for the grid elements at a particular pivot.
        /// </summary>
        /// <param name="pivot">The pivot to get the grid elements offset for.</param>
        /// <returns>A Vector2 of the grid elements offset for the pivot.</returns>
        protected Vector2 GetElementOffsetForPivot(in GridPivots pivot)
        {
            //Store these quick-access values, as this makes it more readable and easier to modify
            float xRight = CellSize.X;
            float xMid = xRight / 2f;
            float yBottom = CellSize.Y;
            float yMid = yBottom / 2f;

            switch (pivot)
            {
                case GridPivots.UpperCenter: return new Vector2(xMid, 0f);
                case GridPivots.UpperRight: return new Vector2(xRight, 0f);
                case GridPivots.CenterLeft: return new Vector2(0f, yMid);
                case GridPivots.Center: return new Vector2(xMid, yMid);
                case GridPivots.CenterRight: return new Vector2(xRight, yMid);
                case GridPivots.BottomLeft: return new Vector2(0f, yBottom);
                case GridPivots.BottomCenter: return new Vector2(xMid, yBottom);
                case GridPivots.BottomRight: return new Vector2(xRight, yBottom);
                case GridPivots.UpperLeft:
                default: return Vector2.Zero;
            }
        }

        /// <summary>
        /// Gets the spacing of a grid element at a column and row number for a pivot.
        /// </summary>
        /// <param name="column">The zero-based column number.</param>
        /// <param name="row">The zero-based row number.</param>
        /// <param name="pivot">The pivot to get the spacing for.</param>
        /// <returns>A Vector2 of the spacing of the grid element for the pivot.</returns>
        protected Vector2 GetSpacingAtColumnRow(in int column, in int row, in GridPivots pivot)
        {
            //Return on invalid input
            if (Columns <= 0 || Rows <= 0)
            {
                Console.WriteLine($"{nameof(Columns)}:{Columns} or {nameof(Rows)}:{Rows} is less than or equal to 0!");
                return Vector2.Zero;
            }
            else if (column < 0 || row < 0)
            {
                Console.WriteLine($"{nameof(column)}:{column} or {nameof(row)}:{row} is less than 0!");
                return Vector2.Zero;
            }

            Vector2 finalSpacing = Vector2.Zero;

            //The pivot column and pivot row
            //We offset the column and row by these to get the spacing
            //The further an element is from the pivot, the greater it will multiply the spacing value by
            float pivotCol = 0;
            float pivotRow = 0;

            //The column pivot for the left grid pivots is the leftmost element
            if (pivot == GridPivots.UpperLeft || pivot == GridPivots.CenterLeft || pivot == GridPivots.BottomLeft)
            {
                pivotCol = 0;
            }
            //The row pivot for the upper grid pivots is the uppermost element
            if (pivot == GridPivots.UpperLeft || pivot == GridPivots.UpperCenter || pivot == GridPivots.UpperRight)
            {
                pivotRow = 0;
            }

            //The column pivot for the right grid pivots is the rightmost element
            if (pivot == GridPivots.UpperRight || pivot == GridPivots.CenterRight || pivot == GridPivots.BottomRight)
            {
                pivotCol = (Columns - 1);
            }
            //The row pivot for the bottom grid pivots is the bottommost element
            if (pivot == GridPivots.BottomLeft || pivot == GridPivots.BottomCenter || pivot == GridPivots.BottomRight)
            {
                pivotRow = (Rows - 1);
            }

            //The column pivot for the center grid pivots is the centered element
            //In the event of an even number of columns, this will be halfway between the two middle columns (Ex: .5 for 4 columns)
            if (pivot == GridPivots.UpperCenter || pivot == GridPivots.Center || pivot == GridPivots.BottomCenter)
            {
                pivotCol = ((Columns - 1) / 2f);
            }
            //The row pivot for the center grid pivots is the centered element
            //In the event of an even number of rows, this will be halfway between the two middle rows (Ex: .5 for 4 rows)
            if (pivot == GridPivots.CenterLeft || pivot == GridPivots.Center || pivot == GridPivots.CenterRight)
            {
                pivotRow = ((Rows - 1) / 2f);
            }

            //Subtract from the pivot
            //The center two elements for even numbers of columns/rows will be separated from each other by half for centered pivots
            float pivotColDiff = (column - pivotCol);
            float pivotRowDiff = (row - pivotRow);

            finalSpacing.X = pivotColDiff * Spacing.X;
            finalSpacing.Y = pivotRowDiff * Spacing.Y;

            return finalSpacing;
        }

        public bool IsPointInGridElement(in int index, in Point point)
        {
            if (index < 0 || index >= GridElements.Count)
            {
                //Console.WriteLine($"index {index} is out of the grid's range!");
                return false;
            }

            Vector2 elementPos = GetPositionAtIndex(index);
            Rectangle elementRect = new Rectangle((int)elementPos.X, (int)elementPos.Y, (int)CellSize.X, (int)CellSize.Y);
            bool rectContainsPoint = elementRect.Contains(point);

            //Console.WriteLine($"Rect: {rect} | Point: {point} | Contains: {contains}");

            return rectContainsPoint;
        } 

        //NOTE: Use for debugging only
        public void DrawGridBounds(SpriteBatch spriteBatch, Texture2D tex, in Rectangle sourceRect)
        {
            for (int i = 0; i < MaxElementsInGrid; i++)
            {
                Vector2 child = GetPositionAtIndex(i);
                spriteBatch.DrawHollowRect(tex, sourceRect,
                    new Rectangle((int)child.X, (int)child.Y, (int)CellSize.X, (int)CellSize.Y), Color.White, .3f, 1);
            }
        }

        /// <summary>
        /// Padding used for the grid.
        /// </summary>
        public struct Padding
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;

            /// <summary>
            /// The total padding as a Vector2.
            /// </summary>
            public Vector2 TotalPadding => new Vector2(Right - Left, Bottom - Top);

            public Padding(in int left, in int right, in int top, in int bottom)
            {
                Left = left;
                Right = right;
                Top = top;
                Bottom = bottom;
            }
        }
    }
}
