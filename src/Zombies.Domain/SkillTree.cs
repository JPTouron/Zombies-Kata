using System.Collections.Generic;

namespace Zombies.Domain
{
    public interface ISkillTree
    {
        IReadOnlyCollection<Skill> Skills();
    }

    public class SkillTreeFactory
    {
        public ISkillTree Create(ISkilledSurvivor survivor)
        {
            return new SkillTree(survivor);
        }
    }

    public class Skill
    {
        private readonly ISkilledSurvivor survivor;
        private bool isUnlocked;

        public Skill(ISkilledSurvivor survivor, string name, int experiencePoinsToUnlock)
        {
            this.survivor = survivor;
            Name = name;
            ExperiencePoinsToUnlock = experiencePoinsToUnlock;
            isUnlocked = false;
        }

        public string Name { get; }

        public int ExperiencePoinsToUnlock { get; }

        public Level UnlocksAtLevel => ExperiencePoinsToUnlock switch
        {
            >= 0 and <= 5 => Level.Blue,
            >= 6 and <= 17 => Level.Yellow,
            >= 18 and <= 41 => Level.Orange,
            _ => Level.Red    // default value
        };

        public bool IsAvailable => survivor.Experience >= ExperiencePoinsToUnlock;

        public bool IsUnavailable => IsAvailable == false;

        public bool IsUnlocked => isUnlocked;

        public bool IsLocked => isUnlocked == false;

        public void UnlockSkill()
        {
            if (IsAvailable)
                isUnlocked = true;
        }
    }

    public class SkillTree : ISkillTree
    {
        private readonly ISkilledSurvivor survivor;
        private IList<Skill> skills;

        public SkillTree(ISkilledSurvivor survivor)
        {
            this.survivor = survivor;
            CreateSkillsTree(survivor);
        }

        public IReadOnlyCollection<Skill> Skills() => (IReadOnlyCollection<Skill>)skills;

        private void CreateSkillsTree(ISkilledSurvivor survivor)
        {
            skills = new List<Skill> {new Skill(survivor,"+1 Action", 6),
                                      new Skill(survivor,"+1 Die (Ranged)", 18),
                                      new Skill(survivor,"+1 Die (Melee)", 42),
                                      new Skill(survivor,"+1 Free Move Action", 61),
                                      new Skill(survivor,"Hoard", 82),
                                      new Skill(survivor,"Tough", 129),
                                     };
        }
    }
}