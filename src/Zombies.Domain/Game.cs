using System;
using System.Collections.Generic;
using System.Linq;

namespace Zombies.Domain
{
    public interface ISurvivorName
    {
        string Name { get; }
    }

    public interface IPlayingSurvivor : ISurvivorName
    {
        bool IsAlive { get; }

        bool IsDead { get; }

        Level Level { get; }
    }

    public partial class Game : IGameEvents
    {
        private readonly IGameHistoryTracker history;
        private IList<IPlayingSurvivor> survivors;

        private Level lastUpgradedLevel;

        public Game(IGameHistoryTracker history)
        {
            survivors = new List<IPlayingSurvivor>();
            this.history = history;
            lastUpgradedLevel = Level;
            TrackThisGameInHistoryAndRecordStart(history);
        }

        public event SurvivorJoinedTheGameEventHandler survivorJoinedTheGameEventHandler;

        public event GameEndedEventHandler gameEndedEventHandler;

        public event GameStartedEventHandler gameStartedEventHandler;

        public event GameLeveledUpEventHandler gameLeveledUpEventHandler;

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

        private void TrackSurvivorHistoryAndRecordSurvivorAddedGame(IPlayingSurvivor s)
        {
            history.TrackSurvivor(s);
            survivorJoinedTheGameEventHandler?.Invoke(s.Name);

            SubscribeToSurvivorEvents(s);
        }

        private void SubscribeToSurvivorEvents(IPlayingSurvivor s)
        {
            var ss = (ISurvivorBasicEvents)s;

            ss.survivorDiedEventHandler += OnSurvivorDiedEventHandler;
            ss.survivorHasLeveledUpEventHandler += OnSurvivorHasLeveledUpEventHandler;
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
            if (survivors.SingleOrDefault(x => x.Name == survivorName) is ISurvivorBasicEvents maybeSurvivor)
            {
                maybeSurvivor.survivorDiedEventHandler -= OnSurvivorDiedEventHandler;
                maybeSurvivor.survivorHasLeveledUpEventHandler -= OnSurvivorHasLeveledUpEventHandler;
            }
        }
    }
}