using System;
using System.Collections.Generic;
using Zombies.Application.HistoryRecording.GameHistory.Events;
using Zombies.Domain;
using Zombies.Domain.Gear;
using Zombies.Domain.Survivors;
using static Zombies.Application.HistoryRecording.SuvivorHistory.ISurvivorEventsPublisher;

namespace Zombies.Application.HistoryRecording.SuvivorHistory
{
    internal class HistoricSurvivor : ISurvivor, ISurvivorEventsPublisher
    {
        private readonly ISurvivorEventsRecorder historicEvents;
        private readonly Survivor survivor;
        private SurvivorDiedEventPublisher survivorDiedEvent;
        private SurvivorLeveledUpEventPublisher survivorLeveledUpEvent;

        public HistoricSurvivor(Survivor survivor, ISurvivorEventsRecorder historicEvents)
        {
            this.survivor = survivor;
            this.historicEvents = historicEvents;
        }

        public int ExperiencePoints => survivor.ExperienceValue;

        public IEquipment LeftHandEquip
        {
            get => survivor.LeftHandEquip;
            set
            {
                survivor.LeftHandEquip = value;
                historicEvents.GrabbedEquipment(this, value, ISurvivorEventsRecorder.Hand.Left);
            }
        }

        public IEquipment RightHandEquip
        {
            get => survivor.RightHandEquip;
            set
            {
                survivor.RightHandEquip = value;
                historicEvents.GrabbedEquipment(this, value, ISurvivorEventsRecorder.Hand.Right);
            }
        }

        public IReadOnlyCollection<IEquipment> BackPack => survivor.BackPack;

        public int BackPackCapacity => survivor.BackPackCapacity;

        public HealthState CurrentState => survivor.CurrentState;

        public XpLevel Level => survivor.Level;

        public string Name => survivor.Name;

        public int RemainingActions => survivor.RemainingActions;

        public int Wounds => survivor.Wounds;

        public void AddEquipment(IEquipment equipment)
        {
            survivor.AddEquipment(equipment);
            historicEvents.AddedEquipment(this, equipment);
        }

        public void Kill(Zombie zombie)
        {
            var currLevel = Level;
            survivor.Kill(zombie);
            historicEvents.KilledAZombie(this);

            if (currLevel != Level)
            {
                historicEvents.LeveledUp(this);
                FireEvent(survivorLeveledUpEvent);
            }
        }

        public void RegisterSurvivorDiedEvent(ISurvivorEventsSubscriber subscriber)
        {
            survivorDiedEvent = new SurvivorDiedEventPublisher(subscriber.SurvivorDiedEventSubscriber);
        }

        public void RegisterSurvivorLeveledUpEvent(ISurvivorEventsSubscriber subscriber)
        {
            survivorLeveledUpEvent = new SurvivorLeveledUpEventPublisher(subscriber.SurvivorLeveledUpEventSubscriber);
        }

        public void Wound(int inflictedWounds)
        {
            survivor.Wound(inflictedWounds);
            historicEvents.Wounded(this);

            if (CurrentState == HealthState.Dead)
            {
                historicEvents.Died(this);
                FireEvent(survivorDiedEvent);
            }
        }

        private void FireEvent(Delegate action)
        {
            if (action != null)
                action.DynamicInvoke();
        }
    }
}