using System.Text;

namespace Lingo.TestTools
{
    public static class RandomExtensions
    {
        public static bool NextBool(this Random random)
        {
            return random.Next(0, 2) == 1;
        }

        public static string NextString(this Random random)
        {
            return Guid.NewGuid().ToString();
        }

        public static string NextWord(this Random random, int length)
        {
            char[] letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            var builder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                int index = random.Next(letters.Length);
                builder.Append(letters[index]);
            }

            return builder.ToString();
        }

        public static string NextWord(this Random random)
        {
            return NextWord(random, 5);
        }

        public static string[] NextWords(this Random random, int numberOfWords, int wordLength = 5)
        {
            var words = new string[numberOfWords];
            for (int i = 0; i < numberOfWords; i++)
            {
                words[i] = NextWord(random, wordLength);
            }
            return words;
        }
    }
}
