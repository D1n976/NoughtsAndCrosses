using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfNougtsAndCrossesLib
{
    public static class MessagesByGameStatus
    {
        private static Dictionary<GameStatus, string> mesByStatus = new Dictionary<GameStatus, string>()
        {
            { GameStatus.NotStart, "Игра не началась" },
            { GameStatus.Start, "Игра началась" },
            { GameStatus.VictoryOfCrosses, "Крестики победили" },
            { GameStatus.VictoryOfNougth, "Нолики победили" },
            { GameStatus.Draw, "Ничья" }
        };

        public static string GetMessage(GameStatus gameStatus) => mesByStatus[gameStatus];
    }
}
