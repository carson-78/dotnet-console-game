namespace Utility;

struct Vector2 {
   public Vector2 (int _x, int _y) {
      x = _x;
      y = _y;
   }
   public int x;
   public int y;

   public static Vector2 Zero => new(0, 0);
   public static Vector2 Right => new(0, 1);
   public static Vector2 Left => new(0, -1);
   public static Vector2 Up => new(-1, 0);
   public static Vector2 Down => new(1, 0);

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
