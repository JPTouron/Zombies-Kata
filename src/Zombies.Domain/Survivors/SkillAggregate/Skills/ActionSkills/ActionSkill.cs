namespace Zombies.Domain.Survivors.SkillAggregate.ActionSkills
{
    public abstract class ActionSkill : SkillBase
    {

        protected ActionSkill(short remaining,string skillName,XpLevel unlockableAt) : base("Action",skillName,unlockableAt)
        {
            Remaining = remaining;

        }
        public short Remaining { get; }



    }
    internal class NormalActionSkill : ActionSkill
    {

        public NormalActionSkill() : base(3,"Normal action", XpLevel.Blue)
        {

        }
    }
    internal class EnhancedActionSkill : ActionSkill
    {

        public EnhancedActionSkill( XpLevel unlockableAt) : base(4, "Action +1",unlockableAt)
        {

        }
    }

}
