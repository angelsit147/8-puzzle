using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace _8Puzzles
{
    internal class PuzzleSolver
    {
        private readonly GameBoard gameBoard;
        private readonly Button[] buttons;

        // Конструктор: Ініціалізує солвер із дошкою гри та копіює масив кнопок
        public PuzzleSolver(GameBoard gameBoard)
        {
            this.gameBoard = gameBoard;
            var actualButtons = gameBoard.GetType().GetField("buttons", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(gameBoard) as Button[];
            buttons = new Button[actualButtons.Length];
            Array.Copy(actualButtons, buttons, actualButtons.Length);
        }

        // Виконує випадкове перемішування дошки
        public void RandomShuffle()
        {
            buttons[0].Text = "1";
            buttons[1].Text = "2";
            buttons[2].Text = "3";
            buttons[3].Text = "4";
            buttons[4].Text = "5";
            buttons[5].Text = "6";
            buttons[6].Text = "7";
            buttons[7].Text = "8";
            buttons[8].Text = "";

            Random random = new Random();
            int numberOfShuffles = random.Next(100, 200);

            for (int i = 0; i < numberOfShuffles; i++)
            {
                List<Button> possibleMoves = GetPossibleMoves();
                if (possibleMoves.Count > 0)
                {
                    int randomIndex = random.Next(possibleMoves.Count);
                    Button buttonToMove = possibleMoves[randomIndex];
                    MoveTile(buttonToMove);
                }
            }

            gameBoard.SaveCurrentState();
            gameBoard.Counter = 0;
            gameBoard.UpdateCounterDisplay();
            gameBoard.ResetTimer();
            gameBoard.ClearLabels();
        }

        // Перевіряє, чи розв’язана головоломка, і сповіщає про перемогу
        public void SolutionChecker()
        {
            if (IsSolved())
            {
                gameBoard.StopTimer();
                MessageBox.Show("Congratulations! You won!", "8-puzzle", MessageBoxButtons.OK, MessageBoxIcon.Information);
                gameBoard.UserWon = true;
            }
        }

        // Перевіряє, чи є хід валідним
        public bool IsValidMove(Button clickedButton)
        {
            List<Button> possibleMoves = GetPossibleMoves();
            return possibleMoves.Contains(clickedButton);
        }

        // Виконує хід, переміщаючи плитку на порожнє місце
        public void MoveTile(Button clickedButton)
        {
            if (clickedButton != null && IsValidMove(clickedButton))
            {
                EmptySpotChecker(clickedButton, GetEmptySpot());
            }
        }

        // Перевіряє, чи дошка в розв’язаному стані
        public bool IsSolved()
        {
            return buttons[0].Text == "1" && buttons[1].Text == "2" && buttons[2].Text == "3" &&
                   buttons[3].Text == "4" && buttons[4].Text == "5" && buttons[5].Text == "6" &&
                   buttons[6].Text == "7" && buttons[7].Text == "8" && buttons[8].Text == "";
        }

        // Перевіряє, чи конфігурація дошки має розв’язок
        public bool IsSolvable(string[] board)
        {
            int inversions = 0;
            int n = board.Length;

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (board[i] != "" && board[j] != "")
                    {
                        try
                        {
                            int value1 = int.Parse(board[i]);
                            int value2 = int.Parse(board[j]);
                            if (value1 > value2)
                            {
                                inversions++;
                            }
                        }
                        catch (FormatException)
                        {
                            return false;
                        }
                    }
                }
            }
            return (inversions % 2 == 0);
        }

        // Переміщує плитку на порожнє місце та оновлює лічильник ходів
        private void EmptySpotChecker(Button button1, Button button2)
        {
            if (button2 != null && button2.Text == "")
            {
                button2.Text = button1.Text;
                button1.Text = "";
                gameBoard.Counter++;
                gameBoard.UpdateCounterDisplay();
            }
        }

        // Повертає список можливих ходів для порожньої клітинки
        private List<Button> GetPossibleMoves()
        {
            List<Button> possibleMoves = new List<Button>();
            Button emptySpot = GetEmptySpot();
            if (emptySpot == null) return possibleMoves;

            int emptyIndex = Array.IndexOf(buttons, emptySpot);
            int row = emptyIndex / 3;
            int col = emptyIndex % 3;

            if (row > 0)
                possibleMoves.Add(buttons[(row - 1) * 3 + col]);
            if (row < 2)
                possibleMoves.Add(buttons[(row + 1) * 3 + col]);
            if (col > 0)
                possibleMoves.Add(buttons[row * 3 + (col - 1)]);
            if (col < 2)
                possibleMoves.Add(buttons[row * 3 + (col + 1)]);

            return possibleMoves;
        }

        // Повертає кнопку, що відповідає порожній клітинці
        private Button GetEmptySpot()
        {
            return buttons.FirstOrDefault(b => b.Text == "");
        }
    }
}
