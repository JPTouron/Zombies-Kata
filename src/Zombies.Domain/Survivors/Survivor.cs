using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using Zombies.Domain.BuildingBocks;
using Zombies.Domain.Gear;
using Zombies.Domain.Survivors.SkillAggregate.Tree;

namespace Zombies.Domain.Survivors
{
    //JP: DO THIS CHANGES:
    /*
    1- HEALTH, EXPERIENCE AND MAYBE INV HANDLER TO VALUE OBJECTS AND IMMUTABLE
    2- INV HANDLER SHOULD HAVE THE CAPACITY HANDLING STRIPPED FROM IT, IS WAY TOO COMPLEX CODE
    3- ENCAPSULATE ALL THE SURVIVOR INTEGRATION CLASSES WITHIN SURVIVOR AND HIDE THEM, NO NEED TO HAVE THEM AVAILABLE FOR USAGE ON THE OUTSIDE (AT LEAST TRY TO DO THIS)
    4- BASE ON THESE 2 COURSES: V KORIKOV DDD AND PLOEH OUTSIDE-IN TDD
    5- INTEGRATE THE GAME AND HISTORY STUFF INTO DOMAIN LAYER
     
     */

    public sealed class Survivor : IAggregateRoot
    {
        private readonly Experience experience;
        private readonly SkillTree skillTree;
        private readonly Health health;
        private readonly InventoryHandler inventory;
        private IEquipment leftHandEquip;
        private IEquipment rightHandEquip;

        public Survivor(string name, InventoryHandler inventory, Health health, Experience experience, SkillTree skillTree)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(inventory, nameof(inventory));
            Guard.Against.Null(health, nameof(health));
            Guard.Against.Null(experience, nameof(experience));

            Name = name;
            this.inventory = inventory;
            this.health = health;
            this.experience = experience;
            this.skillTree = skillTree;
            leftHandEquip = new NoEquipment();
            rightHandEquip = new NoEquipment();
        }

        public IReadOnlyCollection<IEquipment> BackPack => inventory.Items;

        public int BackPackCapacity => inventory.Capacity;

        public int ExperienceValue => experience.ExperiencePoints;

        public IEquipment LeftHandEquip
        {
            get => leftHandEquip; set
            {
                ValidateEquipmentExistsInInventory(value);
                leftHandEquip = value;

                SwitchFromRightHandIfRequired(value);
            }
        }

        public string Name { get; }

        public int RemainingActions => skillTree.Action.Remaining;

        public IEquipment RightHandEquip
        {
            get => rightHandEquip;
            set
            {
                ValidateEquipmentExistsInInventory(value);
                rightHandEquip = value;

                SwitchFromLeftHandIfRequired(value);
            }
        }

        public HealthState CurrentState => health.CurrentState;

        public XpLevel Level => experience.Level;// => skilltree.level

        public int Wounds => health.Wounds;

        public void AddEquipment(IEquipment equipment)
        {
            if (SurvivorIsAlive())
                inventory.AddEquipment(equipment);
        }

        public void Kill(Zombie zombie)
        {
            if (SurvivorIsAlive())
                skillTree.IncreaseExperience();
                //experience.Increase();
        }

        public void Wound(int inflictedWounds)
        {
            if (SurvivorIsAlive())
            {
                health.Wound(inflictedWounds);
                inventory.ReduceCapacityBy(1);
            }
        }

        private bool SurvivorIsAlive()
        {
            return CurrentState == HealthState.Alive;
        }

        private void SwitchFromLeftHandIfRequired(IEquipment value)
        {
            if (leftHandEquip == value)
                leftHandEquip = new NoEquipment();
        }

        private void SwitchFromRightHandIfRequired(IEquipment value)
        {
            if (rightHandEquip == value)
                rightHandEquip = new NoEquipment();
        }

        private void ValidateEquipmentExistsInInventory(IEquipment value)
        {
            if (!inventory.ContainsEquipment(value))
                throw new InvalidOperationException($"The equipment {value.Name}, is currently not in your inventory. Please use an equipment from your inventory.");
        }
    }
}