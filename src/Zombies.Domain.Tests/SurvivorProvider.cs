using AutoFixture;

namespace Zombies.Domain.Tests
{
    internal static class SurvivorProvider
    {
        public static Survivor CreateRandomSurvivor(string? name = null)
        {
            if (name == null)
                name = new Fixture().Create<string>();

            var s =  Survivor.CreateWithEmptySkillTree(name);
            return s;
        }
    }
}