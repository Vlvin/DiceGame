using System.Security.Cryptography;

namespace DiceGame.src
{
    public class CSGenValue
    {
        public int Value { get; init; }
        public String PubKeyHex
        {
            get => HashConverter.HashToString(PubKey);
        }
        public byte[] PubKey { get; init; }
        public byte[] SecretKey { get; init; }
        public String SecretKeyHex
        {
            get => HashConverter.HashToString(SecretKey);
        }

        public CSGenValue(int toExclusive)
        {
            Value = RandomNumberGenerator.GetInt32(toExclusive);
            const int SECRET_SIZE = 20;
            SecretKey = RandomNumberGenerator.GetBytes(SECRET_SIZE);
            PubKey = HMACSHA3_256.HashData(SecretKey, BitConverter.GetBytes(Value));
        }
    }
}
