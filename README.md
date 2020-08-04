# TRBotBingo
A simple Bingo board written with [MonoGame](https://github.com/MonoGame/MonoGame) primarily to interface with [TRBot](https://github.com/teamradish/TRTwitchPlaysBot). 

Using TRBot's `BingoCommand`, you can mark bingo tiles on the bingo board. Alternatively, you can simply click on them to mark them.

## Configuration
Upon starting TRBotBingo, a `BingoConfig.txt` file is created in the same directory as the executable. All the options are detailed below:

* BingoColumns - How many columns are on the bingo board
* BingoRows - How many rows are on the bingo board
* BingoBoardCellSize - The size of each tile on the bingo board
* BingoBoardPosOffset - The initial position of the bingo board
* BingoBoardSpacing - How far apart each tile on the bingo board is spaced from one another
* BingoPipeFilePath - The path to the pipe file that TRBot can use to interact with the bingo board. This is created automatically. The default is the same folder as the executable.
* WindowSize - The size of the window for the bingo game.
* FPS - How fast to run the bingo game in FPS. The board isn't demanding, so only set this higher if you want updates to reflect faster. Recommended: 15

Right-click to toggle showing outlines of the bingo board's tiles. This is handy for choosing the values you need for your board.

Feel free to replace **BingoBoard.png** and **Mark.png** in the Content folder with your own assets. If you change their names, make sure to change the references to the new names in the **Content.mgcb** file, either with a text editor or the MonoGame Pipeline Tool.

## Building
Prerequisites:
* Install [MonoGame](https://github.com/MonoGame/MonoGame). TRBotBingo uses the DesktopGL template.
* Clone the repo with `git clone https://github.com/teamradish/TRBotBingo.git`
  * Alternatively, download the zip.
* [.NET Core 3.1 SDK and Runtime](https://dotnet.microsoft.com/download/dotnet-core)
  * Before installing, set the `DOTNET_CLI_TELEMETRY_OPTOUT` environment variable to 1 if you don't want dotnet CLI commands sending telemetry.

Command line:
* Main directory: `cd TRBotBingo`
* Building: `dotnet build`
* Publishing: `dotnet publish -c (config) -o (dir) --self-contained --runtime (RID)`
  * config = "Debug" or "Release"
  * dir = output directory
  * [RID](https://github.com/dotnet/runtime/blob/master/src/libraries/pkg/Microsoft.NETCore.Platforms/runtime.json) = usually "win-x64" or "linux-x64". See link for a full list of runtime identifiers.
  * Example: `dotnet publish -c Debug -o TRBotBingo --self-contained --runtime linux-x64`

## License
TRBotBingo is licensed under the [Mozilla Public License 2.0](https://www.mozilla.org/en-US/MPL/2.0/).

See the [LICENSE](https://github.com/teamradish/TRBotBingo/blob/master/LICENSE) file for the full terms. See the [Dependency Licenses](https://github.com/teamradish/TRBotBingo/blob/master/Dependency%20Licenses) file for the licenses of third party libraries used by TRBotBingo.
