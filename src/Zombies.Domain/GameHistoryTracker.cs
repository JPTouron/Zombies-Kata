using System;
using System.Collections.Generic;
using System.Linq;

namespace Zombies.Domain
{
    public interface IClock
    {
        public DateTime Now { get; }
    }

    public interface ISurvivorHistoryTrackingEvents : IGameSurvivorTrackingEvents
    {
        event SurvivorAddedEquipmentEventHandler survivorAddedEquipmentEventHandler;

        event SurvivorWoundedEventHandler survivorWoundedEventHandler;

        string Name { get; }
    }

    public interface IRecordedIncident
    {
        string Incident { get; }

        DateTime IncidentDate { get; }
    }

    public interface IGameHistoryTrackingEvents
    {
        event SurvivorJoinedTheGameEventHandler survivorJoinedTheGameEventHandler;

        event GameEndedEventHandler gameEndedEventHandler;

        event GameStartedEventHandler gameStartedEventHandler;

        event GameLeveledUpEventHandler gameLeveledUpEventHandler;
    }

    public partial class Game
    {
        /// <summary>
        /// Factory method to provide the history class without exposing the impl details
        /// </summary>
        public static IGameHistoryTracker CreateGameHistoryTracker(IClock clock)
        {
            return new GameHistoryTracker(clock);
        }

        private class GameHistoryTracker : IGameHistoryTracker
        {
            private readonly IClock clock;

            private IList<IRecordedIncident> recordedIncidents;

            private IList<ISurvivorHistoryTrackingEvents> trackedSurvivors;

            public GameHistoryTracker(IClock clock)
            {
                this.clock = clock;
                recordedIncidents = new List<IRecordedIncident>();
                trackedSurvivors = new List<ISurvivorHistoryTrackingEvents>();
            }

            public IReadOnlyCollection<IRecordedIncident> RecordedIncidents => (IReadOnlyCollection<IRecordedIncident>)recordedIncidents;

            public void TrackSurvivor(IPlayingSurvivor survivor)
            {
                SubscribeToSurvivorEvents(survivor);
            }

            public void TrackGame(IGameHistoryTrackingEvents game)
            {
                SubscribeToGameEvents(game);
            }

            private void RecordIncident(string incident)
            {
                var ri = new RecordedIncident(incident, clock);
                recordedIncidents.Add(ri);
            }

            private void SubscribeToSurvivorEvents(IPlayingSurvivor survivor)
            {
                var survivorEvs = (ISurvivorHistoryTrackingEvents)survivor;

                survivorEvs.survivorDiedEventHandler += OnSurvivorDiedEventHandler;
                survivorEvs.survivorAddedEquipmentEventHandler += OnSurvivorAddedEquipmentEventHandler;
                survivorEvs.survivorWoundedEventHandler += OnSurvivorWoundedEventHandler;
                survivorEvs.survivorHasLeveledUpEventHandler += OnSurvivorHasLeveledUpEventHandler;
                trackedSurvivors.Add(survivorEvs);
            }

            private void OnSurvivorHasLeveledUpEventHandler(string survivorName, Level newLevel)
            {
                RecordIncident($"Survivor {survivorName} LeveledUp to level: {newLevel}!");
            }

            private void OnSurvivorWoundedEventHandler(string survivorName)
            {
                RecordIncident($"Survivor {survivorName} has been wounded!");
            }

            private void OnSurvivorAddedEquipmentEventHandler(string survivorName, string addedEquipment)
            {
                RecordIncident($"Survivor {survivorName} acquired {addedEquipment}");
            }

            private void OnSurvivorDiedEventHandler(string survivorName)
            {
                RecordIncident($"Survivor {survivorName} has died!");

                UnsubscribeFromSurvivorEvents(survivorName);
            }

            private void UnsubscribeFromSurvivorEvents(string survivorName)
            {
                if (trackedSurvivors.SingleOrDefault(x => x.Name == survivorName) is ISurvivorHistoryTrackingEvents maybeSurvivor)
                {
                    maybeSurvivor.survivorDiedEventHandler -= OnSurvivorDiedEventHandler;
                    maybeSurvivor.survivorAddedEquipmentEventHandler -= OnSurvivorAddedEquipmentEventHandler;
                    maybeSurvivor.survivorWoundedEventHandler -= OnSurvivorWoundedEventHandler;
                    maybeSurvivor.survivorHasLeveledUpEventHandler -= OnSurvivorHasLeveledUpEventHandler;
                }
            }

            private void SubscribeToGameEvents(IGameHistoryTrackingEvents game)
            {
                game.survivorJoinedTheGameEventHandler += OnSurvivorJoinedTheGameEventHandler;
                game.gameEndedEventHandler += OnGameEndedEventHandler;
                game.gameStartedEventHandler += OnGameStartedEventHandler;
                game.gameLeveledUpEventHandler += OnGameLeveledUpEventHandler;
            }

            private void OnGameLeveledUpEventHandler(Level newLevel)
            {
                RecordIncident($"The Game has LeveledUp to level: {newLevel}!");
            }

            private void OnGameStartedEventHandler()
            {
                RecordIncident("A new game has started");
            }

            private void OnGameEndedEventHandler(IGameHistoryTrackingEvents game, Level gameLevel)
            {
                RecordIncident($"The Game has Ended. All survivors have died... Max level reached: {gameLevel}");
                UnsusbcribeFromGameEvents(game);
            }

            private void UnsusbcribeFromGameEvents(IGameHistoryTrackingEvents game)
            {
                game.survivorJoinedTheGameEventHandler -= OnSurvivorJoinedTheGameEventHandler;
                game.gameEndedEventHandler -= OnGameEndedEventHandler;
                game.gameStartedEventHandler -= OnGameStartedEventHandler;
                game.gameLeveledUpEventHandler -= OnGameLeveledUpEventHandler;
            }

            private void OnSurvivorJoinedTheGameEventHandler(string survivorName)
            {
                RecordIncident($"Survivor {survivorName } has joined the game");
            }

            private class RecordedIncident : IRecordedIncident
            {
                public RecordedIncident(string incident, IClock clock)
                {
                    Incident = incident;
                    IncidentDate = clock.Now;
                }

                public DateTime IncidentDate { get; }

                public string Incident { get; }
            }
        }
    }
}