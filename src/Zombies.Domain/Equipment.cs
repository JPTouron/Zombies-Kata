using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;

namespace Zombies.Domain
{
    public interface IEquippable
    {
        void AddEquipment(Equipment equipment);
    }

    public class Equipment
    {
        public Equipment(string Name)
        {
            Guard.Against.NullOrWhiteSpace(Name, nameof(Name));
            this.Name = Name;
        }

        public string Name { get; }
    }

    public class Inventory : IEquippable
    {
        private const int initialMaxCapacity = 5;
        private int currentCapacity;
        private IList<Equipment> items;

        public Inventory()
        {
            items = new List<Equipment>();
            currentCapacity = initialMaxCapacity;
        }

        public void AddEquipment(Equipment equipment)
        {
            Guard.Against.Null(equipment, nameof(equipment));

            if (items.Count == currentCapacity)
                throw new InvalidOperationException($"Cannot add more items to equipment. Inventory at full capacity: {currentCapacity}");

            items.Add(equipment);

        }

        public IReadOnlyCollection<Equipment> Items => (IReadOnlyCollection<Equipment>)items;

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