using System.Collections.Generic;

namespace Zombies.Domain.Survivors.SkillAggregate.LevelSkills
{
    internal abstract class LevelSkillBase
    {

        public IReadOnlyCollection<SkillBase> Skills => (IReadOnlyCollection<SkillBase>)skills;
        protected IList<SkillBase> skills;
        public LevelSkillBase(short slots)
        {
            skills = new List<SkillBase>(slots);
            InitializeSkills();
        }

        protected abstract void InitializeSkills();
    }

}
