using System;
using System.Collections.Generic;
using Zombies.Domain;
using Zombies.Domain.Gear;

namespace Zombies.Application.History
{

    internal interface IGameEventsSubscriber
    {
        void SurvivorDiedNotificationHandler();
        void UserLeveledUpNotificationHandler();
    }
    internal abstract class SurvivorEventBase {

        protected readonly ISurvivor survivor;

        protected SurvivorEventBase(ISurvivor survivor) => this.survivor = survivor;
        public string Name => survivor.Name;
    }
    internal sealed class SurvivorAddedToGameEvent : SurvivorEventBase
    {
        public SurvivorAddedToGameEvent(ISurvivor survivor) : base(survivor)
        {
        }
    }

    internal sealed class SurvivorEquipmentEvent:SurvivorEventBase {


        public SurvivorEquipmentEvent(ISurvivor survivor, IEquipment equipmentAdded) : base(survivor)
        {
            EquipmentAdded = equipmentAdded;
        }

        public IEquipment EquipmentAdded { get; }
    }

    internal sealed class GameLeveledUpEvent {
        public GameLeveledUpEvent(XpLevel level)
        {
            Level = level;
        }

        public XpLevel Level { get; }
    }
    internal sealed class SurvivorEvent:SurvivorEventBase
    {

        public SurvivorEvent(ISurvivor survivor) : base(survivor) { }

        public XpLevel Level => survivor.Level;

        public int Wounds => survivor.Wounds;

    }

    
    internal interface ISurvivorHistoricEvents
    {

        void AddedEquipment(SurvivorEquipmentEvent eventData);
        void LeveledUp(SurvivorEvent eventData);
        void Wounded(SurvivorEvent eventData);
        void Died(SurvivorEvent eventData);
    }

    internal class HistoryRecorder: ISurvivorHistoricEvents,IGameHistoricEvents, IGameHistory
    {
        private IList<string> history;
        static HistoryRecorder instance;
        public static HistoryRecorder Instance() {

            if (instance == null)
                instance = new HistoryRecorder();
            return instance;
        }
        private HistoryRecorder()
        {
            history = new List<string>();
        }

        public IList<string> Events => history;



        public void Died(SurvivorEvent eventData)
        {
            Record($"{eventData.Name} has died!");

        }

        public void LeveledUp(SurvivorEvent eventData)
        {
            Record($"{eventData.Name} has leveled up to {eventData.Level}!");

        }

        private void Record(string msg)
        {

            history.Add(DateTime.Now.ToShortDateString() + " - " + msg);
        }

        public void Wounded(SurvivorEvent eventData)
        {
            Record($"{eventData.Name} has been wounded {eventData.Wounds} times!");

        }

        public void AddedEquipment(SurvivorEquipmentEvent eventData)
        {
            Record($"{eventData.Name} has added {eventData.EquipmentAdded.Name}");

        }

        public void GameStarted()
        {
            Record($"A new game has started!");

        }

        public void GameFinished()
        {
            Record($"Game has ended, no more survivors are left!");

        }

        public void SurvivorAdded(SurvivorAddedToGameEvent eventData)
        {
            Record($"Survivor {eventData.Name}, has been added to the game");

        }

        public void GameLeveledUp(GameLeveledUpEvent eventData)
        {
            Record($"Game reached {eventData.Level} level!");

        }
    }

    public interface IGameHistory
    {
        IList<string> Events { get; }

    }

    internal interface IGameHistoricEvents
    {
        void GameStarted();
        void GameFinished();
        void SurvivorAdded(SurvivorAddedToGameEvent eventData);
        void GameLeveledUp(GameLeveledUpEvent eventData);

    }
}