using AutoFixture;
using AutoFixture.AutoMoq;
using System;
using Xunit;

namespace Zombies.Domain.Tests
{
    public class InventoryShould
    {
        private IFixture fixture;
        private Inventory sut;

        public InventoryShould()
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
            sut = new Inventory();
            var e = fixture.Create<Equipment>();

            for (int i = 0; i < itemsCount; i++)
            {
                sut.AddEquipment(e);
                Assert.Equal(i + 1, sut.Items.Count);
            }
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

        [Theory]
        [InlineData(new object[] { 6 })]
        [InlineData(new object[] { 7 })]
        [InlineData(new object[] { 10 })]
        public void ThrowWhenMoreThanFiveElementsAreTriedToBeAdded(int itemsCount)
        {
            sut = new Inventory();
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