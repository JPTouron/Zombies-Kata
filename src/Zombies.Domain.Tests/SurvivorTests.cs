using AutoFixture;
using Moq;
using Xunit;
using Zombies.Domain.GameHistory;
using Zombies.Domain.SurvivorModel.EquipmentModel;

namespace Zombies.Domain.Tests;

public class SurvivorTests
{
    private IFixture fixture;
    private SurvivorProvider survivorProvider;
    HistoryTrackerFactory historyTrackerFactory;
    Mock<IClock> clock;

    public SurvivorTests()
    {
        fixture = new Fixture();
        survivorProvider = new SurvivorProvider(fixture);

        clock = fixture.Create<Mock<IClock>>();
        historyTrackerFactory = new HistoryTrackerFactory(clock.Object);
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
        Assert.Throws<ArgumentException>(() => Survivor.Create(emptyName, historyTrackerFactory.CreateHistoryTracker()));
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

    [Theory]
    [InlineData("a")]
    [InlineData("b", "c")]
    public void GivenAValidSurvivor_WhenAddingHandEquipment_ThenCanHaveUpToTwoElements(params string[] equipmentToAdd)
    {
        var survivor = survivorProvider.CreateValid();

        foreach (var item in equipmentToAdd)
        {
            survivor.AddHandEquipment(item);
        }

        Assert.Equal(equipmentToAdd, survivor.InHandEquipment);
    }

    [Fact]
    public void GivenAValidSurvivor_WhenAddingAThirdHandEquipment_ThenThrows()
    {
        var survivor = survivorProvider.CreateValid();

        survivor.AddHandEquipment("a");
        survivor.AddHandEquipment("b");

        Assert.Throws<EquipmentFullException>(() => survivor.AddHandEquipment("c"));
    }

    [Theory]
    [InlineData((string)null)]
    [InlineData("")]
    [InlineData(" ")]
    public void GivenAValidSurvivor_WhenAddingAHandEquipmentWithoutAName_ThenThrows(string equipmentName)
    {
        var survivor = survivorProvider.CreateValid();

        Assert.Throws<ArgumentException>(() => survivor.AddHandEquipment(equipmentName));
    }

    [Theory]
    [InlineData("a")]
    [InlineData("b", "c")]
    [InlineData("b", "c", "d")]
    public void GivenAValidSurvivor_WhenAddingReserveEquipment_ThenCanHaveUpToThreeElements(params string[] equipmentToAdd)
    {
        var survivor = survivorProvider.CreateValid();

        foreach (var item in equipmentToAdd)
        {
            survivor.AddInReserveEquipment(item);
        }

        Assert.Equal(equipmentToAdd, survivor.InReserveEquipment);
    }

    [Fact]
    public void GivenAValidSurvivor_WhenAddingAFourthReserveEquipment_ThenThrows()
    {
        var survivor = survivorProvider.CreateValid();

        survivor.AddInReserveEquipment("a");
        survivor.AddInReserveEquipment("b");
        survivor.AddInReserveEquipment("c");

        Assert.Throws<EquipmentFullException>(() => survivor.AddInReserveEquipment("d"));
    }

    [Theory]
    [InlineData((string)null)]
    [InlineData("")]
    [InlineData(" ")]
    public void GivenAValidSurvivor_WhenAddingAReserveEquipmentWithoutAName_ThenThrows(string equipmentName)
    {
        var survivor = survivorProvider.CreateValid();

        Assert.Throws<ArgumentException>(() => survivor.AddInReserveEquipment(equipmentName));
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    [InlineData(3, 0)]
    public void GivenAValidSurvivor_WhenWounded_ThenInReserveCapacityDecreasesByAmountOfWounds(int woundsToInflict, int expectedEquipmentCapacityAfterWound)
    {
        var survivor = survivorProvider.CreateValid();

        survivor.AddInReserveEquipment("a");
        survivor.AddInReserveEquipment("b");
        survivor.AddInReserveEquipment("c");

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