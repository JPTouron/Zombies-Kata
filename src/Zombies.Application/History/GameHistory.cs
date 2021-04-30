using System;
using System.Collections.Generic;
using Zombies.Application.History.EventMessages;
using Zombies.Domain;
using Zombies.Domain.Gear;

namespace Zombies.Application.History
{
    public interface IGameHistory
    {
        IList<HistoryRecord> Events { get; }
    }

    internal interface IGameEventsSubscriber
    {
        void SurvivorDiedNotificationHandler();

        void UserLeveledUpNotificationHandler();
    }

    internal interface IGameHistoricEvents
    {
        void GameFinished();

        void GameLeveledUp(XpLevel level);

        void GameStarted();

        void SurvivorAdded(ISurvivor survivor);
    }

    internal interface ISurvivorHistoricEvents
    {
        void AddedEquipment(ISurvivor survivor, IEquipment equipmentAdded);

        void Died(ISurvivor survivor);

        void LeveledUp(ISurvivor survivor);

        void Wounded(ISurvivor survivor);
    }

    public class HistoryRecord
    {
        public HistoryRecord(string message, DateTime timeStamp)
        {
            Message = message;
            TimeStamp = timeStamp;
        }

        public string Message { get; }

        public DateTime TimeStamp { get; }

        public static implicit operator String(HistoryRecord record)
        {
            return record.ToString();
        }

        public override string ToString()
        {
            return TimeStamp.ToShortDateString() + " - " + Message;
        }
    }

    internal class HistoryRecorder : ISurvivorHistoricEvents, IGameHistoricEvents, IGameHistory
    {
        private static HistoryRecorder instance;
        private IList<HistoryRecord> history;

        private HistoryRecorder()
        {
            history = new List<HistoryRecord>();
        }

        public IList<HistoryRecord> Events => history;

        public static HistoryRecorder Instance()
        {
            if (instance == null)
                instance = new HistoryRecorder();
            return instance;
        }

        public void AddedEquipment(ISurvivor survivor, IEquipment equipmentAdded)
        {
            var eventData = new SurvivorEquipmentEventMessage(survivor, equipmentAdded);
            Record(eventData.Message);
        }

        public void Died(ISurvivor survivor)
        {
            var eventData = new SurvivorDiedEventMessage(survivor);
            Record(eventData.Message);
        }

        public void GameFinished()
        {
            Record(new GameFinishedEventMessage().Message);
        }

        public void GameLeveledUp(XpLevel level)
        {
            var eventData = new GameLeveledUpEventMessage(level);
            Record(eventData.Message);
        }

        public void GameStarted()
        {
            Record(new GameStartedEventMessage().Message);
        }

        public void LeveledUp(ISurvivor survivor)
        {
            var eventData = new SurvivorLeveledUpEventMessage(survivor);
            Record(eventData.Message);
        }

        public void SurvivorAdded(ISurvivor survivor)
        {
            var eventData = new SurvivorAddedToGameEventMessage(survivor);
            Record(eventData.Message);
        }

        public void Wounded(ISurvivor survivor)
        {
            var eventData = new SurvivorWoundedEventMessage(survivor);
            Record(eventData.Message);
        }

        private void Record(string msg)
        {
            history.Add(new HistoryRecord(msg, DateTime.Now));
        }
    }
}