using System.Collections.Generic;

namespace Sokoban
{
    //A class to hold the current game state.
    //Includes map wall data, player position, current moves, whether or not we've won and a list of elements.
    internal class GameData
    {
        public CellType[,] MapData;
        public int PlayerX;
        public int PlayerY;
        public int Moves;
        public bool Won;
        public List<GameElement> ElementData;
    }
}
