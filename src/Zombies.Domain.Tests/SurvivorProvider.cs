using AutoFixture;

namespace Zombies.Domain.Tests
{
    internal static class SurvivorProvider
    {
        public static Survivor CreateRandomSurvivor(string? name = null, SkillTreeFactory? treeFactory = null)
        {
            if (name == null)
                name = new Fixture().Create<string>();

            if (treeFactory == null)
                treeFactory = new SkillTreeFactory();

            var s = new Survivor(name, treeFactory);
            return s;
        }
    }
}