using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSocket
{
    class BattleShip
    {
        enum SHIP_ORIENTATION { HORIZONTAL, VERTICAL };
        internal List<int> position { get; set; }
        internal int health { get; set; }
        internal int orientation { get; set; }
        internal BattleShip()
        {
            position = new List<int>();
            health = 0;
        }

        internal void BeingAttack()
        {
            health--;
        }

        internal bool Destroyed()
        {
            return health == 0;
        }

        internal void AppendPosition(int pos)
        {
            position.Add(pos);
            health++;
        }

        internal void DetermineOrientation(int tableSize)
        {
            int orient = (int)SHIP_ORIENTATION.HORIZONTAL;
            if (position.Count > 0)
            {
                int x = position[0] / tableSize + 1;
                int y = position[0] % tableSize + 1;
                if (position.Count > 1)
                {
                    int nx = position[1] / tableSize + 1;
                    int ny = position[1] % tableSize + 1;
                    if (ny == y) orient = (int)SHIP_ORIENTATION.VERTICAL;
                }
                orientation = orient;
            }
            else
            {
                Console.WriteLine("Battle Ship size = 0! Error Detected");
            }
        }
    }
}
