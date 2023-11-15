using AutoFixture;
using Moq;
using Zombies.Domain.GameHistory;

namespace Zombies.Domain.Tests;

public static class SurvivorExtensions
{
    public static void IncreaseSurvivorLevel(this ISurvivor survivor1, ISurvivor.SurvivorLevel desiredSurvivorLevel)
    {
        while (survivor1.Level < desiredSurvivorLevel)
        {
            var zombie = new Zombie();
            while (zombie.IsAlive)
                survivor1.HitZombie(zombie);
        }
    }

    public static void Kill(this ISurvivor survivor)
    {
        while (survivor.IsAlive)
        {
            survivor.InflictWound(1);
        }
    }
}

public class SurvivorProvider
{
    private readonly IFixture fixture;

    public SurvivorProvider(IFixture fixture)
    {
        this.fixture = fixture;
    }

    public ISurvivor CreateValid(string? name = null,
                                 ISurvivorHistoryTracker? survivorHistoryTracker = null)//,
                                                                                        //ISurvivorNotifications? survivorNotifications = null)
    {
        name ??= fixture.Create<string>();

        survivorHistoryTracker ??= new HistoryTrackerFactory(fixture.Create<Mock<IClock>>().Object).CreateHistoryTracker();
        //survivorNotifications ??= new HistoryTrackerFactory(fixture.Create<Mock<IClock>>().Object).CreateHistoryTracker();

        var survivor = Survivor.Create(name, survivorHistoryTracker);//, SurvivorNotifications);

        return survivor;
    }
}