using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using Zombies.Domain.Gear;

namespace Zombies.Domain.Survivors
{
    public sealed class InventoryHandler
    {
        private const int initialMaxCapacity = 5;
        private IList<InventorySlot> items;

        public InventoryHandler()
        {
            InitializeItems(initialMaxCapacity);
        }

        public IReadOnlyCollection<IEquipment> Items
        {
            get
            {
                var result = items.Where(x => x.IsUsed && x.IsEnabled).Select(x => x.Equipment).ToList();

                return result;
            }
        }

        public void AddEquipment(IEquipment equipment)
        {
            Guard.Against.Null(equipment, nameof(equipment));

            var availableSlots = GetAvailableSlots();

            if (availableSlots.Count() == 0)
                throw new InvalidOperationException($"Cannot add more items to equipment. Inventory at full capacity: {CurrentCapacity()}");

            var slot = availableSlots.First();
            slot.Equipment = equipment;
        }

        public bool ContainsEquipment(IEquipment equipment)
        {
            return items.Any(x => x.Equipment.Equals(equipment));
        }

        public void ReduceCapacityBy(int reduction)
        {
            if (reduction > 0)
            {
                var currentCapacity = CurrentCapacity();

                if (reduction >= currentCapacity)
                    ClearCapacity();
                else
                {
                    if (items.Any(x => x.IsEnabled))
                    {
                        var sorted = items.OrderBy(x => x.IsUsed).ToList();

                        for (int i = 0; i < reduction; i++)
                        {
                            sorted[i].IsEnabled = false;
                        }
                    }
                }
            }
        }

        private void ClearCapacity()
        {
            foreach (var item in items)
                item.IsEnabled = false;
        }

        private int CurrentCapacity()
        {
            var capacity = items.Count(x => x.IsEnabled);
            return capacity;
        }

        private IEnumerable<InventorySlot> GetAvailableSlots()
        {
            return items.Where(x => x.IsEnabled && !x.IsUsed);
        }

        private void InitializeItems(int initialMaxCapacity)
        {
            items = new List<InventorySlot>(initialMaxCapacity);

            for (int i = 0; i < initialMaxCapacity; i++)
                items.Add(new InventorySlot(new NoEquipment()));
        }

        private class InventorySlot
        {
            private bool isEnabled;

            public InventorySlot(IEquipment equipment)
            {
                Equipment = equipment;
                IsEnabled = true;
            }

            public IEquipment Equipment { get; set; }

            public bool IsEnabled
            {
                get => isEnabled;
                set
                {
                    if (value == false)
                        Equipment = new NoEquipment();
                    isEnabled = value;
                }
            }

            public bool IsUsed => Equipment is NoEquipment == false;
        }
    }
}