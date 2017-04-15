using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snaky
{
    public class Map
    {
        private GameCell[,] _cells;
        private int _width;
        private int _height;

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public GameCell this[int y, int x]
        {
            get { return _cells[y, x]; }
        }

        public Map(int width, int height)
        {
            _width = width;
            _height = height;
            _cells = new GameCell[_height,_width];
            FillMap();
        }

        private void FillMap()
        {
            if (_cells == null) return;
            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    _cells[y,x] = new GameCell();
                }
            }
        }

        public void PlaceSnake(Snake snake)
        {
            var tail = snake.Tail;
            do
            {
                this[tail.Y, tail.X].IsSnakeOver = true;
                if (tail.HasHead)
                    tail = tail.Head;
            } while (tail.HasHead);
        }
    }
}
