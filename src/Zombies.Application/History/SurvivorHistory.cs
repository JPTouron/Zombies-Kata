using System;
using System.Collections.Generic;
using Zombies.Domain;
using Zombies.Domain.Gear;

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
        private readonly ISurvivor survivor;
        private SurvivorDiedEvent survivorDiedNotifier;
        private SurvivorLeveledUpEvent survivorLeveledUpNotifier;

        public SurvivorHistory(ISurvivor survivor, ISurvivorHistoricEvents historicEvents)
        {
            this.survivor = survivor;
            this.historicEvents = historicEvents;
        }

        public IEquipment LeftHandEquip { get => survivor.LeftHandEquip; set => survivor.LeftHandEquip = value; }

        public IEquipment RightHandEquip { get => survivor.RightHandEquip; set => survivor.RightHandEquip = value; }

        public IReadOnlyCollection<IEquipment> BackPack => survivor.BackPack;

        public IHealth.State CurrentState => survivor.CurrentState;

        public int ExperienceValue => survivor.ExperienceValue;

        public XpLevel Level => survivor.Level;

        public string Name => survivor.Name;

        public int RemainingActions => survivor.RemainingActions;

        public int Wounds => survivor.Wounds;

        public void AddEquipment(IEquipment equipment)
        {
            survivor.AddEquipment(equipment);
            historicEvents.AddedEquipment(new SurvivorEquipmentEvent(this, equipment));
        }

        public void Kill(Zombie zombie)
        {
            var currLevel = Level;
            survivor.Kill(zombie);

            if (currLevel != Level)
            {
                historicEvents.LeveledUp(new SurvivorEvent(this));
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
            historicEvents.Wounded(new SurvivorEvent(this));

            if (CurrentState == IHealth.State.Dead)
            {
                historicEvents.Died(new SurvivorEvent(this));
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