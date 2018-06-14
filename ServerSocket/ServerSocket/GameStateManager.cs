using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSocket
{
    class GameStateManager
    {
        const int MAXIMUM_MAP_SIZE = 10;
        internal Player player2 { get; set; }
        internal Player player1 { get; set; }
        int[] player1BattleMap { get; set; }
        int[] player2BattleMap { get; set; }
        int tableSize { get; set; }

        internal GameStateManager(string player1, string player2)
        {
            this.player1 = new Player();
            this.player2 = new Player();
            this.player1.playerName = player1;
            this.player2.playerName = player2;
            this.player1.isOnline = this.player2.isOnline = true;
        }

        internal void SetPlayerShip(string playerName, string battleMap, int tableSize)
        {
            if (playerName == player1.playerName) player1.SetBattleShip(battleMap, tableSize);
            else if (playerName == player2.playerName) player2.SetBattleShip(battleMap, tableSize);
        }

        internal Dictionary<string, string> ProcessPlayerMove(string playerName, int x, int y)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            int score = 0;
            if (playerName == player1.playerName)
            {
                result = player2.ReceiveAttack(x, y, ref score);
                player1.score += score;
            }
            else if (playerName == player2.playerName)
            {
                result = player1.ReceiveAttack(x, y, ref score);
                player2.score += score;
            }
            return result;
        }

        internal bool StartGame()
        {
            return player1.ready & player2.ready;
        }

        internal string GetWinner()
        {
            if (player1.score > player2.score) return player1.playerName;
            return player2.playerName;
        }

        internal string GetLoser()
        {
            if (player2.score > player1.score) return player1.playerName;
            return player2.playerName;
        }

        internal string GetOpponentPlayerName(string playerName)
        {
            if (playerName == player1.playerName) return player2.playerName;
            return player1.playerName;
        }

        internal bool Crashed()
        {
            return !(player1.isOnline & player2.isOnline);
        }
    }
}
