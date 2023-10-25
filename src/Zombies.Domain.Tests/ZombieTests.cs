using Xunit;
using static Zombies.Domain.Zombie;

namespace Zombies.Domain.Tests;

public class ZombieTests
{
    [Fact]
    public void GivenAValidZombieIsCreated_ThenItHasExpectedInitialState()
    {
        var expectedStartingHealth = 5;
        var expectedStartingStatus = Zombie.ZombieStatus.Alive;
        var zombie = new Zombie();

        Assert.Equal(expectedStartingHealth, zombie.Health);
        Assert.Equal(expectedStartingStatus, zombie.Status);
    }

    [Theory]
    [InlineData(1, 4)]
    [InlineData(2, 3)]
    [InlineData(3, 2)]
    [InlineData(4, 1)]
    [InlineData(5, 0)]
    public void GivenAValidZombieIsCreated_WhenZombieIsWounded_ThenItDecreasesItsHealthByTheTimesItWasWounded(int wounds, int expectedHealthPointsLeft)
    {
        var zombie = new Zombie();
        zombie.InflictWound(wounds);

        Assert.Equal(expectedHealthPointsLeft, zombie.Health);
    }

    [Fact]
    public void GivenAValidZombieIsCreated_WhenZombieIsReachesZeroHealth_ThenItIsDead()
    {
        var zombie = new Zombie();
        zombie.InflictWound(zombie.Health);

        Assert.Equal(0, zombie.Health);
        Assert.True(zombie.IsDead);
    }

    [Theory]
    [InlineData(1, 4, ZombieStatus.Alive)]
    [InlineData(2, 3, ZombieStatus.Alive)]
    [InlineData(3, 2, ZombieStatus.Alive)]
    [InlineData(4, 1, ZombieStatus.Alive)]
    [InlineData(5, 0, ZombieStatus.Dead)]
    public void GivenAValidZombieIsCreated_WhenZombieIsWounded_IsIsDeadOnlyWhenReachingZeroHealth(int wounds,
                                                                                                  int expectedHealthPointsLeft,
                                                                                                  ZombieStatus expectedStatus)
    {
        var zombie = new Zombie();
        zombie.InflictWound(wounds);

        Assert.Equal(expectedHealthPointsLeft, zombie.Health);
        Assert.Equal(expectedStatus, expectedStatus);
    }
}