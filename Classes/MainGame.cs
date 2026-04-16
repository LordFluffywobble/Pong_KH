using System.Diagnostics;

public sealed class MainGame
{
    // grid/board
    private int _width;
    private int _height;

    // "core" game fields
    private Paddle? _leftPaddle;
    private Paddle? _rightPaddle;
    private Ball? _ball;
    private Renderer? _renderer;
    private InputHandler? _inputHandler;

    // score & game state
    private int _leftSideScore;
    private int _rightSideScore;
    private bool _gameStateIsRunning;

    private int _cpuDelayTick;

    public MainGame(int width, int height)
    {
        if (width < 40)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "The width cannot be less than 40 pixels");
        }
        if (height < 15)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "The heigth cannot be lower 15 pixels of the terminal screen.");
        }

        _width = width;
        _height = height;

        // we can set the heigh & width of the paddles & the center bar
        int paddleHeight = 4;
        int centerY = height / 2 - paddleHeight / 2;

        _leftPaddle = new Paddle(x: 2, y: centerY, height: paddleHeight);
        _rightPaddle = new Paddle(x: width - 3, y: centerY, height: paddleHeight);

        _ball = new Ball(width / 2, height / 2, velocityX: -1, velocityY: -1);

        _renderer = new Renderer(width, height);
        _inputHandler = new InputHandler();
    }

    // Run the main game loop

    /// <summary>
    /// Run the main game loop
    /// </summary>
    public void RunGame()
    {
        // first we initalize the console
        InitalizeConsole();
        // we then show the start screen
        ShowStartScreen();
        // we then set the _gameStateIsRunning field to true
        _gameStateIsRunning = true;
        // set a target FPS
        const int targetFPS = 30;
        // set up a target "time" for our FPS to try to hit
        TimeSpan frameTime = TimeSpan.FromMilliseconds(1000.0 / targetFPS);
        // set up a stopwatch
        Stopwatch stopWatch = Stopwatch.StartNew();
        // get the previous "ticks"
        long getPreviousTicks = stopWatch.ElapsedMilliseconds;

        while (_gameStateIsRunning)
        {
            // handle collision and properly draw the game map (in our current build: the collision is not handled when the ball falls below the X-axis)
            EnsureConsoleIsLargeEnough();
            // get the current tick
            long currentTicks = stopWatch.ElapsedMilliseconds;
            // get the current milliseconds
            long elapedMilliseconds = currentTicks - getPreviousTicks;

            if (elapedMilliseconds < frameTime.TotalMilliseconds)
            {
                // set the current CPU thread to "sleep" for 1 millisecond
                Thread.Sleep(1);
                continue;
            }
            // set the previous ticks capture to the current captured ticks
            getPreviousTicks = currentTicks;
            // handle the user input
            HandleInput();
            // Update the ball position
            Update();
            // Render the game
            Render();
        }
    }



    // Helper methods, private scope, visible only inside this class

    /// <summary>
    /// Helper method used for initalizing the console
    /// </summary>
    private void InitalizeConsole()
    {
        _width = Math.Max(40, Console.WindowWidth);
        _height = Math.Max(15, Console.WindowHeight - 1);
        // Clear the console feed.
        Console.Clear();
    }

    /// <summary>
    /// Create all the game objects and place them properly on the terminal screen
    /// </summary>
    private void CreateGameObjects()
    {
        // create the paddle objects
        int paddleHeight = 4;
        int centerY = _height / 2 - paddleHeight  / 2;

        _leftPaddle = new Paddle(x: 2, y: centerY, height: paddleHeight);
        _rightPaddle = new Paddle(x: _width - 3, y: centerY, height: paddleHeight);

        // create the ball object
        _ball = new Ball(
            x: _width / 2,
            y: _height / 2,
            velocityX: -1,
            velocityY: -1
        );

        _renderer = new Renderer(width: _width, height: _height);
    }

    /// <summary>
    /// A helper method that attempts to verify if the console window is large enough for the game or not, if it is not large enough, we can let the user resize it inside the method
    /// </summary>
    private void EnsureConsoleIsLargeEnough()
    {
        // minimum height and width of the terminal screen
        const int minimumWidth = 40;
        const int minimumHeight = 15;

        while(Console.WindowWidth < minimumWidth || Console.WindowHeight < minimumHeight + 1)
        {
            Console.Clear();
            Console.SetCursorPosition(0,0);
            Console.WriteLine("The terminal window is currently too small for Pong on this platform");
            Console.WriteLine($"Minimum terminal size: {minimumWidth}x{minimumHeight}");
            Console.WriteLine($"Current termimal size: {Console.WindowWidth}x{Console.WindowHeight}");
            Console.WriteLine();
            Console.WriteLine("Please enlarge the terminal window to continue...");
            Console.WriteLine("Or press the Escape key to exit...");

            if(Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true).Key;
                if(key == ConsoleKey.Escape)
                {
                    _gameStateIsRunning = false;
                    return;
                }
            }
            Thread.Sleep(100);
        }

        int newWidth = Console.WindowWidth;
        int newHeight = Console.WindowHeight - 1;

        if(newWidth != _width || newHeight != _height)
        {
            _width = newWidth;
            _height = newHeight;

            CreateGameObjects();
            Console.Clear();
        }

        if(OperatingSystem.IsWindows())
        {
            Console.SetWindowSize(minimumWidth, minimumHeight);
        }
    }

    /// <summary>
    /// Helper method that shows the start screen, this is hardcoded.
    /// </summary>
    private void ShowStartScreen()
    {
        Console.Clear();
        Console.SetCursorPosition(0, 2);
        Console.WriteLine("=================");
        Console.WriteLine("      PONG       ");
        Console.WriteLine("=================");
        Console.WriteLine(); // newline \n
        Console.WriteLine("Left paddle: W / S"); // Select side (default: left)
        Console.WriteLine("Right paddle: Key: UpArrow / Key: DownArrow");
        Console.WriteLine("Exit game: Escape\n");
        Console.WriteLine("Press any key to start the game...");

        Console.ReadKey(intercept: true);
    }

    /// <summary>
    /// Helper method that handles the incoming userinput
    /// </summary>
    private void HandleInput()
    {
        while (Console.KeyAvailable)
        {
            ConsoleKey key = Console.ReadKey(intercept: true).Key;

            switch (key)
            {
                case ConsoleKey.W:
                    _leftPaddle!.MoveUp(minimumYvalue: 1);
                    break;
                case ConsoleKey.S:
                    _leftPaddle!.MoveDown(maximumYvalue: _height - 2);
                    break;
                case ConsoleKey.UpArrow:
                    _rightPaddle!.MoveUp(minimumYvalue: 1);
                    break;
                case ConsoleKey.DownArrow:
                    _rightPaddle!.MoveDown(maximumYvalue: _height - 2);
                    break;
                // final check, escape key
                case ConsoleKey.Escape:
                    _gameStateIsRunning = false;
                    break;
            }
        }
    }

    /// <summary>
    ///  Update the ball's position
    /// </summary>
    private void Update()
    {
        UpdateEnemyCPU();
        _ball!.Move();

        HandleWallCollision();
        HandlePaddleCollision();
        HandleScore();
    }

    /// <summary>
    /// Update the enemy CPU
    /// </summary>
    private void UpdateEnemyCPU()
    {
        _cpuDelayTick++;

        if(_cpuDelayTick % 2 != 0)
        {
            return;
        }

        int paddleCenter = _rightPaddle!.Y + _rightPaddle.Height / 2;
        int deadZone = 1;

        if(_ball!.Y < paddleCenter - deadZone)
        {
            _rightPaddle.MoveUp(minimumYvalue: 1);
        }
        else if(_ball!.Y > paddleCenter + deadZone) 
        {
            _rightPaddle.MoveDown(maximumYvalue: _height - 2);
        }

    }

    /// <summary>
    /// Handle wall collision. When the ball hits the wall, a collision has occured and the opponent gains 1 point.
    /// </summary>
    private void HandleWallCollision()
    {
        if (_ball!.Y <= 1)
        {
            _ball!.Y = 1;
            _ball!.VelocityY *= -1;
        }
        else if (_ball!.Y >= _height - 2)
        {
            _ball!.Y = _height - 2;
            _ball!.VelocityY *= -1;
        }
    }

    /// <summary>
    /// Handle the paddle collision, if the paddle is outside the bounds of the game-grid, a collision MUST occur.
    /// </summary>
    private void HandlePaddleCollision()
    {
        // left side paddle
        if (_ball!.X == _leftPaddle!.X + 1 && _ball!.Y >= _leftPaddle!.Y && _ball!.Y < _leftPaddle!.Y + _leftPaddle!.Height)
        {
            _ball!.X = _leftPaddle!.X + 1;
            _ball!.VelocityX = 1;
            _ball!.VelocityY = CalculateBounceDirection(_leftPaddle, _ball);
        }

        // right side paddle
        if (_ball!.X == _rightPaddle!.X - 1 && _ball!.Y >= _rightPaddle!.Y && _ball!.Y < _rightPaddle!.Y + _rightPaddle!.Height)
        {
            _ball!.X = _rightPaddle!.X - 1;
            _ball!.VelocityX = -1;
            _ball!.VelocityY = CalculateBounceDirection(_rightPaddle, _ball);
        }
    }

    /// <summary>
    /// Calculate the direction of the ball when it bounces from 1 racket to the other
    /// </summary>
    /// <param name="paddle">the paddle the ball hits</param>
    /// <param name="ball">the ball object</param>
    /// <returns></returns>
    private static int CalculateBounceDirection(Paddle paddle, Ball ball)
    {
        int paddleCenter = paddle.Y + paddle.Height / 2;
        int relativeImpact = ball.Y - paddleCenter;

        if (relativeImpact < 0)
        {
            return -1;
        }

        if (relativeImpact > 0)
        {
            return 1;
        }

        return 0;
    }

    /// <summary>
    /// Helper method that handles the current score for each player
    /// </summary>
    private void HandleScore()
    {
        if (_ball!.X <= 0)
        {
            _rightSideScore++;
            ResetBallPosition(ballDirectionX: -1);
        }
        else if (_ball!.X >= _width - 1)
        {
            _leftSideScore++;
            ResetBallPosition(ballDirectionX: 1);
        }
    }

    /// <summary>
    /// Helper method that resets the ball's X position
    /// </summary>
    /// <param name="ballDirectionX">current X position of the ball object</param>
    private void ResetBallPosition(int ballDirectionX)
    {
        _leftPaddle!.Reset(y: _height / 2 - _leftPaddle.Height / 2);
        _rightPaddle!.Reset(y: _height / 2 - _rightPaddle.Height / 2);

        _ball!.Reset(
            x: _width / 2,
            y: _height / 2,
            velocityX: ballDirectionX,
            velocityY: ballDirectionX > 0 ? 1 : -1
        );

        Render();
        Thread.Sleep(700);
    }

    private void Render()
    {
        _renderer!.RenderGame(
            leftSidePaddle: _leftPaddle!,
            rightSidePaddle: _rightPaddle!,
            ball: _ball!,
            leftSideScore: _leftSideScore,
            rightSideScore: _rightSideScore
        );
    }
}