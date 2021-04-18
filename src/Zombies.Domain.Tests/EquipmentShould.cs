using System;
using Xunit;

namespace Zombies.Domain.Tests
{
    public class EquipmentShould {

        public class BeCreated {

            Equipment sut;

            [Fact]
            public void WithAName() {

                var name = "Sword";
                sut = new Equipment(name);

                Assert.Equal(name, sut.Name);

            }

            [Fact]
            public void ThrowWhenNameIsEmpty()
            {

                var name = string.Empty;
                Assert.Throws<ArgumentException>(() => new Equipment(name));

            }

            [Fact]
            public void ThrowWhenNameIsNull()
            {
                string name = null;
                Assert.Throws<ArgumentNullException>(() => new Equipment(name));


            }

        }

    }
}
