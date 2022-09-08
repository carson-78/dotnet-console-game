using System;
using System.Timers;

namespace Snake;

internal class Program {
   static double timeInterval = 1000; //milisecond

   static System.Timers.Timer timer = new System.Timers.Timer();
   static char[,] array = new char[5,10] {
      {'O','O','O','O','O','O','O','O','O','O'},
      {'O','O','O','O','O','O','O','O','O','O'},
      {'O','O','O','O','O','O','O','O','O','O'},
      {'O','O','O','O','O','O','O','O','O','O'},
      {'O','O','O','O','O','O','O','O','O','O'}
   };
   static int dir_x = 0;
   static int dir_y = 0;

   static int cur_x = 0;
   static int cur_y = 0;

   static void Main (string[] args) {
      timer.Enabled = true;
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
                  dir_x = 0;
                  dir_y = -1;
                  break;
               case ConsoleKey.DownArrow:
                  dir_x = 0;
                  dir_y = 1;
                  break;
               case ConsoleKey.LeftArrow:
                  dir_x = -1;
                  dir_y = 0;
                  break;
               case ConsoleKey.RightArrow:
                  dir_x = 1;
                  dir_y = 0;
                  break;
            }
         }
      }
   }

   private static void Update (object? sender, ElapsedEventArgs e) {
      // clear and reload
      Console.Clear();
      // boundary
      if(cur_x < array.GetLength(1) -1) {
         cur_x += dir_x;
      }
      if (cur_y < array.GetLength(0) - 1) {
         cur_y += dir_y;
      }

      for (int i = 0; i < array.GetLength(0); i++) {
         for (int j = 0; j < array.GetLength(1); j++) {
            if (i == cur_y && j == cur_x) {
               Console.Write('-');
               continue;
            }
            Console.Write(array[i, j]);
         }
         Console.WriteLine();
      }
      Output();
   }
   static void Output () {

      Console.WriteLine($"\nCurrent Position = {cur_x} , {cur_y}");
      Console.WriteLine($"Update Interval (ms) = {timeInterval}");
   }
}
