using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace _8Puzzles
{
    /// <summary>
    /// Керує логікою гри 8-пазлів, включаючи перемішування, перевірку ходів і розв’язність.
    /// </summary>
    internal class PuzzleSolver
    {
        /// <summary>
        /// Об’єкт дошки гри, що містить кнопки та керує станом гри.
        /// </summary>
        private readonly GameBoard gameBoard;

        /// <summary>
        /// Масив кнопок, що представляють клітинки дошки гри (1–8 і порожня клітинка).
        /// </summary>
        private readonly Button[] buttons;

        /// <summary>
        /// Ініціалізує солвер із дошкою гри та копіює масив кнопок.
        /// </summary>
        /// <param name="gameBoard">Об’єкт дошки гри для взаємодії з інтерфейсом.</param>
        public PuzzleSolver(GameBoard gameBoard)
        {
            this.gameBoard = gameBoard;
            /// <summary>
            /// Масив кнопок, отриманий із приватного поля gameBoard через рефлексію.
            /// </summary>
            var actualButtons = gameBoard.GetType().GetField("buttons", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(gameBoard) as Button[];
            buttons = new Button[actualButtons.Length];
            Array.Copy(actualButtons, buttons, actualButtons.Length);
        }

        /// <summary>
        /// Виконує випадкове перемішування дошки шляхом виконання випадкових ходів.
        /// </summary>
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

            /// <summary>
            /// Генератор випадкових чисел для вибору ходів і кількості перемішувань.
            /// </summary>
            Random random = new Random();
            /// <summary>
            /// Кількість випадкових ходів для перемішування (від 100 до 199).
            /// </summary>
            int numberOfShuffles = random.Next(100, 200);

            for (int i = 0; i < numberOfShuffles; i++)
            {
                /// <summary>
                /// Список кнопок, які можна перемістити на порожню клітинку.
                /// </summary>
                List<Button> possibleMoves = GetPossibleMoves();
                if (possibleMoves.Count > 0)
                {
                    /// <summary>
                    /// Випадковий індекс для вибору кнопки з можливих ходів.
                    /// </summary>
                    int randomIndex = random.Next(possibleMoves.Count);
                    /// <summary>
                    /// Кнопка, яка буде переміщена на порожню клітинку.
                    /// </summary>
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

        /// <summary>
        /// Перевіряє, чи розв’язана головоломка, і сповіщає про перемогу користувача.
        /// </summary>
        public void SolutionChecker()
        {
            if (IsSolved())
            {
                gameBoard.StopTimer();
                MessageBox.Show("Congratulations! You won!", "8-puzzle", MessageBoxButtons.OK, MessageBoxIcon.Information);
                gameBoard.UserWon = true;
            }
        }

        /// <summary>
        /// Перевіряє, чи є хід валідним для вибраної кнопки.
        /// </summary>
        /// <param name="clickedButton">Кнопка, на яку натиснув користувач.</param>
        /// <returns>Повертає true, якщо хід валідний, інакше false.</returns>
        public bool IsValidMove(Button clickedButton)
        {
            /// <summary>
            /// Список кнопок, які можна перемістити на порожню клітинку.
            /// </summary>
            List<Button> possibleMoves = GetPossibleMoves();
            return possibleMoves.Contains(clickedButton);
        }

        /// <summary>
        /// Виконує хід, переміщаючи плитку на порожнє місце, якщо хід валідний.
        /// </summary>
        /// <param name="clickedButton">Кнопка, що відповідає плитці для переміщення.</param>
        public void MoveTile(Button clickedButton)
        {
            if (clickedButton != null && IsValidMove(clickedButton))
            {
                EmptySpotChecker(clickedButton, GetEmptySpot());
            }
        }

        /// <summary>
        /// Перевіряє, чи дошка перебуває в розв’язаному стані.
        /// </summary>
        /// <returns>Повертає true, якщо дошка розв’язана, інакше false.</returns>
        public bool IsSolved()
        {
            return buttons[0].Text == "1" && buttons[1].Text == "2" && buttons[2].Text == "3" &&
                   buttons[3].Text == "4" && buttons[4].Text == "5" && buttons[5].Text == "6" &&
                   buttons[6].Text == "7" && buttons[7].Text == "8" && buttons[8].Text == "";
        }

        /// <summary>
        /// Перевіряє, чи конфігурація дошки має розв’язок на основі кількості інверсій.
        /// </summary>
        /// <param name="board">Масив рядків, що представляє поточний стан дошки.</param>
        /// <returns>Повертає true, якщо конфігурація розв’язна, інакше false.</returns>
        public bool IsSolvable(string[] board)
        {
            /// <summary>
            /// Лічильник інверсій у послідовності чисел на дошці.
            /// </summary>
            int inversions = 0;
            /// <summary>
            /// Довжина масиву, що представляє дошку (9 елементів).
            /// </summary>
            int n = board.Length;

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (board[i] != "" && board[j] != "")
                    {
                        try
                        {
                            /// <summary>
                            /// Число, отримане з тексту кнопки за індексом i.
                            /// </summary>
                            int value1 = int.Parse(board[i]);
                            /// <summary>
                            /// Число, отримане з тексту кнопки за індексом j.
                            /// </summary>
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

        /// <summary>
        /// Переміщує плитку на порожнє місце та оновлює лічильник ходів.
        /// </summary>
        /// <param name="button1">Кнопка, що відповідає плитці для переміщення.</param>
        /// <param name="button2">Кнопка, що відповідає порожній клітинці.</param>
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

        /// <summary>
        /// Повертає список кнопок, які можна перемістити на порожню клітинку.
        /// </summary>
        /// <returns>Список кнопок, що представляють можливі ходи.</returns>
        private List<Button> GetPossibleMoves()
        {
            /// <summary>
            /// Список кнопок, які можна перемістити на порожню клітинку.
            /// </summary>
            List<Button> possibleMoves = new List<Button>();
            /// <summary>
            /// Кнопка, що відповідає порожній клітинці.
            /// </summary>
            Button emptySpot = GetEmptySpot();
            if (emptySpot == null) return possibleMoves;

            /// <summary>
            /// Індекс порожньої клітинки в масиві кнопок.
            /// </summary>
            int emptyIndex = Array.IndexOf(buttons, emptySpot);
            /// <summary>
            /// Номер рядка порожньої клітинки (0–2).
            /// </summary>
            int row = emptyIndex / 3;
            /// <summary>
            /// Номер стовпця порожньої клітинки (0–2).
            /// </summary>
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

        /// <summary>
        /// Повертає кнопку, що відповідає порожній клітинці.
        /// </summary>
        /// <returns>Кнопка порожньої клітинки або null, якщо такої немає.</returns>
        private Button GetEmptySpot()
        {
            return buttons.FirstOrDefault(b => b.Text == "");
        }
    }
}