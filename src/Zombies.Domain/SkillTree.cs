using System.Collections.Generic;

namespace Zombies.Domain
{
    public interface ISkillTree
    {
        IReadOnlyCollection<string> UnlockedSkills(ISkilledSurvivor survivor);
    }

    public class SkillTree : ISkillTree
    {
        public SkillTree()
        {
        }

        public IReadOnlyCollection<string> UnlockedSkills(ISkilledSurvivor survivor) {

            if (survivor.Level== Level.Yellow)
            {
                return new List<string> { "Action +1" };
            }
                return new List<string>();
        }
    }
}