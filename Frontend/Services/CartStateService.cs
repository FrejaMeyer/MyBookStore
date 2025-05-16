namespace Frontend.Services;

public class CartStateService
{
    public int ItemCount { get; private set; }

    public event Action? OnChange;

    public void Increment(int amount = 1)
    {
        ItemCount += amount;
        OnChange?.Invoke();
    }

    public void SetCount(int count)
    {
        ItemCount = count;
        OnChange?.Invoke();
    }

    public void Decrement(int amount = 1)
    {
        ItemCount = Math.Max(0, ItemCount - amount);
        OnChange?.Invoke();
    }
}

