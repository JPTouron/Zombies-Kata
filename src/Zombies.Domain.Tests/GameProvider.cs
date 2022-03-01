using AutoFixture;
using System.Collections.Generic;

namespace Zombies.Domain.Tests
{
    internal static class GameProvider
    {
        internal static Game CreateGameWithMultipleRandomPlayers(out IEnumerable<Survivor> survivors, int survivorsToAdd)
        {
            var g = new Game();

            survivors = new Fixture().CreateMany<Survivor>(survivorsToAdd);

            foreach (var s in survivors)
            {
                g.AddSurvivor(s);
            }

            return g;
        }
    }
}