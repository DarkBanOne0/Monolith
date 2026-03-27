namespace Content.Client._Exodus.Bosses;

public struct TextColorData
{
    public Func<float, Color> CalculateSelection;

    public TextColorData(Color color)
    {
        CalculateSelection = _ => color;
    }

    public TextColorData(Func<float, Color> calculateSelection)
    {
        CalculateSelection = calculateSelection;
    }

    public readonly Color Calculate(float ratio) => CalculateSelection(ratio);

    public static implicit operator TextColorData(Color c) => new(c);
}
