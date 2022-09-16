using System;
using System.Diagnostics;
using System.Timers;
namespace Snake;

class Map {
   private int _x;
   private int _y;
   private char[,] array;

   public int SizeX => _x; //get x length
   public int SizeY => _y; //get y length
   public Map (int x,int y) {
      _x = x; 
      _y = y;
      array = new char[x,y];
   }

}

internal class Snake {
   //getlength(0) = y
   //getlength(1) = x
   Map? map;

   string[] diff_string = new string[3] { "Easy" , "Normal" , "Hard" };
   int[] diff_int = new int[3] { 500,200,100 };
   string[] map_size = new string[3] { "Small", "Normal", "Big " };
   int[] map_int = new int[3] { 10, 20, 30 };

   Queue<Vector2> queue = new Queue<Vector2>();
   System.Timers.Timer timer = new System.Timers.Timer();
   Stopwatch watch = new Stopwatch();
   Random random = new Random();
   //===============================================================
   int length;
   char[,]? array;
   Vector2 direction = Vector2.Zero;
   Vector2 current = Vector2.Zero;
   Vector2 rdmFoodPos;
   ConsoleKeyInfo key;

   enum Direction { Left, Right, Up, Down };
   Direction dir;
   //===============================================================

   // food
   bool canSpawnFood = true;
   int selected = 0;

   static void Main (string[] args) {
      // entry
      Snake p = new Snake();
      p.Start();
   }

   void UI (string[] lists) {
      Console.SetCursorPosition(0, 3);
      for (int i = 0; i < lists.Length; i++) {
         if (i == selected) {
            Console.WriteLine($"> {lists[i]}");
            continue;
         }
         Console.WriteLine($"@ {lists[i]}");
      }
   }

   void Start () {
      Console.Clear();

      // wait key pressed
      Console.WriteLine(".Net Console Snake Game v2 09/09/2022");
      Console.WriteLine("=== Pick speed ===");

      // select difficulty
      UI(diff_string);
      int pick_diff = SetDifficulty(diff_string);

      selected = 0; // reset selected value
      // select map size
      UI(map_size);
      int pick_map = SetDifficulty(map_size);
      // set map size
      array = new char[map_int[pick_map],map_int[pick_map]];
      //map = new Map(map_int[pick_map], map_int[pick_map]);
      Vector2 randomPos = randomStartPos();
      // reset value
      ResetValue(randomPos);

      Console.Clear();
      // enable timer and set speed
      timer.Enabled = true;
      timer.Start();
      timer.Interval = diff_int[pick_diff];

      // init game screen and value
      for (int i = 0; i < array.GetLength(0); i++) {
         for (int j = 0; j < array.GetLength(1); j++) {
            
            if (j == 0)                           // draw top
               array[j, i] = '=';
            else if (j == array.GetLength(0) - 1) // draw bottom
               array[j, i] = '=';
            else if (i == 0)                      // draw left
               array[j, i] = '|';
            else if (i == array.GetLength(1) - 1) // draw right
               array[j, i] = '|';
            else                                  // draw middle
               array[j, i] = ' ';
         }
      }

      queue.Enqueue(randomPos);
      RandomStartDir();

      // start game update
      timer.Elapsed += Update;
      while (true) {
         GetKey();
      }
   }
   int SetDifficulty (string[] arr) {
      // pick speed
      while (true) {
         var k = Console.ReadKey().Key;
         if (k == ConsoleKey.UpArrow && selected > 0) {
            selected--;
            UI(arr);
         }
         else if (k == ConsoleKey.DownArrow && selected < 3) {
            selected++;
            UI(arr);
         }
         else if (k == ConsoleKey.Enter) {
            // start watch
            watch.Start();
            return selected;
         }
      }
   }

   void GetKey () {
      if (Console.KeyAvailable) {
         key = Console.ReadKey(true);
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
            array[rdmFoodPos.y, rdmFoodPos.x] = '!';
            canSpawnFood = false;
         }
      }

      // add current position
      current.x += direction.x;
      current.y += direction.y;
      // if head collide with body
      if (queue.Contains(current)) {
         Lose();
      }
      else {
         queue.Enqueue(current);

         // if not collide with food keep dequeuing
         if (current != rdmFoodPos) {
            Vector2 front = queue.Peek();
            array[front.y, front.x] = ' '; 
            queue.Dequeue();
         }
         // after collide with food dont dequeue
         else {
            canSpawnFood = true;
            length++;
         }

         // check collision
         if (current.x < array.GetLength(1) -1 && current.y < array.GetLength(0) -1 &&
            current.x > 0 && current.y > 0) {
            foreach (var q in queue) {
               array[q.y, q.x] = '@';
            }
            for (int i = 0; i < array.GetLength(0); i++) {
               for (int j = 0; j < array.GetLength(1); j++) { // >>>>>>> V
                  Console.Write(array[i, j]);
               }
               Console.WriteLine();
            }
            //Output();
         }

         // if collide
         else {
            Lose();
         }
      }
   }
   void Lose () {
      // reset time
      watch.Stop();
      watch.Reset();
      // stop timer
      timer.Enabled = false;
      timer.Stop();
      timer.Elapsed -= Update;

      Console.SetCursorPosition(0, array.GetLength(0) + 4);
      Console.WriteLine("========== You Lose! =========");
      Console.WriteLine($"===== Survived Time {watch.ElapsedMilliseconds / 1000} s =====");

      Console.WriteLine("Press any key to restart!");
      Console.ReadKey();
      Start();
   }
   void UpdateDirection () {
      if (direction == Vector2.Up || direction == Vector2.Down) {
         switch (key.Key) {
            case ConsoleKey.LeftArrow:
               direction = Vector2.Left;
               break;
            case ConsoleKey.RightArrow:
               direction = Vector2.Right;
               break;
         }
      }
      else if (direction == Vector2.Left || direction == Vector2.Right) {
         switch (key.Key) {
            case ConsoleKey.UpArrow:
               direction = Vector2.Up;
               break;
            case ConsoleKey.DownArrow:
               direction = Vector2.Down;
               break;
         }
      }
   }

   void RandomStartDir () {
      dir = (Direction)random.Next(0, 4);
      switch (dir) {
         case Direction.Left:
            direction = Vector2.Left;
            break;
         case Direction.Right:
            direction = Vector2.Right;
            break;
         case Direction.Up:
            direction = Vector2.Up;
            break;
         case Direction.Down:
            direction = Vector2.Down;
            break;
      }
   }

   Vector2 RandFoodPos () {
      return new Vector2(random.Next(2, array.GetLength(1) - 2),
         random.Next(2, array.GetLength(0) - 2));
   }

   

   void Output () {
      Console.WriteLine($"\nCurrent Position = {current.x} , {current.y}");
      Console.WriteLine($"Length = {length}");
      Console.WriteLine($"Update Interval (ms) = {diff_int[selected]}");
      Console.WriteLine($"Survied Time = {Math.Round(watch.ElapsedMilliseconds * 0.001, 2)}");
      Console.WriteLine($"Food position = {rdmFoodPos.x} , {rdmFoodPos.y}");
   }

   void ResetValue (Vector2 s_pos) {
      // clear queue
      Console.Clear();
      queue = new Queue<Vector2>();
      length = 1;
      direction = Vector2.Zero;
      current = s_pos;
      canSpawnFood = true;
   }

   Vector2 randomStartPos () {
      return new Vector2(random.Next(0 + 3, array.GetLength(1) - 3),
         random.Next(0 + 3, array.GetLength(0) - 3));
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

   public override bool Equals (object? obj) {
      if (obj == null) return false;
      return (this.x == ((Vector2)obj).x && this.y == ((Vector2)obj).y);
   }

   public override int GetHashCode () {
      throw new NotImplementedException();
   }
}
