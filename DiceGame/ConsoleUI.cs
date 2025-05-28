using ConsoleTables;

namespace DiceGame;

public class ConsoleUI
{
    public List<Dice>? Dices { get; set; }
    public int ShowGuessPrompt(int max, string hmac)
    {
        Console.WriteLine("Let's determine who makes the first move.\n" +
            $"I selected a random value in 0..{max}\n(HMAC={hmac}).\n" +
            "Try to guess my selection.");
        ShowRangeAsMenu(0, max);
        return GetUserInput(1);
    }

    public int ShowRollPrompt(string player, int maxNumber, string hmac)
    {
        Console.WriteLine($"It's time for {player} roll.\n" +
            "I selected a random value in the range 0..5\n" +
            $"(HMAC={hmac}).\n Add your number modulo {maxNumber}.");
        ShowRangeAsMenu(0, maxNumber);
        return GetUserInput(5);
    }

    public int ShowFirstAndMovePrompt(List<Dice> availableDices, Dice selectedDice)
    {
        Console.WriteLine($"I make the first move and choose the {selectedDice} dice.\n" +
            $"Choose your dice:");
        DisplayDiceMenu(availableDices);
        return GetDiceChoice(availableDices);
    }

    public int ShowDiceSelectionPrompt(List<Dice> availableDices)
    {
        Console.WriteLine("You guessed correctly and make the first move\n" +
            "Please choose your dice: ");
        DisplayDiceMenu(availableDices);
        return GetDiceChoice(availableDices);
    }

    private void DisplayDiceMenu(List<Dice> availableDices)
    {
        for (int i = 0; i < availableDices.Count; i++)
            Console.WriteLine($"{i} - {availableDices[i]}");
        DisplayNavigation();
    }

    private void ShowRangeAsMenu(int min, int max)
    {
        for (int i = min; i <= max; i++)
            Console.WriteLine($"{i} - {i}");
        DisplayNavigation();
    }

    public void ShowDiceSelection(Dice dice)
    {
        Console.WriteLine($"I chose the {dice} dice.");
    }

    private int GetUserInput(int max)
    {
        while (true)
        {
            var input = ReadInput();
            if (input == "X")
                Environment.Exit(0);
            if (input == "?")
            {
                ShowProbabilityTable();
                continue;
            }
            if (!int.TryParse(input, out int value))
            {
                ShowInvalid("Input must be a number.");
                continue;
            }
            if (value < 0 || value > max)
            {
                ShowInvalid($"Value must be between 0 and {max}.");
                continue;
            }
            return value;
        }
    }

    private int GetDiceChoice(List<Dice> dices)
    {
        while (true)
        {
            var input = ReadInput();
            if (input == "X")
                Environment.Exit(0);
            if (input == "?")
            {
                ShowProbabilityTable();
                continue;
            }
            if (!int.TryParse(input, out int value) || value >= dices.Count)
            {
                ShowInvalid("Invalid selection. Please choose one of the listed options.");
                continue;
            }
            return value;
        }
    }

    public void ShowFinalReveal(int computerValue, string key)
    {
        Console.WriteLine($"My selection: {computerValue} (KEY={key}).");
    }

    public void ShowRollResult(string player, int choice1, int choice2, int result)
    {
        Console.WriteLine("The fair number generation result is " +
            $"{choice1} + {choice2} = {(choice1 + choice2) % 6} " +
            $"(mod 6).\n{player} roll result is {result}.");
    }

    public void ShowGameResult(int user, int computer)
    {
        Console.WriteLine($"\nYour roll: {user}, My roll: {computer}");
        if (user > computer)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🎉 You win!");
        }
        else if (computer > user)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("💻 I win!");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("🤝 It's a draw!");
        }
        Console.ResetColor();
    }

    private void DisplayNavigation()
    {
        Console.WriteLine("X - exit");
        Console.WriteLine("? - help");
        Console.Write("Your selection: ");
    }

    public void ShowDiceError(string rawInput)
    {
        ShowInvalid($"Invalid dice format: {rawInput}. Expected format: 2,2,4,4,9,9");
        Environment.Exit(1);
    }

    public void ShowDiceCountError()
    {
        ShowInvalid("At least 3 valid dice are required.");
        Environment.Exit(1);
    }

    private void ShowInvalid(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"❌ {message}");
        Console.ResetColor();
    }

    private string ReadInput()
        => Console.ReadLine()?.Trim().ToUpperInvariant() ?? default!;

    public void ShowProbabilityTable()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n📊 Probability of the win for the user:\n");
        Console.ResetColor();
        var headers = new[] { "User \\ Computer" }.Concat(Dices.Select(d => d.ToString())).ToArray();
        var table = new ConsoleTable(headers);
        foreach (var userDice in Dices)
        {
            var row = new List<string> { userDice.ToString() };
            foreach (var compDice in Dices)
            {
                row.Add(userDice == compDice
                    ? "- (0.3333)"
                    : CalculateProbability(userDice, compDice).ToString("F4"));
            }
            table.AddRow([.. row]);
        }

        table.Write(Format.Minimal);
    }

    private double CalculateProbability(Dice dice1, Dice dice2)
    {
        int wins = 0;
        foreach (var faceA in dice1)
            foreach (var faceB in dice2)
                if (faceA > faceB)
                    wins++;

        return wins / 36.0;
    }
}
