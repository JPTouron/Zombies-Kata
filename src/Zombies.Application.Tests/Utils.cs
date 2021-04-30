using AutoFixture;
using Zombies.Application.HistoryRecording.GameHistory;
using Zombies.Application.HistoryRecording.Infrastructure;
using Zombies.Application.HistoryRecording.SuvivorHistory;
using Zombies.Domain.Survivors;

namespace Zombies.Application.Tests
{
    internal static class Utils
    {
        internal static Game CreateGame(IGameEventsRecorder gameHistoricEvents = null)
        {
            gameHistoricEvents ??= HistoryRecorder.Instance();
            return new Game(gameHistoricEvents);
        }

        internal static ISurvivor CreateHistorySurvivor(string name = null, Health health = null, InventoryHandler inventoryHandler = null, Experience xp = null, ISurvivorEventsRecorder history = null)
        {
            var randomName = new Fixture().Create<string>();
            name ??= randomName;
            health ??= new Health();
            inventoryHandler ??= new InventoryHandler();
            xp ??= new Experience();
            history ??= HistoryRecorder.Instance();

            var s = new Survivor(name, inventoryHandler, health, xp);

            return new HistoricSurvivor(s, history);
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