using AutoFixture;

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