using System;
using System.Collections.Generic;
using System.Text;

namespace Zombies.Domain.Survivors.SkillAggregate.Skills.HoardSkills
{
    public abstract class InventorySkill : SkillBase
    {
        protected InventorySkill(  string skillName, XpLevel unlockableAt) : base("Capacity", skillName, unlockableAt)
        {
        }

        //JP: WE HAVE TO DEFINE HERE HOW THE SKILL WOULD BE DEFINED
        //MEANING, WE NEED TO ALTER THE MAX CAPACITY OF INVENTORY
        //THAT WOULD ENTAIL EXTRACTING CAPACITY AS A DEPENDENCY ON INVENTORY
        //AND THEN PROVIDING A WAY TO ALTER IT
    }

    public class NormalCapacitySkill : InventorySkill
    {
        public NormalCapacitySkill( ) : base("Normal Capacity", XpLevel.Blue)
        {
        }
    }
    public class HoardCapacitySkill : InventorySkill
    {
        public HoardCapacitySkill(XpLevel unlockableAt) : base("Hoarder", unlockableAt)
        {
        }
    }
}
