using System;
using System.Text;
using Zombies.Domain.Survivors.SkillAggregate.Tree;

namespace Zombies.Domain.Survivors.SkillAggregate
{

    public abstract class SkillBase : ISkillName
    {
        public SkillBase(string idName,string skillName,XpLevel unlockableAt)
        {
            IdName = idName;
            SkillName = skillName;
            UnlockableAt = unlockableAt;
        }
        public string IdName { get; }
        public string SkillName { get; }
        public XpLevel UnlockableAt { get; }
        public bool Unlocked { get; protected set; }
    }

}
