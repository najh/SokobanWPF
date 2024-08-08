using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Sokoban
{
    //Class used to draw the game state to the form canvas.
    class Graphics
    {
        //Define game rows/columns
        internal int SIZE = 10;

        //Variables to hold the calculated play area size.
        private readonly int CELL_SIZE;
        private readonly int BOX_SIZE;
        private readonly int GOAL_SIZE;
        private readonly int PLAYER_SIZE;

        //Colours
        private readonly Brush BRUSH_GRID = Brushes.LightSteelBlue;
        private readonly Brush BRUSH_WALL = Brushes.SlateGray;
        private readonly Brush BRUSH_BOX = Brushes.RosyBrown;
        private readonly Brush BRUSH_GOAL = Brushes.LightSeaGreen;
        private readonly Brush BRUSH_CHAR = Brushes.PaleVioletRed;

        //Canvas for graphics and label for move count.
        private readonly Canvas canvas;
        private readonly Label label;

        internal Graphics(Canvas canvas, Label label)
        {
            //Accept form elements and calculate scale of play area accordingly.
            this.canvas = canvas;
            this.label = label;
            CELL_SIZE = (int)(canvas.Width / 10);
            BOX_SIZE = (int)(CELL_SIZE * 0.75);
            GOAL_SIZE = (int)(CELL_SIZE * 0.5);
            PLAYER_SIZE = (int)(CELL_SIZE * 0.35);
        }
        
        internal void Draw(GameData data)
        {
            //Wipe canvas clean and draw grid.
            canvas.Children.Clear();
            for(int i = 0; i < SIZE; i++)
            {
                DrawGridLineVertical(i);
                DrawGridLineHorizontal(i);
            }
            DrawGridLineVertical(SIZE);
            DrawGridLineHorizontal(SIZE);

            //Draw map
            for (int i = 0; i < SIZE; i++)
                for (int j = 0; j < SIZE; j++)
                    if (data.MapData[i, j] == CellType.WALL)
                        DrawSquare(i, j, CELL_SIZE, BRUSH_WALL);

            //Draw boxes, then goals.
            foreach (var element in data.ElementData.Where(e => e.Type == ElementType.BOX))
                DrawSquare(element.Position_Y, element.Position_X, BOX_SIZE, BRUSH_BOX);
            foreach (var element in data.ElementData.Where(e => e.Type == ElementType.GOAL))
                DrawGoal(element.Position_Y, element.Position_X);

            //Draw player and update move count.
            DrawPlayer(data.PlayerY, data.PlayerX);
            label.Content = string.Format("Moves: {0}", data.Moves);
        }

        private void DrawLine(Brush colour, int X1, int Y1, int X2, int Y2)
        {
            //Draws a line of a specific colour from X1Y1 to X2Y2
            var line = new Line();
            line.Stroke = colour;
            line.StrokeThickness = 2;
            line.X1 = X1;
            line.Y1 = Y1;
            line.X2 = X2;
            line.Y2 = Y2;
            canvas.Children.Add(line);
        }

        private void DrawGridLineHorizontal(int row)
        {
            //Draws a line across the width of the canvas at a specific row.
            DrawLine(BRUSH_GRID, 0, CELL_SIZE * row, CELL_SIZE * SIZE, CELL_SIZE * row);
        }

        private void DrawGridLineVertical(int column)
        {
            //Draws a line across the height of the canvas at a specific column.
            DrawLine(BRUSH_GRID, CELL_SIZE * column, 0, CELL_SIZE * column, CELL_SIZE * SIZE);
        }

        private void DrawGoal(int row, int column)
        {
            //Draws an diamond inside of the specified row/column.
            DrawSquare(row, column, GOAL_SIZE, BRUSH_GOAL, 45);
            
        }

        private void DrawSquare(int row, int column, int size, Brush colour, int angle = 0)
        {
            //Draws a square inside the specified row/column of a specific size, colour and rotation.
            var rect = new Rectangle();
            rect.Fill = rect.Stroke = colour;
            rect.StrokeThickness = 0;
            rect.Width = rect.Height = size;
            rect.RenderTransformOrigin = new Point(0.5, 0.5);
            rect.RenderTransform = new RotateTransform(angle);
            canvas.Children.Add(rect);
            var offset = (CELL_SIZE - size) / 2;
            Canvas.SetTop(rect, CELL_SIZE * row + offset);
            Canvas.SetLeft(rect, CELL_SIZE * column + offset);
        }

        private void DrawPlayer(int row, int column)
        {
            //Draws a square for the player at a specified row/column.
            DrawSquare(row, column, PLAYER_SIZE, BRUSH_CHAR);
        }
    }
}
