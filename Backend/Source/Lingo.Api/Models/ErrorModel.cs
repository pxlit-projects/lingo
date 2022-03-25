
namespace Lingo.Api.Models
{
    public class ErrorModel
    {
        public string Message { get; }

        public ErrorModel(Exception exception)
        {
            Message = exception.Message;
        }

        public ErrorModel(string message)
        {
            Message = message;
        }
    }
}