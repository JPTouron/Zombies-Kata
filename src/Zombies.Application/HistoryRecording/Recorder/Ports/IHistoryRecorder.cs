using Zombies.Application.HistoryRecording.GameHistory;
using Zombies.Application.HistoryRecording.GameHistory.Public;
using Zombies.Application.HistoryRecording.SuvivorHistory;

namespace Zombies.Application.HistoryRecording.Recorder
{
    public interface IHistoryRecorder : ISurvivorEventsRecorder,
                                            IGameEventsRecorder,
                                            IGameHistoryListable
    {
    }
}