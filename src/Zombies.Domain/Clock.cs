namespace Zombies.Domain;

public interface IClock
{
    DateTime UtcNow { get; }
}

internal class Clock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}