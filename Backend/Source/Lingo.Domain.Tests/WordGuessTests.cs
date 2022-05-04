using System;
using System.Text;
using Guts.Client.Core;
using Lingo.Domain.Puzzle;
using Lingo.TestTools;
using NUnit.Framework;

namespace Lingo.Domain.Tests;

[ProjectComponentTestFixture("1TINProject", "Lingo", "WordGuess", @"Lingo.Domain\Puzzle\WordGuess.cs;Lingo.Domain\Puzzle\LetterMatch.cs")]
public class WordGuessTests : TestBase
{
    [MonitoredTest("Constructor - Word and solution have different lengths - Should throw argument exception")]
    public void _01_Constructor_WordAndSolutionHaveDifferentLengths_ShouldThrowArgumentException()
    {
        //Arrange
        string word = RandomGenerator.NextWord(5);
        int delta = 1;
        if (RandomGenerator.NextBool()) delta *= -1;
        string solution = RandomGenerator.NextWord(word.Length + delta);

        //Act + Assert
        Assert.That(() => new WordGuess(word, solution), Throws.ArgumentException.With.Message.Contains("lengte").IgnoreCase);
    }

    [MonitoredTest("Constructor - Should set word property correctly")]
    public void _02_Constructor_ShouldSetWordPropertyCorrectly()
    {
        //Arrange
        string word = RandomGenerator.NextWord();
        string solution = RandomGenerator.NextWord();

        //Act
        WordGuess guess = new WordGuess(word, solution);
        
        //Assert
        Assert.That(guess.Word, Is.EqualTo(word));
    }

    [MonitoredTest("Constructor - Basic cases - Should set letter matches correctly")]
    [TestCase("AB", "CD", new [] { LetterMatch.DoesNotOccur, LetterMatch.DoesNotOccur })]
    [TestCase("AB", "AB", new[] { LetterMatch.Correct, LetterMatch.Correct })]
    [TestCase("AB", "BA", new[] { LetterMatch.CorrectButInWrongPosition, LetterMatch.CorrectButInWrongPosition })]
    [TestCase("BAKT", "BOEK", new[] { LetterMatch.Correct, LetterMatch.DoesNotOccur, LetterMatch.DoesNotOccur, LetterMatch.CorrectButInWrongPosition })]
    public void _03_Constructor_BasicCases_ShouldSetLetterMatchesCorrectly(string solution, string word, LetterMatch[] expectedLetterMatches)
    {
        TestLetterMatches(solution, word, expectedLetterMatches);
    }

    [MonitoredTest("Constructor - Duplicate letter cases - Should set letter matches correctly")]
    [TestCase("ABC", "BCB", new[] { LetterMatch.CorrectButInWrongPosition, LetterMatch.CorrectButInWrongPosition, LetterMatch.DoesNotOccur })]
    [TestCase("RAAR", "BABA", new[] { LetterMatch.DoesNotOccur, LetterMatch.Correct, LetterMatch.DoesNotOccur, LetterMatch.CorrectButInWrongPosition })]
    [TestCase("BABA", "RAAR", new[] { LetterMatch.DoesNotOccur, LetterMatch.Correct, LetterMatch.CorrectButInWrongPosition, LetterMatch.DoesNotOccur })]
    [TestCase("ADEM", "BACA", new[] { LetterMatch.DoesNotOccur, LetterMatch.CorrectButInWrongPosition, LetterMatch.DoesNotOccur, LetterMatch.DoesNotOccur })]
    public void _04_Constructor_DuplicateLetterCases_ShouldSetLetterMatchesCorrectly(string solution, string word, LetterMatch[] expectedLetterMatches)
    {
        TestLetterMatches(solution, word, expectedLetterMatches);
    }

    [MonitoredTest("Constructor - Exact match on second duplicate letter - Should set letter matches correctly")]
    [TestCase("STAR", "RAAM", new[] { LetterMatch.CorrectButInWrongPosition, LetterMatch.DoesNotOccur, LetterMatch.Correct, LetterMatch.DoesNotOccur })]
    public void _05_Constructor_ExactMatchOnSecondDuplicateLetter_ShouldSetLetterMatchesCorrectly(string solution, string word, LetterMatch[] expectedLetterMatches)
    {
        TestLetterMatches(solution, word, expectedLetterMatches);
    }

    private void TestLetterMatches(string solution, string word, LetterMatch[] expectedLetterMatches)
    {
        //Act
        WordGuess guess = new WordGuess(word, solution);

        //Assert
        Assert.That(guess.LetterMatches, Is.EqualTo(expectedLetterMatches), () =>
        {
            var builder = new StringBuilder();
            builder.AppendLine($"When the word is '{word}' ");
            builder.AppendLine($"and the solution is '{solution}', ");
            builder.AppendLine($"the matches should be [{string.Join(',', expectedLetterMatches)}] ");
            builder.AppendLine($"but were [{string.Join(',', guess.LetterMatches ?? Array.Empty<LetterMatch>())}]");
            return builder.ToString();
        });
    }
}