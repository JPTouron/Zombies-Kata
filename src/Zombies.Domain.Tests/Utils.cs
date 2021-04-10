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
    }
}