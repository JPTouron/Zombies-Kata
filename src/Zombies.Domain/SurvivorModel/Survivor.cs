using Zombies.Domain.GameHistory;

namespace Zombies.Domain;

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

    void InflictWound(int inflictedWounds);

    void HitZombie(Zombie zombie);

    void AddHandEquipment(string equipmentName);

    void AddInReserveEquipment(string equipmentName);
}

public partial class Survivor : ISurvivor
{
    private const int maxHealth = 2;
    private readonly ISurvivorHistoryTracker historyTracker;
    private Equipment equipment;
    private ISurvivor.SurvivorLevel previousLevel;

    internal Survivor(string name, ISurvivorHistoryTracker historyTracker)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(nameof(name), "The survivor name is required and cannot be empty");

        Name = name;
        this.historyTracker = historyTracker;
        Wounds = 0;
        Experience = 0;
        ResetPreviousLevel();
        RemainingActions = 3;
        equipment = new Equipment();
    }

    public int Wounds { get; private set; }

    public ISurvivor.SurvivorStatus Status => HasRemainingHealth() ? ISurvivor.SurvivorStatus.Alive : ISurvivor.SurvivorStatus.Dead;

    public string Name { get; }

    public int RemainingActions { get; }

    public IReadOnlyCollection<string> InHandEquipment => equipment.InHandEquipment;

    public IReadOnlyCollection<string> InReserveEquipment => equipment.InReserveEquipment;

    public int InReserveEquipmentCapacity => equipment.CurrentMaximumInReserveEquipmentSize;

    public bool IsDead => Status == ISurvivor.SurvivorStatus.Dead;

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

    public static ISurvivor Create(string name, ISurvivorHistoryTracker historyTracker)
    {
        return new Survivor(name, historyTracker);
    }

    public void AddHandEquipment(string equipmentName)
    {
        equipment.AddEquipment(Equipment.EquipmentType.InHand, equipmentName);
        historyTracker.RecordSurvivorAcquiredEquipment(Name, equipmentName);
    }

    public void AddInReserveEquipment(string equipmentName)
    {
        equipment.AddEquipment(Equipment.EquipmentType.InReserve, equipmentName);
        historyTracker.RecordSurvivorAcquiredEquipment(Name, equipmentName);
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
            historyTracker.RecordSurvivorWasWounded(Name, inflictedWounds, RemainingHealth());
        else
            historyTracker.RecordSurvivorDied(Name);
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
            historyTracker.RecordSurvivorLeveledUp(Name, Level);
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