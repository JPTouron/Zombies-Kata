namespace Zombies.Domain;

public class Zombie
{
    private const int MaxHealth = 5;
    private const int MinHealth = 0;

    public Zombie()
    {
        Health = MaxHealth;
    }

    public enum ZombieStatus
    {
        Alive,
        Dead
    }

    public int Health { get; private set; }

    public bool IsDead => !IsAlive;

    public bool IsAlive => Health > 0;

    public ZombieStatus Status => Health > 0 ? ZombieStatus.Alive : ZombieStatus.Dead;

    public void InflictWound(int inflictedWounds)
    {
        if (Health > MinHealth)
        {
            Health -= inflictedWounds;
        }
    }
}