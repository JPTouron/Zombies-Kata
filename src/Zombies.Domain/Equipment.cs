using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using Zombies.Domain.BuildingBocks;

namespace Zombies.Domain
{
    public interface IEquippable
    {
        void AddEquipment(Equipment equipment);
    }
    public interface IInventoryable {
        IReadOnlyCollection<Equipment> Items { get; }

    }
    public class Equipment : ValueObject<Equipment>
    {
        public Equipment(string Name)
        {
            Guard.Against.NullOrWhiteSpace(Name, nameof(Name));
            this.Name = Name;
        }

        public string Name { get; }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        protected override bool InternalEquals(Equipment other)
        {
            return Name.Equals(other.Name);
        }
    }

    public class InventoryHandler : IEquippable,IInventoryable
    {
        private const int initialMaxCapacity = 5;
        private int currentCapacity;
        private IList<Equipment> items;

        public InventoryHandler()
        {
            items = new List<Equipment>();
            currentCapacity = initialMaxCapacity;
        }

        public IReadOnlyCollection<Equipment> Items => (IReadOnlyCollection<Equipment>)items;

        public void AddEquipment(Equipment equipment)
        {
            Guard.Against.Null(equipment, nameof(equipment));

            if (items.Count == currentCapacity)
                throw new InvalidOperationException($"Cannot add more items to equipment. Inventory at full capacity: {currentCapacity}");

            items.Add(equipment);
        }

        public void ReduceCapacityBy(int reduction)
        {
            if (reduction >= currentCapacity)
                items.Clear();
            else
                for (int i = 0; i < reduction; i++)
                    items.Remove(items[i]);
        }
    }
}