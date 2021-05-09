using System;
using System.Collections.Generic;
using System.Linq;
using Zombies.Domain.Survivors.SkillAggregate.ActionSkills;
using Zombies.Domain.Survivors.SkillAggregate.Skills.HoardSkills;

namespace Zombies.Domain.Survivors.SkillAggregate.Tree
{
    public interface ISkillTreeRenderer
    {
        /// <summary>
        /// shows the user the skll tree as a list or tree graph or whatev
        /// </summary>
        string Render();
    }
    /// <summary>
    /// should be a provider of skills by means of strategy pattern
    /// </summary>
    internal interface ISkillsProvider
    {


    }

    public interface ISkillName
    {
        string IdName { get; }
    }
    /// <summary>
    /// Gets a list of avail skills given an amount of exp points
    /// </summary>
    public interface ISkillSelector
    {
        IReadOnlyCollection<ISkillName> GetAvailableSkills();


    }
    /// <summary>
    /// allows to apply a particular skill
    /// </summary>
    public interface ISkillUnlocker
    {


        void ApplySkill(ISkillName skill);
    }

    //JP: CONTINUE WITH: INVENTORY SKILL AND EXTRACTING CAPACITY FROM IT AND PLACING INVENTORY AS A DEP ON SKILLTREE
    
    //JP: SKILL TREE SHOULD HAVE A DEPENDENCY ON EXPERIENCE, AND ALL ACCESS TO EXPERIENCE SHOULD GO THROUGH SKILLTREE
    //MOST LIKELY WILL HAVE TO GIVE THIS, A DEPENDENCY OVER INVENTORY, AND OVER ALL THINGS UNDER SKILL TREE..... IT MAKES SENSE...
    public class SkillTree : ISkillTreeRenderer, ISkillSelector, ISkillUnlocker
    {
        private readonly Experience experience;
        IList<SkillBase> skills;
        public void IncreaseExperience()
        {
            experience.Increase();
        }
        public SkillTree(Experience experience)
        {
            this.experience = experience;
            skills = new List<SkillBase>();
            skills.Add(new NormalActionSkill());
        }
        public void ApplySkill(ISkillName skill)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ISkillName> GetAvailableSkills()
        {
            /* pseudo-code
                if ( experience.ExperiencePoints == ...)
                {

                }
             */
            throw new NotImplementedException();
        }

        public string Render()
        {
            throw new NotImplementedException();
        }

        public ActionSkill Action
        {
            get
            {
                //this would always return an action and then, when it gets upgraded from normal to enhanced (by calling applyskill meth) then 
                //when we call this again we'll return the enhanced instead of the normal skill
                return (ActionSkill)skills.Single(x => x.IdName == "Action");

            }

        }

        public InventorySkill Inventory
        {
            get
            {
                //this would always return an action and then, when it gets upgraded from normal to enhanced (by calling applyskill meth) then 
                //when we call this again we'll return the enhanced instead of the normal skill
                return (InventorySkill)skills.Single(x => x.IdName == "Capacity");

            }

        }
    }
}