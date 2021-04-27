using System;
using System.Collections.Generic;
using System.Text;
using Zombies.Domain.Gear;
using Zombies.Domain.Inventory;

namespace Zombies.Domain
{

    public class SurvivorProvider
    {
        public ISurvivor CreateSurvivor(string name)
        {
            return new Survivor(name, new InventoryHandler(), new Health(), new Experience());
        }
    }
    public class EquipmentProvider
    {
        public IEquipment CreateEquipment(string name)
        {
            return new Equipment(name);
        }
    }
}
