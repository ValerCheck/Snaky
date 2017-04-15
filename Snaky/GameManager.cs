using System;
using System.Collections.Generic;
using System.Timers;

namespace Snaky
{
    public class GameManager
    {
        private int _mapWidth = 50;
        private int _mapHeight = 30;
        private int _refreshRate = 100;
        private int _gameInfoPanelHeight = 3;
        private Timer _timer;
        private Snake _snake;
        private Map _map;
        private List<GameCell> _cellsWithSnake = new List<GameCell>();
        private ISnakeGameVisualState<string> _gameState;
        private GameState _state = GameState.Start;
        private readonly object _cellsWithSnakeLock = new object();
        private Food _food;

        public delegate void StateChanged(string state);

        public StateChanged StateChangedEvent { get; set; }

        public GameState GameState => _state;

        private string VisualState => _gameState.State;

        public Map Map => _map;

        public Snake Snake => _snake;
        
        public GameManager(int areaWidth, int areaHeight)
        {
            _mapHeight = areaHeight - _gameInfoPanelHeight;
            _mapWidth = areaWidth;
            InitializeComponents();
            _gameState = new ConsoleSnakeGameState(this);
        }

        public void StartGame()
        {
            _timer.Enabled = true;
            _timer?.Start();
            _state = GameState.Pause;
            UpdateLogicState();
            UpdateVisualState();
            StateChangedEvent(VisualState);
            GameLoop();
        }

        private void GenerateFood()
        {
            var rand = new Random();

            if (_food?.HasBeenEaten ?? true)
            {
                var x = rand.Next(0, _mapWidth);
                var y = rand.Next(0, _mapHeight);
                _food = new Food(x, y);
                Map[y, x].ExtraItem = _food;
            }
        }

        private void GameLoop()
        {
            while (GameState != GameState.End)
            {
                HandleInput(Console.ReadKey());
            }
        }

        public void PauseGame()
        {
            _timer?.Stop();
            _state = GameState.Pause;
        }

        public void QuitGame()
        {
            _timer?.Stop();
            _state = GameState.End;
        }

        private void UpdateVisualState()
        {
            UpdateStateObject();
        }

        private void UpdateLogicState()
        {
            UpdateSnakePosition();
            UpdateFood();
            UpdateMap();
        }

        private void UpdateFood()
        {
            if (_food?.HasBeenEaten ?? true)
                GenerateFood();

            if (_food != null && Map[_food.Y, _food.X].IsSnakeOver)
            {
                _snake.Eat(_food);
                Map[_food.Y, _food.X].ExtraItem = null;
            }
        }

        private void UpdateStateObject()
        {
            _gameState.UpdateState();
        }

        private void UpdateSnakePosition()
        {
            _snake.Move();
        }

        private void UpdateMap()
        {
            lock (_cellsWithSnakeLock)
            {
                foreach (var cell in _cellsWithSnake)
                    cell.IsSnakeOver = false;
                _cellsWithSnake.Clear();

                var tail = _snake.Tail;

                do
                {
                    _map[tail.Y, tail.X].IsSnakeOver = true;
                    _cellsWithSnake.Add(_map[tail.Y, tail.X]);
                    tail = tail.Head;
                } while (tail?.HasHead ?? false);
            }
        }

        private void InitializeComponents()
        {
            _snake = new Snake();
            _snake.SetBounds(_mapWidth,_mapHeight);
            _map = new Map(_mapWidth,_mapHeight);
            _timer = new Timer(_refreshRate);
            _timer.Elapsed += (obj, args) =>
            {
                UpdateLogicState();
                UpdateVisualState();
                StateChangedEvent(VisualState);
            };
        }

        public void HandleInput(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    if (GameState == GameState.Pause)
                        StartGameAction();
                    break;
                case ConsoleKey.UpArrow:
                    if (GameState == GameState.GameLoop)
                        Snake.ChangeDirection(Direction.Vertical, -1);
                    break;
                case ConsoleKey.DownArrow:
                    if (GameState == GameState.GameLoop)
                        Snake.ChangeDirection(Direction.Vertical, 1);
                    break;
                case ConsoleKey.LeftArrow:
                    if (GameState == GameState.GameLoop)
                        Snake.ChangeDirection(Direction.Horizontal, -1);
                    break;
                case ConsoleKey.RightArrow:
                    if (GameState == GameState.GameLoop)
                        Snake.ChangeDirection(Direction.Horizontal, 1);
                    break;
                case ConsoleKey.Escape:
                    QuitGame();
                    break;
            }
        }

        private void StartGameAction()
        {
            _state = GameState.GameLoop;
            _snake.ChangeDirection(Direction.Horizontal, 1);
        }
    }
}
