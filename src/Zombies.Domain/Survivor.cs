using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using Zombies.Domain.BuildingBocks;
using Zombies.Domain.Gear;
using Zombies.Domain.Inventory;
using static Zombies.Domain.IHealth;

namespace Zombies.Domain
{
    public class Survivor : IAggregateRoot, IHealth, IExperience
    {
        private readonly Experience experience;
        private readonly IHealth health;
        private readonly IInventoryHandler inventory;
        private IEquipment leftHandEquip;
        private IEquipment rightHandEquip;

        public Survivor(string name, IInventoryHandler inventory, IHealth health, Experience experience)
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

        public void Kill(Zombie zombie)
        {
            experience.Increase();
        }

        public IReadOnlyCollection<IEquipment> BackPack => inventory.Items;

        public int ExperienceValue => experience.ExperienceValue;

        public IEquipment LeftHandEquip
        {
            get => leftHandEquip; set
            {
                ValidateEquipmentExistsInInventory(value);

                leftHandEquip = value;
            }
        }

        public XpLevel Level => experience.Level;

        //TODO: Kill(Zombie zombie)
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

        public State CurrentState => health.CurrentState;

        public int Wounds => health.Wounds;

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