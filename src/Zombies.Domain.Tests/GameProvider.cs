using AutoFixture;
using System.Collections.Generic;

namespace Zombies.Domain.Tests
{
    internal static class GameProvider
    {
        internal static IEnumerable<Survivor> AddSurvivorsToAGame(IFixture fixture, Game g, int survivorsToAdd)
        {
            var survivors = fixture.CreateMany<Survivor>(survivorsToAdd);

            foreach (var s in survivors)
                g.AddSurvivor(s);

            return survivors;
        }
    }
}