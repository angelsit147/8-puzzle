using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace _8Puzzles
{
    /// <summary>
    /// Керує дошкою гри 8-пазлів, включаючи кнопки, таймер, лічильник ходів і історію станів.
    /// </summary>
    internal class GameBoard
    {
        /// <summary>
        /// Масив кнопок, що представляють клітинки дошки гри (1–8 і порожня клітинка).
        /// </summary>
        private readonly Button[] buttons;

        /// <summary>
        /// Мітка для відображення кількості ходів.
        /// </summary>
        private readonly Label swapsLabel;

        /// <summary>
        /// Мітка для відображення часу гри.
        /// </summary>
        private readonly Label timerLabel;

        /// <summary>
        /// Секундомір для відстеження часу гри.
        /// </summary>
        private Stopwatch stopwatch;

        /// <summary>
        /// Таймер для періодичного оновлення мітки часу.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Стек для збереження історії станів дошки.
        /// </summary>
        private Stack<string[]> history = new Stack<string[]>();

        /// <summary>
        /// Лічильник кількості ходів у грі.
        /// </summary>
        public int Counter { get; set; }

        /// <summary>
        /// Прапорець, що вказує, чи виконується ручне перемішування.
        /// </summary>
        public bool IsManualShuffling { get; set; }

        /// <summary>
        /// Прапорець, що вказує, чи виграв користувач.
        /// </summary>
        public bool UserWon { get; set; }

        /// <summary>
        /// Ініціалізує дошку гри, кнопки, мітки та таймер.
        /// </summary>
        /// <param name="buttons">Масив кнопок, що представляють клітинки дошки.</param>
        /// <param name="swapsLabel">Мітка для відображення кількості ходів.</param>
        /// <param name="timerLabel">Мітка для відображення часу гри.</param>
        public GameBoard(Button[] buttons, Label swapsLabel, Label timerLabel)
        {
            this.buttons = buttons;
            this.swapsLabel = swapsLabel;
            this.timerLabel = timerLabel;
            stopwatch = new Stopwatch();
            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// Оновлює відображення часу на мітці таймера.
        /// </summary>
        /// <param name="sender">Об’єкт, що викликав подію (таймер).</param>
        /// <param name="e">Аргументи події таймера.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            /// <summary>
            /// Поточний час, що минув від початку гри.
            /// </summary>
            TimeSpan elapsed = stopwatch.Elapsed;
            timerLabel.Text = $"Time: {elapsed.Minutes:D2}:{elapsed.Seconds:D2}.{elapsed.Milliseconds / 100}";
        }

        /// <summary>
        /// Запускає таймер, якщо це перший хід і гравець ще не виграв.
        /// </summary>
        public void StartTimerIfFirstMove()
        {
            if (!stopwatch.IsRunning && !UserWon)
            {
                stopwatch.Start();
                timer.Start();
            }
        }

        /// <summary>
        /// Запускає таймер гри.
        /// </summary>
        public void StartTimer()
        {
            stopwatch.Start();
            timer.Start();
        }

        /// <summary>
        /// Зупиняє таймер гри.
        /// </summary>
        public void StopTimer()
        {
            stopwatch.Stop();
            timer.Stop();
        }

        /// <summary>
        /// Скидає таймер і оновлює мітку часу.
        /// </summary>
        public void ResetTimer()
        {
            stopwatch.Reset();
            timer.Stop();
            timerLabel.Text = "Time: 00:00.0";
        }

        /// <summary>
        /// Зберігає поточний стан дошки в історії.
        /// </summary>
        public void SaveCurrentState()
        {
            /// <summary>
            /// Масив рядків, що представляє поточний стан дошки.
            /// </summary>
            string[] currentState = new string[9];
            for (int i = 0; i < 9; i++)
            {
                currentState[i] = buttons[i].Text;
            }
            history.Push(currentState);
        }

        /// <summary>
        /// Скидає гру до початкового стану з історії.
        /// </summary>
        public void ResetGame()
        {
            if (history.Count > 0)
            {
                UserWon = false;
                /// <summary>
                /// Початковий стан дошки, отриманий із історії.
                /// </summary>
                string[] initialState = history.ToArray()[history.Count - 1];
                for (int i = 0; i < 9; i++)
                {
                    buttons[i].Text = initialState[i];
                }
                history.Clear();
                SaveCurrentState();
                Counter = 0;
                ResetTimer();
                UpdateCounterDisplay();
                ClearLabels();
            }
            else
            {
                MessageBox.Show("History is empty.", "Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Налаштовує дошку для ручного перемішування.
        /// </summary>
        /// <param name="startGameButton">Кнопка для запуску гри після введення конфігурації.</param>
        public void ShuffleByHand(Button startGameButton)
        {
            EnableGameButtons(true);
            ClearGameButtons();
            history.Clear();
            Counter = 0;
            ResetTimer();
            UpdateCounterDisplay();
            MessageBox.Show("Please, click on the buttons and enter the numbers (1-8 and one empty).", "Shuffle by hand", MessageBoxButtons.OK, MessageBoxIcon.Information);
            foreach (var button in buttons)
            {
                button.KeyPress += GameButton_KeyPress;
            }
            IsManualShuffling = true;
        }

        /// <summary>
        /// Перевіряє валідність ручного введення (числа 1-8 і один порожній елемент).
        /// </summary>
        /// <returns>Повертає true, якщо введення валідне, інакше false.</returns>
        public bool IsManualInputValid()
        {
            /// <summary>
            /// Список текстів кнопок для перевірки введення.
            /// </summary>
            List<string> numbers = new List<string>();
            foreach (var button in buttons)
            {
                numbers.Add(button.Text);
            }
            /// <summary>
            /// Кількість порожніх клітинок на дошці.
            /// </summary>
            int emptyCount = numbers.Count(s => s == "");
            if (emptyCount != 1)
            {
                return false;
            }
            for (int i = 1; i <= 8; i++)
            {
                if (!numbers.Contains(i.ToString()))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Повертає поточний стан дошки як масив рядків.
        /// </summary>
        /// <returns>Масив рядків, що представляє тексти кнопок дошки.</returns>
        public string[] GetCurrentBoard()
        {
            /// <summary>
            /// Масив рядків для збереження поточного стану дошки.
            /// </summary>
            string[] currentBoard = new string[9];
            for (int i = 0; i < 9; i++)
            {
                currentBoard[i] = buttons[i].Text;
            }
            return currentBoard;
        }

        /// <summary>
        /// Повертає поточний стан дошки як матрицю та координати порожньої клітинки.
        /// </summary>
        /// <returns>Кортеж із матрицею стану дошки та координатами порожньої клітинки (X, Y).</returns>
        public (int[,], int, int) GetCurrentState()
        {
            /// <summary>
            /// Матриця 3x3 для представлення стану дошки.
            /// </summary>
            int[,] state = new int[3, 3];
            /// <summary>
            /// Координата X порожньої клітинки (стовпець).
            /// </summary>
            int emptyX = -1;
            /// <summary>
            /// Координата Y порожньої клітинки (рядок).
            /// </summary>
            int emptyY = -1;
            for (int i = 0; i < buttons.Length; i++)
            {
                /// <summary>
                /// Номер рядка для поточної кнопки.
                /// </summary>
                int row = i / 3;
                /// <summary>
                /// Номер стовпця для поточної кнопки.
                /// </summary>
                int column = i % 3;
                /// <summary>
                /// Текст поточної кнопки.
                /// </summary>
                var buttonText = buttons[i].Text;
                if (string.IsNullOrEmpty(buttonText))
                {
                    emptyX = column;
                    emptyY = row;
                }

                state[row, column] = string.IsNullOrEmpty(buttonText) ? 0 : int.Parse(buttonText);
            }

            return (state, emptyX, emptyY);
        }

        /// <summary>
        /// Очищає текст на всіх кнопках дошки.
        /// </summary>
        public void ClearGameButtons()
        {
            foreach (var button in buttons)
            {
                button.Text = "";
            }
        }

        /// <summary>
        /// Вмикає або вимикає кнопки дошки.
        /// </summary>
        /// <param name="enable">Прапорець для увімкнення (true) або вимкнення (false) кнопок.</param>
        public void EnableGameButtons(bool enable)
        {
            foreach (var button in buttons)
            {
                button.Enabled = enable;
            }
        }

        /// <summary>
        /// Оновлює відображення лічильника ходів на мітці.
        /// </summary>
        public void UpdateCounterDisplay()
        {
            swapsLabel.Text = "Number of swaps: " + Counter;
        }

        /// <summary>
        /// Очищає мітки ходів і таймера.
        /// </summary>
        public void ClearLabels()
        {
            swapsLabel.Text = "Number of swaps: 0";
            timerLabel.Text = "Time: 00:00.0";
        }

        /// <summary>
        /// Скасовує останній хід, повертаючи попередній стан дошки.
        /// </summary>
        public void UndoMove()
        {
            if (history.Count > 1)
            {
                history.Pop();
                /// <summary>
                /// Попередній стан дошки, отриманий із історії.
                /// </summary>
                string[] previousState = history.Peek();
                for (int i = 0; i < 9; i++)
                {
                    buttons[i].Text = previousState[i];
                }
            }
            else
            {
                MessageBox.Show("Move history is empty.", "Previous", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Очищає історію ходів.
        /// </summary>
        public void ResetHistory()
        {
            history.Clear();
        }

        /// <summary>
        /// Видаляє обробники подій натискання клавіш із кнопок.
        /// </summary>
        public void RemoveKeyPressHandlers()
        {
            foreach (var button in buttons)
            {
                button.KeyPress -= GameButton_KeyPress;
            }
        }

        /// <summary>
        /// Обробляє введення чисел у кнопки під час ручного перемішування.
        /// </summary>
        /// <param name="sender">Об’єкт, що викликав подію (кнопка).</param>
        /// <param name="e">Аргументи події натискання клавіші.</param>
        private void GameButton_KeyPress(object sender, KeyPressEventArgs e)
        {
            /// <summary>
            /// Кнопка, на яку натиснуто клавішу.
            /// </summary>
            Button clickedButton = (Button)sender;
            if (char.IsDigit(e.KeyChar) && clickedButton.Text.Length < 1)
            {
                clickedButton.Text = e.KeyChar.ToString();
            }
            else if (e.KeyChar == '\b')
            {
                clickedButton.Text = "";
            }
            e.Handled = true;
        }

        /// <summary>
        /// Оновлює стан дошки на основі заданої матриці.
        /// </summary>
        /// <param name="state">Матриця 3x3, що представляє новий стан дошки.</param>
        public void UpdateState(int[,] state)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                /// <summary>
                /// Номер рядка для поточної кнопки.
                /// </summary>
                int row = i / 3;
                /// <summary>
                /// Номер стовпця для поточної кнопки.
                /// </summary>
                int column = i % 3;
                buttons[i].Text = state[row, column] == 0 ? "" : state[row, column].ToString();
            }
        }
    }
}