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
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private BingoBoard bingoBoard = null;
        private BingoSettings BingoConfig = null;

        private string ConfigFile = "./BingoConfig.txt";

        private const int IPC_THREAD_SLEEP = 100;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 635;
            _graphics.PreferredBackBufferHeight = 835;

            //Fixed timestep to enforce the framerate
            IsFixedTimeStep = true;
        }

        protected override void Initialize()
        {
            //Load the bingo config file or create one if it doesn't exist
            CreateOrLoadBingoConfig();

            //Change window size
            if (BingoConfig.WindowSize != null)
            {
                Vector2 windowSize = BingoConfig.WindowSize.Value;

                _graphics.PreferredBackBufferWidth = (int)windowSize.X;
                _graphics.PreferredBackBufferHeight = (int)windowSize.Y;
            }

            //Set to the desired FPS value; don't go below 1
            if (BingoConfig.FPS < 1d)
            {
                BingoConfig.FPS = 1d;
            }

            double val = Math.Round(1d / BingoConfig.FPS, 7);

            //Create from ticks for precision
            TargetElapsedTime = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond * val));

            //Start a new thread for the bingo IPC (inter-process communication) server
            //This will mark bingo tiles using data from another process, such as TRBot
            ThreadPool.QueueUserWorkItem(new WaitCallback(ListenForStringData));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Texture2D boardTex = Content.Load<Texture2D>("BingoBoard");
            Texture2D markTex = Content.Load<Texture2D>("Mark");

            //new Vector2(115, 115)
            bingoBoard = new BingoBoard(BingoConfig.BingoColumns, BingoConfig.BingoRows, BingoConfig.BingoBoardCellSize, boardTex, markTex);
            //bingoBoard.BingoGrid.ChangeGridPadding(-10, 0, -200 - 10, 0);
            bingoBoard.BingoGrid.Position = BingoConfig.BingoBoardPosOffset;
            bingoBoard.BingoGrid.Spacing = new Vector2(10, 10);
            bingoBoard.SetDebugSquare(Content.Load<Texture2D>("DebugSquare"));

            bingoBoard.Initialize();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            Content.Unload();

            base.UnloadContent();
        }

        private void CreateOrLoadBingoConfig()
        {
            //Look for a config file and create it if it doesn't exist
            if (File.Exists(ConfigFile) == false)
            {
                BingoConfig = new BingoSettings();
                string text = JsonConvert.SerializeObject(BingoConfig, Formatting.Indented);

                if (string.IsNullOrEmpty(text) == false)
                {
                    try
                    {
                        File.WriteAllText(ConfigFile, text);

                        Console.WriteLine("Created Bingo config!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Unable to save Bingo config file: {e.Message}");
                    }
                }
            }
            else
            {
                try
                {
                    string text = File.ReadAllText(ConfigFile);
                    BingoConfig = JsonConvert.DeserializeObject<BingoSettings>(text);

                    Console.WriteLine($"Loaded Bingo config at {Path.GetFullPath(ConfigFile)}!");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to load Bingo config file: {e.Message}. Defaulting to a default config.");
                    
                    BingoConfig = new BingoSettings(); 
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            bingoBoard.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Render the board
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
            bingoBoard.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Starts up a local named pipe server to listen for connections from local external processes.
        /// The data received is used to mark bingo tiles.
        /// </summary>
        private void ListenForStringData(object data)
        {
            using NamedPipeServerStream namedPipeServer = new NamedPipeServerStream(BingoConfig.BingoPipeFilePath, PipeDirection.In);

            //Wait for a client to connect
            Console.WriteLine($"Bingo pipe started at \"{BingoConfig.BingoPipeFilePath}\". Now available for clients (Ex. TRBot)!");
            
            //Keep looking for connections
            while (true)
            {
                namedPipeServer.WaitForConnection();

                //Console.WriteLine("Client connected!");

                try
                {
                    //Read the input
                    //It's important that leaveOpen is true for the StreamReader, otherwise the pipe would be disposed when it is
                    using (StreamReader streamReader = new StreamReader(namedPipeServer, Encoding.UTF8, leaveOpen:true))
                    {
                        string bingoText = streamReader.ReadLine();
                        bingoBoard.ToggleMarkBingoTile(bingoText);
                    }
                }
                // Catch the IOException that is raised if the pipe is broken
                // or disconnected.
                catch (IOException e)
                {
                    Console.WriteLine($"ERROR: {e.Message}");
                }
                finally
                {
                    //Close the pipe after processing the data so we can accept a new client
                    namedPipeServer.Disconnect();
                }

                //Suspend the thread for a little after disconnecting to avoid high CPU usage
                Thread.Sleep(IPC_THREAD_SLEEP);
            }
        }
    }
}
