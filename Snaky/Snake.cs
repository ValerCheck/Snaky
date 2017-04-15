using System;
using System.Collections;
using System.Collections.Generic;

namespace Snaky
{
    public enum SnakeNodeType
    {
        Head = 1,
        Tail = -1
    }

    public enum VelocityValue
    {
        Forward = 1,
        Backward = -1,
        Stop = 0
    }
    
    public enum Direction
    {
        Default = 0,
        Vertical = 1,
        Horizontal = 2
    }

    public struct Velocity
    {
        private VelocityValue _vertical;
        private VelocityValue _horizontal;

        public bool FullStop
        {
            get { return Vertical == 0 && Horizontal == 0; }
        }

        public VelocityValue Vertical
        {
            get { return _vertical; }
            set
            {
                if (value != 0)
                {
                    Horizontal = 0;
                }
                _vertical = value;
            }
        }

        public VelocityValue Horizontal
        {
            get { return _horizontal; }
            set
            {
                if (value != 0)
                {
                    Vertical = 0;
                }
                _horizontal = value;
            }
        }
        
        public Velocity(int hVelocity, int vVelocity) : this()
        {
            if (hVelocity == vVelocity && hVelocity != 0 && vVelocity != 0)
                throw new ArgumentException("Horizontal and Vertical velocities cannot be the same");

            Vertical = (VelocityValue)vVelocity;
            Horizontal = (VelocityValue)hVelocity;
        }
    }

    public class Snake
    {
        private int _initialLength = 4;
        private int _initialX;
        private int _initialY;
        private Velocity _velocity;
        private int _boundWidth;
        private int _boundHeight;
        private Queue<SnakePart> _partsToAdd = new Queue<SnakePart>();
        private SnakePart _nextPartToAdd;

        public SnakePart Head { get; private set; }
        public SnakePart Tail { get; private set; }

        public Snake(int initX = 0, int initY = 0)
        {
            _initialX = initX;
            _initialY = initY;
            Create();
        }

        public void SetBounds(int width, int height)
        {
            _boundHeight = height;
            _boundWidth = width;
        }

        public void Eat(Food food)
        {
            var newPart = new SnakePart(food.X,food.Y);
            food.HasBeenEaten = true;
            _partsToAdd.Enqueue(newPart);
        }

        private void Create()
        {
            var head = new SnakePart(_initialX,_initialY);
            Tail = head;
            for (var i = 0; i < _initialLength - 1; i++)
            {
                var next = new SnakePart(head.X + 1, head.Y);
                next.SetConnection(head, -1);
                head.SetConnection(next, 1);
                head = next;
            }
            Head = head;
            _velocity = new Velocity(0,0);
        }

        public void ChangeDirection(Direction dir, int val)
        {
            if (Math.Abs(val) > 1)
                return;

            try
            {
                ChangeDirection(dir, (VelocityValue) val);
            }
            catch { }
        }

        private void ChangeDirection(Direction dir, VelocityValue val)
        {
            if (dir == Direction.Horizontal)
            {
                if ((int)_velocity.Horizontal + (int)val == 0)
                    throw new ArgumentException("Velocity cannot be changed with opposite value");

                _velocity.Horizontal = val;
            }
            else if (dir == Direction.Vertical)
            {
                if ((int)_velocity.Vertical + (int)val == 0)
                    throw new ArgumentException("Velocity cannot be changed with opposite value");
                _velocity.Vertical = val;
            }
        }

        private bool _isTailOverNewPart = false;

        public void Move()
        {
            if (_velocity.FullStop) return;
            var newHead = Tail;

            Tail = Tail.Head;
            Tail.SetConnection(null, -1);

            newHead.SetConnection(Head,-1);
            newHead.SetConnection(null, 1);

            Head.SetConnection(newHead, 1);
            Head = newHead;

            newHead.CloneXY(newHead.Tail);
            newHead.Move(_velocity, _boundWidth, _boundHeight);

            if (_isTailOverNewPart)
            {
                Tail.SetConnection(_nextPartToAdd, -1);
                _nextPartToAdd.SetConnection(Tail, 1);
                Tail = _nextPartToAdd;
                _nextPartToAdd = null;
                _isTailOverNewPart = false;
            }

            if (_nextPartToAdd == null && _partsToAdd.Count > 0) _nextPartToAdd = _partsToAdd.Dequeue();

            if (_nextPartToAdd != null && (_nextPartToAdd.X == Tail.X &&
                                           _nextPartToAdd.Y == Tail.Y))
                _isTailOverNewPart = true;
        }
    }

    public class SnakePart
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public SnakePart Head { get; private set; }
        public SnakePart Tail { get; private set; }

        public bool HasHead => Head != null;
        public bool HasTail => Tail != null;

        public SnakePart(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void CloneXY(SnakePart part)
        {
            X = part.X;
            Y = part.Y;
        }

        public void Move(Velocity velocity, int boundWidth = 0, int boundHeight = 0)
        {
            X += (int) velocity.Horizontal;
            Y += (int) velocity.Vertical;
            if (boundWidth != 0)
            {
                if (X < 0) X = boundWidth - 1;
                else if (X >= boundWidth) X = 0;
            }
            if (boundHeight != 0)
            {
                if (Y < 0) Y = boundHeight - 1;
                else if (Y >= boundHeight) Y = 0;
            }
        }

        public SnakePart(int x, int y, SnakePart head, SnakePart tail) : this(x,y)
        {
            Head = head;
            Tail = tail;
        }

        internal void SetConnection(SnakePart node, int type)
        {
            if (Math.Abs(type) != 1)
                throw new ArgumentException("Type cannot be other than 1", nameof(type));

            SetConnection(node, (SnakeNodeType) type);
        }

        internal void SetConnection(SnakePart node, SnakeNodeType nodeType = SnakeNodeType.Head)
        {
            //var deltaX = Math.Abs(X - node.X);
            //var deltaY = Math.Abs(Y - node.Y);

            //if (deltaX != 1 && deltaY != 1)
            //    throw new ArgumentException($"{(nodeType == SnakeNodeType.Head ? "Head" : "Tail")} of the snake cannot be the same or be farther from current part more than one cell",nameof(node));
            
            if (nodeType == SnakeNodeType.Head)
                Head = node;
            else
                Tail = node;
        }
    }
}
