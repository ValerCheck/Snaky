using System;
namespace Snaky
{
    public class Food : IGameComponent
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public bool HasBeenEaten { get; set; }

        public Food(int x, int y)
        {
            X = x;
            Y = y;
            HasBeenEaten = false;
        }

        public void AffectSnake(Snake snake)
        {
            snake.Eat(this);
            HasBeenEaten = true;
        }
    }
}
