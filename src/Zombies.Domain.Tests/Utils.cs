using AutoFixture;
using Zombies.Domain.Gear;
using Zombies.Domain.Inventory;

namespace Zombies.Domain.Tests
{
    internal static class Utils
    {
        internal static InventoryHandler CreateInventoryWithItems(int? size = null)
        {
            var sut = new InventoryHandler();

            size ??= 5;

            for (int i = 0; i < size; i++)
            {
                var e = new Fixture().Create<Equipment>();
                sut.AddEquipment(e);
            }

            return sut;
        }

        internal static Survivor CreateSurvivor(string name = null, IHealth health = null, InventoryHandler inventoryHandler = null, Experience xp = null)
        {
            var randomName = new Fixture().Create<string>();
            name ??= randomName;
            health ??= new Health();
            inventoryHandler ??= new InventoryHandler();
            xp ??= new Experience();

            return new Survivor(name, inventoryHandler, health, xp);
        }
    }
}