/// <summary>
/// A simple CPU game controller
/// </summary>
public sealed class CpuGameController
{
    private readonly int _updateIntervalFrames;
    private readonly int _deadZone;
    private int _frameCounter;

    public CpuGameController(int updateIntervalFrames, int deadZone)
    {
        _updateIntervalFrames = Math.Max(1, updateIntervalFrames);
        _deadZone = Math.Max(0, deadZone);
    }

    public void Update(Paddle paddle, Ball ball, int maxY)
    {
        // increment the frame counter
        _frameCounter++;

        if (_frameCounter % _updateIntervalFrames != 0)
        {
            return;
        }

        int paddleCenter = paddle.Y + paddle.Height / 2;

        if (ball.Y < paddleCenter - _deadZone)
        {
            paddle.MoveUp(minimumYvalue: 1);
        }
        else if (ball.Y > paddleCenter + _deadZone)
        {
            paddle.MoveDown(maxY);
        }
    }
}