using Ardalis.GuardClauses;
using System.Collections.Generic;
using Zombies.Domain.BuildingBocks;
using static Zombies.Domain.IHealth;

namespace Zombies.Domain
{
    public class Survivor : IAggregateRoot, IHealth, IInventoryable
    {
        private readonly InventoryHandler inventoryHandler;
        private readonly IHealth health;
        private Equipment leftHandEquip;
        private Equipment rightHandEquip;

        public Survivor(string name, InventoryHandler inventoryHandler, IHealth health)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(inventoryHandler, nameof(inventoryHandler));
            Guard.Against.Null(health, nameof(health));

            Name = name;
            RemainingActions = 3;
            this.inventoryHandler = inventoryHandler;
            this.health = health;

        }

        public Equipment LeftHandEquip { get => leftHandEquip; set => leftHandEquip = value; }

        public string Name { get; }

        public int RemainingActions { get; }

        public Equipment RightHandEquip { get => rightHandEquip; set => rightHandEquip = value; }

        public State CurrentState => health.CurrentState;

        public int Wounds => health.Wounds;

        public IReadOnlyCollection<Equipment> Items => inventoryHandler.Items;

        public void Wound(int inflictedWounds)
        {
            health.Wound(inflictedWounds);
        }
    }
}