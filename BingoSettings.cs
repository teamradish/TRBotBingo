/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * TRBotBingo
 */

using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace TRBotBingo
{
    public class BingoSettings
    {
        public int BingoColumns = 5;
        public int BingoRows = 5;
        public Vector2 BingoBoardCellSize = Vector2.Zero;
        public Vector2 BingoBoardPosOffset = Vector2.Zero;
        public Vector2 BingoBoardSpacing = Vector2.Zero;
        public string BingoPipeFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BingoPipe");
        public Vector2? WindowSize = null;
        public double FPS = 15d;
    }
}
