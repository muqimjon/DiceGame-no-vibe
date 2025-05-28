
namespace DiceGame;

public class Engine
{
    private readonly ConsoleUI ui;

    public Engine(string[] args)
    {

        var dices = ParseDiceArguments(args);
        ui = new(dices);
        var userFirst = DetermineWhoStarts();
        var (userDice, computerDice) = SelectDice(dices, userFirst);

        var userRoll = Roll(userDice, "Your");
        var computerRoll = Roll(computerDice, "My");

        ui.ShowGameResult(userRoll, computerRoll);
    }

    private List<Dice> ParseDiceArguments(string[] args)
    {
        CheckIsValidDiceArguments(args);
        return [.. args.Select(arg
            => new Dice([.. arg.Split(",").Select(int.Parse)]))];
    }

    private bool DetermineWhoStarts()
    {
        Security security = new(2);
        var hmac = security.GetHmac();
        var userChoice = ui.ShowGuessPrompt(1, hmac);
        var computerChoice = security.GetValue();
        ui.ShowFinalReveal(computerChoice, security.GetKey());
        return userChoice == computerChoice;
    }

    private (Dice user, Dice computer) SelectDice(List<Dice> dices, bool userFirst)
    {
        if (userFirst)
        {
            var userIndex = ui.ShowDiceSelectionPrompt(dices);
            var remaining = dices.Where((_, i) => i != userIndex).ToList();
            var security = new Security(remaining.Count);
            ui.ShowDiceSelection(remaining[security.GetValue()]);
            return (dices[userIndex], remaining[security.GetValue()]);
        }
        else
        {
            var security = new Security(dices.Count);
            var computer = dices[security.GetValue()];
            var choices = dices.Where(i => i != computer).ToList();
            var userIndex = ui.ShowFirstAndMovePrompt(choices, computer);
            return (choices[userIndex], computer);
        }
    }

    private int Roll(Dice dice, string player)
    {
        var security = new Security(6);
        var userAdd = ui.ShowRollPrompt(player, 5, security.GetHmac());
        var computerNum = security.GetValue();
        var index = (computerNum + userAdd) % 6;
        ui.ShowFinalReveal(computerNum, security.GetKey());
        var result = dice.Face(index);

        ui.ShowRollResult(player, userAdd, computerNum, result);
        return result;
    }

    private void CheckIsValidDiceArguments(string[] args)
    {
        if (args.Length < 3)
            ui.ShowDiceCountError();
        foreach (var arg in args)
        {
            var parts = arg.Split(',');
            if (parts.Count(x => int.TryParse(x, out _)) != 6)
                ui.ShowDiceError(arg);
        }
    }
}
