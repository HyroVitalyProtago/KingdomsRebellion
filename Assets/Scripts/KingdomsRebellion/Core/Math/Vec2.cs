
namespace KingdomsRebellion.Core.Math {

	/*
	 * Light implementation of Vector2 with integers
	 */
	public struct Vec2 {
		
		int _x, _y;
		
		public Vec2(int x, int y) {
			_x = x; 
			_y = y;
		}
		
		public static Vec2 operator -(Vec2 v) {
			return new Vec2(-v._x, -v._y);
		}
		
		public static Vec2 operator +(Vec2 v1, Vec2 v2) { 
			return new Vec2(v1._x + v2._x, v1._y + v2._y);
		}
		
		public static Vec2 operator -(Vec2 v1, Vec2 v2) {
			return new Vec2(v1._x - v2._x, v1._y - v2._y);
		}
		
		public static Vec2 operator *(Vec2 v, int scalar) {
			return new Vec2(v._x * scalar, v._y * scalar);
		}
		
		public static Vec2 operator *(int scalar, Vec2 v) {
			return new Vec2(v._x * scalar, v._y * scalar);
		}
		
		public static int Dist(Vec2 v1, Vec2 v2) {
			return v1.Dist(v2);
		}
		
		public int Dist(Vec2 v) {
			int x = _x - v._x;
			int y = _y - v._y;
			return (x * x) + (y * y);
		}
	}
}
