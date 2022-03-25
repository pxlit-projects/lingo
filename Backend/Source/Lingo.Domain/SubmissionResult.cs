namespace Lingo.Domain;

//DO NOT TOUCH THIS FILE!
public class SubmissionResult
{
    /// <summary>
    /// Indicated if the submission resulted in the player losing its turn.
    /// </summary>
    public bool LostTurn { get; }

    /// <summary>
    /// When <see cref="LostTurn"/> is true, this property will contain the reason why.
    /// </summary>
    public string Reason { get; private set; }

    private SubmissionResult(bool lostTurn)
    {
        LostTurn = lostTurn;
        Reason = string.Empty;
    }

    public static SubmissionResult CreateKeepTurnResult()
    {
        return new SubmissionResult(false);
    }

    public static SubmissionResult CreateLoseTurnResult(string reason)
    {
        return new SubmissionResult(true)
        {
            Reason = reason
        };
    }
}