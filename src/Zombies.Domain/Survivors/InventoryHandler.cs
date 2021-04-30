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

        public int Capacity => CurrentCapacity();

        public IReadOnlyCollection<IEquipment> Items
        {
            get
            {
                var result = items.Where(x => x.HasEquipment && x.IsEnabled).Select(x => x.Equipment).ToList();

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
                if (ReductionExceedsCurrentCapacity(reduction))
                    ClearCapacity();
                else
                {
                    if (ThereAreAnyEnabledSlots())
                        ReduceCapacity(reduction);
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
            return items.Where(x => x.IsEnabled && !x.HasEquipment);
        }

        private void InitializeItems(int initialMaxCapacity)
        {
            items = new List<InventorySlot>(initialMaxCapacity);

            for (int i = 0; i < initialMaxCapacity; i++)
                items.Add(new InventorySlot(new NoEquipment()));
        }

        private void ReduceCapacity(int reduction)
        {
            var itemsSortedByHavingEquipment = items.OrderBy(x => x.HasEquipment).ToList();

            for (int i = 0; i < reduction; i++)
            {
                itemsSortedByHavingEquipment[i].IsEnabled = false;
            }
        }

        private bool ReductionExceedsCurrentCapacity(int reduction)
        {
            return reduction >= CurrentCapacity();
        }

        private bool ThereAreAnyEnabledSlots()
        {
            return items.Any(x => x.IsEnabled);
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

            public bool HasEquipment => Equipment is NoEquipment == false;

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
        }
    }
}