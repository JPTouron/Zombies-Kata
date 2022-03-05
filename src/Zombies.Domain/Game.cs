using System;
using System.Collections.Generic;
using System.Linq;

namespace Zombies.Domain
{
    public interface IClock
    {
        public DateTime Now { get; }
    }

    public interface ISurvivorEvents
    {
        event SurvivorAddedEquipmentEventHandler survivorAddedEquipmentEventHandler;

        event SurvivorDiedEventHandler survivorDiedEventHandler;
    }

    public delegate void SurvivorAddedEquipmentEventHandler(string survivorName, string addedEquipment);

    public delegate void SurvivorDiedEventHandler(string survivorName);

    public class Game
    {
        private IList<Survivor> survivors;

        private List<string> history;

        public Game(IClock clock)
        {
            survivors = new List<Survivor>();
            history = new List<string> { clock.Now.ToString() };
        }

        public int Survivors => survivors.Count;

        public bool HasEnded => survivors.All(x => x.IsDead);

        public Level Level => survivors.Where(x => x.IsAlive)
                                      .Select(x => x.Level)
                                      .DefaultIfEmpty(Level.Blue)
                                      .Max();

        public IReadOnlyCollection<string> History => history;

        public void AddSurvivor(Survivor s)
        {
            if (survivors.Any(x => x.Name == s.Name))
                throw new InvalidOperationException($"A player with name {s.Name} already exists, cannot add another survivor with that name to the game.");

            survivors.Add(s);

            history.Add($"Survivor {s.Name} has joined the game");

            SubscribeToSurvivorEvents(s);
        }

        private void SubscribeToSurvivorEvents(ISurvivorEvents s)
        {
            s.survivorAddedEquipmentEventHandler += OnSurvivorAddedEquipment;
            s.survivorDiedEventHandler += OnSurvivorDied;
        }

        private void OnSurvivorDied(string survivorName)
        {
            UnsubscribeEventsFrom(survivorName);
        }

        private void UnsubscribeEventsFrom(string survivorName)
        {
            var maybeSurvivor = survivors.SingleOrDefault(x => x.Name == survivorName) as ISurvivorEvents;
            if (maybeSurvivor != null)
            {
                maybeSurvivor.survivorAddedEquipmentEventHandler -= OnSurvivorAddedEquipment;
                maybeSurvivor.survivorDiedEventHandler -= OnSurvivorDied;
            }
        }

        private void OnSurvivorAddedEquipment(string survivorName, string addedEquipment)
        {
            history.Add($"Survivor {survivorName} acquired {addedEquipment}");
        }
    }
}