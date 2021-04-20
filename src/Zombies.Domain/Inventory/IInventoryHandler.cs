using System.Collections.Generic;
using Zombies.Domain.Gear;

namespace Zombies.Domain.Inventory
{
    public interface IInventoryHandler
    {
        IReadOnlyCollection<IEquipment> Items { get; }

        void AddEquipment(IEquipment equipment);

        bool ContainsEquipment(IEquipment equipment);

        void ReduceCapacityBy(int reduction);
    }
}