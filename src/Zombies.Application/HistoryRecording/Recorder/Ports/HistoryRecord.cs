using System;

namespace Zombies.Application.HistoryRecording.Recorder
{
    public class HistoryRecord
    {
        public HistoryRecord(string message, DateTime timeStamp)
        {
            Message = message;
            TimeStamp = timeStamp;
        }

        public string Message { get; }

        public DateTime TimeStamp { get; }

        public static implicit operator string(HistoryRecord record)
        {
            return record.ToString();
        }

        public override string ToString()
        {
            return TimeStamp.ToString() + " - " + Message;
        }
    }
}