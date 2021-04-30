using System;
using System.Collections.Generic;
using Zombies.Application.HistoryRecording.GameHistory.Events;
using Zombies.Application.HistoryRecording.Recorder;
using Zombies.Application.HistoryRecording.SuvivorHistory.Events;
using Zombies.Domain;
using Zombies.Domain.Gear;
using static Zombies.Application.HistoryRecording.SuvivorHistory.ISurvivorEventsRecorder;

namespace Zombies.Application.HistoryRecording.Infrastructure
{
    internal class HistoryRecorder : IHistoryRecorder
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
            Record(eventData);
        }

        public void Died(ISurvivor survivor)
        {
            var eventData = new SurvivorDiedEventMessage(survivor);
            Record(eventData);
        }

        public void GameFinished()
        {
            Record(new GameFinishedEventMessage());
        }

        public void GameLeveledUp(XpLevel level)
        {
            var eventData = new GameLeveledUpEventMessage(level);
            Record(eventData);
        }

        public void GameStarted()
        {
            Record(new GameStartedEventMessage());
        }

        public void GrabbedEquipment(ISurvivor survivor, IEquipment equipment, Hand hand)
        {
            Record(new SurvivorGrabbedWeaponEventMessage(survivor, equipment, hand));
        }

        public void KilledAZombie(ISurvivor survivor)
        {
            var eventData = new SurvivorKilledAZombieEventMessage(survivor);
            Record(eventData);
        }

        public void LeveledUp(ISurvivor survivor)
        {
            var eventData = new SurvivorLeveledUpEventMessage(survivor);
            Record(eventData);
        }

        public void SurvivorAdded(ISurvivor survivor)
        {
            var eventData = new SurvivorAddedToGameEventMessage(survivor);
            Record(eventData);
        }

        public void Wounded(ISurvivor survivor)
        {
            var eventData = new SurvivorWoundedEventMessage(survivor);
            Record(eventData);
        }

        private void Record(string msg)
        {
            history.Add(new HistoryRecord(msg, DateTime.Now));
        }
    }
}