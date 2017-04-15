using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snaky
{
    public interface IGameComponent
    {
        int X { get; }
        int Y { get; }
        void AffectSnake(Snake snake);
    }
}
