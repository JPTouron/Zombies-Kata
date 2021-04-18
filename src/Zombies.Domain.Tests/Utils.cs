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

        public static Inventory CreateInventoryWithItems(int? size = null)
        {
            var sut = new Inventory();

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