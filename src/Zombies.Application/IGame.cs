using Zombies.Application.HistoryRecording.GameHistory.Public;
using Zombies.Domain;

namespace Zombies.Application
{
    public interface IGame : IGameHistoryListable
    {
        public enum GameState
        {
            OnGoing,
            Finished
        }

        int ExperiencePoints { get; }

        XpLevel Level { get; }

        GameState State { get; }

        int SurvivorCount { get; }

        ISurvivor AddSurvivor(string name);
    }
}