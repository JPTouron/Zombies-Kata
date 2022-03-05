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


    public delegate void SurvivorAddedEquipmentEventHandler(string survivorName, string addedEquipment);

    public delegate void SurvivorDiedEventHandler(string survivorName);

    public partial class Game : IGameEvents
    {
        private readonly IGameHistoryTracker history;
        private IList<IPlayingSurvivor> survivors;

        public Game(IGameHistoryTracker history)
        {
            survivors = new List<IPlayingSurvivor>();
            this.history = history;
            TrackThisGameInHistoryAndRecordStart(history);
        }

        public event SurvivorJoinedTheGameEventHandler survivorJoinedTheGameEventHandler;

        public event GameEndedEventHandler gameEndedEventHandler;

        public event GameStartedEventHandler gameStartedEventHandler;

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
            if (gameStartedEventHandler != null)
                gameStartedEventHandler();
        }

        private void TrackSurvivorHistoryAndRecordSurvivorAddedGame(IPlayingSurvivor s)
        {
            history.TrackSurvivor(s);
            if (survivorJoinedTheGameEventHandler != null)
                survivorJoinedTheGameEventHandler(s.Name);
        }
    }
}