namespace Zombies.Domain;

public interface ISurvivor
{
    public enum SurvivorStatus
    {
        Alive,
        Dead
    }

    string Name { get; }

    SurvivorStatus Status { get; }

    int Wounds { get; }

    int RemainingActions { get; }

    void InflictWound(int inflictedWounds);
}

public class Survivor : ISurvivor
{
    private Survivor(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(nameof(name), "The survivor name is required and cannot be empty");

        Name = name;
        Wounds = 0;
        RemainingActions = 3;
    }

    public int Wounds { get; private set; }

    public ISurvivor.SurvivorStatus Status => Wounds < 2 ? ISurvivor.SurvivorStatus.Alive : ISurvivor.SurvivorStatus.Dead;

    public string Name { get; }

    public int RemainingActions { get; }

    public static ISurvivor Create(string name)
    {
        return new Survivor(name);
    }

    public void InflictWound(int inflictedWounds)
    {
        Wounds += inflictedWounds;
    }
}
