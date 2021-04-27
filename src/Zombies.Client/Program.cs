using System;
using System.Linq;
using System.Text;
using Zombies.Application;
using Zombies.Domain;

namespace Zombies.Client
{
    class Program
    {
        static void Main(string[] args)
        {

            //JP: todos los accesibility modifiers estan bien, desde aca no puedo CREAR nada de domain, solo leer
            /*lo que falta son: 
             * tests para todo lo q es history
             * definir bien las interfaces internal y public para mantener encapsulamiento
             * actualizar los tests q fallen por accesibilidad
             * definir bien los factories (ya estan practicamente hechos)
             * no mucho mas
             
            
             
             */
            var g = new GameProvider().CreateGame();

            var s = new Application.SurvivorProvider().CreateSurvivor("Juan");
            g.AddSurvivor(s);

            s.Wound(1);
            s.Wound(1);

            var sb = new StringBuilder();
            g.Events.ToList().ForEach(x => sb.AppendLine(x));

            //JP: IT WORKS!
            Console.WriteLine("Hello World!");
            Console.WriteLine(sb.ToString());
        }
    }
}
