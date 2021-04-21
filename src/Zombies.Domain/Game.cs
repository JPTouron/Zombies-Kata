using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zombies.Domain
{
    public class Game
    {

        public enum GamesState
        {

            OnGoing,
            Finished
        }

        public GamesState State
        {
            get
            {

                if (survivors.Any(x => x.CurrentState == IHealth.State.Alive))
                    return GamesState.OnGoing;
                else
                    return GamesState.Finished;
            }
        }

        private IList<Survivor> survivors;

        public Game()
        {
            survivors = new List<Survivor>();
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