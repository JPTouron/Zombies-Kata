using Ardalis.GuardClauses;
using System.Collections.Generic;
using System.Linq;

namespace Zombies.Domain
{
    public interface ISkillTree
    {
        IReadOnlyCollection<BaseSkill> UnlockedSkills { get; }

        IReadOnlyCollection<BaseSkill> PotentialSkills { get; }

        IReadOnlyCollection<IExistingSkill> AllSkills { get; }
    }

    public delegate void SkillWasUnlockedEventHandler(string skillName);

    public class SkillTreeFactory
    {
        public ISkillTree Create(ISkilledSurvivor survivor)
        {
            return new SkillTree(survivor);
        }
    }

    public abstract class BaseSkill : IExistingSkill
    {
        protected readonly ISkilledSurvivor survivor;
        protected bool isUnlocked;

        public event SkillWasUnlockedEventHandler skillWasUnlockedEventHandler;

        protected BaseSkill(ISkilledSurvivor survivor, string name, int experiencePoinsRequiredToUnlock)
        {
            Guard.Against.Null(survivor, nameof(survivor));
            Guard.Against.NullOrEmpty(name, nameof(name));
            Guard.Against.NegativeOrZero(experiencePoinsRequiredToUnlock, nameof(experiencePoinsRequiredToUnlock));

            this.survivor = survivor;
            Name = name;
            ExperiencePoinsRequiredToUnlock = experiencePoinsRequiredToUnlock;
            isUnlocked = false;
        }

        public string Name { get; }

        public int ExperiencePoinsRequiredToUnlock { get; }

        public Level UnlocksAtLevel => ExperiencePoinsRequiredToUnlock switch
        {
            >= 0 and <= 5 => Level.Blue,
            >= 6 and <= 17 => Level.Yellow,
            >= 18 and <= 41 => Level.Orange,
            _ => Level.Red    // default value
        };

        public bool IsAvailable => survivor.Experience >= ExperiencePoinsRequiredToUnlock;

        public abstract bool IsUnlocked { get; }

        public bool IsLocked => IsUnlocked == false;

        public abstract bool IsPotential { get; }

        public abstract bool IsAutoUnlockable { get; }

        public void Unlock()
        {
            if (IsAvailable && IsUnlocked == false)
            {
                isUnlocked = true;
                skillWasUnlockedEventHandler?.Invoke(Name);
            }
        }
    }

    public class AutoUnlockableSkill : BaseSkill
    {
        private const int autoUnlockableXPThreshold = 50;

        public AutoUnlockableSkill(ISkilledSurvivor survivor, string name, int experiencePoinsRequiredToUnlock)
            : base(survivor, name, experiencePoinsRequiredToUnlock)
        {
        }

        public override bool IsUnlocked
        {
            get
            {
                if (IsAvailable && IsAutoUnlockable)
                    isUnlocked = true;

                var result = isUnlocked;

                return result;
            }
        }

        public override bool IsPotential => false;

        public override bool IsAutoUnlockable => true;
    }

    public class PotentialSkill : BaseSkill
    {
        public PotentialSkill(ISkilledSurvivor survivor, string name, int experiencePoinsRequiredToUnlock)
            : base(survivor, name, experiencePoinsRequiredToUnlock)
        {
            Guard.Against.OutOfRange(experiencePoinsRequiredToUnlock, nameof(experiencePoinsRequiredToUnlock), 1, 49, $"The {experiencePoinsRequiredToUnlock} of a PotentialSkill should be between 1 and 49");
        }

        public override bool IsUnlocked => isUnlocked;

        public override bool IsPotential => true;

        public override bool IsAutoUnlockable => false;
    }

    public class SkillTree : ISkillTree
    {
        private IList<BaseSkill> skills;

        public SkillTree(ISkilledSurvivor survivor)
        {
            CreateSkillsTree(survivor);
        }

        public IReadOnlyCollection<BaseSkill> UnlockedSkills => skills.Where(x => x.IsUnlocked).ToList();

        public IReadOnlyCollection<BaseSkill> PotentialSkills => skills.OfType<PotentialSkill>().Where(x => x.IsLocked).ToList();

        public IReadOnlyCollection<IExistingSkill> AllSkills => (IReadOnlyCollection<IExistingSkill>)skills;

        private void CreateSkillsTree(ISkilledSurvivor survivor)
        {
            skills = new List<BaseSkill> {new AutoUnlockableSkill(survivor,"+1 Action", 6),
                                      new PotentialSkill(survivor,"+1 Die (Ranged)", 18),
                                      new PotentialSkill(survivor,"+1 Die (Melee)", 42),
                                      new AutoUnlockableSkill(survivor,"+1 Free Move Action", 61),
                                      new AutoUnlockableSkill(survivor,"Hoard", 82),
                                      new AutoUnlockableSkill(survivor,"Tough", 129),
                                     };
        }
    }
}