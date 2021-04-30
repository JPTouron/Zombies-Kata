using AutoFixture;
using Zombies.Application.History;
using Zombies.Domain.Survivors;

namespace Zombies.Application.Tests
{
    internal static class Utils
    {
        internal static Game CreateGame()
        {
            return new Game(HistoryRecorder.Instance());
        }

        internal static ISurvivor CreateHistorySurvivor(string name = null, Health health = null, InventoryHandler inventoryHandler = null, Experience xp = null, ISurvivorHistoricEvents history = null)
        {
            var randomName = new Fixture().Create<string>();
            name ??= randomName;
            health ??= new Health();
            inventoryHandler ??= new InventoryHandler();
            xp ??= new Experience();
            history ??= HistoryRecorder.Instance();

            var s = new Survivor(name, inventoryHandler, health, xp);

            return new SurvivorHistory(s, history);
        }

        internal static Experience CreateXP(int experienceValue = 0)
        {
            var xp = new Experience();

            if (experienceValue > 0)
                for (int i = 0; i < experienceValue; i++)
                    xp.Increase();

            return xp;
        }
    }
}