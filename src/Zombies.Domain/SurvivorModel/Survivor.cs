using Zombies.Domain.WeaponsModel;

namespace Zombies.Domain;

public delegate void SurvivorLeveledUpEventHandler(string survivorName, ISurvivor.SurvivorLevel newLevel);

public delegate void SurvivorDiedEventHandler(string survivorName);

public delegate void SurvivorWoundedEventHandler(string survivorName, int woundsReceived, int currentHealth);

public delegate void SurvivorAcquiredEquipmentEventHandler(string survivorName, string equipmentName);

public interface ISurvivor
{
    public enum OrangeSkills
    {
        PlusOneDieRanged,
        PlusOneDieMelee
    }

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

    IReadOnlyCollection<IWeapon> InHandEquipment { get; }

    IReadOnlyCollection<IWeapon> InReserveEquipment { get; }

    int InReserveEquipmentCapacity { get; }

    int Experience { get; }

    SurvivorLevel Level { get; }

    bool IsDead { get; }

    bool IsAlive { get; }

    void InflictWound(int inflictedWounds);

    void HitZombie(Zombie zombie);

    //jp: this should only have a single slot (one weapon in hand only)
    void AddHandEquipment(IWeapon weapon);

    void AddInReserveEquipment(IWeapon weapon);

    void UnlockOrangeSkill(OrangeSkills skillToUnlock);
}

internal interface ISurvivorNotifications
{
    event SurvivorAcquiredEquipmentEventHandler OnSurvivorAcquiredEquipment;

    event SurvivorWoundedEventHandler OnSurvivorWounded;

    event SurvivorDiedEventHandler OnSurvivorDied;

    event SurvivorLeveledUpEventHandler OnSurvivorLeveledUp;
}

internal interface ISkill
{
    int Increase { get; }

    string Name { get; }

    string Description { get; }
}

public class Skill
{
}

public class Skills
{ }

public partial class Survivor : ISurvivor, ISurvivorNotifications
{
    private const int maxHealth = 2;

    private Equipment equipment;

    private ISurvivor.SurvivorLevel previousLevel;

    private int remainingActions = 0;

    private IReadOnlyList<ISurvivor.OrangeSkills> orangeSkills;

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
        remainingActions = 3;
        equipment = new Equipment();

        OnSurvivorAcquiredEquipment = delegate { };
        OnSurvivorWounded = delegate { };
        OnSurvivorDied = delegate { };
        OnSurvivorLeveledUp = delegate { };

        orangeSkills = new List<ISurvivor.OrangeSkills>();
    }

    public int Wounds { get; private set; }

    public ISurvivor.SurvivorStatus Status => HasRemainingHealth() ? ISurvivor.SurvivorStatus.Alive : ISurvivor.SurvivorStatus.Dead;

    public string Name { get; }

    public int RemainingActions
    {
        get
        {
            if (Level >= ISurvivor.SurvivorLevel.Yellow)
                return remainingActions + 1;
            else
                return remainingActions;
        }
    }

    public IReadOnlyCollection<IWeapon> InHandEquipment => equipment.InHandEquipment;

    public IReadOnlyCollection<IWeapon> InReserveEquipment => equipment.InReserveEquipment;

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

    public void AddHandEquipment(IWeapon weapon)
    {
        weapon = EnhanceWeaponIfApplicable(weapon);

        equipment.AddEquipment(Equipment.EquipmentType.InHand, weapon);
        OnSurvivorAcquiredEquipment(Name, weapon.Name);
    }

    public void AddInReserveEquipment(IWeapon weapon)
    {
        weapon = EnhanceWeaponIfApplicable(weapon);

        equipment.AddEquipment(Equipment.EquipmentType.InReserve, weapon);
        OnSurvivorAcquiredEquipment(Name, weapon.Name);
    }

    public void HitZombie(Zombie zombie)
    {
        var woundsToInflict = 1;

        if (InHandEquipment.Count > 0)
            woundsToInflict = InHandEquipment.FirstOrDefault()?.Damage ?? woundsToInflict;
        else
            woundsToInflict = InReserveEquipment.FirstOrDefault()?.Damage ?? woundsToInflict;


        //jp: the change for the skills related to damage and weapons should be around this hitting part,
        //then we need to just update the wounds to inflict parameter according to the currently skills and damage values for the current survivor
        //this is a way simpler approach instead of having to be aware of newly added weapons and existing weapons and such
        //it could happen that at some point we want a detail of the weapons hit damage, and that could force us to change the strategy, but also,
        //we can have an iface for the survivor to pass in the current attack skills related to weapons to calculate these values on the fly

        zombie.InflictWound(woundsToInflict);
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

    public void UnlockOrangeSkill(ISurvivor.OrangeSkills skillToUnlock)
    {
        if (skillToUnlock == ISurvivor.OrangeSkills.PlusOneDieMelee)
            equipment.EnhanceMeleeWeapons(1);
        else
            equipment.EnhanceRangedWeapons(1);

        var tempy = orangeSkills.ToList();
        tempy.Add(skillToUnlock);
        orangeSkills = tempy;
    }

    private IWeapon EnhanceWeaponIfApplicable(IWeapon weapon)
    {
        if (orangeSkills.Count > 0 && orangeSkills.Count < 2)
        {
            if (orangeSkills.First() == ISurvivor.OrangeSkills.PlusOneDieMelee && weapon is IMeleeWeapon)
                weapon = new EnhancedWeapon(weapon, 1);
            else
                weapon = new EnhancedWeapon(weapon, 1);
        }
        else if (orangeSkills.Count == 2)
            weapon = new EnhancedWeapon(weapon, 1);

        return weapon;
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