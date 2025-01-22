using DiceGame.src;
using System.Security.Cryptography;
using ConsoleTables;

namespace DiceGame
{
    public class Game
    {
        public static List<Die> ParseDice(String[] diceInfo)
        {
            if (diceInfo.Length < GameConfig.MIN_DICE_COUNT)
                throw new GameException($"Expected at least {GameConfig.MIN_DICE_COUNT} dice. got {diceInfo.Length}");
            var result = new List<Die>();
            foreach (String dieInfo in diceInfo)
            {
                result.Add(new Die(dieInfo));
            }
            return result;
        }

        public static double WinProbability(Die mine, Die oposite) 
        {
          var winnings = 0;
          foreach (var myface in mine.Faces) 
          {
            foreach (var opface in oposite.Faces) 
            {
              winnings += Convert.ToInt32(myface > opface);
            }
          }

          return winnings / Convert.ToDouble(mine.Faces.Count * oposite.Faces.Count);
        }

        public static void Main(String[] args)
        {
            List<Die> dice = [];
            try
            {
                dice = ParseDice(args);
            }
            catch (GameException e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            Console.WriteLine("Win Probability");
            var table = new ConsoleTable(new string[] {"User \\ Computer"}.Concat(dice.Select(die => $"Die {die.ToString()}")).ToArray());
            foreach (var die in dice)
            {
              table.AddRow(new string[] {$"Die {die.ToString()}"}.Concat(dice.Select(die2 => $"{WinProbability(die, die2)}")).ToArray());
            }
            table.Write(Format.Alternative);
            var game = new Game();
            game.Run(dice);
        }

        private int _getUserInput(int lowerBound, int upperBound, int except = -1)
        {
            string exceptMessage = except != -1 ? $"except {except}" : "";
            Console.WriteLine($"In range {lowerBound}..{upperBound} {exceptMessage}");
            int userGuessValue = -1;
            do
            {
                try
                {
                    userGuessValue = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("This is not an integer");
                    userGuessValue = -1;
                }
                if ((lowerBound <= userGuessValue) && (userGuessValue < upperBound) && (userGuessValue != except))
                    return userGuessValue;
                Console.WriteLine("Your value is not in range");
            } while (true);
        }

        private int _letUserChooseDie(List<Die> dice, int computerChoice = -1)
        {
            for (var i = 0; i < dice.Count; i++)
            {
                if (i == computerChoice)
                    continue;
                Console.WriteLine($"{i} {dice[i]}");
            }
            Console.WriteLine("Choose die by printing number");
            return _getUserInput(0, dice.Count, computerChoice);
        }

        private int _letComputerChooseDie(List<Die> dice, int userChoice = -1)
        {
            var computerChoice = -1;
            while (computerChoice < 0 || computerChoice >= dice.Count || computerChoice == userChoice)
                computerChoice = RandomNumberGenerator.GetInt32(dice.Count);
            return computerChoice;
        }

        // result is Tuple of (die index of Computer Choice, die index of User Choice)
        private Tuple<int, int> _chooseDice(List<Die> dice, bool isUserGoingFirst)
        {
            int computerChoice = -1;
            int userChoice = -1;
            if (isUserGoingFirst)
            {
                userChoice = _letUserChooseDie(dice, computerChoice);
                Console.WriteLine($"You chose Die {dice[userChoice]}");
            }
            computerChoice = _letComputerChooseDie(dice, userChoice);
            Console.WriteLine($"I chose Die {dice[computerChoice]}");

            if (!isUserGoingFirst)
            {
                userChoice = _letUserChooseDie(dice, computerChoice);
                Console.WriteLine($"You chose Die {dice[userChoice]}");
            }
            return new Tuple<int, int>(computerChoice, userChoice);
        }

        private int _rollDie(Die die)
        {
            var computerValue = new CSGenValue(die.Faces.Count);
            Console.WriteLine($"I pick up value from range 0..{die.Faces.Count} (HMAC: {computerValue.PubKeyHex}");
            Console.WriteLine("Choose a Value");
            var userValue = _getUserInput(0, die.Faces.Count);
            Console.WriteLine($"My value was {computerValue.Value}");
            Console.WriteLine($"(secret key: {computerValue.SecretKeyHex})");
            var result = (userValue + computerValue.Value) % die.Faces.Count;
            Console.WriteLine($"Result: ({computerValue.Value} + {userValue}) mod {die.Faces.Count} = {result}");
            Console.WriteLine($"Roll is {die.Faces[result]}");
            return die.Faces[result];
        }

        public void Run(List<Die> dice)
        {
            var computerValue = new CSGenValue(2);
            Console.WriteLine($"(HMAC: {computerValue.PubKeyHex})");
            Console.WriteLine("Guess value");
            var lowerBound = GameConfig.GUESS_RANGE.Start.Value;
            var upperBound = GameConfig.GUESS_RANGE.End.Value;
            var userGuessValue = _getUserInput(lowerBound, upperBound);
            Console.WriteLine($"Your guess is {userGuessValue}");
            Console.WriteLine($"My value was {computerValue.Value}");
            Console.WriteLine($"(Secret Key: {computerValue.SecretKeyHex})");
            var chosenDiceI = _chooseDice(dice, userGuessValue == computerValue.Value);

            Console.WriteLine("I am going to roll first");
            var compDie = dice.ElementAt(chosenDiceI.Item1);
            Console.WriteLine(compDie);
            var computerRoll = _rollDie(compDie);
            Console.WriteLine("Now it's your turn to Roll");
            var userDie = dice.ElementAt(chosenDiceI.Item2);
            Console.WriteLine(userDie);
            var userRoll = _rollDie(userDie);
            if (computerRoll > userRoll)
                Console.WriteLine($"Computer wins ({computerRoll} > {userRoll})");
            else if (computerRoll < userRoll)
                Console.WriteLine($"User wins ({userRoll} > {computerRoll})");
            else
                Console.WriteLine($"Tie ({computerRoll} = {userRoll})");
        }
    };
}
