using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using Zombies.Domain.Gear;



/*
 * JP: /!\ IMPORTANT! - THIS'D BE HOW WE ROLL TO HAVE THE LESS IMPACT POSSIBLE, EVENTS / COMMAND PATTERN,
 * FOR EX, ON INVENTORY STUFF TO INCREASE WHATEV IS COOL THAT INVENTORY KNOWS HOW TO UPDATE ITSELF GIVEN A  SKILL
 * SO A MODULE LIKE INVENTORY CAN GET A MESSAGE ABOUT ONE OF A SET OF SKILLS AND THEN DO WHATEV TO IMPLEMENT THE SKILL
 * WE'D MARKE THE TREE WITH AN UNLOCKED SKILL AND DONE
 * THE SKILL TREE WOULD TRIGGER MESSAGES AND THE SURVIVOR SIGNALS THE SKILL IT WANTS (OR MAYBE THAT COMES FROM THE GAME DIRECTLY)
 * WE COULD USE MEDIATOR PATTERN AND SUCH
 */


namespace Zombies.Domain.Survivors
{
    /*
     THIS....

    a survivor has a list of equips associated to it
    the list capacity can increase (with a skill)
    or decrease as a side effect of the game (when survivor is wounded)

    that functionality has to be encapsulated in a class (inventory)

    - inventory 
    VALUE OBJECT
    adds equipment, maybe even removes it and checks if an equip exists within it
    howev, the inventory does not manage capacity or the actual list of equip

    - list of equip, 
    ENTITY
    is finite, can change size, can have slots that are either, empty, with equip or disabled 
    only enabled slots count for capacity and only enable slots may or may not contain equip
    this is what gets affected by size changes and skills added
    this most likely should be encapsulated in a class
    this has to be able to be constructed with a predetermined list of equipment (or empty) as we load survivors from a db
    PERSISTED in a table with 4 cols, fk to survivor, bool IsEnabled,fk to equip / null, 
    (maybe another field marking the skill that the record is from, normal or hoard skill?, 
    OR (preferred)
    maybe another table with same struct that is somthg like slotskilled that can be considered loading from the when we have a  new slot cuz of a skill)


    -slot
    VALUE OBJ
    this is what gets persisted to the db, each slot gotta be associated to a list of equip, so it should have a list of equip id as an FK

    - CONCLUSION:
    process might be, load the list of equipments and create obj graph like this:
    inventory --* list of equip --* IList<Slot>
                                --* IList<SlotSkill> (slots that are added through added skills)
    
    when a slotskill is unlocked, then we persist the new slot, and reload the list of equip + UPDATE the existing inventory that was already fed into 
    the survivor through constructor

    this UPDATE process should be trigger by a message from the outside (user choosing to unlock the skill) that can take 1 of 2 forms:
    + an event that would be handled by the inventory VO
    + a call to a public method on the inventory VO from the skill tree itself 
    (this is why the skill tree takes a dep on the Inventory, but only an abstraction of it to enable this sort of messaging, this way survivor is left out of it)

    summarising:
    skill tree ctor:
        SkillTree(IInventoryUpdateHandler)

    survivor ctor:
        Survivor(SkillTree, Inventory)



     
     */



    public sealed class InventoryHandler
    {
        private const int initialMaxCapacity = 5;
        private IList<InventorySlot> items;

        public InventoryHandler()
        {
            InitializeItems(initialMaxCapacity);
        }

        public int Capacity => CurrentCapacity();

        public IReadOnlyCollection<IEquipment> Items
        {
            get
            {
                var result = items.Where(x => x.HasEquipment && x.IsEnabled).Select(x => x.Equipment).ToList();

                return result;
            }
        }

        public void AddEquipment(IEquipment equipment)
        {
            Guard.Against.Null(equipment, nameof(equipment));

            var availableSlots = GetAvailableSlots();

            if (availableSlots.Count() == 0)
                throw new InvalidOperationException($"Cannot add more items to equipment. Inventory at full capacity: {CurrentCapacity()}");

            var slot = availableSlots.First();
            slot.Equipment = equipment;
        }

        public bool ContainsEquipment(IEquipment equipment)
        {
            return items.Any(x => x.Equipment.Equals(equipment));
        }

        public void ReduceCapacityBy(int reduction)
        {
            if (reduction > 0)
            {
                if (ReductionExceedsCurrentCapacity(reduction))
                    ClearCapacity();
                else
                {
                    if (ThereAreAnyEnabledSlots())
                        ReduceCapacity(reduction);
                }
            }
        }

        private void ClearCapacity()
        {
            foreach (var item in items)
                item.IsEnabled = false;
        }

        private int CurrentCapacity()
        {
            var capacity = items.Count(x => x.IsEnabled);
            return capacity;
        }

        private IEnumerable<InventorySlot> GetAvailableSlots()
        {
            return items.Where(x => x.IsEnabled && !x.HasEquipment);
        }

        private void InitializeItems(int initialMaxCapacity)
        {
            items = new List<InventorySlot>(initialMaxCapacity);

            for (int i = 0; i < initialMaxCapacity; i++)
                items.Add(new InventorySlot(new NoEquipment()));
        }

        private void ReduceCapacity(int reduction)
        {
            var itemsSortedByHavingEquipment = items.OrderBy(x => x.HasEquipment).ToList();

            for (int i = 0; i < reduction; i++)
            {
                itemsSortedByHavingEquipment[i].IsEnabled = false;
            }
        }

        private bool ReductionExceedsCurrentCapacity(int reduction)
        {
            return reduction >= CurrentCapacity();
        }

        private bool ThereAreAnyEnabledSlots()
        {
            return items.Any(x => x.IsEnabled);
        }

        private class InventorySlot
        {
            private bool isEnabled;

            public InventorySlot(IEquipment equipment)
            {
                Equipment = equipment;
                IsEnabled = true;
            }

            public IEquipment Equipment { get; set; }

            public bool HasEquipment => Equipment is NoEquipment == false;

            public bool IsEnabled
            {
                get => isEnabled;
                set
                {
                    if (value == false)
                        Equipment = new NoEquipment();
                    isEnabled = value;
                }
            }
        }
    }
}