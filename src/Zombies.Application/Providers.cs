using Zombies.Application.History;
using Zombies.Domain.Gear;
using Zombies.Domain.Survivors;

namespace Zombies.Application
{
    public static class Providers
    {
        public static IEquipment Equipment(string name)
        {
            return new Equipment(name);
        }

        public static IGame Game()
        {
            return new Game(HistoryRecorder.Instance());
        }

        public static ISurvivor Survivor(string name)
        {
            var s = new Survivor(name, new InventoryHandler(), new Health(), new Experience());
            return new SurvivorHistory(s, HistoryRecorder.Instance());
        }
    }
}