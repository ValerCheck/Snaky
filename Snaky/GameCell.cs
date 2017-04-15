using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snaky
{
    public class GameCell
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public IGameComponent ExtraItem { get; set; }
        public bool IsSnakeOver { get; set; }

        public bool HasFood => (ExtraItem as Food) != null;
    }
}
