using static Zombies.Domain.GameModel.IGame;

namespace Zombies.Domain.GameModel;

public interface IGame
{
    public enum GameState
    {
        Started,
        Ended
    }

    GameState State { get; }

    int SurvivorsInGame { get; }

    IReadOnlyList<string> SurvivorNames { get; }

    void AddSurvivor(string survivorName);

    ILiteSurvivor GetSurvivor(string survivorName);

    void WoundSurvivor(string survivorName);
}

public class Game : IGame
{
    private List<ISurvivor> survivors;

    private Game()
    {
        survivors = new List<ISurvivor>();
    }

    public GameState State
    {
        get
        {
            if (survivors.Count > 0 && survivors.All(x => ((ILiteSurvivor)x).IsDead))
                return GameState.Ended;
            else
                return GameState.Started;
        }
    }

    public int SurvivorsInGame => survivors.Count;

    public IReadOnlyList<string> SurvivorNames => survivors.Select(x => x.Name).ToList();

    public static IGame Start()
    {
        return new Game();
    }

    public void AddSurvivor(string survivorName)
    {
        if (survivors.Any(x => string.Compare(x.Name, survivorName) == 0))
            throw new SurvivorAlreadyExistsInGameException();

        survivors.Add(Survivor.Create(survivorName));
    }

    public ILiteSurvivor GetSurvivor(string survivorName)
    {
        return (ILiteSurvivor)GetSurvivorFromList(survivorName);
    }

    public void WoundSurvivor(string survivorName)
    {
        var survivor = GetSurvivorFromList(survivorName);
        survivor.InflictWound(1);
    }

    private ISurvivor GetSurvivorFromList(string survivorName)
    {
        return survivors.Single(x => string.Compare(x.Name, survivorName) == 0);
    }
}