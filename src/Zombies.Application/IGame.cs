using Zombies.Application.History;
using Zombies.Domain;

namespace Zombies.Application
{
    public interface IGame : IGameHistory
    {
        public enum GameState
        {
            OnGoing,
            Finished
        }

        int ExperienceValue { get; }

        XpLevel Level { get; }

        GameState State { get; }

        int SurvivorCount { get; }

        ISurvivor AddSurvivor(string name);
    }
}