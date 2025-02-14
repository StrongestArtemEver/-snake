﻿namespace _snake
{
    class GameState
    {
        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score {get ; private set;}
        public bool GameOver { get; private set; }

        private readonly LinkedList<Position> snakePosition = new LinkedList<Position>();
        private readonly Random random = new Random();

        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols];
            Dir = Direction.Right;

            AddSnake();
            AddFood();
        }

        public void AddSnake()
        {
            int r = Rows / 2;

            for(int c = 1;c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePosition.AddFirst(new Position(r, c));
            }
        }

        //итератор
        public IEnumerable<Position> EmptyPosition()
        {
            for (int r = 0;r < Rows; r++)
            {
                for(int c = 0;c < Cols; c++)
                {
                    if (Grid[r,c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        private void AddFood()
        {
            List<Position> emptyPositions = new List<Position>(EmptyPosition());
            if(emptyPositions.Count == 0)
            {
                return;
            }

            Position pos = emptyPositions[random.Next(emptyPositions.Count)];

            Grid[pos.Row, pos.Col] = GridValue.Food;
        }

        public Position HeadPosition()
        {
            return snakePosition.First.Value;
        }

        public Position TailPosition()
        {
            return snakePosition.Last.Value;
        }

        public IEnumerable<Position> SnakePositions()
        {
            return snakePosition;
        }

        private void AddHead(Position pos)
        {
            snakePosition.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }

        private void RemoveTail()
        {
            Position tail = snakePosition.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePosition.RemoveLast();
        }

        public void ChangeDirection(Direction dir)
        {
            Dir = dir;

        }

        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Col < 0 || pos.Row >= Rows || pos.Col >= Cols;
        }

        public GridValue WillHit(Position newHeadPos)
        {
            if (OutsideGrid(newHeadPos))
            {
                return GridValue.Outside;
            }else if (newHeadPos == TailPosition())
            {
                return GridValue.Empty;
            }

            return Grid[newHeadPos.Row, newHeadPos.Col];
        }

        public void Move()
        {
            Position newHeadPosition = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPosition);
            if(hit == GridValue.Outside || hit == GridValue.Snake)
            {
                GameOver = true;
            }
            else if (hit == GridValue.Food)
            {
                AddHead(newHeadPosition);
                Score++;
                AddFood();
            }
            else if(hit == GridValue.Empty)
            {
                
                RemoveTail();
                AddHead(newHeadPosition);
            }

        }
    }
}
