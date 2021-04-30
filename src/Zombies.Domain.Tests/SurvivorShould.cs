using AutoFixture;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Zombies.Domain.Gear;
using Zombies.Domain.Survivors;

namespace Zombies.Domain.Tests
{
    public class SurvivorShould
    {
        [Fact]
        public void DieWhenTwoWoundsAreInflicted()
        {
            var inflictedWounds = 2;
            var expectedState = HealthState.Dead;
            var sut = Utils.CreateSurvivor();

            sut.Wound(inflictedWounds);

            Assert.Equal(expectedState, sut.CurrentState);
        }

        [Theory]
        [InlineData(new object[] { 1, 1 })]
        [InlineData(new object[] { 3, 3 })]
        [InlineData(new object[] { 10, 10 })]
        public void IncreaseExperiencePointsByOneWhenKillingAZombie(int zombiesKilled, int experienceGained)
        {
            var sut = Utils.CreateSurvivor();

            for (int i = 0; i < zombiesKilled; i++)
            {
                var zombie = new Zombie();
                sut.Kill(zombie);
            }
            Assert.Equal(experienceGained, sut.ExperienceValue);
        }

        [Theory]
        [InlineData(new object[] { 3, 0 })]
        [InlineData(new object[] { 6, 6 })]
        [InlineData(new object[] { 18, 18 })]
        [InlineData(new object[] { 42, 42 })]
        [InlineData(new object[] { 68, 42 })]
        public void IncreaseXPLevelWhenKillingEnoughZombies(int zombiesKilled, int expectedXpLevel)
        {
            var sut = Utils.CreateSurvivor();

            for (int i = 0; i < zombiesKilled; i++)
            {
                var zombie = new Zombie();
                sut.Kill(zombie);
            }
            Assert.Equal((XpLevel)expectedXpLevel, sut.Level);
        }

        [Fact]
        public void NotBeAbleToAddEquipmentWhenDead()
        {
            var sut = Utils.CreateDeadSurvivor();
            var initialInventoryCount = sut.BackPack.Count;

            var e = new Fixture().Create<Equipment>();
            sut.AddEquipment(e);

            Assert.Equal(initialInventoryCount, sut.BackPack.Count);
        }

        [Fact]
        public void NotBeAbleToHaveMoreWoundsWhenDead()
        {
            var sut = Utils.CreateDeadSurvivor();
            var expectedMaxWounds = sut.Wounds;

            sut.Wound(1);

            Assert.Equal(expectedMaxWounds, sut.Wounds);
        }

        [Fact]
        public void NotBeAbleToKillWhenDead()
        {
            var sut = Utils.CreateDeadSurvivor();
            var initialXpValue = sut.ExperienceValue;

            sut.Kill(new Zombie());

            Assert.Equal(initialXpValue, sut.ExperienceValue);
        }

        [Fact]
        public void NotDieWhenASingleWoundIsInflicted()
        {
            var inflictedWounds = 1;
            var expectedState = HealthState.Alive;
            var sut = Utils.CreateSurvivor();

            sut.Wound(inflictedWounds);

            Assert.Equal(expectedState, sut.CurrentState);
        }

        [Fact]
        public void NotHaveItsInventoryCapacityWhenDead()
        {
            var sut = Utils.CreateDeadSurvivor();
            var expectedCapacity = sut.BackPackCapacity;

            sut.Wound(1);

            Assert.Equal(expectedCapacity, sut.BackPackCapacity);
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

                Assert.Throws<InvalidOperationException>(() => sut.LeftHandEquip = item);
            }
        }

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

        public class BeCreated
        {
            private Survivor sut;

            [Fact]
            public void WithANameAnInventoryHandlerAHealthAndZeroWoundsAndThreeRemainingActionsAndAliveAndNoInventoryAndNoExperience()
            {
                var name = "JP";
                var expectedWounds = 0;
                var expectedInventoryCount = 0;
                var expectedExperiencePoints = 0;
                var expectedXPLevel = XpLevel.Blue;
                var expectedRemainingActions = 3;
                var expectedState = HealthState.Alive;

                sut = Utils.CreateSurvivor(name);

                Assert.Equal(name, sut.Name);
                Assert.Equal(expectedWounds, sut.Wounds);
                Assert.Equal(expectedRemainingActions, sut.RemainingActions);
                Assert.Equal(expectedState, sut.CurrentState);
                Assert.Equal(expectedInventoryCount, sut.BackPack.Count);
                Assert.Equal(expectedExperiencePoints, sut.ExperienceValue);
                Assert.Equal(expectedXPLevel, sut.Level);
            }

            [Theory]
            [ClassData(typeof(InvalidCreateData))]
            internal void ThrowWhenConstructorParameterIsInvalid(string name, InventoryHandler inventoryHandler, Health health, Experience xp, Type exceptionType)
            {
                Assert.Throws(exceptionType, () => new Survivor(name, inventoryHandler, health, xp));
            }

            private class InvalidCreateData : IEnumerable<object[]>
            {
                public IEnumerator<object[]> GetEnumerator()
                {
                    var inventory = new InventoryHandler();
                    var health = new Health();
                    var xp = new Experience();
                    var name = "JP";

                    yield return new object[] { string.Empty, inventory, health, xp, typeof(ArgumentException) };
                    yield return new object[] { null, inventory, health, xp, typeof(ArgumentNullException) };
                    yield return new object[] { name, null, health, xp, typeof(ArgumentNullException) };
                    yield return new object[] { name, inventory, null, xp, typeof(ArgumentNullException) };
                    yield return new object[] { name, inventory, health, null, typeof(ArgumentNullException) };
                }

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            }
        }
    }
}