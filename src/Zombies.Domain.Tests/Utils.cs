using AutoFixture;

namespace Zombies.Domain.Tests
{
    public static class Utils
    {
        public static InventoryHandler CreateInventoryWithItems(int? size = null)
        {
            var sut = new InventoryHandler();

            if (size == null)
            {
                var e = new Fixture().Create<Equipment>();
                for (int i = 0; i < 5; i++)
                    sut.AddEquipment(e);
            }

            return sut;
        }

        public static Survivor CreateSurvivor(string name = null, IHealth health =null, InventoryHandler inventoryHandler=null)
        {
            var randomName = new Fixture().Create<string>();
            name ??= randomName;
            health ??= new Health();
            inventoryHandler ??= new InventoryHandler();


            return new Survivor(name,inventoryHandler, health);
        }
    }
}