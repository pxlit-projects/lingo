using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guts.Client.Core;
using Guts.Client.Core.TestTools;
using Lingo.Domain.Puzzle;
using Lingo.Domain.Puzzle.Contracts;
using Lingo.TestTools;
using NUnit.Framework;

namespace Lingo.Domain.Tests
{
    public class StandardWordPuzzleTests : TestBase
    {
        private string _solution;
        private HashSet<string> _wordDictionary;
        private IWordPuzzle _puzzle;
        private IList<string> _otherWords;
        private string _iWordPuzzleCodeHash;
        private string _iPuzzleCodeHash;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _iWordPuzzleCodeHash = Solution.Current.GetFileHash(@"Lingo.Domain\Puzzle\Contracts\IWordPuzzle.cs");
            _iPuzzleCodeHash = Solution.Current.GetFileHash(@"Lingo.Domain\Puzzle\Contracts\IPuzzle.cs");
        }

        [SetUp]
        public void BeforeEachTest()
        {
            _wordDictionary = new HashSet<string>();

            _solution = RandomGenerator.NextWord();
            _wordDictionary.Add(_solution);

            _otherWords = RandomGenerator.NextWords(10);
            foreach (string existingWord in _otherWords)
            {
                _wordDictionary.Add(existingWord);
            }

            _puzzle = new StandardWordPuzzle(_solution, _wordDictionary) as IWordPuzzle;
        }

        [MonitoredTest("Should implement IWordPuzzle")]
        public void _01_ShouldImplementIWordPuzzle()
        {
            //Assert
            AssertThatInterfacesHaveNotChanged();
            Assert.That(_puzzle, Is.Not.Null);
        }

        [MonitoredTest("Type - Should return a standard word puzzle")]
        public void _02_Type_ShouldReturnStandardWordPuzzle()
        {
            AssertThatInterfacesHaveNotChanged();

            //Assert
            Assert.That(_puzzle.Type, Is.EqualTo(nameof(StandardWordPuzzle)), $"The 'Type' of the puzzle should always return '{nameof(StandardWordPuzzle)}'.");
        }

        [MonitoredTest("Constructor - Should generate an id")]
        public void _03_Constructor_ShouldGenerateAnId()
        {
            AssertThatInterfacesHaveNotChanged();

            //Assert
            Assert.That(_puzzle.Id, Is.Not.EqualTo(Guid.Empty), "The 'Id' of the puzzle should have a non-empty GUID after construction.");
        }

        [MonitoredTest("Constructor - Should reveal the first letter of the solution")]
        public void _04_Constructor_ShouldRevealTheFirstLetterOfTheSolution()
        {
            AssertThatInterfacesHaveNotChanged();

            //Assert
            Assert.That(_puzzle.RevealedLetters, Is.Not.Null, "The 'RevealedLetters' property should not be NULL after construction.");
            Assert.That(_puzzle.RevealedLetters.Length, Is.EqualTo(_solution.Length),
                "The number of characters in the 'RevealedLetters' should be equal to the number of letters in the solution word.");
            Assert.That(_puzzle.RevealedLetters[0], Is.EqualTo(_solution[0]),
                "The first character in de 'RevealedLetters' property should be the first letter in the solution word.");
            Assert.That(_puzzle.RevealedLetters.Skip(1).All(letter => letter == '.'), Is.True,
                "Except for the first one, all the characters in the 'RevealedLetters' should be equal to '.' (dot).");
        }

        [MonitoredTest("Constructor - Should set the word length")]
        public void _05_Constructor_ShouldSetTheWordLength()
        {
            AssertThatInterfacesHaveNotChanged();

            //Arrange
            int wordLength = RandomGenerator.Next(4, 11);
            _solution = RandomGenerator.NextWord(wordLength);

            //Act
            _puzzle = new StandardWordPuzzle(_solution, _wordDictionary) as IWordPuzzle;

            //Assert
            Assert.That(_puzzle.WordLength, Is.EqualTo(wordLength), "The 'WordLength' property should return the length of the solution word.");
        }

        [MonitoredTest("Score - Not all letters are revealed - Should return zero")]
        public void _06_Score_NotAllLettersAreRevealed_ShouldReturnZero()
        {
            AssertThatInterfacesHaveNotChanged();

            //Arrange
            for (int i = 0; i < _solution.Length - 1; i++)
            {
                _puzzle.RevealedLetters[i] = _solution[i];
            }
            _puzzle.RevealedLetters[_solution.Length - 1] = '.'; //last letter not revealed

            //Act
            int score = _puzzle.Score;

            //Assert
            Assert.That(score, Is.Zero,
                $"The score should be zero when the solution is '{_solution}' " +
                $"and the revealed letters are '{new string(_puzzle.RevealedLetters)}' (last letter not revealed).");
        }

        [MonitoredTest("Score - All letters are revealed - Should return 25")]
        public void _07_Score_AllLettersAreRevealed_ShouldReturn25()
        {
            AssertThatInterfacesHaveNotChanged();

            //Arrange
            for (int i = 0; i < _solution.Length; i++)
            {
                _puzzle.RevealedLetters[i] = _solution[i];
            }

            //Act
            int score = _puzzle.Score;

            //Assert
            Assert.That(score, Is.EqualTo(25),
                $"The score should be 25 when the solution is '{_solution}' " +
                $"and the revealed letters are '{new string(_puzzle.RevealedLetters)}'. " +
                "Tip: calculate the score on-the-fly using the revealed letters.");
        }

        [MonitoredTest("Guesses - After construction - Should return an empty list")]
        public void _08_Guesses_AfterConstruction_ShouldReturnAnEmptyList()
        {
            AssertThatInterfacesHaveNotChanged();

            //Assert
            Assert.That(_puzzle.Guesses, Is.Not.Null, "Tip1: use a backing field. Tip2: 'List<WordGuess>' implements 'IReadOnlyList<WordGuess>'.");
            Assert.That(_puzzle.Guesses.Count, Is.Zero);
        }

        [MonitoredTest("SubmitAnswer - Incorrect existing word - Should register a guess and indicate that the turn is not lost")]
        public void _09_SubmitAnswer_IncorrectExistingWord_ShouldRegisterAGuessAndIndicateThatTheTurnIsNotLost()
        {
            AssertThatInterfacesHaveNotChanged();

            string answer = _otherWords[0];
            TestSubmissionOfIncorrectExistingWord(answer, out WordGuess submittedGuess);
            Assert.That(submittedGuess.Word, Is.EqualTo(answer), "The 'Word' property of the added guess should be equal to the submitted answer.");
        }

        [MonitoredTest("SubmitAnswer - Answer in lower case - Should convert answer tot uppercase")]
        public void _10_SubmitAnswer_AnswerInLowerCase_ShouldConvertAnswerToUpperCase()
        {
            AssertThatInterfacesHaveNotChanged();

            string lowerCaseAnswer = _otherWords[0].ToLower();

            WordGuess submittedGuess = null;
            try
            {
                TestSubmissionOfIncorrectExistingWord(lowerCaseAnswer, out submittedGuess);
            }
            catch (Exception)
            {
                Assert.Fail($"Make sure the test '{nameof(_09_SubmitAnswer_IncorrectExistingWord_ShouldRegisterAGuessAndIndicateThatTheTurnIsNotLost)}' is green first.");
            }

            Assert.That(submittedGuess?.Word, Is.EqualTo(lowerCaseAnswer.ToUpper()),
                "The 'Word' property of the added guess should be equal to the uppercase version of the submitted answer.");
        }

        private void TestSubmissionOfIncorrectExistingWord(string answer, out WordGuess submittedGuess)
        {
            //Act
            SubmissionResult result = _puzzle.SubmitAnswer(answer);

            //Assert
            Assert.That(result, Is.Not.Null, "The returned result cannot be NULL.");
            Assert.That(result.LostTurn, Is.False,
                "The returned result indicated that the turn is lost. " +
                "Submitting an incorrect existing word does not mean you lose your turn.");

            Assert.That(_puzzle.Guesses.Count, Is.EqualTo(1), "There should be exactly one guess in the 'Guesses' property of the puzzle.");
            submittedGuess = _puzzle.Guesses[0];
        }

        [MonitoredTest("SubmitAnswer - Length of answer is not the same as length of solution - Should not register a guess and indicate that the turn is lost")]
        public void _11_SubmitAnswer_LengthOfAnswerIsNotTheSameAsLengthOfSolution_ShouldNotRegisterAGuessAndIndicateThatTheTurnIsLost()
        {
            AssertThatInterfacesHaveNotChanged();

            //Arrange
            int delta = 1;
            if (RandomGenerator.NextBool()) delta *= -1;
            string invalidAnswer = RandomGenerator.NextWord(_solution.Length + delta);

            //Act
            SubmissionResult result = _puzzle.SubmitAnswer(invalidAnswer);

            //Assert
            Assert.That(result, Is.Not.Null, "The returned result cannot be NULL.");
            Assert.That(result.LostTurn, Is.True, "The returned result does not indicate that the turn is lost.");
            Assert.That(result.Reason, Does.Contain(_solution.Length.ToString()),
                $"The reason in the returned result should mention the expected length ('{_solution.Length}') of an answer.");

            Assert.That(_puzzle.Guesses.Count, Is.Zero, "There shouldn't be any guesses in the 'Guesses' property of the puzzle.");
        }

        [MonitoredTest("SubmitAnswer - Word not in dicationary - Should not register a guess and indicate that the turn is lost")]
        public void _12_SubmitAnswer_WordNotInDictionary_ShouldNotRegisterAGuessAndIndicateThatTheTurnIsLost()
        {
            AssertThatInterfacesHaveNotChanged();

            //Arrange
            string nonExistingWord = RandomGenerator.NextWord();
            while (_wordDictionary.Contains(nonExistingWord))
            {
                nonExistingWord = RandomGenerator.NextWord();
            }

            //Act
            SubmissionResult result = _puzzle.SubmitAnswer(nonExistingWord);

            //Assert
            Assert.That(result, Is.Not.Null, "The returned result cannot be NULL.");
            Assert.That(result.LostTurn, Is.True, "The returned result does not indicate that the turn is lost.");
            Assert.That(result.Reason, Does.Contain("woordenboek").IgnoreCase,
                "The reason in the returned result should contain the word 'woordenboek'.");
            Assert.That(result.Reason, Does.Contain(nonExistingWord).IgnoreCase,
                "The reason in the returned result should contain the submitted word.");

            Assert.That(_puzzle.Guesses.Count, Is.Zero, "There shouldn't be any guesses in the 'Guesses' property of the puzzle.");
        }

        [MonitoredTest("SubmitAnswer - Already 4 guesses made - Answer is an incorrect existing word - should register a fifth guess and indicate that the turn is lost")]
        public void _13_SubmitAnswer_Already4GuessesMade_AnswerIsAnIncorrectExistingWord_ShouldRegisterAFifthGuessAndIndicateThatTheTurnIsLost()
        {
            AssertThatInterfacesHaveNotChanged();

            //Arrange
            for (int i = 0; i < 4; i++)
            {
                _puzzle.SubmitAnswer(_otherWords[i]);
            }

            string answer = _otherWords[4];

            //Act
            SubmissionResult result = _puzzle.SubmitAnswer(answer);

            //Assert
            Assert.That(result, Is.Not.Null, "The returned result cannot be NULL.");
            Assert.That(result.LostTurn, Is.True, "The returned result does not indicate that the turn is lost. " +
                                                   "Why is the turn lost? -> Because a fifth guess is made, the current player cannot keep playing.");
            Assert.That(result.Reason, Does.Contain("pogingen").IgnoreCase, "The reason in the returned result should contain the word 'pogingen'.");

            Assert.That(_puzzle.Guesses.Count, Is.EqualTo(5), "There should be exactly 5 guesses in the 'Guesses' property of the puzzle.");
        }

        [MonitoredTest("SubmitAnswer - Should reveal the correctly positioned letters")]
        [TestCase("KOPJES", new string[]{}, "K.....")]
        [TestCase("KOPJES", new[] { "KALIUM" }, "K.....")]
        [TestCase("KOPJES", new[] { "KOEIEN" }, "KO..E.")]
        [TestCase("KOPJES", new[] { "KAMERS", "KOEIEN" }, "KO..ES")]
        [TestCase("KOPJES", new[] { "KALIUM", "KOPJES" }, "KOPJES")]
        [TestCase("MUGGEN", new[] { "MOEDIG", "MAGERE", "MIDDAG" }, "M.G...")]
        public void _14_SubmitAnswer_ShouldRevealTheCorrectlyPositionedLetters(string solution, string[] answers, string expectedRevealedLetters)
        {
            AssertThatInterfacesHaveNotChanged();

            //Arrange
            _wordDictionary = new HashSet<string> { solution };
            _puzzle = new StandardWordPuzzle(solution, _wordDictionary) as IWordPuzzle;

            //Act
            foreach (string answer in answers)
            {
                _wordDictionary.Add(answer);
                _puzzle.SubmitAnswer(answer);
            }

            //Assert
            string revealedLetters = new string(_puzzle.RevealedLetters);
            Assert.That(revealedLetters, Is.EqualTo(expectedRevealedLetters), () =>
            {
                var builder = new StringBuilder();
                builder.AppendLine($"When the solution is '{solution}'");
                switch (answers.Length)
                {
                    case 0:
                        builder.AppendLine("and no answers are submitted yet, ");
                        break;
                    case 1:
                        builder.AppendLine($"and one answer '{answers[0]}' was submitted, ");
                        break;
                    default:
                        builder.AppendLine($"and the submitted answers are '{string.Join(',', answers)}', ");
                        break;
                }
                builder.AppendLine($"the revealed letters should be '{expectedRevealedLetters}', but were '{revealedLetters}'.");
                builder.AppendLine();
                builder.AppendLine($"Tip: make sure the tests on the '{nameof(WordGuess)}' class are green first!!");
                return builder.ToString();
            });
        }

        [MonitoredTest("IsFinished - Word not guessed yet and less than 6 guesses made - Should return false")]
        public void _15_IsFinished_WordNotGuessedYetAndLessThan6GuessesMade_ShouldReturnFalse()
        {
            AssertThatInterfacesHaveNotChanged();

            //Arrange
            int numberOfGuesses = RandomGenerator.Next(1, 6);
            for (int i = 0; i < numberOfGuesses; i++)
            {
                _puzzle.SubmitAnswer(_otherWords[i]);
            }

            //Act + Assert
            Assert.That(_puzzle.IsFinished, Is.False);
        }

        [MonitoredTest("IsFinished - Word was guessed - Should return true")]
        public void _16_IsFinished_WordWasGuessed_ShouldReturnTrue()
        {
            AssertThatInterfacesHaveNotChanged();

            //Arrange
            _puzzle.SubmitAnswer(_solution);

            //Act + Assert
            Assert.That(_puzzle.IsFinished, Is.True);
        }

        [MonitoredTest("IsFinished - Word not guessed yet but 6 guesses made - Should return true")]
        public void _17_IsFinished_WordNotGuessedYetBut6GuessesMade_ShouldReturnTrue()
        {
            AssertThatInterfacesHaveNotChanged();

            //Arrange
            for (int i = 0; i < 6; i++)
            {
                _puzzle.SubmitAnswer(_otherWords[i]);
            }

            //Act + Assert
            Assert.That(_puzzle.IsFinished, Is.True);
        }

        [MonitoredTest("RevealPart - Should reveal the first letter that is not revealed yet")]
        [TestCase("MUGGEN", "M.....", "MU....")]
        [TestCase("MUGGEN", "M..G..", "MU.G..")]
        [TestCase("MUGGEN", "MUGGEN", "MUGGEN")]
        public void _18_RevealPart_ShouldRevealTheFirstLetterThatIsNotRevealedYet(string solution, string currentRevealedLetters, string expectedRevealedLetters)
        {
            AssertThatInterfacesHaveNotChanged();

            //Arrange
            _puzzle = new StandardWordPuzzle(solution, _wordDictionary) as IWordPuzzle;

            for (int i = 0; i < currentRevealedLetters.Length; i++)
            {
                _puzzle.RevealedLetters[i] = currentRevealedLetters[i];
            }

            //Act
            _puzzle.RevealPart();

            //Assert
            string actualRevealedLetters = new string(_puzzle.RevealedLetters);
            Assert.That(actualRevealedLetters, Is.EqualTo(expectedRevealedLetters),
                $"When the solution is '{solution}' and the current revealed letters are '{currentRevealedLetters}', " +
                $"the revealed letters should become '{expectedRevealedLetters}' but were '{actualRevealedLetters}'.");
        }

        [MonitoredTest("RevealPart - Only one letter not revealed - Should do nothing")]
        [TestCase("MUGGEN", "MUGGE.")]
        [TestCase("MUGGEN", "MU.GEN")]
        public void _19_RevealPart_OnlyOneLetterNotRevealed_ShouldDoNothing(string solution, string currentRevealedLetters)
        {
            AssertThatInterfacesHaveNotChanged();

            //Arrange
            _puzzle = new StandardWordPuzzle(solution, _wordDictionary) as IWordPuzzle;

            for (int i = 0; i < currentRevealedLetters.Length; i++)
            {
                _puzzle.RevealedLetters[i] = currentRevealedLetters[i];
            }

            //Act
            _puzzle.RevealPart();

            //Assert
            string actualRevealedLetters = new string(_puzzle.RevealedLetters);
            Assert.That(actualRevealedLetters, Is.EqualTo(currentRevealedLetters),
                $"When the solution is '{solution}' and the current revealed letters are '{currentRevealedLetters}', " +
                $"the revealed letters should not change but were '{actualRevealedLetters}'.");
        }

        private void AssertThatInterfacesHaveNotChanged()
        {
            Assert.That(_iPuzzleCodeHash, Is.EqualTo("E2-F7-EA-FE-FA-5C-2D-F5-B9-D3-0B-34-BF-3B-0C-6C"),
                "The code of the IPuzzle interface has changed. This is not allowed. Undo your changes in 'IPuzzle.cs'");
            Assert.That(_iWordPuzzleCodeHash, Is.EqualTo("39-3F-8F-3B-05-24-3E-2B-33-9E-6B-C5-B8-82-64-66"),
                "The code of the IWordPuzzle interface has changed. This is not allowed. Undo your changes in 'IWordPuzzle.cs'");
        }
    }
}