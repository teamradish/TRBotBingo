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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TRBotBingo
{
    /// <summary>
    /// An item on a grid.
    /// </summary>
    public class GridItem
    {
        public Vector2 Position = Vector2.Zero;
        public bool Marked = false;

        public GridItem(in Vector2 position, in bool marked)
        {
            Position = position;
            Marked = marked;
        }
    }
}