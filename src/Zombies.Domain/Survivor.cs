using Ardalis.GuardClauses;
using System.Collections.Generic;
using System.Linq;

namespace Zombies.Domain
{
    public interface IZombieUnderAttack
    {
        bool IsAlive { get; }

        bool IsDead { get; }

        void Wound(IKillingSurvivor killingSurvivor);
    }

    public interface ISurvivor : IPlayingSurvivor, IKillingSurvivor, ISurvivorHistoryTrackingEvents
    { }

    public interface IExistingSkill
    {
        event SkillWasUnlockedEventHandler skillWasUnlockedEventHandler;

        int ExperiencePoinsRequiredToUnlock { get; }

        string Name { get; }

        bool IsUnlocked { get; }
    }

    public delegate void SurvivorAddedEquipmentEventHandler(string survivorName, string addedEquipment);

    public delegate void SurvivorDiedEventHandler(string survivorName);

    public delegate void SurvivorWoundedEventHandler(string survivorName);

    public delegate void SurvivorHasLeveledUpEventHandler(string survivorName, Level newLevel);

    public delegate void SurvivorHasUnlockedANewSkillEventHandler(string survivorName, string skillName);

    public delegate void SurvivorJoinedTheGameEventHandler(string survivorName);

    public enum Level
    {
        Blue,
        Yellow,
        Orange,
        Red
    }

    public class Survivor : ISurvivor
    {
        private IList<string> equipmentInHand;

        private IList<string> equipmentInReserve;

        private Level lastLevelObtained;

        private ISkillTree skillTree;

        private int availableActionsInTurn;

        public event SurvivorAddedEquipmentEventHandler survivorAddedEquipmentEventHandler;

        public event SurvivorDiedEventHandler survivorDiedEventHandler;

        public event SurvivorWoundedEventHandler survivorWoundedEventHandler;

        public event SurvivorHasLeveledUpEventHandler survivorHasLeveledUpEventHandler;

        public event SurvivorHasUnlockedANewSkillEventHandler survivorHasUnlockedANewSkillEventHandler;

        public Survivor(string name, SkillTreeFactory skillTree)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));
            Guard.Against.Null(skillTree, nameof(skillTree));

            Name = name;
            Wounds = 0;
            Experience = 0;
            availableActionsInTurn = 3;
            lastLevelObtained = Level;
            CreateSkillTree(skillTree);
            equipmentInHand = new List<string>();
            equipmentInReserve = new List<string>();
        }

        public string Name { get; }

        public int Wounds { get; private set; }

        public int AvailableActionsInTurn => availableActionsInTurn;

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

        public IReadOnlyCollection<BaseSkill> UnlockedSkills => skillTree.UnlockedSkills;

        public IReadOnlyCollection<BaseSkill> PotentialSkills => skillTree.PotentialSkills;

        public void Attack(IZombieUnderAttack z)
        {
            z.Wound(this);

            if (z.IsDead == true)
            {
                Experience++;

                LevelUpSurvivorIfEnoughExperiencePointsReached();
                RecordHistoryIfNewSkillWasUnlocked();
            }
        }

        public void AddEquipment(string equipmentName)
        {
            Guard.Against.NullOrEmpty(equipmentName, nameof(equipmentName));

            var maxReserveSize = 3;
            if (HoardSkillIsUnlocked())
                maxReserveSize = 4;

            if (InHand == 2 && InReserve < maxReserveSize)
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
                FireDeadEventAndUnsubscribeFromSkillEvents();
        }

        private void CreateSkillTree(SkillTreeFactory skillTree)
        {
            this.skillTree = skillTree.Create(this);
            SubscribeToSkillEvents();
        }

        private void SubscribeToSkillEvents()
        {
            skillTree.AllSkills.ToList().ForEach(x => x.skillWasUnlockedEventHandler += OnSkillWasUnlockedEventHandler);
        }

        private void UnSubscribeFromSkillEvents()
        {
            skillTree.AllSkills.ToList().ForEach(x => x.skillWasUnlockedEventHandler -= OnSkillWasUnlockedEventHandler);
        }

        private void OnSkillWasUnlockedEventHandler(string skillName)
        {
            survivorHasUnlockedANewSkillEventHandler?.Invoke(Name, skillName);
        }

        private void FireDeadEventAndUnsubscribeFromSkillEvents()
        {
            survivorDiedEventHandler(Name);

            UnSubscribeFromSkillEvents();
        }

        private void RecordHistoryIfNewSkillWasUnlocked()
        {
            var maybeSkillUnlocked = skillTree.AllSkills.SingleOrDefault(x => x.ExperiencePoinsRequiredToUnlock == Experience && x.IsUnlocked);

            if (maybeSkillUnlocked != null)
                survivorHasUnlockedANewSkillEventHandler?.Invoke(Name, maybeSkillUnlocked.Name);
        }

        private bool HoardSkillIsUnlocked()
        {
            return UnlockedSkills.Any(x => x.Name == "Hoard");
        }

        private void LevelUpSurvivorIfEnoughExperiencePointsReached()
        {
            var hasLeveledUp = lastLevelObtained < Level;
            if (hasLeveledUp)
            {
                lastLevelObtained = Level;
                survivorHasLeveledUpEventHandler?.Invoke(Name, Level);

                if (Level == Level.Yellow)
                    availableActionsInTurn++;
            }
        }
    }
}