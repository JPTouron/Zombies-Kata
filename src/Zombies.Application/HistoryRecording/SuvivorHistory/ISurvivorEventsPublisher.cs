using Zombies.Application.HistoryRecording.GameHistory.Events;

namespace Zombies.Application.HistoryRecording.SuvivorHistory
{
    internal interface ISurvivorEventsPublisher
    {
        delegate void SurvivorDiedEventPublisher();

        delegate void SurvivorLeveledUpEventPublisher();
        void RegisterSurvivorDiedEvent(ISurvivorEventsSubscriber subscriber);
        void RegisterSurvivorLeveledUpEvent(ISurvivorEventsSubscriber subscriber);

    }
}