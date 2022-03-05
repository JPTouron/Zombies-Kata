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

    public delegate void SurvivorAddedEquipmentEventHandler(string survivorName, string addedEquipment);

    public delegate void SurvivorDiedEventHandler(string survivorName);

    public delegate void SurvivorWoundedEventHandler(string survivorName);

    public delegate void SurvivorHasLeveledUpEventHandler(string survivorName, Level newLevel);

    public delegate void SurvivorJoinedTheGameEventHandler(string survivorName);

    public enum Level
    {
        Blue,
        Yellow,
        Orange,
        Red
    }

    public class Survivor : IPlayingSurvivor, IKillingSurvivor, ISurvivorHistoryTrackingEvents
    {
        private IList<string> equipmentInHand;

        private IList<string> equipmentInReserve;

        private Level lastLevelObtained;

        public event SurvivorAddedEquipmentEventHandler survivorAddedEquipmentEventHandler;

        public event SurvivorDiedEventHandler survivorDiedEventHandler;

        public event SurvivorWoundedEventHandler survivorWoundedEventHandler;

        public event SurvivorHasLeveledUpEventHandler survivorHasLeveledUpEventHandler;

        private Survivor(string name)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));

            Name = name;
            Wounds = 0;
            Experience = 0;
            AvailableActionsInTurn = 3;
            lastLevelObtained = Level;
            Skills = new SkillTree();
            equipmentInHand = new List<string>();
            equipmentInReserve = new List<string>();
        }

        private Survivor(string name, ISkillTree skillTree) : this(name)
        {
            Guard.Against.Null(skillTree, nameof(skillTree));

            Skills = skillTree;
        }

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

        public ISkillTree Skills { get; }

        public static Survivor CreateWithEmptySkillTree(string name)
        {
            return new Survivor(name);
        }

        public static Survivor CreateWithSkillTree(string name, ISkillTree skillTree)
        {
            return new Survivor(name, skillTree);
        }

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