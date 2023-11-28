namespace Zombies.Domain.WeaponsModel;

public interface IWeapon
{
    int Damage { get; }

    public string Name { get; }
}

public interface IMeleeWeapon : IWeapon
{
}

public interface IRangeWeapon : IWeapon
{
}


public abstract class DefaultMeleeWeapon : IMeleeWeapon
{
    public virtual int Damage => 1;

    public abstract string Name { get; }
}

public class Bat : DefaultMeleeWeapon
{
    public override string Name => "Bat";
}

public abstract class DefaultRangeWeapon : IRangeWeapon
{
    public virtual int Damage => 1;

    public abstract string Name { get; }
}

public class Rockslinger : DefaultRangeWeapon
{
    public override string Name => "Rock Slinger";
}

public class EnhancedWeapon : IWeapon
{
    private int dealtDamage = 0;

    public EnhancedWeapon(IWeapon weapon, int damageCountIncrease)
    {
        dealtDamage = weapon.Damage + damageCountIncrease;
        Name = weapon.Name;
    }

    public int Damage => dealtDamage;

    public string Name { get; }
}


