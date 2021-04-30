using System;
using System.Collections.Generic;
using Zombies.Domain;
using Zombies.Domain.Gear;
using Zombies.Domain.Survivors;

namespace Zombies.Application.History
{
    internal interface ISurvivorEvents
    {
        void SetGame(Game game); //should be an internal iface for the game
    }

    internal delegate void SurvivorDiedEvent();

    internal delegate void SurvivorLeveledUpEvent();

    internal class SurvivorHistory : ISurvivor, ISurvivorEvents
    {
        private readonly ISurvivorHistoricEvents historicEvents;
        private readonly Survivor survivor;
        private SurvivorDiedEvent survivorDiedNotifier;
        private SurvivorLeveledUpEvent survivorLeveledUpNotifier;

        public SurvivorHistory(Survivor survivor, ISurvivorHistoricEvents historicEvents)
        {
            this.survivor = survivor;
            this.historicEvents = historicEvents;
        }

        public int BackPackCapacity => survivor.BackPackCapacity;

        public IEquipment LeftHandEquip { get => survivor.LeftHandEquip; set => survivor.LeftHandEquip = value; }

        public IEquipment RightHandEquip { get => survivor.RightHandEquip; set => survivor.RightHandEquip = value; }

        public IReadOnlyCollection<IEquipment> BackPack => survivor.BackPack;

        public HealthState CurrentState => survivor.CurrentState;

        public int ExperiencePoints => survivor.ExperienceValue;

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

            if (currLevel != Level)
            {
                historicEvents.LeveledUp(this);
                FireEvent(survivorLeveledUpNotifier);
            }
        }

        public void SetGame(Game game)
        {
            survivorLeveledUpNotifier = new SurvivorLeveledUpEvent(game.UserLeveledUpNotificationHandler);
            survivorDiedNotifier = new SurvivorDiedEvent(game.SurvivorDiedNotificationHandler);
        }

        public void Wound(int inflictedWounds)
        {
            survivor.Wound(inflictedWounds);
            historicEvents.Wounded(this);

            if (CurrentState == HealthState.Dead)
            {
                historicEvents.Died(this);
                FireEvent(survivorDiedNotifier);
            }
        }

        private void FireEvent(Delegate action)
        {
            if (action != null)
                action.DynamicInvoke();
        }
    }
}