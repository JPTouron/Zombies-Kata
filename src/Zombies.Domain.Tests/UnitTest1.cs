using AutoFixture;
using Xunit;

namespace Zombies.Domain.Tests;

public class SurvivorProvider
{
    private readonly IFixture fixture;

    public SurvivorProvider(IFixture fixture)
    {
        this.fixture = fixture;
    }

    public ISurvivor CreateValid(string? name = null)
    {
        name ??= fixture.Create<string>();

        var survivor = Survivor.Create(name);

        return survivor;
    }
}

public class SurvivorTests
{
    private IFixture fixture;
    private SurvivorProvider survivorProvider;

    public SurvivorTests()
    {
        fixture = new Fixture();
        survivorProvider = new SurvivorProvider(fixture);
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

        var survivor = survivorProvider.CreateValid(expectedSurvivorName);

        Assert.Equal(expectedStartingWounds, survivor.Wounds);
        Assert.Equal(expectedSurvivorName, survivor.Name);
        Assert.Equal(expectedStartingSurvivorStatus, survivor.Status);
        Assert.Equal(expectedStartingActionsPerTurn, survivor.RemainingActions);
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
}