using _8Puzzles.Logging;
using _8Puzzles.Solvers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _8Puzzles
{
    public partial class PuzzlesForm : Form
    {
        private GameBoard gameBoard;
        private PuzzleSolver puzzleSolver;
        private bool IsSolving = false;
        private ILogger logger;

        // Конструктор: Ініціалізує форму гри, компоненти дошки, солвер, логер і налаштовує обробники подій
        public PuzzlesForm()
        {
            InitializeComponent();
            var buttons = new Button[] { button1, button2, button3, button4, button5, button6, button7, button8, button9 };

            gameBoard = new GameBoard(buttons, swapsLabel, timerLabel);
            puzzleSolver = new PuzzleSolver(gameBoard);
            gameBoard.EnableGameButtons(true);
            shuffleTypeBox.DropDownStyle = ComboBoxStyle.DropDownList;
            shuffleTypeBox.SelectedIndex = 0;
            methodsBox.DropDownStyle = ComboBoxStyle.DropDownList;
            methodsBox.SelectedIndex = 0;
            gameBoard.ClearLabels();

            foreach (var button in buttons)
            {
                button.Click += (object sender, EventArgs e) => HandleButtonClick(button);
            }

            logger = new TextLogger("SolverResults.txt");
        }

        // Завантажує форму, ініціалізує гру випадковим перемішуванням і очищає мітки.
        private void PuzzlesForm_Load(object sender, EventArgs e)
        {
            puzzleSolver.RandomShuffle();
            gameBoard.ClearLabels();
            RestoreButtonStates();
        }

        // Обробляє натискання кнопки на ігровій дошці, виконує хід, якщо він валідний.
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

        // Скидає гру до початкового стану після підтвердження користувача.
        private void resetButton_Click(object sender, EventArgs e)
        {
            if (gameBoard.Counter == 0 && !gameBoard.UserWon)
            {
                MessageBox.Show("8-puzzle already has an initial placement.", "Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
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

        // Починає нову гру з вибраним типом перемішування(випадкове або ручне)
        private void newGameButton_Click(object sender, EventArgs e)
        {
            if (shuffleTypeBox.SelectedIndex <= 0)
            {
                MessageBox.Show("Please, select shuffle type.", "New Game", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (gameBoard.IsManualShuffling)
                {
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

        // Запускає гру після ручного введення, перевіряє валідність і розв’язність конфігурації.
        private void startGameButton_Click(object sender, EventArgs e)
        {
            if (gameBoard.IsManualInputValid())
            {
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

        // Скасовує останній хід, повертаючи попередній стан гри.
        private void previousButton_Click(object sender, EventArgs e)
        {
            gameBoard.UndoMove();
            gameBoard.EnableGameButtons(true);
            RestoreButtonStates();
        }

        // Виходить із програми після підтвердження користувача.
        private void exitButton_Click(object sender, EventArgs e)
        {
            DialogResult iExit = MessageBox.Show("Confirm if you want to exit", "8-puzzle", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (iExit == DialogResult.Yes)
            {
                Application.ExitThread();
            }
        }

        // Обробляє закриття форми, запитуючи підтвердження виходу.
        private void PuzzlesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult iExit = MessageBox.Show("Confirm if you want to exit", "8-puzzle", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (iExit == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        // Розв’язує головоломку вибраним методом (LDFS, BFS, IDS) і відображає анімацію рішення.
        private async void solveButton_Click(object sender, EventArgs e)
        {
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
                bool solved = false;
                IsSolving = true;
                DisableButtonsForAutomatedSolving();

                (int[,] initialState, int _, int _) = gameBoard.GetCurrentState();
                Stopwatch solverTimer = Stopwatch.StartNew();
                int[,] finalState = null;
                int solverSwapCount = 0;
                int manualSwapCount = gameBoard.Counter;

                ISolver solver = null;
                if (selectedMethod == "LDFS")
                {
                    solver = new LDFSSolver(50, 500);
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
                    (int[,] start, int x, int y) = gameBoard.GetCurrentState();
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

        // Вимикає кнопки під час ручного перемішування, дозволяючи вводити конфігурацію.
        private void DisableButtonsForManualShuffling()
        {
            resetButton.Enabled = false;
            previousButton.Enabled = false;
            solveButton.Enabled = false;
            methodsBox.Enabled = false;
            startGameButton.Enabled = true;
            startGameButton.Visible = true;
            gameBoard.EnableGameButtons(true);
        }

        // Вимикає кнопки під час автоматичного розв’язання головоломки.
        private void DisableButtonsForAutomatedSolving()
        {
            newGameButton.Enabled = false;
            resetButton.Enabled = false;
            previousButton.Enabled = false;
            solveButton.Enabled = false;
            startGameButton.Enabled = false;
            shuffleTypeBox.Enabled = false;
            methodsBox.Enabled = false;
            gameBoard.EnableGameButtons(false);
        }

        // Відновлює стан кнопок залежно від поточного стану гри.
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
