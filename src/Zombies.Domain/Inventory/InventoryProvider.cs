namespace Zombies.Domain.Inventory
{
    public class InventoryProvider
    {
        public IInventoryHandler GetInventory()
        {
            return new InventoryHandler();
        }
    }
}