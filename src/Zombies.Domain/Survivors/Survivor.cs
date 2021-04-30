using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using Zombies.Domain.BuildingBocks;
using Zombies.Domain.Gear;

namespace Zombies.Domain.Survivors
{
    //JP: MISSING TEST: IF A SURVIVOR IS DEAD, IT CANNOT DO SHIT!
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
        }

        public IReadOnlyCollection<IEquipment> BackPack => inventory.Items;

        public IEquipment LeftHandEquip
        {
            get => leftHandEquip; set
            {
                ValidateEquipmentExistsInInventory(value);

                leftHandEquip = value;
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
            }
        }

        public HealthState CurrentState => health.CurrentState;

        public int ExperienceValue => experience.ExperienceValue;

        public XpLevel Level => experience.Level;

        public int Wounds => health.Wounds;

        public void AddEquipment(IEquipment equipment)
        {
            inventory.AddEquipment(equipment);
        }

        public void Kill(Zombie zombie)
        {
            experience.Increase();
        }

        public void Wound(int inflictedWounds)
        {
            health.Wound(inflictedWounds);
            inventory.ReduceCapacityBy(1);
        }

        private void ValidateEquipmentExistsInInventory(IEquipment value)
        {
            if (!inventory.ContainsEquipment(value))
                throw new InvalidOperationException($"The equipment {value.Name}, is currently not in your inventory. Please use an equipment from your inventory.");
        }
    }
}