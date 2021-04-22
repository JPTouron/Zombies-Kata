using Ardalis.GuardClauses;
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

        public enum GameState
        {
            OnGoing,
            Finished
        }

        public GameState State
        {
            get
            {
                if (survivors.Any(x => x.CurrentState == IHealth.State.Alive))
                    return GameState.OnGoing;
                else
                    return GameState.Finished;
            }
        }

        public int SurvivorCount => survivors.Count;

        public void AddSurvivor(Survivor survivor)
        {
            Guard.Against.Null(survivor, nameof(survivor));

            if (survivors.Contains(survivor))
                throw new InvalidOperationException($"A survivor with the name {survivor.Name} already exists.");

            survivors.Add(survivor);
        }
    }
}