using System.Text;
using System.Security.Cryptography;

namespace DiceGame.src
{
    public struct Die
    {
        public Die(String dieInfo)
        {
            string[] faces = dieInfo.Split(',');
            Faces = [];
            foreach (var face in faces)
            {
                try
                {
                    Faces.Add(Int32.Parse(face.ToCharArray()));
                }
                catch (FormatException)
                {
                    if (face == "")
                        throw new GameException("Wrong format, perhaps you mistyped comma");
                    throw new GameException($"'{face}' is not an integer");
                }
            }
            if (Faces.Count < 2)
              throw new GameException($"You cant roll die with only {Faces.Count} sides");
        }

        override public String ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[ ");
            Faces.ForEach(elem => builder.Append($"{elem} "));
            builder.Append("]");
            return builder.ToString();
        }

        public List<int> Faces { get; init; }
    }
}
