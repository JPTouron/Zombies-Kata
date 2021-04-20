using Xunit;
using Zombies.Domain.Gear;

namespace Zombies.Domain.Tests
{
    public class NoEquipmentShould
    {
        public class BeCreated
        {
            private NoEquipment sut;

            [Fact]
            public void WithAnEmptyName()
            {
                sut = new NoEquipment();

                Assert.Equal(string.Empty, sut.Name);
            }
        }
    }
}