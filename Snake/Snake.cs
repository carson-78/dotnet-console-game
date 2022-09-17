using System;
using System.Diagnostics;
using System.Timers;
namespace Snake;

class Map {
   private int x;
   private int y;
   private char[,] array;

   public int SizeX => x; //get x length
   public int SizeY => y; //get y length
   public Map (int size_x, int size_y) {
      x = size_x;
      y = size_y;
      array = new char[size_x, size_y];
   }

   public char GetValue (int x, int y) {
      return array[x, y];
   }
   public void SetValue (int x, int y, char c) {
      array[x, y] = c;
   }

}

internal class Snake {
   //getlength(0) = y
   //getlength(1) = x
   Map? map;

   Queue<Vector2> queue = new Queue<Vector2>();
   System.Timers.Timer timer = new System.Timers.Timer();
   Stopwatch watch = new Stopwatch();
   Random random = new Random();
   //===============================================================

   string[] diff_string = new string[3] { "Easy", "Normal", "Hard" };
   int[] diff_int = new int[3] { 500, 200, 100 };
   string[] map_size = new string[4] { "Small", "Normal", "Big ","Huge" };
   Vector2[] map_int = new Vector2[4] { new Vector2(15, 15), new Vector2(20, 20), new Vector2(20, 35) , new Vector2(25,50) };
   int pick_diff = 0;
   int pick_map = 0;
   //===============================================================
   int length;
   Vector2 direction = Vector2.Zero;
   Vector2 current = Vector2.Zero;
   Vector2 rdmFoodPos;
   ConsoleKeyInfo key;

   enum Direction { Left, Right, Up, Down };
   Direction dir;
   //===============================================================
   // food
   bool canSpawnFood = true;

   static void Main (string[] args) {
      // entry
      Console.WindowHeight = 40;
      Console.WindowWidth = 120;
      Snake p = new Snake();
      p.Start();
   }

   void Start () {
      Console.Clear();
      Console.WriteLine(".Net Console Snake Game v2 09/09/2022");
      Console.WriteLine("=== Pick speed ===");

      //========== select difficulty ==================
      UI(diff_string,ref pick_diff);
      pick_diff = Setting(diff_string,ref pick_diff);

      // select map size
      UI(map_size,ref pick_map);
      pick_map = Setting(map_size,ref pick_map);
      map = new Map(map_int[pick_map].x, map_int[pick_map].y);
      //===============================================

      Vector2 randomPos = randomStartPos();
      // reset value
      ResetValue(randomPos);

      Console.Clear();
      // enable timer and set speed
      timer.Enabled = true;
      timer.Start();
      timer.Interval = diff_int[pick_diff];

      // init game screen and value
      for (int i = 0; i < map.SizeX; i++) { // i = y
         for (int j = 0; j < map.SizeY; j++) { // j = x

            if (i == 0)                  // draw top
               map.SetValue(i, j, '=');
            else if (i == map.SizeX - 1) // draw bottom
               map.SetValue(i, j, '=');
            else if (j == 0)             // draw left
               map.SetValue(i, j, '|');
            else if (j == map.SizeY - 1) // draw right
               map.SetValue(i, j, '|');
            else                         // draw middle
               map.SetValue(i, j, ' ');
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
   int Setting (string[] arr,ref int selected) {
      // pick speed
      while (true) {
         var k = Console.ReadKey().Key;
         if (k == ConsoleKey.UpArrow && selected > 0) {
            selected--;
            UI(arr,ref selected);
         }
         else if (k == ConsoleKey.DownArrow && selected < arr.Length-1) {
            selected++;
            UI(arr,ref selected);
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
         if (map.GetValue(rdmFoodPos.x, rdmFoodPos.y) != '@') {
            map.SetValue(rdmFoodPos.x, rdmFoodPos.y, '!');
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
            map.SetValue(front.x, front.y, ' ');
            queue.Dequeue();
         }
         // after collide with food dont dequeue
         else {
            canSpawnFood = true;
            length++;
         }

         // check collision
         if (current.x < map.SizeX - 1 && current.y < map.SizeY - 1 &&
            current.x > 0 && current.y > 0) {
            foreach (var q in queue) {
               map.SetValue(q.x, q.y, '@');
            }
            DrawMap(map);
            Output();
         }

         // if collide
         else {
            Lose();
         }
      }
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
   void Lose () {
      double time = Math.Round(watch.ElapsedMilliseconds * 0.001, 2);
      double score = Math.Round(time * 13 * length);
      // reset time
      watch.Stop();
      watch.Reset();
      // stop timer
      timer.Enabled = false;
      timer.Stop();
      timer.Elapsed -= Update;

     // Console.SetCursorPosition(0,);
      Console.WriteLine("========== You Lose! =========");
      Console.WriteLine($"======= Score {score} =======");

      Console.WriteLine("Press any key to restart!");
      Console.ReadKey();
      Start();
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
      return new Vector2(random.Next(2, map.SizeX - 2),
         random.Next(2, map.SizeY - 2));
   }

   void Output () {
      Console.WriteLine($"\nLength = {length}");
      Console.WriteLine($"Update Interval (ms) = {diff_int[pick_diff]}");
      Console.WriteLine($"Survied Time = {Math.Round(watch.ElapsedMilliseconds * 0.001, 2)}");
      Console.WriteLine($"Map Size {map.SizeX} , {map.SizeY}");
   }

   void UI (string[] lists, ref int selected) {
      Console.SetCursorPosition(0, 3);
      for (int i = 0; i < lists.Length; i++) {
         if (i == selected) {
            Console.WriteLine($"> {lists[i]}");
            continue;
         }
         Console.WriteLine($"@ {lists[i]}");
      }
   }

   void DrawMap (Map map) {
      for (int i = 0; i < map.SizeX; i++) {
         for (int j = 0; j < map.SizeY; j++) {
            Console.Write(map.GetValue(i,j));
         }
         Console.WriteLine();
      }
   }

   void ResetValue (Vector2 s_pos) {
      // clear queue
      Console.Clear();
      queue = new Queue<Vector2>();
      length = 1;
      direction = Vector2.Zero;
      current = s_pos;
      canSpawnFood = true;
      //
   }

   Vector2 randomStartPos () { // random 0 to last 3 in x and y
      return new Vector2(random.Next(0, map.SizeX - 3),
         random.Next(0, map.SizeY - 3));
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
   public static Vector2 Right => new(0,1);
   public static Vector2 Left => new(0,-1);
   public static Vector2 Up => new(-1,0); 
   public static Vector2 Down => new(1,0); 

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
