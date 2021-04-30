using Xunit;
using Zombies.Application.HistoryRecording.SuvivorHistory;
using Zombies.Domain.Gear;

namespace Zombies.Application.Tests
{
    public class ProvidersShould
    {
        [Fact]
        public void ProvideEquipment()
        {
            var name = "name";
            var e = Providers.Equipment(name);

            Assert.NotNull(e);
            Assert.Equal(name, e.Name);
            Assert.IsAssignableFrom<IEquipment>(e);
            Assert.IsAssignableFrom<Equipment>(e);
        }

        [Fact]
        public void ProvideGame()
        {
            var e = Providers.Game();

            Assert.NotNull(e);
            Assert.IsAssignableFrom<Game>(e);
        }

        [Fact]
        public void ProvideSurvivor()
        {
            var name = "name";
            var e = Providers.Survivor(name);

            Assert.NotNull(e);
            Assert.Equal(name, e.Name);
            Assert.IsAssignableFrom<HistoricSurvivor>(e);
            Assert.IsAssignableFrom<ISurvivor>(e);
        }
    }
}