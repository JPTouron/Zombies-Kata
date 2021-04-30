using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using Zombies.Domain.BuildingBocks;
using Zombies.Domain.Gear;

namespace Zombies.Domain.Survivors
{
    public sealed class Survivor : IAggregateRoot
    {
        private readonly Experience experience;
        private readonly Health health;
        private readonly InventoryHandler inventory;
        private IEquipment leftHandEquip;
        private IEquipment rightHandEquip;

        public Survivor(string name, InventoryHandler inventory, Health health, Experience experience)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(inventory, nameof(inventory));
            Guard.Against.Null(health, nameof(health));
            Guard.Against.Null(experience, nameof(experience));

            Name = name;
            RemainingActions = 3;
            this.inventory = inventory;
            this.health = health;
            this.experience = experience;

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

        public int RemainingActions { get; }

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

        public XpLevel Level => experience.Level;

        public int Wounds => health.Wounds;

        public void AddEquipment(IEquipment equipment)
        {
            if (SurvivorIsAlive())
                inventory.AddEquipment(equipment);
        }

        public void Kill(Zombie zombie)
        {
            if (SurvivorIsAlive())
                experience.Increase();
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