public sealed class Paddle
{
    /// <summary>
    /// X value property
    /// </summary>
    public int X { get; }
    /// <summary>
    /// Y value property. This property has a private set value that cannot be overwritten outside the Paddle class
    /// </summary>
    public int Y { get; private set; }
    public int Height { get; }

    public Paddle(int x, int y, int height)
    {
        if (height < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        X = x;
        Y = y;
        Height = height;
    }

    /// <summary>
    /// Move the Y position up
    /// </summary>
    /// <param name="minimumYvalue">minimum Y value</param>
    public void MoveUp(int minimumYvalue)
    {
        if (Y > minimumYvalue)
        {
            Y--;
        }
    }

    /// <summary>
    /// Move the Y position down
    /// </summary>
    /// <param name="maximumYvalue">maximum Y value</param>
    public void MoveDown(int maximumYvalue)
    {
        if (Y + Height - 1 < maximumYvalue)
        {
            Y++;
        }
    }

    /// <summary>
    /// Reset the value f Y
    /// </summary>
    /// <param name="y">incoming Y value</param>
    public void Reset(int y)
    {
        Y = y;
    }
}