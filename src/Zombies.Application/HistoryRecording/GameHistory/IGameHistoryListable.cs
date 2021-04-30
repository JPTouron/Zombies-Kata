using System.Collections.Generic;
using Zombies.Application.HistoryRecording.Recorder;

namespace Zombies.Application.HistoryRecording.GameHistory.Public
{
    public interface IGameHistoryListable
    {
        IList<HistoryRecord> Events { get; }
    }
}