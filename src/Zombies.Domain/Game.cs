using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Zombies.Domain
{
    public class Game : IExperience
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

        public int ExperienceValue => MaxOrDefault(survivors, x => x.ExperienceValue);

        public XpLevel Level => MaxOrDefaultXPLevel(survivors, x => x.Level);

        public void AddSurvivor(Survivor survivor)
        {
            Guard.Against.Null(survivor, nameof(survivor));

            if (survivors.Contains(survivor))
                throw new InvalidOperationException($"A survivor with the name {survivor.Name} already exists.");

            survivors.Add(survivor);
        }

        private int MaxOrDefault<T>(IList<T> source, Expression<Func<T, int?>> selector, int nullValue = 0)
        {
            return source.AsQueryable().Max(selector) ?? nullValue;
        }

        private XpLevel MaxOrDefaultXPLevel<T>(IList<T> source, Expression<Func<T, XpLevel?>> selector, XpLevel nullValue = 0)
        {
            return source.AsQueryable().Max(selector) ?? nullValue;
        }
    }
}