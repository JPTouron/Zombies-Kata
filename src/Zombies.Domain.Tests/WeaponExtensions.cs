using Zombies.Domain.WeaponsModel;

namespace Zombies.Domain.Tests;


public class AdHocWeapon : IWeapon
{

    public AdHocWeapon(string name, int damage = 0)
    {
        Name = name;
        Damage = damage;
    }

    public int Damage { get; }

    public string Name { get; }
}

public static class WeaponExtensions
{

    public static IWeapon CreateWeaponFromType(this Type weaponType)
    {
        var myInstance = (IWeapon)Activator.CreateInstance(weaponType.GetType());

        return myInstance;
    }

}
