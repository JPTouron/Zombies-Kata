using Zombies.Domain.GameModel;

namespace Zombies.Domain.GameHistory;

public interface IHistoric
{
    IReadOnlyList<GameEvent> History { get; }
}

public interface IGameHistoryTracker : IHistoric
{
    void RecordGameStarted();

    void RecordSurvivorAdded(string survivorName);

    void RecordGameLevelChanged(IGame.GameLevel newLevel);

    void RecordGameEnded();

    void RecordSurvivorAcquiredEquipment(string survivorName, string equipmentName);

    void RecordSurvivorWasWounded(string survivorName, int woundsReceived, int currentHealth);

    void RecordSurvivorDied(string survivorName);

    void RecordSurvivorLeveledUp(string survivorName, ISurvivor.SurvivorLevel newLevel);
}

public interface IHistoryTracker : IGameHistoryTracker
{
}

public class GameEvent
{
    public GameEvent(HistoryEventTypes type, string message, DateTime dateTime)
    {
        Type = type;
        Message = message;
        DateTime = dateTime;
    }

    public enum HistoryEventTypes
    {
        GameStarted,
        SurvivorAddedToGame,
        SurvivorAcquiredEquipment,
        SurvivorWasWounded,
        SurvivorDied,
        SurvivorLeveledUp,
        GameLeveledUp,
        GameEnded
    }

    public string Message { get; }

    public HistoryEventTypes Type { get; }

    public DateTime DateTime { get; }
}

public class HistoryTrackerFactory
{
    private readonly IClock clock;

    public HistoryTrackerFactory(IClock clock)
    {
        this.clock = clock;
    }

    public IHistoryTracker CreateHistoryTracker()
    {
        return new HistoryTracker(clock);
    }
}

internal class HistoryTracker : IHistoryTracker
{
    private readonly IClock clock;
    private IList<GameEvent> gameEvents;

    public HistoryTracker(IClock clock)
    {
        this.clock = clock;
        gameEvents = new List<GameEvent>();
    }

    public IReadOnlyList<GameEvent> History => gameEvents.ToList();

    public void RecordGameEnded()
    {
        gameEvents.Add(new GameEvent(GameEvent.HistoryEventTypes.GameEnded, $"Game Ended, all survivors have died!", clock.UtcNow));
    }

    public void RecordGameLevelChanged(IGame.GameLevel newLevel)
    {
        gameEvents.Add(new GameEvent(GameEvent.HistoryEventTypes.GameLeveledUp, $"Game reached a new level: {newLevel}!", clock.UtcNow));
    }

    public void RecordGameStarted()
    {
        gameEvents.Add(new GameEvent(GameEvent.HistoryEventTypes.GameStarted, "A new game has started!", clock.UtcNow));
    }

    public void RecordSurvivorAcquiredEquipment(string survivorName, string equipmentName)
    {
        gameEvents.Add(new GameEvent(GameEvent.HistoryEventTypes.SurvivorAcquiredEquipment, $"Survivor {survivorName} acquired {equipmentName}", clock.UtcNow));
    }

    public void RecordSurvivorAdded(string survivorName)
    {
        gameEvents.Add(new GameEvent(GameEvent.HistoryEventTypes.SurvivorAddedToGame, $"Survivor {survivorName} just joined the game", clock.UtcNow));
    }

    public void RecordSurvivorDied(string survivorName)
    {
        gameEvents.Add(new GameEvent(GameEvent.HistoryEventTypes.SurvivorDied, $"Survivor {survivorName} has died", clock.UtcNow));
    }

    public void RecordSurvivorLeveledUp(string survivorName, ISurvivor.SurvivorLevel newLevel)
    {
        gameEvents.Add(new GameEvent(GameEvent.HistoryEventTypes.SurvivorLeveledUp, $"Survivor {survivorName} has leveled up to {newLevel} level!", clock.UtcNow));
    }

    public void RecordSurvivorWasWounded(string survivorName, int woundsReceived, int currentHealth)
    {
        gameEvents.Add(new GameEvent(GameEvent.HistoryEventTypes.SurvivorWasWounded, $"Survivor {survivorName} was wounded {woundsReceived} times, remaining health: {currentHealth}", clock.UtcNow));
    }
}