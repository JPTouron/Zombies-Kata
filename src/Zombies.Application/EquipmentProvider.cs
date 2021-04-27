using System;
using System.Collections.Generic;
using System.Text;
using Zombies.Application.History;
using Zombies.Domain;
using Zombies.Domain.Gear;
using Zombies.Domain.Inventory;

namespace Zombies.Application
{



    public class GameProvider
    {


        public IGame CreateGame()
        {

            return new Game(HistoryRecorder.Instance());
        }

    }
}
