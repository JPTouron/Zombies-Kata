namespace Zombies.Application.HistoryRecording.GameHistory.Events
{
    internal interface ISurvivorEventsSubscriber
    {
        void SurvivorDiedEventSubscriber();

        void SurvivorLeveledUpEventSubscriber();
    }
}