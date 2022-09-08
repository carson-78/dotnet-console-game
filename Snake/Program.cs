using System;
using System.Text;
using System.Timers;

namespace Snake;

internal class Program {
   //getlength(0) = y
   //getlength(1) = x
   const int size_x = 20;
   const int size_y = 20;
   static double timeInterval = 200; //milisecond

   static Queue<Vector2> queue = new Queue<Vector2>();
   static System.Timers.Timer timer = new System.Timers.Timer();
   static Random random = new Random();
   //============================================================
   static char[,] array = new char[size_x,size_y];
   static Vector2 direction = Vector2.Zero;
   static Vector2 current = Vector2.Zero;
   static Vector2 rdmFoodPos;
   //============================================================
   // food
   static bool canSpawnFood = true;

   static void Main (string[] args) {
      //// init value
      for (int i = 0; i < array.GetLength(0); i++) {
         for (int j = 0; j < array.GetLength(1); j++) {
            array[i, j] = '#';
         }
      }
      queue.Enqueue(new Vector2(0, 0));
      timer.Enabled = true;
      timer.Start();
      timer.Interval = timeInterval;
      timer.Elapsed += Update;
      GetKey();

   }
   static void GetKey () {
      while (true) {
         if (Console.KeyAvailable) {
            var key = Console.ReadKey(true);
            switch (key.Key) {
               case ConsoleKey.UpArrow:
                  direction = Vector2.Up;
                  break;
               case ConsoleKey.DownArrow:
                  direction = Vector2.Down;
                  break;
               case ConsoleKey.LeftArrow:
                  direction = Vector2.Left;
                  break;
               case ConsoleKey.RightArrow:
                  direction = Vector2.Right;
                  break;
            }
         }
      }
   }

   private static void Update (object? sender, ElapsedEventArgs e) {
      // clear and reload
      //Console.Clear();
      Console.SetCursorPosition(0, 0);
      // spawn food if possible
      if (canSpawnFood) {
         rdmFoodPos = RandFoodPos();
         if (array[rdmFoodPos.y, rdmFoodPos.x] != '@') {
            array[rdmFoodPos.y, rdmFoodPos.x] = 'X';
            canSpawnFood = false;
         }
      }

      // add current position
      current.x += direction.x;
      current.y += direction.y;
      queue.Enqueue(current);

      // if current position is collide with food dont dequeue
      //else
      // reset the value and dequeue the tail
      if (current != rdmFoodPos) {
         Vector2 front = queue.Peek();
         array[front.y, front.x] = '#';
         queue.Dequeue();
      }
      

      // boundary
      if (current.x < array.GetLength(1) && current.y < array.GetLength(0) &&
         current.x > -1 && current.y > -1) {
         foreach (var q in queue) {
            array[q.y, q.x] = '@';
         }
         for (int i = 0; i < array.GetLength(0); i++) {
            for (int j = 0; j < array.GetLength(1); j++) { // >>>>>>> V
               Console.Write(array[i, j]);
            }
            Console.WriteLine();
         }

         Output();
      }

      // outside boundary
      else {
         Console.SetCursorPosition(0, array.GetLength(0)+4);
         Console.WriteLine("========== You Lose! =========");
         timer.Stop();
      }

   }
   static Vector2 RandFoodPos () {
      return new Vector2(random.Next(array.GetLength(1) - 1),
         random.Next(array.GetLength(0) - 1));
   }

   static void Output () {
      Console.WriteLine($"\nCurrent Position = {current.x} , {current.y}");
      Console.WriteLine($"Update Interval (ms) = {timeInterval}");
      Console.WriteLine($"food position = {rdmFoodPos.x} , {rdmFoodPos.y}");
   }
}

struct Vector2 {
   public Vector2 (int _x, int _y) {
      x = _x;
      y = _y;
   }
   public int x;
   public int y;

   public static Vector2 Zero => new(0, 0);
   public static Vector2 Right => new(1, 0);
   public static Vector2 Left => new(-1, 0);
   public static Vector2 Up => new(0, -1);
   public static Vector2 Down => new(0, 1);

   public static Vector2 operator + (Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
   public static bool operator == (Vector2 a, Vector2 b) {
      return a.x == b.x && a.y == b.y;
   }
   public static bool operator != (Vector2 a, Vector2 b) {
      return !(a == b);
   }

   public override bool Equals (object obj) {
      throw new NotImplementedException();
   }

   public override int GetHashCode () {
      throw new NotImplementedException();
   }
}
