namespace Zombies.Domain;

public delegate void SurvivorLeveledUpEventHandler(string survivorName, ISurvivor.SurvivorLevel newLevel);

public delegate void SurvivorDiedEventHandler(string survivorName);

public delegate void SurvivorWoundedEventHandler(string survivorName, int woundsReceived, int currentHealth);

public delegate void SurvivorAcquiredEquipmentEventHandler(string survivorName, string equipmentName);

public interface ISurvivor
{
    public enum SurvivorStatus
    {
        Alive,
        Dead
    }

    public enum SurvivorLevel
    {
        Blue = 0,
        Yellow = 6,
        Orange = 18,
        Red = 42
    }

    string Name { get; }

    SurvivorStatus Status { get; }

    int Wounds { get; }

    int RemainingActions { get; }

    IReadOnlyCollection<string> InHandEquipment { get; }

    IReadOnlyCollection<string> InReserveEquipment { get; }

    int InReserveEquipmentCapacity { get; }

    int Experience { get; }

    SurvivorLevel Level { get; }

    bool IsDead { get; }

    bool IsAlive { get; }

    void InflictWound(int inflictedWounds);

    void HitZombie(Zombie zombie);

    void AddHandEquipment(string equipmentName);

    void AddInReserveEquipment(string equipmentName);
}

internal interface ISurvivorNotifications
{
    event SurvivorAcquiredEquipmentEventHandler OnSurvivorAcquiredEquipment;

    event SurvivorWoundedEventHandler OnSurvivorWounded;

    event SurvivorDiedEventHandler OnSurvivorDied;

    event SurvivorLeveledUpEventHandler OnSurvivorLeveledUp;
}

public partial class Survivor : ISurvivor, ISurvivorNotifications
{
    private const int maxHealth = 2;

    private Equipment equipment;

    private ISurvivor.SurvivorLevel previousLevel;

    public event SurvivorAcquiredEquipmentEventHandler OnSurvivorAcquiredEquipment;

    public event SurvivorWoundedEventHandler OnSurvivorWounded;

    public event SurvivorDiedEventHandler OnSurvivorDied;

    public event SurvivorLeveledUpEventHandler OnSurvivorLeveledUp;

    internal Survivor(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(nameof(name), "The survivor name is required and cannot be empty");

        Name = name;
        Wounds = 0;
        Experience = 0;
        ResetPreviousLevel();
        RemainingActions = 3;
        equipment = new Equipment();

        OnSurvivorAcquiredEquipment = delegate { };
        OnSurvivorWounded = delegate { };
        OnSurvivorDied = delegate { };
        OnSurvivorLeveledUp = delegate { };
    }

    public int Wounds { get; private set; }

    public ISurvivor.SurvivorStatus Status => HasRemainingHealth() ? ISurvivor.SurvivorStatus.Alive : ISurvivor.SurvivorStatus.Dead;

    public string Name { get; }

    public int RemainingActions { get; }

    public IReadOnlyCollection<string> InHandEquipment => equipment.InHandEquipment;

    public IReadOnlyCollection<string> InReserveEquipment => equipment.InReserveEquipment;

    public int InReserveEquipmentCapacity => equipment.CurrentMaximumInReserveEquipmentSize;

    public bool IsDead => Status == ISurvivor.SurvivorStatus.Dead;

    public bool IsAlive => !IsDead;

    public int Experience { get; private set; }

    public ISurvivor.SurvivorLevel Level
    {
        get
        {
            if ((int)ISurvivor.SurvivorLevel.Blue >= Experience && Experience < (int)ISurvivor.SurvivorLevel.Yellow)
                return ISurvivor.SurvivorLevel.Blue;

            if ((int)ISurvivor.SurvivorLevel.Yellow >= Experience && Experience < (int)ISurvivor.SurvivorLevel.Orange)
                return ISurvivor.SurvivorLevel.Yellow;

            if ((int)ISurvivor.SurvivorLevel.Orange >= Experience && Experience < (int)ISurvivor.SurvivorLevel.Red)
                return ISurvivor.SurvivorLevel.Orange;

            return ISurvivor.SurvivorLevel.Red;
        }
    }

    public static ISurvivor Create(string name)
    {
        return new Survivor(name);
    }

    public void AddHandEquipment(string equipmentName)
    {
        equipment.AddEquipment(Equipment.EquipmentType.InHand, equipmentName);
        OnSurvivorAcquiredEquipment(Name, equipmentName);
    }

    public void AddInReserveEquipment(string equipmentName)
    {
        equipment.AddEquipment(Equipment.EquipmentType.InReserve, equipmentName);
        OnSurvivorAcquiredEquipment(Name, equipmentName);
    }

    public void HitZombie(Zombie zombie)
    {
        zombie.InflictWound(1);
        IncreaseExperienceByOneIfZombieDead(zombie);
    }

    public void InflictWound(int inflictedWounds)
    {
        Wounds += inflictedWounds;

        DecreaseEquipmentCapacityBasedOnWounds(inflictedWounds);

        if (HasRemainingHealth())
            OnSurvivorWounded(Name, inflictedWounds, RemainingHealth());
        else
        {
            OnSurvivorDied(Name);
        }
    }

    private bool HasRemainingHealth()
    {
        return RemainingHealth() >= 1;
    }

    private void IncreaseExperienceByOneIfZombieDead(Zombie zombie)
    {
        if (zombie.IsDead)
        {
            Experience++;
            RecordIfLevelUp();
        }
    }

    private void RecordIfLevelUp()
    {
        if (previousLevel < Level)
        {
            ResetPreviousLevel();
            OnSurvivorLeveledUp(Name, Level);
        }
    }

    private void ResetPreviousLevel()
    {
        previousLevel = Level;
    }

    private int RemainingHealth()
    {
        return maxHealth - Wounds;
    }

    private void DecreaseEquipmentCapacityBasedOnWounds(int inflictedWounds)
    {
        for (int i = 0; i < inflictedWounds; i++)
            equipment.DecreaseInReserveCapactityByOne();
    }
}