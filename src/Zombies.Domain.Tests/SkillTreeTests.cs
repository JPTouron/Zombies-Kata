using AutoFixture;
using Moq;
using Xunit;
using Zombies.Domain.WeaponsModel;

namespace Zombies.Domain.Tests;

public class SkillTreeTests
{
    private IFixture fixture;
    private SurvivorProvider survivorProvider;
    private Mock<IClock> clock;

    public SkillTreeTests()
    {
        fixture = new Fixture();
        survivorProvider = new SurvivorProvider(fixture);

        clock = fixture.Create<Mock<IClock>>();
    }

    [Fact]
    public void GivenAValidSurvivor_WhenSurvivorReachesLevelYellow_ThenAnExtraActionSkillIsUnlocked()
    {
        var expectedActions = 4;

        var survivor = survivorProvider.CreateValid();
        survivor.IncreaseSurvivorLevel(ISurvivor.SurvivorLevel.Yellow);

        Assert.Equal(expectedActions, survivor.RemainingActions);
    }

    [Theory]
    [InlineData(ISurvivor.OrangeSkills.PlusOneDieRanged)]
    [InlineData(ISurvivor.OrangeSkills.PlusOneDieMelee)]
    public void GivenAValidSurvivor_WhenSurvivorReachesLevelOrange_ThenAnOptionalWeaponTypeIncreasesDamageOnCurrentlyObtainedWeapons(ISurvivor.OrangeSkills skillToUnlock)
    {
        var expectedHits = 3;

        var survivor = survivorProvider.CreateValid();

        IWeapon weaponToAdd;
        if (skillToUnlock == ISurvivor.OrangeSkills.PlusOneDieMelee)
            weaponToAdd = new Bat();
        else
            weaponToAdd = new Rockslinger();

        survivor.AddHandEquipment(weaponToAdd);

        survivor.IncreaseSurvivorLevel(ISurvivor.SurvivorLevel.Orange);
        survivor.UnlockOrangeSkill(skillToUnlock);

        var z = new Zombie();
        var actualHits = 0;
        while (z.IsAlive)
        {
            survivor.HitZombie(z);
            actualHits++;
        }

        Assert.Equal(expectedHits, actualHits);
    }

    [Theory]
    [InlineData(ISurvivor.OrangeSkills.PlusOneDieRanged)]
    [InlineData(ISurvivor.OrangeSkills.PlusOneDieMelee)]
    public void GivenAValidSurvivor_WhenSurvivorReachesLevelOrangeAndWeaponIsAddedInHand_ThenAnOptionalWeaponTypeIncreasesDamageWeapons(ISurvivor.OrangeSkills skillToUnlock)
    {
        var expectedHits = 3;

        var survivor = survivorProvider.CreateValid();

        survivor.IncreaseSurvivorLevel(ISurvivor.SurvivorLevel.Orange);
        survivor.UnlockOrangeSkill(skillToUnlock);

        IWeapon weaponToAdd;
        if (skillToUnlock == ISurvivor.OrangeSkills.PlusOneDieMelee)
            weaponToAdd = new Bat();
        else
            weaponToAdd = new Rockslinger();

        survivor.AddHandEquipment(weaponToAdd);

        var z = new Zombie();
        var actualHits = 0;
        while (z.IsAlive)
        {
            survivor.HitZombie(z);
            actualHits++;
        }

        Assert.Equal(expectedHits, actualHits);
    }

    [Theory]
    [InlineData(ISurvivor.OrangeSkills.PlusOneDieRanged)]
    [InlineData(ISurvivor.OrangeSkills.PlusOneDieMelee)]
    public void GivenAValidSurvivor_WhenSurvivorReachesLevelOrangeAndWeaponIsAddedInReserve_ThenAnOptionalWeaponTypeIncreasesDamageWeapons(ISurvivor.OrangeSkills skillToUnlock)
    {
        var expectedHits = 3;

        var survivor = survivorProvider.CreateValid();

        survivor.IncreaseSurvivorLevel(ISurvivor.SurvivorLevel.Orange);
        survivor.UnlockOrangeSkill(skillToUnlock);

        IWeapon weaponToAdd;
        if (skillToUnlock == ISurvivor.OrangeSkills.PlusOneDieMelee)
            weaponToAdd = new Bat();
        else
            weaponToAdd = new Rockslinger();

        survivor.AddInReserveEquipment(weaponToAdd);

        var z = new Zombie();
        var actualHits = 0;
        while (z.IsAlive)
        {
            survivor.HitZombie(z);
            actualHits++;
        }

        Assert.Equal(expectedHits, actualHits);
    }
}