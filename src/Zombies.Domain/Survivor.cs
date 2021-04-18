using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using Zombies.Domain.BuildingBocks;
using static Zombies.Domain.IHealth;

namespace Zombies.Domain
{
    public class Survivor : IAggregateRoot, IHealth
    {
        private readonly IHealth health;
        private readonly InventoryHandler inventory;
        private Equipment leftHandEquip;
        private Equipment rightHandEquip;

        public Survivor(string name, InventoryHandler inventory, IHealth health)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(inventory, nameof(inventory));
            Guard.Against.Null(health, nameof(health));

            Name = name;
            RemainingActions = 3;
            this.inventory = inventory;
            this.health = health;
        }

        public Equipment LeftHandEquip
        {
            get => leftHandEquip; set
            {
                ValidateEquipmentExistsInInventory(value);

                leftHandEquip = value;
            }
        }

        public string Name { get; }

        public int RemainingActions { get; }

        public Equipment RightHandEquip
        {
            get => rightHandEquip;
            set
            {
                ValidateEquipmentExistsInInventory(value);

                rightHandEquip = value;
            }
        }

        public State CurrentState => health.CurrentState;

        public IReadOnlyCollection<Equipment> BackPack => inventory.Items;

        public int Wounds => health.Wounds;

        public void Wound(int inflictedWounds)
        {

            health.Wound(inflictedWounds);
            inventory.ReduceCapacityBy(1);
        }

        private void ValidateEquipmentExistsInInventory(Equipment value)
        {
            if (!inventory.ContainsEquipment(value))
                throw new InvalidOperationException($"The equipment {value.Name}, is currently not in your inventory. Please use an equipment from your inventory.");
        }
    }
}