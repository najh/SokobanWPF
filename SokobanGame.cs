using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Sokoban
{
    class SokobanGame
    {

        //Variables to hold the various parts of the game.
        private Graphics graphics;
        private GameData data;
        private LevelLoader levelLoader;

        //Current level count
        private int level = 0;
        
        //Dispatcher for threaded access to the main UI, used to avoid issues with timing.
        private Dispatcher dispatcher;

        public SokobanGame(Canvas canvas, Label label)
        {
            //Configure graphics and load into the current (first) level.
            graphics = new Graphics(canvas, label);
            levelLoader = new LevelLoader();
            dispatcher = canvas.Dispatcher;
            Load();
        }

        private void Load()
        {
            //Pass level count to level loader and Draw the resulting game state.
            data = levelLoader.Load(level, graphics.SIZE);
            DrawGame();
        }

        private void NextLevel()
        {
            //Increment the current level count and load the resulting level.
            level++;
            Load();
        }

        internal void DrawGame()
        {
            //Ensure that we've got something to draw, then pass it to graphics to handle rendering.
            if (data == null) return;
            graphics.Draw(data);
        }

        internal void Move(int x, int y)
        {
            //Ensure we have osmething to play, and are not waiting for the end of the previous level.
            if (data == null) return;
            if (data.Won) return;

            //The target cell is the current position + our movement X/Y
            var cellX = data.PlayerX + x;
            var cellY = data.PlayerY + y;

            //If the target area is a wall, quit early.
            if (data.MapData[cellY, cellX] == CellType.WALL) return;

            //Variable to determine illegal moves.
            bool moveBlocked = false;

            //Iterate over elements and interact with them if necessary.
            for(int i = 0; i < data.ElementData.Count; i++)
            {
                var element = data.ElementData[i];
                //If the current element is in our target cell, interact with it.
                if (element.Position_X == cellX && element.Position_Y == cellY)
                {
                    switch (element.Type)
                    {
                        case ElementType.BOX:
                            //We check the next cell along after our target cell for emptiness.
                            var adjacentSpaceX = cellX + x;
                            var adjacentSpaceY = cellY + y;
                            
                            //If it's empty, check for other boxes.
                            if (data.MapData[adjacentSpaceY, adjacentSpaceX] == CellType.EMPTY)
                            {
                                if (HasBox(adjacentSpaceY, adjacentSpaceX))
                                {
                                    //If a box occupies the target space, block the move.
                                    moveBlocked = true;
                                }
                                else
                                {
                                    //If the adjacent space is clear, move the box into it and update its position in game state.
                                    element.Position_X = adjacentSpaceX;
                                    element.Position_Y = adjacentSpaceY;
                                    data.ElementData[i] = element;
                                }
                            }
                            else
                            {
                                //If it's not empty, block the move.
                                moveBlocked = true;
                            }
                            break;
                    }
                }
            }

            //If the move wasn't blocked, move the player into the new position.
            if(!moveBlocked)
                MovePlayerTo(cellY, cellX);

            //Determine victory state by iterating all goals and boxes, comparing positions for equality.
            var goals = data.ElementData.Where(e => e.Type == ElementType.GOAL);
            var boxes = data.ElementData.Where(b => b.Type == ElementType.BOX);
            var goalCount = goals.Count();
            var filledGoals = goals.Intersect(boxes, new GoalComparer()).Count();

            if (goalCount == filledGoals)
            {
                //If we have reached the victory state, pause the game by raising the "Won" flag.
                //This ignores inputs until the next level is loaded, delayed by 2 seconds.
                data.Won = true;
                var t = new Thread(() =>
                {
                    Thread.Sleep(2000);
                    dispatcher.Invoke(new Action(() => NextLevel()));   //Avoid threading issues.
                });
                t.Start();
            }

            //Refresh the interface.
            DrawGame();
        }

        private void MovePlayerTo(int row, int column)
        {
            //Move the player to a position and increment move count.
            data.PlayerX = column;
            data.PlayerY = row;
            data.Moves++;
        }
        
        private bool HasBox(int row, int column)
        {
            //Checks for any boxes in the specified row/column.
            return data.ElementData.Where(e => e.Type == ElementType.BOX && e.Position_X == column && e.Position_Y == row).Count() > 0;
        }

        internal void Restart()
        {
            //Invokes the level load to discard progress.
            Load();
        }
    }

    class GoalComparer : IEqualityComparer<GameElement>
    {
        public bool Equals(GameElement x, GameElement y)
        {
            //Return true if a space contains both a Goal and a Box, else false.
            return y.Type == ElementType.GOAL &&
                x.Type == ElementType.BOX &&
                x.Position_X == y.Position_X &&
                x.Position_Y == y.Position_Y;
        }

        public int GetHashCode(GameElement obj)
        {
            return -1;  //Unused.
        }
    }
}
