using Zombies.Domain;

namespace Zombies.Application.HistoryRecording.GameHistory
{
    public interface IGameEventsRecorder
    {
        void GameFinished();

        void GameLeveledUp(XpLevel level);

        void GameStarted();

        void SurvivorAdded(ISurvivor survivor);
    }
}