using AutoFixture;
using AutoFixture.AutoMoq;
using System;
using System.Linq;
using Xunit;
using Zombies.Domain.Gear;
using Zombies.Domain.Inventory;

namespace Zombies.Domain.Tests
{
    public class InventoryHandlerShould
    {
        private IFixture fixture;
        private InventoryHandler sut;

        public InventoryHandlerShould()
        {
            fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        }

        [Theory]
        [InlineData(new object[] { 1 })]
        [InlineData(new object[] { 2 })]
        [InlineData(new object[] { 3 })]
        [InlineData(new object[] { 4 })]
        [InlineData(new object[] { 5 })]
        public void AddItemsWhileWithinCapacity(int itemsCount)
        {
            sut = new InventoryHandler();
            var e = fixture.Create<Equipment>();

            for (int i = 0; i < itemsCount; i++)
            {
                sut.AddEquipment(e);
                Assert.Equal(i + 1, sut.Items.Count);
            }
        }

        [Fact]
        public void GetCapacityReducedByOneAndNotReturnTheLostItem()
        {
            var sut = Utils.CreateInventoryWithItems();

            var originalItems = sut.Items.ToList();
            var equipment = sut.Items.First();

            sut.ReduceCapacityBy(1);

            var reducedItems = sut.Items.ToList();

            Assert.Equal(originalItems.Count, reducedItems.Count + 1);
            Assert.DoesNotContain(reducedItems, x => x.Name == equipment.Name);
        }

        [Fact]
        public void ReduceCapacityByNegativeValueHasNoEffects()
        {
            var reduceBy = -1;
            int usedSlots = 5;
            var sut = Utils.CreateInventoryWithItems(usedSlots);

            sut.ReduceCapacityBy(reduceBy);

            var reducedItems = sut.Items.ToList();

            Assert.Equal(usedSlots, reducedItems.Count);
        }

        [Theory]
        [InlineData(new object[] { 1, 4 })]
        [InlineData(new object[] { 4, 1 })]
        public void ReduceCapacityPreferringUnusedEquipmentSlots(int usedSlots, int reduceBy)
        {
            var sut = Utils.CreateInventoryWithItems(usedSlots);

            var startingEquipment = sut.Items;

            sut.ReduceCapacityBy(reduceBy);

            var reducedItems = sut.Items.ToList();

            Assert.Equal(usedSlots, reducedItems.Count);

            foreach (var item in startingEquipment)
                Assert.Contains(reducedItems, x => x.Name == item.Name);
        }

        [Theory]
        [InlineData(new object[] { 1 })]
        [InlineData(new object[] { 2 })]
        [InlineData(new object[] { 5 })]
        [InlineData(new object[] { 20 })]
        public void ReduceTheCapacityByTheSpecifiedAmountOrEmptyItIfSpecifiedAmountIsLargerThanCurrentCapacity(int reduction)
        {
            var maximumCapacity = 5;
            var expectedCapacity = maximumCapacity - reduction;

            expectedCapacity = expectedCapacity < 0 ? 0 : expectedCapacity;

            var sut = Utils.CreateInventoryWithItems();

            sut.ReduceCapacityBy(reduction);

            Assert.Equal(expectedCapacity, sut.Items.Count);
        }

        [Fact]
        public void ReducingCapacityWhenThereAreNoItemsDoesNotHaveImpact()
        {
            var reduction = 1;
            var expectedCapacityAfterReduction = 0;
            var sut = new InventoryHandler();

            sut.ReduceCapacityBy(reduction);

            Assert.Equal(expectedCapacityAfterReduction, sut.Items.Count);
        }

        [Fact]
        public void ReturnFalseIfAnItemDoesNotExistInInventory()
        {
            sut = Utils.CreateInventoryWithItems(3);

            var equipment = fixture.Create<Equipment>();

            var result = sut.ContainsEquipment(equipment);

            Assert.False(result);
        }

        [Theory]
        [InlineData(new object[] { 1 })]
        [InlineData(new object[] { 2 })]
        [InlineData(new object[] { 3 })]
        [InlineData(new object[] { 4 })]
        [InlineData(new object[] { 5 })]
        public void ReturnOnlyTheSlotsUsed(int usedSlots)
        {
            var sut = Utils.CreateInventoryWithItems(usedSlots);

            var reducedItems = sut.Items.ToList();

            Assert.Equal(usedSlots, reducedItems.Count);
        }

        [Fact]
        public void ReturnTrueIfAnItemExistsInInventory()
        {
            sut = Utils.CreateInventoryWithItems(3);

            var equipment = fixture.Create<Equipment>();
            sut.AddEquipment(equipment);

            var result = sut.ContainsEquipment(equipment);

            Assert.True(result);
        }

        [Theory]
        [InlineData(new object[] { 6 })]
        [InlineData(new object[] { 7 })]
        [InlineData(new object[] { 10 })]
        public void ThrowWhenMoreThanFiveElementsAreTriedToBeAdded(int itemsCount)
        {
            sut = new InventoryHandler();
            var e = fixture.Create<Equipment>();

            for (int i = 0; i < itemsCount; i++)
            {
                if (i < 5)
                    sut.AddEquipment(e);
                else
                    Assert.Throws<InvalidOperationException>(() => sut.AddEquipment(e));
            }
        }
    }
}