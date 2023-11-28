using AutoFixture;
using Moq;
using Xunit;
using Zombies.Domain.SurvivorModel.EquipmentModel;
using Zombies.Domain.WeaponsModel;

namespace Zombies.Domain.Tests;

public class SurvivorTests
{
    private IFixture fixture;
    private SurvivorProvider survivorProvider;
    private Mock<IClock> clock;

    public SurvivorTests()
    {
        fixture = new Fixture();
        survivorProvider = new SurvivorProvider(fixture);

        clock = fixture.Create<Mock<IClock>>();
    }

    [Fact]
    public void GivenIWantToCreateASurvivor_ThenIHaveToProvideConstructorParameters()
    {
        var expectedSurvivorName = "juan";

        var survivor = survivorProvider.CreateValid(expectedSurvivorName);

        Assert.Equal(expectedSurvivorName, survivor.Name);
    }

    [Fact]
    public void GivenAValidSurvivorIsCreated_ThenItHasExpectedInitialState()
    {
        var expectedSurvivorName = "juan";
        var expectedStartingWounds = 0;
        var expectedStartingSurvivorStatus = ISurvivor.SurvivorStatus.Alive;
        var expectedStartingActionsPerTurn = 3;
        var expectedStartingExperience = 0;
        var expectedStartingLevel = ISurvivor.SurvivorLevel.Blue;

        var survivor = survivorProvider.CreateValid(expectedSurvivorName);

        Assert.Equal(expectedStartingWounds, survivor.Wounds);
        Assert.Equal(expectedSurvivorName, survivor.Name);
        Assert.Equal(expectedStartingSurvivorStatus, survivor.Status);
        Assert.Equal(expectedStartingActionsPerTurn, survivor.RemainingActions);
        Assert.Equal(expectedStartingExperience, survivor.Experience);
        Assert.Equal(expectedStartingLevel, survivor.Level);
        Assert.Empty(survivor.InHandEquipment);
        Assert.Empty(survivor.InReserveEquipment);
    }

    [Theory]
    [InlineData((string)null)]
    [InlineData("")]
    [InlineData(" ")]
    public void GivenIWantToCreateASurvivor_ItFailsIfNameIsMissing(string emptyName)
    {
        Assert.Throws<ArgumentException>(() => Survivor.Create(emptyName));
    }

    [Theory]
    [InlineData(1, ISurvivor.SurvivorStatus.Alive)]
    [InlineData(2, ISurvivor.SurvivorStatus.Dead)]
    public void GivenAValidSurvivor_WhenItReceivesTwoWounds_ThenSurvivorDies(int inflictedWounds, ISurvivor.SurvivorStatus expectedFinalStatus)
    {
        var survivor = survivorProvider.CreateValid();

        survivor.InflictWound(inflictedWounds);

        Assert.Equal(expectedFinalStatus, survivor.Status);
    }

    [Fact]
    public void GivenAValidSurvivor_WhenAddingHandEquipment_ThenCanHaveUpToTwoElements()
    {
        var expectedTotalEquipmentCount = 2;
        var survivor = survivorProvider.CreateValid();

        survivor.AddHandEquipment(new Bat());
        survivor.AddHandEquipment(new Rockslinger());

        Assert.Equal(expectedTotalEquipmentCount, survivor.InHandEquipment.Count);
    }

    [Fact]
    public void GivenAValidSurvivor_WhenAddingAThirdHandEquipment_ThenThrows()
    {
        var survivor = survivorProvider.CreateValid();

        survivor.AddHandEquipment(new Bat());
        survivor.AddHandEquipment(new Rockslinger());

        Assert.Throws<EquipmentFullException>(() => survivor.AddHandEquipment(new Bat()));
    }

    [Theory]
    [InlineData((string)null)]
    [InlineData("")]
    [InlineData(" ")]
    public void GivenAValidSurvivor_WhenAddingAHandEquipmentWithoutAName_ThenThrows(string equipmentName)
    {
        var survivor = survivorProvider.CreateValid();

        Assert.Throws<ArgumentException>(() => survivor.AddHandEquipment(new AdHocWeapon(equipmentName)));
    }

    [Fact]
    public void GivenAValidSurvivor_WhenAddingReserveEquipment_ThenCanHaveUpToThreeElements()
    {
        var expectedTotalEquipmentCount = 3;

        var survivor = survivorProvider.CreateValid();

        survivor.AddInReserveEquipment(new Bat());
        survivor.AddInReserveEquipment(new Rockslinger());
        survivor.AddInReserveEquipment(new Bat());

        Assert.Equal(expectedTotalEquipmentCount, survivor.InReserveEquipment.Count);
    }

    [Fact]
    public void GivenAValidSurvivor_WhenAddingAFourthReserveEquipment_ThenThrows()
    {
        var survivor = survivorProvider.CreateValid();

        survivor.AddInReserveEquipment(new Bat());
        survivor.AddInReserveEquipment(new Rockslinger());
        survivor.AddInReserveEquipment(new Bat());

        Assert.Throws<EquipmentFullException>(() => survivor.AddInReserveEquipment(new Rockslinger()));
    }

    [Theory]
    [InlineData((string)null)]
    [InlineData("")]
    [InlineData(" ")]
    public void GivenAValidSurvivor_WhenAddingAReserveEquipmentWithoutAName_ThenThrows(string equipmentName)
    {
        var survivor = survivorProvider.CreateValid();

        Assert.Throws<ArgumentException>(() => survivor.AddInReserveEquipment(new AdHocWeapon(equipmentName)));
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    [InlineData(3, 0)]
    public void GivenAValidSurvivor_WhenWounded_ThenInReserveCapacityDecreasesByAmountOfWounds(int woundsToInflict, int expectedEquipmentCapacityAfterWound)
    {
        var survivor = survivorProvider.CreateValid();

        survivor.AddInReserveEquipment(new Bat());
        survivor.AddInReserveEquipment(new Bat());
        survivor.AddInReserveEquipment(new Bat());

        survivor.InflictWound(woundsToInflict);

        Assert.Equal(expectedEquipmentCapacityAfterWound, survivor.InReserveEquipmentCapacity);
    }

    [Fact]
    public void GivenAValidSurvivor_WhenZombieIsKilledByASurvivor_ThenSurvivorIncreasesExperienceByOnePoint()
    {
        var zombie = new Zombie();
        var survivor = survivorProvider.CreateValid();
        var expectedSurvivorExperience = 1;
        while (zombie.IsAlive)
            survivor.HitZombie(zombie);

        Assert.True(zombie.IsDead);
        Assert.Equal(expectedSurvivorExperience, survivor.Experience);
    }

    [Theory]
    [InlineData(0, ISurvivor.SurvivorLevel.Blue)]
    [InlineData(6, ISurvivor.SurvivorLevel.Yellow)]
    [InlineData(18, ISurvivor.SurvivorLevel.Orange)]
    [InlineData(42, ISurvivor.SurvivorLevel.Red)]
    public void GivenAValidSurvivor_WhenAnExperienceThresholdIsGained_ThenALevelIsReached(int experienceToGain, ISurvivor.SurvivorLevel expectedLevelReached)
    {
        var survivor = survivorProvider.CreateValid();
        while (survivor.Experience < experienceToGain)
        {
            var zombie = new Zombie();
            while (zombie.IsAlive)
                survivor.HitZombie(zombie);
        }

        Assert.Equal(expectedLevelReached, survivor.Level);
    }
}