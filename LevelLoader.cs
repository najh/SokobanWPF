using System.Collections.Generic;
using System.IO;

namespace Sokoban
{
    //Parses level data and initialises game state.
    class LevelLoader
    {
        internal GameData Load(int level, int size)
        {
            //Search for a level file of the provided index.
            var fileName = string.Format("level{0}.txt", level);
            var fi = new FileInfo(fileName);
            if (!fi.Exists) return null;

            //Read the level file data and validate against expected size.
            var data = File.ReadAllLines(fi.FullName);
            if (data.Length != size) return null;
            
            //Initialise values for the game, then read in individual rows from data.
            var playerX = -1;
            var playerY = -1;
            var mapData = new CellType[10, 10];
            var elementData = new List<GameElement>();
            for(int i = 0; i < 10; i++)
            {
                //Validate row length (columns) against expected size.
                var row = data[i];
                if (row.Length != size) return null;

                //Iterate columns.
                for (int j = 0; j < 10; j++)
                {
                    //Determine contents of each column in the row and build game state accordingly.
                    var cell = row[j];
                    CellType ct;
                    switch(cell)
                    {
                        case '░':   //Nothing, empty.
                            ct = CellType.EMPTY;
                            break;
                        case '█':   //A solid wall.
                            ct = CellType.WALL;
                            break;
                        case '#':   //The player starting position.
                            ct = CellType.EMPTY;
                            playerX = j;
                            playerY = i;
                            break;
                        case '+':   //A box.
                            ct = CellType.EMPTY;
                            elementData.Add(new GameElement()
                            {
                                Type = ElementType.BOX,
                                Position_Y = i,
                                Position_X = j,
                            });
                            break;
                        case '×':   //A goal position.
                            ct = CellType.EMPTY;
                            elementData.Add(new GameElement()
                            {
                                Type = ElementType.GOAL,
                                Position_Y = i,
                                Position_X = j,
                            });
                            break;
                        case '*':   //Both a box and a goal position.
                            ct = CellType.EMPTY;
                            elementData.Add(new GameElement()
                            {
                                Type = ElementType.BOX,
                                Position_Y = i,
                                Position_X = j,
                            });
                            elementData.Add(new GameElement()
                            {
                                Type = ElementType.GOAL,
                                Position_Y = i,
                                Position_X = j,
                            });
                            break;
                        default:    //Unknown/Invalid
                            ct = CellType.EMPTY;
                            break;
                    }
                    mapData[i, j] = ct;
                }
            }
            //Ensure that the player position is valid, then return all parsed data.
            if (playerX == -1 || playerY == -1) return null;
            var gameData = new GameData()
            {
                MapData = mapData,
                ElementData = elementData,
                PlayerX = playerX,
                PlayerY = playerY,
                Moves = 0
            };
            return gameData;
        }
    }
}
