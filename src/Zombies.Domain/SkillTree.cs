using System.Collections.Generic;

namespace Zombies.Domain
{
    public interface ISkillTree
    {
        IReadOnlyCollection<string> UnlockedSkills(ISkilledSurvivor survivor);
        IReadOnlyCollection<string> PotentialSkills(ISkilledSurvivor survivor);
    }

    public class SkillTree : ISkillTree
    {
        public SkillTree()
        {
        }

        public IReadOnlyCollection<string> PotentialSkills(ISkilledSurvivor survivor)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<string> UnlockedSkills(ISkilledSurvivor survivor) {

            if (survivor.Level== Level.Yellow)
            {
                return new List<string> { "+1 Action" };
            }
                return new List<string>();
        }
    }
}