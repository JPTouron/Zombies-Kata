using AutoFixture;

namespace Zombies.Domain.Tests
{
    public static class Utils
    {
        public static Survivor CreateSurvivor(string name = null)
        {
            var randomName = new Fixture().Create<string>();
            name ??= randomName;

            return new Survivor(name);
        }

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


    }
}