using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Zombies.Application.HistoryRecording.GameHistory;
using Zombies.Application.HistoryRecording.GameHistory.Events;
using Zombies.Application.HistoryRecording.GameHistory.Public;
using Zombies.Application.HistoryRecording.Recorder;
using Zombies.Application.HistoryRecording.SuvivorHistory;
using Zombies.Domain;
using static Zombies.Application.IGame;

namespace Zombies.Application
{
    internal class Game : ISurvivorEventsSubscriber, IGame
    {
        private readonly IGameEventsRecorder gameEventsRecorder;
        private XpLevel? currentXPLevel;
        private IList<ISurvivor> survivors;

        public Game(IGameEventsRecorder gameEventsRecorder)
        {
            survivors = new List<ISurvivor>();
            currentXPLevel = null;
            this.gameEventsRecorder = gameEventsRecorder;
            gameEventsRecorder.GameStarted();
        }

        public GameState State
        {
            get
            {
                if (survivors.Any(x => x.CurrentState == HealthState.Alive))
                    return GameState.OnGoing;
                else
                    return GameState.Finished;
            }
        }

        public int SurvivorCount => survivors.Count;

        public IList<HistoryRecord> Events => ((IGameHistoryListable)gameEventsRecorder).Events;

        public int ExperiencePoints => MaxOrDefault(survivors, x => x.ExperiencePoints);

        public XpLevel Level => MaxOrDefaultXPLevel(survivors, x => x.Level);

        public ISurvivor AddSurvivor(string name)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));

            VerifySurvivorHasAUniqueName(name, survivors.Select(x => x.Name));

            var survivor = Providers.Survivor(name);

            survivors.Add(survivor);

            SubscribeToSurivorEvents(survivor);

            gameEventsRecorder.SurvivorAdded(survivor);

            return survivor;
        }

        public void SurvivorDiedEventSubscriber()
        {
            if (State == GameState.Finished)
                gameEventsRecorder.GameFinished();
        }

        public void SurvivorLeveledUpEventSubscriber()
        {
            var newLevel = Level;

            if (CurrentLevelNotInitializedOrLowerThanNewLevel(newLevel))
            {
                gameEventsRecorder.GameLeveledUp(newLevel);
                currentXPLevel = Level;
            }
        }

        private bool CurrentLevelNotInitializedOrLowerThanNewLevel(XpLevel newLevel)
        {
            return currentXPLevel == null || currentXPLevel < newLevel;
        }

        private int MaxOrDefault<T>(IList<T> source, Expression<Func<T, int?>> selector, int nullValue = 0)
        {
            return source.AsQueryable().Max(selector) ?? nullValue;
        }

        private XpLevel MaxOrDefaultXPLevel<T>(IList<T> source, Expression<Func<T, XpLevel?>> selector, XpLevel nullValue = 0)
        {
            return source.AsQueryable().Max(selector) ?? nullValue;
        }

        private void SubscribeToSurivorEvents(ISurvivor survivor)
        {
            var events = survivor as ISurvivorEventsPublisher;

            if (events != null)
            {
                events.RegisterSurvivorDiedEvent(this);
                events.RegisterSurvivorLeveledUpEvent(this);
            }
        }

        private void VerifySurvivorHasAUniqueName(string name, IEnumerable<string> survivorNames)
        {
            var rule = new SurvivorNameMustBeUniqueRule(name, survivorNames.ToList());

            if (rule.IsBroken())
                throw new InvalidOperationException(rule.Message);
        }
    }
}