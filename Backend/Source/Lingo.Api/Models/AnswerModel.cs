using System.ComponentModel.DataAnnotations;

namespace Lingo.Api.Models;

public class AnswerModel
{
    [Required]
    public string Answer { get; set; }
}