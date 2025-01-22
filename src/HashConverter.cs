using System.Text;

namespace DiceGame.src
{
    public static class HashConverter
    {
        public static string HashToString(byte[] hash)
        {
            var builder = new StringBuilder();
            foreach (var hashByte in hash)
                builder.Append(hashByte.ToString("x2"));
            return builder.ToString();
        }
    }
}
