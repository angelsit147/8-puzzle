using _8Puzzles.Logging;
using _8Puzzles.Solvers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _8Puzzles
{
    /// <summary>
    /// Форма для гри 8-пазлів, що керує інтерфейсом користувача та логікою гри.
    /// </summary>
    public partial class PuzzlesForm : Form
    {
        /// <summary>
        /// Об’єкт дошки гри, що керує кнопками, таймером і лічильником ходів.
        /// </summary>
        private GameBoard gameBoard;

        /// <summary>
        /// Об’єкт солвера, що відповідає за логіку перемішування, перевірку ходів і розв’язність.
        /// </summary>
        private PuzzleSolver puzzleSolver;

        /// <summary>
        /// Прапорець, що вказує, чи виконується автоматичне розв’язання головоломки.
        /// </summary>
        private bool IsSolving = false;

        /// <summary>
        /// Логер для збереження результатів розв’язання у файл.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// Ініціалізує нову форму гри, налаштовує компоненти дошки, солвер, логер і обробники подій.
        /// </summary>
        public PuzzlesForm()
        {
            InitializeComponent();
            /// <summary>
            /// Масив кнопок, що представляють клітинки дошки гри (1–8 і порожня клітинка).
            /// </summary>
            var buttons = new Button[] { button1, button2, button3, button4, button5, button6, button7, button8, button9 };

            gameBoard = new GameBoard(buttons, swapsLabel, timerLabel);
            puzzleSolver = new PuzzleSolver(gameBoard);
            gameBoard.EnableGameButtons(true);
            /// <summary>
            /// Комбобокс для вибору типу перемішування (випадкове або ручне).
            /// </summary>
            shuffleTypeBox.DropDownStyle = ComboBoxStyle.DropDownList;
            shuffleTypeBox.SelectedIndex = 0;
            /// <summary>
            /// Комбобокс для вибору методу розв’язання (LDFS, BFS, IDS).
            /// </summary>
            methodsBox.DropDownStyle = ComboBoxStyle.DropDownList;
            methodsBox.SelectedIndex = 0;
            gameBoard.ClearLabels();

            foreach (var button in buttons)
            {
                button.Click += (object sender, EventArgs e) => HandleButtonClick(button);
            }

            logger = new TextLogger("SolverResults.txt");
        }

        /// <summary>
        /// Завантажує форму, виконує випадкове перемішування дошки та очищає мітки.
        /// </summary>
        /// <param name="sender">Об’єкт, що викликав подію (форма).</param>
        /// <param name="e">Аргументи події завантаження форми.</param>
        private void PuzzlesForm_Load(object sender, EventArgs e)
        {
            puzzleSolver.RandomShuffle();
            gameBoard.ClearLabels();
            RestoreButtonStates();
        }

        /// <summary>
        /// Обробляє натискання кнопки на дошці, виконує хід, якщо він валідний, і перевіряє розв’язання.
        /// </summary>
        /// <param name="clickedButton">Кнопка, на яку натиснув користувач.</param>
        private void HandleButtonClick(Button clickedButton)
        {
            if (IsSolving || gameBoard.IsManualShuffling || gameBoard.UserWon)
            {
                return;
            }

            if (puzzleSolver.IsValidMove(clickedButton))
            {
                puzzleSolver.MoveTile(clickedButton);
                gameBoard.SaveCurrentState();
                gameBoard.StartTimerIfFirstMove();
                puzzleSolver.SolutionChecker();
                RestoreButtonStates();
            }
        }

        /// <summary>
        /// Скидає гру до початкового стану після підтвердження користувача.
        /// </summary>
        /// <param name="sender">Об’єкт, що викликав подію (кнопка скидання).</param>
        /// <param name="e">Аргументи події натискання кнопки.</param>
        private void resetButton_Click(object sender, EventArgs e)
        {
            if (gameBoard.Counter == 0 && !gameBoard.UserWon)
            {
                MessageBox.Show("8-puzzle already has an initial placement.", "Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                /// <summary>
                /// Результат діалогу підтвердження скидання гри.
                /// </summary>
                DialogResult iReset = MessageBox.Show("Confirm if you want to reset your solution", "Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (iReset == DialogResult.Yes)
                {
                    gameBoard.ResetGame();
                    gameBoard.UserWon = false;
                    gameBoard.EnableGameButtons(true);
                    RestoreButtonStates();
                }
            }
        }

        /// <summary>
        /// Починає нову гру з вибраним типом перемішування (випадкове або ручне).
        /// </summary>
        /// <param name="sender">Об’єкт, що викликав подію (кнопка нової гри).</param>
        /// <param name="e">Аргументи події натискання кнопки.</param>
        private void newGameButton_Click(object sender, EventArgs e)
        {
            if (shuffleTypeBox.SelectedIndex <= 0)
            {
                MessageBox.Show("Please, select shuffle type.", "New Game", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (gameBoard.IsManualShuffling)
                {
                    /// <summary>
                    /// Кнопка для запуску гри після ручного введення конфігурації.
                    /// </summary>
                    startGameButton.Visible = true;
                    gameBoard.EnableGameButtons(true);
                }
                else
                {
                    startGameButton.Visible = false;
                    gameBoard.IsManualShuffling = false;
                    gameBoard.EnableGameButtons(false);
                    RestoreButtonStates();
                }
            }
            else
            {
                /// <summary>
                /// Вибраний тип перемішування (Random shuffle або Shuffle by hand).
                /// </summary>
                string selectedType = shuffleTypeBox.SelectedItem.ToString();
                if (selectedType == "Random shuffle")
                {
                    gameBoard.UserWon = false;
                    gameBoard.IsManualShuffling = false;
                    gameBoard.ResetHistory();
                    gameBoard.Counter = 0;
                    puzzleSolver.RandomShuffle();
                    gameBoard.SaveCurrentState();
                    gameBoard.EnableGameButtons(true);
                    startGameButton.Visible = false;
                    shuffleTypeBox.SelectedIndex = 0;
                    RestoreButtonStates();
                }
                else if (selectedType == "Shuffle by hand")
                {
                    gameBoard.UserWon = false;
                    gameBoard.IsManualShuffling = true;
                    gameBoard.ShuffleByHand(startGameButton);
                    shuffleTypeBox.SelectedIndex = 0;
                    DisableButtonsForManualShuffling();
                }
                gameBoard.ResetTimer();
                gameBoard.ClearLabels();
            }
        }

        /// <summary>
        /// Запускає гру після ручного введення, перевіряючи валідність і розв’язність конфігурації.
        /// </summary>
        /// <param name="sender">Об’єкт, що викликав подію (кнопка запуску гри).</param>
        /// <param name="e">Аргументи події натискання кнопки.</param>
        private void startGameButton_Click(object sender, EventArgs e)
        {
            if (gameBoard.IsManualInputValid())
            {
                /// <summary>
                /// Поточний стан дошки у вигляді масиву рядків.
                /// </summary>
                string[] currentBoard = gameBoard.GetCurrentBoard();
                if (puzzleSolver.IsSolved())
                {
                    MessageBox.Show("The entered configuration is already solved. Please, enter a different arrangement.", "Invalid Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    gameBoard.ClearGameButtons();
                    return;
                }
                if (puzzleSolver.IsSolvable(currentBoard))
                {
                    gameBoard.SaveCurrentState();
                    gameBoard.IsManualShuffling = false;
                    gameBoard.UserWon = false;
                    gameBoard.RemoveKeyPressHandlers();
                    startGameButton.Visible = false;
                    gameBoard.EnableGameButtons(true);
                    gameBoard.ResetTimer();
                    gameBoard.ClearLabels();
                    RestoreButtonStates();
                }
                else
                {
                    MessageBox.Show("The configuration you entered has no solution. Please, try again or choose 'Random shuffle'.", "Unsolvable Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    gameBoard.ClearGameButtons();
                }
            }
            else
            {
                MessageBox.Show("Please, enter numbers 1-8 exactly once and leave one button empty.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Скасовує останній хід, повертаючи попередній стан дошки.
        /// </summary>
        /// <param name="sender">Об’єкт, що викликав подію (кнопка скасування).</param>
        /// <param name="e">Аргументи події натискання кнопки.</param>
        private void previousButton_Click(object sender, EventArgs e)
        {
            gameBoard.UndoMove();
            gameBoard.EnableGameButtons(true);
            RestoreButtonStates();
        }

        /// <summary>
        /// Виходить із програми після підтвердження користувача.
        /// </summary>
        /// <param name="sender">Об’єкт, що викликав подію (кнопка виходу).</param>
        /// <param name="e">Аргументи події натискання кнопки.</param>
        private void exitButton_Click(object sender, EventArgs e)
        {
            /// <summary>
            /// Результат діалогу підтвердження виходу з програми.
            /// </summary>
            DialogResult iExit = MessageBox.Show("Confirm if you want to exit", "8-puzzle", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (iExit == DialogResult.Yes)
            {
                Application.ExitThread();
            }
        }

        /// <summary>
        /// Обробляє закриття форми, запитуючи підтвердження виходу.
        /// </summary>
        /// <param name="sender">Об’єкт, що викликав подію (форма).</param>
        /// <param name="e">Аргументи події закриття форми.</param>
        private void PuzzlesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            /// <summary>
            /// Результат діалогу підтвердження виходу з програми.
            /// </summary>
            DialogResult iExit = MessageBox.Show("Confirm if you want to exit", "8-puzzle", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (iExit == DialogResult.No)
            {
                /// <summary>
                /// Прапорець для скасування закриття форми.
                /// </summary>
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Розв’язує головоломку вибраним методом (LDFS, BFS, IDS) і відображає анімацію рішення.
        /// </summary>
        /// <param name="sender">Об’єкт, що викликав подію (кнопка розв’язання).</param>
        /// <param name="e">Аргументи події натискання кнопки.</param>
        private async void solveButton_Click(object sender, EventArgs e)
        {
            /// <summary>
            /// Вибраний метод розв’язання (LDFS, BFS, IDS).
            /// </summary>
            string selectedMethod = methodsBox.SelectedItem?.ToString();

            if (selectedMethod == null || selectedMethod == "Choose a method")
            {
                MessageBox.Show("Please, select a solving method.", "Solve", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (puzzleSolver.IsSolved())
            {
                MessageBox.Show("The puzzle is already solved!", "Solve", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (puzzleSolver.IsSolvable(gameBoard.GetCurrentBoard()))
            {
                /// <summary>
                /// Прапорець, що вказує, чи було знайдено розв’язок.
                /// </summary>
                bool solved = false;
                IsSolving = true;
                DisableButtonsForAutomatedSolving();

                /// <summary>
                /// Початковий стан дошки у вигляді матриці чисел.
                /// </summary>
                (int[,] initialState, int _, int _) = gameBoard.GetCurrentState();
                /// <summary>
                /// Секундомір для вимірювання часу виконання алгоритму.
                /// </summary>
                Stopwatch solverTimer = Stopwatch.StartNew();
                /// <summary>
                /// Кінцевий стан дошки після розв’язання.
                /// </summary>
                int[,] finalState = null;
                /// <summary>
                /// Кількість ходів, виконаних солвером.
                /// </summary>
                int solverSwapCount = 0;
                /// <summary>
                /// Кількість ходів, виконаних користувачем вручну.
                /// </summary>
                int manualSwapCount = gameBoard.Counter;

                /// <summary>
                /// Об’єкт солвера, що реалізує вибраний алгоритм (LDFS, BFS, IDS).
                /// </summary>
                ISolver solver = null;
                if (selectedMethod == "LDFS")
                {
                    solver = new LDFSSolver(31, 500);
                }
                else if (selectedMethod == "BFS")
                {
                    solver = new BFSSolver(500);
                }
                else if (selectedMethod == "IDS")
                {
                    solver = new IDSSolver(500);
                }

                try
                {
                    /// <summary>
                    /// Поточний стан дошки та координати порожньої клітинки.
                    /// </summary>
                    (int[,] start, int x, int y) = gameBoard.GetCurrentState();
                    /// <summary>
                    /// Кількість ітерацій алгоритму та список станів для розв’язку.
                    /// </summary>
                    (int iterationCount, List<int[,]> states) = solver.SolvePuzzle(start, y, x);
                    solverTimer.Stop();
                    if (states.Count == 0)
                    {
                        solved = false;
                    }
                    else
                    {
                        solverSwapCount = states.Count - 1;
                        finalState = states[states.Count - 1];
                        gameBoard.ResetTimer();
                        await Task.Run(async () =>
                        {
                            /// <summary>
                            /// Прапорець, що вказує, чи є поточний стан першим у списку.
                            /// </summary>
                            bool isFirstState = true;
                            foreach (var state in states)
                            {
                                Invoke((Action)(() =>
                                {
                                    gameBoard.UpdateState(state);
                                    if (!isFirstState)
                                    {
                                        gameBoard.Counter++;
                                        gameBoard.UpdateCounterDisplay();
                                    }
                                }));
                                isFirstState = false;
                                await Task.Delay(solver.RenderDelay);
                            }
                        });
                        solved = true;
                    }

                    if (solved)
                    {
                        logger.Log(selectedMethod, solverTimer.Elapsed.TotalMilliseconds, solverSwapCount, iterationCount, initialState, finalState);

                        MessageBox.Show($"Puzzle solved using {selectedMethod}!", "Solve", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        gameBoard.UserWon = true;
                        methodsBox.SelectedIndex = 0;
                    }
                    else
                    {
                        gameBoard.Counter = manualSwapCount;
                        MessageBox.Show($"Could not find a solution using {selectedMethod}.", "Solve", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during solving: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    solverTimer.Stop();
                    IsSolving = false;
                    gameBoard.IsManualShuffling = false;
                    RestoreButtonStates();
                }
            }
            else
            {
                MessageBox.Show("The current puzzle configuration is not solvable.", "Unsolvable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RestoreButtonStates();
            }
        }

        /// <summary>
        /// Вимикає кнопки під час ручного перемішування, дозволяючи вводити конфігурацію.
        /// </summary>
        private void DisableButtonsForManualShuffling()
        {
            /// <summary>
            /// Кнопка для скидання гри до початкового стану.
            /// </summary>
            resetButton.Enabled = false;
            /// <summary>
            /// Кнопка для скасування останнього ходу.
            /// </summary>
            previousButton.Enabled = false;
            /// <summary>
            /// Кнопка для запуску автоматичного розв’язання.
            /// </summary>
            solveButton.Enabled = false;
            methodsBox.Enabled = false;
            startGameButton.Enabled = true;
            startGameButton.Visible = true;
            gameBoard.EnableGameButtons(true);
        }

        /// <summary>
        /// Вимикає кнопки під час автоматичного розв’язання головоломки.
        /// </summary>
        private void DisableButtonsForAutomatedSolving()
        {
            /// <summary>
            /// Кнопка для початку нової гри.
            /// </summary>
            newGameButton.Enabled = false;
            resetButton.Enabled = false;
            previousButton.Enabled = false;
            solveButton.Enabled = false;
            startGameButton.Enabled = false;
            shuffleTypeBox.Enabled = false;
            methodsBox.Enabled = false;
            gameBoard.EnableGameButtons(false);
        }

        /// <summary>
        /// Відновлює стан кнопок залежно від поточного стану гри.
        /// </summary>
        private void RestoreButtonStates()
        {
            newGameButton.Enabled = true;
            resetButton.Enabled = true;
            shuffleTypeBox.Enabled = true;
            startGameButton.Enabled = false;
            startGameButton.Visible = false;

            previousButton.Enabled = !gameBoard.UserWon;
            solveButton.Enabled = !gameBoard.UserWon;
            methodsBox.Enabled = !gameBoard.UserWon;
            gameBoard.EnableGameButtons(!gameBoard.UserWon);
        }
    }
}