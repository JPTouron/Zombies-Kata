using AutoFixture;
using System.Collections.Generic;

namespace Zombies.Domain.Tests
{
    internal static class GameProvider
    {
        internal static IEnumerable<Survivor> CreateGameWithMultipleRandomPlayers( Game g, int survivorsToAdd)
        {

            var survivors = new Fixture().CreateMany<Survivor>(survivorsToAdd);

            foreach (var s in survivors)
            {
                g.AddSurvivor(s);
            }

            return survivors;
        }
    }
}