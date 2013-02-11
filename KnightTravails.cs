using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication13
{
    //The purpose of this program is to find the shortest path for a given starting position and ending position for a Knight tour. In this case, Breath First Search (BFS) has been applied. 
    class Program
    {
        static List<Position> history = new List<Position>(); //List of positions that have been visited
        static Queue<Position> queue = new Queue<Position>(); // Queue for the BFS 
        static List<Position> currentDepth = new List<Position>(); //List of positions for the current depth
        static void Main(string[] args)
        {
          
            //User interface 
            Console.WriteLine("This is a Knight's Travails program developed by Yanbin Liu.");
            bool running = true;
            while (running)
            {
                Console.WriteLine("1. Start a new tour");
                Console.WriteLine("2. Exit");

                Console.WriteLine("Please enter your choice:");
                int option = safeReadInteger(0);

                if (option == 2)
                    running = false;
                else if (option == 1)
                    startTour();
                else
                    Console.WriteLine("Invaild input");
            }
        }

        //Get the user input for two positions and start tour
        public static void startTour()
        {
            Console.WriteLine("You are going to enter two position in order to play the tour.");
            Console.WriteLine("The corrent format of a position including two characters which starts with a letter(A-H/a-h: left to right) and follows by a number(1-8 : top to bottom)");
            Position start = null;
            Position end = null;
            bool check = false;

            //Check if the position is vaild
            while (!check)
            {

                Console.WriteLine("Please enter the start position(eg. A8)");
                start = safeReadPosition();
                if (start != null)
                    check = true;

            }
            check = false;

            //Check if the position is vaild
            while (!check)
            {
                Console.WriteLine("Please enter the end position(eg. B6)");
                end = safeReadPosition();
                if (end != null)
                    check = true;
            }

            //Incase that the start and end positions are the same.
            if (start.comparePosition(end))
            {
                Console.WriteLine("Are you serious? The knight already in its destination.");
                return;
            }
            knightTour(start, end);
        }

        //Recursively attemp to move the knight until the problem solve
        public static void knightTour(Position start, Position end)
        {
            bool solved = false;

            queue.Enqueue(start);

            //Make all the possible move 
            while (queue.Count != 0)
            {
                //Make all the possible move for current depth
                while (queue.Count != 0)
                {
                    Position p = queue.Dequeue();

                    Position temp1 = new Position(p.X + 1, p.Y + 2);
                    checkPosition(p, temp1, end);

                    Position temp2 = new Position(p.X + 2, p.Y + 1);
                    checkPosition(p, temp2, end);

                    Position temp3 = new Position(p.X + 2, p.Y - 1);
                    checkPosition(p, temp3, end);

                    Position temp4 = new Position(p.X + 1, p.Y - 2);
                    checkPosition(p, temp4, end);

                    Position temp5 = new Position(p.X - 1, p.Y + 2);
                    checkPosition(p, temp5, end);

                    Position temp6 = new Position(p.X - 2, p.Y + 1);
                    checkPosition(p, temp6, end);

                    Position temp7 = new Position(p.X - 1, p.Y - 2);
                    checkPosition(p, temp7, end);

                    Position temp8 = new Position(p.X - 2, p.Y - 1);
                    checkPosition(p, temp8, end);
                }

                //Check if the problem solved
                foreach (Position a in currentDepth)
                {

                    if (a.comparePosition(end))
                    {
                        solved = true;
                        //Generate the output string
                        List<Position> output = new List<Position>();
                        output.Add(end);
                        Position temp = a;
                        while (temp.Parent != null)
                        {
                            output.Add(temp.Parent);
                            temp = temp.Parent;
                        }

                        Console.Write("The shortest path is ");
                        for (int i = output.Count - 2; i >= 0; i--)
                        {
                            Console.Write(output[i].ToString() + " ");
                        }
                        Console.WriteLine();
                        break;
                    }
                }

                currentDepth.Clear();
            }
            if (!solved)
            {
                Console.WriteLine("Mission impossible!");
            }
        }

        //Make sure the position is vaild and haven't been visited
        public static void checkPosition(Position previousPosition, Position currentPosition, Position end)
        {
            bool traveled = false;

            if (currentPosition.check())
            {
                foreach (Position a in history)
                {
                    if (a.comparePosition(currentPosition) && !end.comparePosition(currentPosition))
                    {

                        traveled = true;
                    }
                }
                if (!traveled)
                {
                    previousPosition.Children.Add(currentPosition);
                    history.Add(previousPosition);
                    currentPosition.Parent = previousPosition;
                    queue.Enqueue(currentPosition);
                    currentDepth.Add(currentPosition);
                }
            }
        }
        public static int safeReadInteger(int defaultVal)
        {
            try
            {
                return int.Parse(System.Console.ReadLine());
            }
            catch
            {
                return defaultVal;
            }
        }

        //Make sure the position from user is appropriate
        public static Position safeReadPosition()
        {

            try
            {
                string inputPosition = Console.ReadLine().ToUpper(); 
                if (inputPosition.Length != 2)
                {
                    Console.WriteLine("Invalid position!");
                    return null;
                }

                int x = (int)inputPosition[0] - 64;
                int y = (int)inputPosition[1] - 48; 

                Position outputPosition = new Position(x, y);
                
                //Check the position is inside the chessboard
                if (outputPosition.check())
                    return outputPosition;
                else
                {
                    Console.WriteLine("Invalid position!");
                    return null;
                }
            }
            catch
            {
                Console.WriteLine("Invalid position!");
                return null;
            }
        }
    }

    class Position
    {
        private int x; //the column position in chessboard (1 to n from left to right)
        private int y; //the row position in chessboard (1 to m from top down)
        private List<Position> children;  //list of steps towards the end position
        private Position parent;

        public int X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        public int Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        public List<Position> Children
        {
            get { return this.children; }
            set { this.children = value; }
        }

        public Position Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        //constructor of Position class
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;

            this.children = new List<Position>();
        }

        //Check if the position is valid
        public bool check()
        {
            int size = 8; // Size of the chessboard
            return !(x < 1 || y < 1 || x > size || y > size);

        }

        //Check if two positions are the same
        public bool comparePosition(Position other)
        {
          return (this.x == other.x && this.y == other.y);
        
        }

        //Generate the output position for the correct format
        public override string ToString()
        {
            char c = (Char)(64 + this.x);
            return c.ToString() + this.y.ToString();
        }
    }
}
