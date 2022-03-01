using System;
using System.Collections.Generic;
using System.Linq;

namespace Zombies.Domain
{
    public class Game
    {
        private IList<Survivor> survivors;

        public Game()
        {
            survivors = new List<Survivor>();
        }

        public int Survivors => survivors.Count;

        public bool HasEnded => survivors.All(x => x.IsAlive == false);

        public void AddSurvivor(Survivor s)
        {
            if (survivors.Any(x => x.Name == s.Name))
                throw new InvalidOperationException($"A player with name {s.Name} already exists, cannot add another survivor with that name to the game.");

            survivors.Add(s);
        }
    }
}