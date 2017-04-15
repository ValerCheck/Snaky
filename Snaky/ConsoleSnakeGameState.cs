using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snaky
{
    public class ConsoleSnakeGameState : ISnakeGameVisualState<string>
    {
        private GameManager _gManager;

        public string State { get; private set; }

        public ConsoleSnakeGameState(GameManager manager)
        {
            _gManager = manager;
        }

        public void UpdateState()
        {
            StringBuilder sb = new StringBuilder();

            for (var i = 0; i < 3; i++)
                sb.AppendLine();

            var map = _gManager.Map;

            for (var i = 0; i < map.Width; i++)
                sb.Append("_");

            sb.AppendLine();

            for (var y = 0; y < map.Height; y++)
            {
                for (var x = 0; x < map.Width; x++)
                {
                    var cell = map[y, x];
                    if (cell.IsSnakeOver)
                    {
                        sb.Append("O");
                    }
                    else if (cell.HasFood)
                    {
                        sb.Append("*");
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                }
                sb.AppendLine();
            }
            State = sb.ToString();
        }
    }
}
