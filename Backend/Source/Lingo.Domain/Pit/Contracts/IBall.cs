namespace Lingo.Domain.Pit.Contracts
{
    /// <summary>
    /// Represents a ball in the ball pit.
    /// </summary>
    public interface IBall
    {
        /// <summary>
        /// The value of the ball. This is significant when the <see cref="Type"/> is blue.
        /// </summary>
        int Value { get; }

        /// <summary>
        /// The type (color) of the ball.
        /// </summary>
        BallType Type { get; }
    }
}