using System;
using System.Collections.Generic;
using System.Linq;

namespace Zombies.Domain
{
    public interface IClock
    {
        public DateTime Now { get; }
    }

    public interface ISurvivorEvents : ISurvivorName
    {
        event SurvivorAddedEquipmentEventHandler survivorAddedEquipmentEventHandler;

        event SurvivorDiedEventHandler survivorDiedEventHandler;
    }

    public interface IRecordedIncident
    {
        string Incident { get; }

        DateTime IncidentDate { get; }
    }

    public interface IGameEvents
    {
        event SurvivorJoinedTheGameEventHandler survivorJoinedTheGameEventHandler;

        event GameEndedEventHandler gameEndedEventHandler;

        event GameStartedEventHandler gameStartedEventHandler;
    }

    public interface IGameHistoryTracker
    {
        IReadOnlyCollection<IRecordedIncident> RecordedIncidents { get; }

        void TrackSurvivor(IPlayingSurvivor survivor);

        void TrackGame(IGameEvents game);
    }

    public delegate void SurvivorJoinedTheGameEventHandler(string survivorName);

    public delegate void GameEndedEventHandler(IGameEvents game);

    public delegate void GameStartedEventHandler();

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

            private IList<ISurvivorEvents> trackedSurvivors;

            public GameHistoryTracker(IClock clock)
            {
                this.clock = clock;
                recordedIncidents = new List<IRecordedIncident>();
                trackedSurvivors = new List<ISurvivorEvents>();
            }

            public IReadOnlyCollection<IRecordedIncident> RecordedIncidents => (IReadOnlyCollection<IRecordedIncident>)recordedIncidents;

            public void TrackSurvivor(IPlayingSurvivor survivor)
            {
                SubscribeToSurvivorEvents(survivor);
            }

            public void TrackGame(IGameEvents game)
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
                var survivorEvs = (ISurvivorEvents)survivor;

                survivorEvs.survivorDiedEventHandler += OnSurvivorDiedEventHandler;
                survivorEvs.survivorAddedEquipmentEventHandler += OnSurvivorAddedEquipmentEventHandler;
                trackedSurvivors.Add(survivorEvs);
            }

            private void OnSurvivorAddedEquipmentEventHandler(string survivorName, string addedEquipment)
            {
                RecordIncident($"Survivor {survivorName} acquired {addedEquipment}");
            }

            private void OnSurvivorDiedEventHandler(string survivorName)
            {
                UnsubscribeEventsFromSurvivor(survivorName);
            }

            private void UnsubscribeEventsFromSurvivor(string survivorName)
            {
                if (trackedSurvivors.SingleOrDefault(x => x.Name == survivorName) is ISurvivorEvents maybeSurvivor)
                {
                    maybeSurvivor.survivorAddedEquipmentEventHandler -= OnSurvivorAddedEquipmentEventHandler;
                    maybeSurvivor.survivorDiedEventHandler -= OnSurvivorDiedEventHandler;
                }
            }

            private void SubscribeToGameEvents(IGameEvents game)
            {
                game.survivorJoinedTheGameEventHandler += OnSurvivorJoinedTheGameEventHandler;
                game.gameEndedEventHandler += OnGameEndedEventHandler;
                game.gameStartedEventHandler += OnGameStartedEventHandler;
            }

            private void OnGameStartedEventHandler()
            {
                RecordIncident("A new game has started");
            }

            private void OnGameEndedEventHandler(IGameEvents game)
            {
                UnsusbcribeFromGameEvents(game);
            }

            private void UnsusbcribeFromGameEvents(IGameEvents game)
            {
                game.survivorJoinedTheGameEventHandler -= OnSurvivorJoinedTheGameEventHandler;
                game.gameEndedEventHandler -= OnGameEndedEventHandler;
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