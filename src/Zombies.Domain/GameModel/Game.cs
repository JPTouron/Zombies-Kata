using Zombies.Domain.GameHistory;
using static Zombies.Domain.GameModel.IGame;

namespace Zombies.Domain.GameModel;

public interface IGame : IHistoric
{
    public enum GameLevel
    {
        Blue = 0,
        Yellow = 6,
        Orange = 18,
        Red = 42
    }

    public enum GameState
    {
        Started,
        Ended
    }

    GameState State { get; }

    int SurvivorsInGame { get; }

    IReadOnlyList<string> SurvivorNames { get; }

    GameLevel Level { get; }

    void AddSurvivor(string survivorName);

    ISurvivor GetSurvivor(string survivorName);

    void WoundSurvivor(string survivorName);
}

public class Game : IGame
{
    private readonly IGameHistoryTracker historyTracker;
    private GameLevel previousLevel;

    private IList<ISurvivor> survivors;

    private Game(IGameHistoryTracker historyTracker)
    {
        survivors = new List<ISurvivor>();
        this.historyTracker = historyTracker;
        historyTracker.RecordGameStarted();
        ResetPreviousLevel();
    }

    public GameState State
    {
        get
        {
            if (survivors.Count > 0 && survivors.All(x => x.IsDead))
                return GameState.Ended;
            else
                return GameState.Started;
        }
    }

    public int SurvivorsInGame => survivors.Count;

    public IReadOnlyList<string> SurvivorNames => survivors.Select(x => x.Name).ToList();

    public GameLevel Level
    {
        get
        {
            if (survivors.Count == 0)
                return GameLevel.Blue;

            var levelInt = (int)survivors.Max(x => x.Level);

            return (GameLevel)levelInt;
        }
    }

    public IReadOnlyList<GameEvent> History => historyTracker.History;

    public static IGame Start(IHistoryTracker historyTracker)
    {
        return new Game(historyTracker);
    }

    public void AddSurvivor(string survivorName)
    {
        if (survivors.Any(x => string.Compare(x.Name, survivorName) == 0))
            throw new SurvivorAlreadyExistsInGameException();

        var survivor = Survivor.Create(survivorName);
        survivors.Add(survivor);
        SubscribeToSurvivorEvents((ISurvivorNotifications)survivor);

        historyTracker.RecordSurvivorAdded(survivorName);
    }

    public ISurvivor GetSurvivor(string survivorName)
    {
        return GetSurvivorFromList(survivorName);
    }

    public void WoundSurvivor(string survivorName)
    {
        var survivor = GetSurvivorFromList(survivorName);
        survivor.InflictWound(1);
    }

    private void TryToUpdateGameLevel()
    {
        var currentLevel = Level;
        if (previousLevel != currentLevel)
        {
            historyTracker.RecordGameLevelChanged(currentLevel);
            ResetPreviousLevel();
        }
    }

    private void TryToUpdateGameState()
    {
        if (State == GameState.Ended)
        {
            historyTracker.RecordGameEnded();
            foreach (var survivor in survivors)
            {
                UnSubscribeToSurvivorEvents((ISurvivorNotifications)survivor);
            }
        }
    }

    private void SubscribeToSurvivorEvents(ISurvivorNotifications survivor)
    {
        survivor.OnSurvivorAcquiredEquipment += OnSurvivorAcquiredEquipment;
        survivor.OnSurvivorWounded += OnSurvivorWounded;
        survivor.OnSurvivorDied += OnSurvivorDied;
        survivor.OnSurvivorLeveledUp += OnSurvivorLeveledUp;
    }

    private void OnSurvivorLeveledUp(string survivorName, ISurvivor.SurvivorLevel newLevel)
    {
        historyTracker.RecordSurvivorLeveledUp(survivorName, newLevel);
        TryToUpdateGameLevel();
    }

    private void OnSurvivorDied(string survivorName)
    {
        historyTracker.RecordSurvivorDied(survivorName);
        TryToUpdateGameState();
    }

    private void OnSurvivorWounded(string survivorName, int woundsReceived, int currentHealth)
    {
        historyTracker.RecordSurvivorWasWounded(survivorName, woundsReceived, currentHealth);
    }

    private void OnSurvivorAcquiredEquipment(string survivorName, string equipmentName)
    {
        historyTracker.RecordSurvivorAcquiredEquipment(survivorName, equipmentName);
    }

    private void UnSubscribeToSurvivorEvents(ISurvivorNotifications survivor)
    {
        survivor.OnSurvivorAcquiredEquipment -= OnSurvivorAcquiredEquipment;
        survivor.OnSurvivorWounded -= OnSurvivorWounded;
        survivor.OnSurvivorDied -= OnSurvivorDied;
        survivor.OnSurvivorLeveledUp -= OnSurvivorLeveledUp;
    }

    private void ResetPreviousLevel()
    {
        previousLevel = Level;
    }

    private ISurvivor GetSurvivorFromList(string survivorName)
    {
        return survivors.Single(x => string.Compare(x.Name, survivorName) == 0);
    }
}