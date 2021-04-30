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
            public void WithNameAsNothing()
            {
                sut = new NoEquipment();

                Assert.Equal("Nothing", sut.Name);
            }
        }
    }
}