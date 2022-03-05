using Ardalis.GuardClauses;
using System.Collections.Generic;

namespace Zombies.Domain
{
    public interface IZombieUnderAttack
    {
        bool IsAlive { get; }

        bool IsDead { get; }

        void Wound(IKillingSurvivor killingSurvivor);
    }

    public enum Level
    {
        Blue,
        Yellow,
        Orange,
        Red
    }

    public class Survivor : IPlayingSurvivor, IKillingSurvivor, ISurvivorEvents
    {
        private List<string> equipmentInHand;

        private List<string> equipmentInReserve;
        private Level lastLevelObtained;

        public Survivor(string name)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));

            Name = name;
            Wounds = 0;
            Experience = 0;
            AvailableActionsInTurn = 3;
            lastLevelObtained = Level;
            equipmentInHand = new List<string>();
            equipmentInReserve = new List<string>();
        }

        public event SurvivorAddedEquipmentEventHandler survivorAddedEquipmentEventHandler;

        public event SurvivorDiedEventHandler survivorDiedEventHandler;

        public event SurvivorWoundedEventHandler survivorWoundedEventHandler;

        public event SurvivorHasLeveledUpEventHandler survivorHasLeveledUpEventHandler;

        public string Name { get; }

        public int Wounds { get; private set; }

        public int AvailableActionsInTurn { get; }

        public int InHand => equipmentInHand.Count;

        public int InReserve => equipmentInReserve.Count;

        public bool IsAlive => Wounds < 2;

        public bool IsDead => IsAlive == false;

        public int Experience { get; private set; }

        public Level Level => Experience switch
        {
            >= 0 and <= 5 => Level.Blue,
            >= 6 and <= 17 => Level.Yellow,
            >= 18 and <= 41 => Level.Orange,
            _ => Level.Red    // default value
        };

        public void Attack(IZombieUnderAttack z)
        {
            z.Wound(this);

            if (z.IsDead == false)
                Experience++;

            UpdateLastLevelObtainedIfLeveledUp();
        }

        public void AddEquipment(string equipmentName)
        {
            Guard.Against.NullOrEmpty(equipmentName, nameof(equipmentName));

            if (InHand == 2 && InReserve < 3)
            {
                equipmentInReserve.Add(equipmentName);
            }

            if (InHand < 2)
            {
                equipmentInHand.Add(equipmentName);
            }

            survivorAddedEquipmentEventHandler?.Invoke(Name, equipmentName);
        }

        public void Wound()
        {
            Wounds++;

            if (IsAlive)
            {
                if (InReserve > 0)
                    equipmentInReserve.RemoveAt(InReserve - 1);

                if (InReserve == 0 && InHand > 0)
                    equipmentInHand.RemoveAt(InHand - 1);

                if (survivorWoundedEventHandler != null)
                    survivorWoundedEventHandler(Name);
            }

            if (IsDead && survivorDiedEventHandler != null)
                survivorDiedEventHandler(Name);
        }

        private void UpdateLastLevelObtainedIfLeveledUp()
        {
            if (lastLevelObtained < Level)
            {
                lastLevelObtained = Level;
                survivorHasLeveledUpEventHandler?.Invoke(Name, Level);
            }
        }
    }
}