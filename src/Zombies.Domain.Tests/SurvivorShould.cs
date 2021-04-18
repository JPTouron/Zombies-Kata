using AutoFixture;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Zombies.Domain.Tests
{
    public class SurvivorShould
    {

        public class BeAbleToUseRightHandThatShould
        {
            [Fact]
            public void BeAbleToHoldInventoryInRightHand()
            {
                var inv = Utils.CreateInventoryWithItems(3);
                var sut = Utils.CreateSurvivor(inventoryHandler: inv);

                var item = inv.Items.First();
                sut.RightHandEquip = item;

                Assert.Equal(item, sut.RightHandEquip);
            }

            [Fact]
            public void ThrowWhenTheEquipmentUsedIsNotInInventory()
            {
                var inv = Utils.CreateInventoryWithItems(3);
                var sut = Utils.CreateSurvivor(inventoryHandler: inv);

                var item = new Fixture().Create<Equipment>();

                Assert.Throws<InvalidOperationException>(() => sut.RightHandEquip = item);
            }
        }
        public class BeAbleToUseLeftHandThatShould
        {
            [Fact]
            public void BeAbleToHoldInventoryInRightHand()
            {
                var inv = Utils.CreateInventoryWithItems(3);
                var sut = Utils.CreateSurvivor(inventoryHandler: inv);

                var item = inv.Items.First();
                sut.LeftHandEquip = item;

                Assert.Equal(item, sut.LeftHandEquip);
            }

            [Fact]
            public void ThrowWhenTheEquipmentUsedIsNotInInventory()
            {
                var inv = Utils.CreateInventoryWithItems(3);
                var sut = Utils.CreateSurvivor(inventoryHandler: inv);

                var item = new Fixture().Create<Equipment>();

                Assert.Throws<InvalidOperationException>(() => sut.LeftHandEquip= item);
            }
        }


        [Fact]
        public void DieWhenTwoWoundsAreInflicted()
        {
            var inflictedWounds = 2;
            var expectedState = IHealth.State.Dead;
            var sut = Utils.CreateSurvivor();

            sut.Wound(inflictedWounds);

            Assert.Equal(expectedState, sut.CurrentState);
        }

        [Fact]
        public void NotDieWhenASingleWoundIsInflicted()
        {
            var inflictedWounds = 1;
            var expectedState = IHealth.State.Alive;
            var sut = Utils.CreateSurvivor();

            sut.Wound(inflictedWounds);

            Assert.Equal(expectedState, sut.CurrentState);
        }

        [Fact]
        public void NotHaveMoreThanTwoWounds()
        {
            var inflictedWounds = 15;
            var expectedMaxWounds = 2;

            var sut = Utils.CreateSurvivor();

            sut.Wound(inflictedWounds);

            Assert.Equal(expectedMaxWounds, sut.Wounds);
        }

        [Fact]
        public void NotHaveMoreWoundsAfterDeath()
        {
            var inflictedWounds = 15;
            var expectedMaxWounds = 2;

            var sut = Utils.CreateSurvivor();

            sut.Wound(inflictedWounds);
            Assert.Equal(IHealth.State.Dead, sut.CurrentState);

            sut.Wound(inflictedWounds);
            Assert.Equal(expectedMaxWounds, sut.Wounds);
        }

        public class BeCreated
        {
            private Survivor sut;

            [Fact]
            public void WithANameAnInventoryHandlerAHealthAndZeroWoundsAndThreeRemainingActionsAndAlive()
            {
                var name = "JP";
                var expectedWounds = 0;
                var expectedRemainingActions = 3;
                var expectedState = IHealth.State.Alive;

                sut = Utils.CreateSurvivor(name);

                Assert.Equal(name, sut.Name);
                Assert.Equal(expectedWounds, sut.Wounds);
                Assert.Equal(expectedRemainingActions, sut.RemainingActions);
                Assert.Equal(expectedState, sut.CurrentState);
            }

            [Theory]
            [ClassData(typeof(InvalidCreateData))]
            internal void ThrowWhenNameIsEmpty(string name, InventoryHandler inventoryHandler, Health health, Type exceptionType)
            {
                Assert.Throws(exceptionType, () => new Survivor(name, inventoryHandler, health));
            }

            private class InvalidCreateData : IEnumerable<object[]>
            {
                public IEnumerator<object[]> GetEnumerator()
                {
                    var inventory = new InventoryHandler();
                    var health = new Health();
                    var name = "JP";

                    yield return new object[] { string.Empty, inventory, health, typeof(ArgumentException) };
                    yield return new object[] { null, inventory, health, typeof(ArgumentNullException) };
                    yield return new object[] { name, null, health, typeof(ArgumentNullException) };
                    yield return new object[] { name, inventory, null, typeof(ArgumentNullException) };
                }

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            }
        }
    }
}