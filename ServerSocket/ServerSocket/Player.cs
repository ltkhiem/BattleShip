using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSocket
{
    class Player
    {
        const int MAX_SHIP_NO = 5;
        internal int score { get; set; }
        int tableSize; 
        internal string playerName { get; set; }
        internal int[][] battleMap { get; set; }
        internal bool ready { get; set; }
        Dictionary<int, BattleShip> shipIdMapping { get; set; }
        internal int totalPieces { get; set; }

        internal Player()
        {
            ready = false;
            score = 0;
            totalPieces = 0;
            shipIdMapping = new Dictionary<int, BattleShip>();
        }

        internal void SetBattleShip(string battleMap, int tableSize)
        {
            this.tableSize = tableSize;
            this.battleMap = new int[tableSize + 1][];
            for (int i = 0; i < tableSize + 1; i++) this.battleMap[i] = new int[tableSize + 1];
            for (int i=0; i<battleMap.Length; i++)
            {
                int x = i / tableSize + 1;
                int y = i % tableSize + 1;
                int.TryParse(battleMap[i].ToString(), out this.battleMap[x][y]);
                if (this.battleMap[x][y] > 0)
                {
                    int shipIndex = this.battleMap[x][y];
                    if (shipIdMapping.ContainsKey(shipIndex) == false)
                    {
                        shipIdMapping.Add(shipIndex, new BattleShip());
                    }
                    shipIdMapping[shipIndex].AppendPosition(i);
                    totalPieces++;
                }
            }
            for (int i = 1; i <= MAX_SHIP_NO; i++)
            {
                if (shipIdMapping.ContainsKey(i))
                {
                    shipIdMapping[i].DetermineOrientation(tableSize);
                }
            }
            ready = true;
        }

        internal Dictionary<string, string> ReceiveAttack(int x, int y, ref int score)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (battleMap[x][y] != 0 && battleMap[x][y] != -1)
            {
                score++;
                totalPieces--;
                int shipId = battleMap[x][y];
                BattleShip ship = shipIdMapping[shipId];
                ship.BeingAttack();
                result.Add("destroyed", ship.Destroyed().ToString());
                if (ship.Destroyed())
                {
                    List<int> position = ship.position;
                    result.Add("orientation", ship.orientation.ToString());
                    result.Add("shipCenter", ship.position[0].ToString());
                    result.Add("shipId", shipId.ToString());
                }
            }
            Console.WriteLine(playerName + " " + totalPieces);
            int pos = (x - 1) * tableSize + (y - 1);
            result.Add("position", pos.ToString());
            result.Add("score", score.ToString());
            result.Add("endGame", (totalPieces == 0).ToString());
            battleMap[x][y] = -1;
            return result;
        }
    }
}
