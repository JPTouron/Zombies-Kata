using AutoFixture;
using Xunit;

namespace Zombies.Application.Tests
{
    public class GameHistoryShould
    {
        [Fact]
        public void BeAbleToAddANewSurivor()
        {
            var sut = Providers.Game();

            sut.AddSurvivor(new Fixture().Create<string>());

            Assert.Equal(1, sut.SurvivorCount);
        }
    }
}