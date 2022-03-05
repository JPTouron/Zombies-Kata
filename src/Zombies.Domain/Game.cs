using System;
using System.Collections.Generic;
using System.Linq;

namespace Zombies.Domain
{
    public interface ISkilledSurvivor
    {
        Level Level { get; }

        IReadOnlyCollection<string> UnlockedSkills { get; }
    }

    public interface IPlayingSurvivor : ISkilledSurvivor
    {
        string Name { get; }

        bool IsAlive { get; }

        bool IsDead { get; }
    }

    public interface IGameSurvivorTrackingEvents
    {
        event SurvivorDiedEventHandler survivorDiedEventHandler;

        event SurvivorHasLeveledUpEventHandler survivorHasLeveledUpEventHandler;
    }

    public interface IGameHistoryTracker
    {
        IReadOnlyCollection<IRecordedIncident> RecordedIncidents { get; }

        void TrackSurvivor(IPlayingSurvivor survivor);

        void TrackGame(IGameHistoryTrackingEvents game);
    }

    public delegate void GameEndedEventHandler(IGameHistoryTrackingEvents game, Level gameLevel);

    public delegate void GameStartedEventHandler();

    public delegate void GameLeveledUpEventHandler(Level newLevel);

    public partial class Game : IGameHistoryTrackingEvents
    {
        private readonly IGameHistoryTracker history;

        private IList<IPlayingSurvivor> survivors;

        private Level lastUpgradedLevel;

        public event SurvivorJoinedTheGameEventHandler survivorJoinedTheGameEventHandler;

        public event GameEndedEventHandler gameEndedEventHandler;

        public event GameStartedEventHandler gameStartedEventHandler;

        public event GameLeveledUpEventHandler gameLeveledUpEventHandler;

        public Game(IGameHistoryTracker history)
        {
            survivors = new List<IPlayingSurvivor>();
            this.history = history;
            lastUpgradedLevel = Level;
            TrackThisGameInHistoryAndRecordStart(history);
        }

        public int PlayingSurvivors => survivors.Count;

        public bool HasEnded => survivors.All(x => x.IsDead);

        public Level Level => survivors.Where(x => x.IsAlive)
                                      .Select(x => x.Level)
                                      .DefaultIfEmpty(Level.Blue)
                                      .Max();

        public IReadOnlyCollection<IRecordedIncident> History => history.RecordedIncidents;

        public void AddSurvivor(IPlayingSurvivor s)
        {
            if (survivors.Any(x => x.Name == s.Name))
                throw new InvalidOperationException($"A player with name {s.Name} already exists, cannot add another survivor with that name to the game.");

            survivors.Add(s);

            TrackSurvivorHistoryAndRecordSurvivorAddedGame(s);
        }

        private void TrackThisGameInHistoryAndRecordStart(IGameHistoryTracker history)
        {
            history.TrackGame(this);
            RecordGameStarted();
        }

        private void RecordGameStarted()
        {
            gameStartedEventHandler?.Invoke();
        }

        private void TrackSurvivorHistoryAndRecordSurvivorAddedGame(IPlayingSurvivor survivor)
        {
            history.TrackSurvivor(survivor);
            survivorJoinedTheGameEventHandler?.Invoke(survivor.Name);

            SubscribeToSurvivorEvents(survivor);
        }

        private void SubscribeToSurvivorEvents(IPlayingSurvivor survivor)
        {
            var survivorEvents = (IGameSurvivorTrackingEvents)survivor;

            survivorEvents.survivorDiedEventHandler += OnSurvivorDiedEventHandler;
            survivorEvents.survivorHasLeveledUpEventHandler += OnSurvivorHasLeveledUpEventHandler;
        }

        private void OnSurvivorHasLeveledUpEventHandler(string survivorName, Level newSurvivorLevel)
        {
            if (lastUpgradedLevel < Level)
            {
                lastUpgradedLevel = Level;
                gameLeveledUpEventHandler?.Invoke(Level);
            }
        }

        private void OnSurvivorDiedEventHandler(string survivorName)
        {
            UnsubscribeFromSurvivorEvents(survivorName);

            if (HasEnded)
                gameEndedEventHandler?.Invoke(this, Level);
        }

        private void UnsubscribeFromSurvivorEvents(string survivorName)
        {
            if (survivors.SingleOrDefault(x => x.Name == survivorName) is IGameSurvivorTrackingEvents maybeSurvivor)
            {
                maybeSurvivor.survivorDiedEventHandler -= OnSurvivorDiedEventHandler;
                maybeSurvivor.survivorHasLeveledUpEventHandler -= OnSurvivorHasLeveledUpEventHandler;
            }
        }
    }
}