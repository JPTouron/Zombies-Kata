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
            //JP: todos los accesibility modifiers estan bien, desde aca no puedo CREAR nada de domain, solo leer
            /*lo que falta son:
             * tests para todo lo q es history
             * definir bien las interfaces internal y public para mantener encapsulamiento
             * actualizar los tests q fallen por accesibilidad
             * definir bien los factories (ya estan practicamente hechos)
             * no mucho mas

             */
            var g = Providers.Game();

            //JP: MAKE A TEST THAT WE CANNOT ADD AN ISURVIVOR TO THE GAME, JUST A ISURVIVORHISTORY
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

            //JP: IT WORKS!
            Console.WriteLine("Hello World!");
            Console.WriteLine(sb.ToString());
        }
    }
}