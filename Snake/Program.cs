using System;
using System.Diagnostics;
using System.Timers;

namespace Snake;
class ABC : EventArgs {
   int x = 9;
}
internal class Program {
   //getlength(0) = y
   //getlength(1) = x
   const int size_x = 20;
   const int size_y = 20;
   double timeInterval = 100; //milisecond

   Queue<Vector2> queue = new Queue<Vector2>();
   System.Timers.Timer timer = new System.Timers.Timer();
   Stopwatch watch = new Stopwatch();
   Random random = new Random();
   //===============================================================
   int length;
   char[,] array = new char[size_x,size_y];
   Vector2 direction = Vector2.Zero;
   Vector2 current = Vector2.Zero;
   Vector2 rdmFoodPos;
   ConsoleKeyInfo[] keys = new ConsoleKeyInfo[1];
   //===============================================================
   string[] lists = new string[4] { "100ms", "200ms", "500ms", "1000ms" };
   int[] i_lists = new int[4] { 100, 200, 500, 1000 };
   // food
   bool canSpawnFood = true;
   int selected = 0;

   static void Main (string[] args) {
      // entry
      Program p = new Program();
      p.Start();
   }

   void UI () {
      Console.SetCursorPosition(0, 3);
      for (int i = 0; i < 4; i++) {
         if (i == selected) {
            Console.WriteLine($"> {lists[i]}");
            continue;
         }
         Console.WriteLine($"@ {lists[i]}");
      }
   }

   void Start () {
      // reset value
      ResetValue();

      // wait key pressed
      Console.WriteLine(".Net Console Snake Game v2 09/09/2022");
      Console.WriteLine("=== Pick speed ===");
      UI(); // init ui text
      // pick speed
      while (true) {
         var k = Console.ReadKey().Key;
         if (k == ConsoleKey.UpArrow && selected > 0) {
            selected--;
            UI();
         }
         else if (k == ConsoleKey.DownArrow && selected < 3) {
            selected++;
            UI();
         }
         else if(k == ConsoleKey.Enter) {
            break;
         }
      }
      Console.Clear();
      // enable timer
      timer.Enabled = true;
      timer.Start();
      timer.Interval = i_lists[selected];

      watch.Start();

      // init game value
      for (int i = 0; i < array.GetLength(0); i++) {
         for (int j = 0; j < array.GetLength(1); j++) {
            array[i, j] = '#';
         }
      }
      queue.Enqueue(new Vector2(0, 0));
      direction = Vector2.Right;

      // start game update
      timer.Elapsed += Update;
      while (true) {
         GetKey();
      }
   }

   void GetKey () {
      if (Console.KeyAvailable) {
         keys[0] = Console.ReadKey(true);
      }
   }

   private void Update (object? sender, ElapsedEventArgs e) {
      // update direction according to last key pressed
      UpdateDirection();
      // refresh screen
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
      // else
      // reset the value and dequeue the tail
      if (current != rdmFoodPos) {
         Vector2 front = queue.Peek();
         array[front.y, front.x] = '#';
         queue.Dequeue();
      }
      else {
         length++;
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
         watch.Stop();
         timer.Enabled = false;
         timer.Stop();
         timer.Elapsed -= Update;

         Console.SetCursorPosition(0, array.GetLength(0)+4);
         Console.WriteLine("========== You Lose! =========");
         Console.WriteLine($"===== Survived Time {watch.ElapsedMilliseconds / 1000} s =====");

         Console.WriteLine("Press any key to restart!");
         Console.ReadKey();
         Start();
      }
   }

   void UpdateDirection () {
      if (direction == Vector2.Up || direction == Vector2.Down) {
         switch (keys[0].Key) {
            case ConsoleKey.LeftArrow:
               direction = Vector2.Left;
               break;
            case ConsoleKey.RightArrow:
               direction = Vector2.Right;
               break;
         }
      }
      else if (direction == Vector2.Left || direction == Vector2.Right) {
         switch (keys[0].Key) {
            case ConsoleKey.UpArrow:
               direction = Vector2.Up;
               break;
            case ConsoleKey.DownArrow:
               direction = Vector2.Down;
               break;
         }
      }
   }

   Vector2 RandFoodPos () {
      return new Vector2(random.Next(array.GetLength(1) - 1),
         random.Next(array.GetLength(0) - 1));
   }

   void Output () {
      Console.WriteLine($"\nCurrent Position = {current.x} , {current.y}");
      Console.WriteLine($"Length = {length}");
      Console.WriteLine($"Update Interval (ms) = {timeInterval}");
      Console.WriteLine($"Survied Time = {Math.Round(watch.ElapsedMilliseconds * 0.001,2)}");
      Console.WriteLine($"Food position = {rdmFoodPos.x} , {rdmFoodPos.y}");
   }

   void ResetValue () {
      // clear queue
      Console.Clear();
      queue = new Queue<Vector2>();
      length = 1;
      direction = Vector2.Zero;
      current = Vector2.Zero;
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
