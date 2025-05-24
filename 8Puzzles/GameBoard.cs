using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace _8Puzzles
{
    internal class GameBoard
    {
        private readonly Button[] buttons;
        private readonly Label swapsLabel;
        private readonly Label timerLabel;
        private Stopwatch stopwatch;
        private Timer timer;
        private Stack<string[]> history = new Stack<string[]>();

        public int Counter { get; set; }
        public bool IsManualShuffling { get; set; }
        public bool UserWon { get; set; }

        // Конструктор: Ініціалізує дошку гри, кнопки, мітки та таймер
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

        // Оновлює відображення часу на мітці таймера
        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = stopwatch.Elapsed;
            timerLabel.Text = $"Time: {elapsed.Minutes:D2}:{elapsed.Seconds:D2}.{elapsed.Milliseconds / 100}";
        }

        // Запускає таймер, якщо це перший хід і гравець ще не виграв
        public void StartTimerIfFirstMove()
        {
            if (!stopwatch.IsRunning && !UserWon)
            {
                stopwatch.Start();
                timer.Start();
            }
        }

        // Запускає таймер гри
        public void StartTimer()
        {
            stopwatch.Start();
            timer.Start();
        }

        // Зупиняє таймер гри
        public void StopTimer()
        {
            stopwatch.Stop();
            timer.Stop();
        }

        // Скидає таймер і оновлює мітку часу
        public void ResetTimer()
        {
            stopwatch.Reset();
            timer.Stop();
            timerLabel.Text = "Time: 00:00.0";
        }

        // Зберігає поточний стан дошки в історії
        public void SaveCurrentState()
        {
            string[] currentState = new string[9];
            for (int i = 0; i < 9; i++)
            {
                currentState[i] = buttons[i].Text;
            }
            history.Push(currentState);
        }

        // Скидає гру до початкового стану з історії
        public void ResetGame()
        {
            if (history.Count > 0)
            {
                UserWon = false;
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

        // Налаштовує дошку для ручного перемішування
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

        // Перевіряє валідність ручного введення (числа 1-8 і один порожній елемент)
        public bool IsManualInputValid()
        {
            List<string> numbers = new List<string>();
            foreach (var button in buttons)
            {
                numbers.Add(button.Text);
            }
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

        // Повертає поточний стан дошки як масив рядків
        public string[] GetCurrentBoard()
        {
            string[] currentBoard = new string[9];
            for (int i = 0; i < 9; i++)
            {
                currentBoard[i] = buttons[i].Text;
            }
            return currentBoard;
        }

        // Повертає поточний стан дошки як матрицю та координати порожньої клітинки
        public (int[,], int, int) GetCurrentState()
        {
            int[,] state = new int[3, 3];
            int emptyX = -1;
            int emptyY = -1;
            for (int i = 0; i < buttons.Length; i++)
            {
                int row = i / 3;
                int column = i % 3;
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

        // Очищає текст на всіх кнопках дошки
        public void ClearGameButtons()
        {
            foreach (var button in buttons)
            {
                button.Text = "";
            }
        }

        // Вмикає або вимикає кнопки дошки
        public void EnableGameButtons(bool enable)
        {
            foreach (var button in buttons)
            {
                button.Enabled = enable;
            }
        }

        // Оновлює відображення лічильника ходів
        public void UpdateCounterDisplay()
        {
            swapsLabel.Text = "Number of swaps: " + Counter;
        }

        // Очищає мітки ходів і таймера
        public void ClearLabels()
        {
            swapsLabel.Text = "Number of swaps: 0";
            timerLabel.Text = "Time: 00:00.0";
        }

        // Скасовує останній хід, повертаючи попередній стан
        public void UndoMove()
        {
            if (history.Count > 1)
            {
                history.Pop();
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

        // Очищає історію ходів
        public void ResetHistory()
        {
            history.Clear();
        }

        // Видаляє обробники подій натискання клавіш із кнопок
        public void RemoveKeyPressHandlers()
        {
            foreach (var button in buttons)
            {
                button.KeyPress -= GameButton_KeyPress;
            }
        }

        // Обробляє введення чисел у кнопки під час ручного перемішування
        private void GameButton_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        // Оновлює стан дошки на основі заданої матриці
        public void UpdateState(int[,] state)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                int row = i / 3;
                int column = i % 3;
                buttons[i].Text = state[row, column] == 0 ? "" : state[row, column].ToString();
            }
        }
    }
}
