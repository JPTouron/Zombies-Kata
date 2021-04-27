using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Zombies.Application.History;
using Zombies.Domain;
using static Zombies.Application.IGame;

namespace Zombies.Application
{

    public class SurvivorProvider {

        public ISurvivor CreateSurvivor(string name) {

            //JP: WE HAVE A PROBLEM HERE... WE HAVE THE PROVIDER IN APPLICATION AND THE ONE IN DOMAIN
            //CLIENT CAN SEE BOTH, THATS THE PROBLEM, IT SHOULD ONLY SEE THIS ONE IN APPLICATION SO WE CAN CREATE
            // A SURVIORHISTORY
            //ONE SOLUTION: MOVE ALL TO DOMAIN... :-(
            //ANOTHER: FIND A WAY TO HAVE APPLICATION USING DOMAIN AS WE SEE FIT AND THEN BLOCKING CLIENT FROM USING DOMAIN DIRECTLY
            return new SurvivorHistory(new Domain.SurvivorProvider().CreateSurvivor(name), HistoryRecorder.Instance());
        }

    
    }


    internal class Game : IExperience, IGameEventsSubscriber, IGame
    {

        private XpLevel? currentXPLevel;
        private IList<ISurvivor> survivors;
        private readonly IGameHistoricEvents historicEvents;

        public Game(IGameHistoricEvents historicEvents)
        {
            survivors = new List<ISurvivor>();
            currentXPLevel = null;
            this.historicEvents = historicEvents;
            historicEvents.GameStarted();
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

        public IList<string> Events => ((IGameHistory)historicEvents).Events;

        public void AddSurvivor(ISurvivor survivor)
        {
            Guard.Against.Null(survivor, nameof(survivor));

            if (survivors.Contains(survivor))
                throw new InvalidOperationException($"A survivor with the name {survivor.Name} already exists.");

            survivors.Add(survivor);

            SubscribeToSurivorEvents(survivor);

            historicEvents.SurvivorAdded(new SurvivorAddedToGameEvent(survivor));

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
            var events = survivor as ISurvivorEvents;
            if (events != null)
                events.SetGame(this);
        }

        public void SurvivorDiedNotificationHandler() { }
        public void UserLeveledUpNotificationHandler()
        {
            if (currentXPLevel != null)
            {
                var newLevel = Level;
                if (currentXPLevel < newLevel)
                    historicEvents.GameLeveledUp(new GameLeveledUpEvent(newLevel));

            }
            currentXPLevel = Level;
        }

    }
}