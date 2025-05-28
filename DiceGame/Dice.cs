using System.Collections;

namespace DiceGame;

public class Dice(int[] sides) : IEnumerable<int>
{
    private readonly int[] sides = sides;
    private readonly Random random = new();

    public int Roll() =>
        sides[random.Next(sides.Length)];

    public override string ToString() =>
        $"[{string.Join(",", sides)}]";

    public int Face(int index)
        => sides[index];

    public IEnumerator<int> GetEnumerator() =>
        ((IEnumerable<int>)sides).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();
}
