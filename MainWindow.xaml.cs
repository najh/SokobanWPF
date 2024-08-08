using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sokoban
{
    public partial class MainWindow : Window
    {
        private SokobanGame sokoban;

        public MainWindow()
        {
            InitializeComponent();
            //Create a new game and pass in necessary controls.
            sokoban = new SokobanGame(gameCanvas, movesLabel);
        }

        private void clickHandler(object sender, RoutedEventArgs e)
        {
            //Trigger game input on button clicks.
            var clickedButton = (Button)sender;
            TriggerAction(clickedButton.Name);  //Fills "overloaded method" requirement.
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //Handle keyboard input.
            switch(e.Key)
            {
                case Key.Left:
                    TriggerAction(Input.LEFT);
                    break;
                case Key.Up:
                    TriggerAction(Input.UP);
                    break;
                case Key.Right:
                    TriggerAction(Input.RIGHT);
                    break;
                case Key.Down:
                    TriggerAction(Input.DOWN);
                    break;
                case Key.R:
                    TriggerAction(Input.RESTART);
                    break;
            }
        }

        private void TriggerAction(Input action)
        {
            //Handle game input (enum)
            switch (action)
            {
                case Input.LEFT:
                    sokoban.Move(-1, 0);
                    break;
                case Input.UP:
                    sokoban.Move(0, -1);
                    break;
                case Input.RIGHT:
                    sokoban.Move(1, 0);
                    break;
                case Input.DOWN:
                    sokoban.Move(0, 1);
                    break;
                case Input.RESTART:
                    sokoban.Restart();
                    break;
            }
        }
        private void TriggerAction(string action)
        {
            //Handle game input (string)
            switch (action)
            {
                case "LEFT":
                    TriggerAction(Input.LEFT);
                    break;
                case "UP":
                    TriggerAction(Input.UP);
                    break;
                case "RIGHT":
                    TriggerAction(Input.RIGHT);
                    break;
                case "DOWN":
                    TriggerAction(Input.DOWN);
                    break;
                case "RESTART":
                    TriggerAction(Input.RESTART);
                    break;
            }
        }
    }
}
