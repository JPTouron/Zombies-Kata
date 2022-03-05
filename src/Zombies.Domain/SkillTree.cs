using System.Collections.Generic;

namespace Zombies.Domain
{
    public interface ISkillTree
    {
        IReadOnlyCollection<string> UnlockedSkills();

        IReadOnlyCollection<string> PotentialSkills();
    }

    public class SkillTreeFactory
    {
        public ISkillTree Create(ISkilledSurvivor survivor)
        {
            return new SkillTree(survivor);
        }
    }

    public class SkillTree : ISkillTree
    {
        private readonly ISkilledSurvivor survivor;

        public SkillTree(ISkilledSurvivor survivor)
        {
            this.survivor = survivor;
        }

        public IReadOnlyCollection<string> PotentialSkills()
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<string> UnlockedSkills()
        {
            if (survivor.Level == Level.Yellow)
            {
                return new List<string> { "+1 Action" };
            }
            return new List<string>();
        }
    }
}