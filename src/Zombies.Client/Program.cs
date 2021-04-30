using System;
using System.Linq;
using System.Text;
using Zombies.Application;
using Zombies.Domain;

namespace Zombies.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var g = Providers.Game();

            var s = g.AddSurvivor("Juan");

            var s2 = g.AddSurvivor("Pedro");

            s.Wound(1);
            s2.Kill(new Zombie());
            var golfClub = Providers.Equipment("golf club");
            s.AddEquipment(golfClub);
            s.RightHandEquip = golfClub;
            for (int i = 0; i < 6; i++)
                s.Kill(new Zombie());
            s.Wound(1);
            s2.Kill(new Zombie());
            s2.Wound(4);

            var sb = new StringBuilder();
            g.Events.ToList().ForEach(x => sb.AppendLine(x));

            Console.WriteLine("Hello World!");
            Console.WriteLine(sb.ToString());
        }
    }
}