using System;
using System.Collections.Generic;
using System.Linq;

namespace Zombies.Domain
{
    public interface IClock
    {
        public DateTime Now { get; }
    }

    public class Game
    {
        private IList<Survivor> survivors;
        private List<string> history;

        public Game(IClock clock)
        {
            survivors = new List<Survivor>();
            history = new List<string> { clock.Now.ToString() };
        }

        public int Survivors => survivors.Count;

        public bool HasEnded => survivors.All(x => x.IsAlive == false);

        public Level Level => survivors.Where(x => x.IsAlive)
                                      .Select(x => x.Level)
                                      .DefaultIfEmpty(Level.Blue)
                                      .Max();

        public IReadOnlyCollection<string> History => history;

        public void AddSurvivor(Survivor s)
        {
            if (survivors.Any(x => x.Name == s.Name))
                throw new InvalidOperationException($"A player with name {s.Name} already exists, cannot add another survivor with that name to the game.");

            survivors.Add(s);


            history.Add($"Survivor {s.Name} has joined the game");

        }
    }
}