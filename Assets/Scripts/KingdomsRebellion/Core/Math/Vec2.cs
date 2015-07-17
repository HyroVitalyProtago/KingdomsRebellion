using System.IO;

namespace KingdomsRebellion.Core.Math {

	/// <summary>
	/// Light implementation of Vector2 with integers.
	/// </summary>
	public class Vec2 {

		public readonly int X, Y;
		
		public Vec2(int x, int y) {
			X = x; 
			Y = y;
		}
		
		public static Vec2 operator -(Vec2 v) {
			return new Vec2(-v.X, -v.Y);
		}
		
		public static Vec2 operator +(Vec2 v1, Vec2 v2) { 
			return new Vec2(v1.X + v2.X, v1.Y + v2.Y);
		}
		
		public static Vec2 operator -(Vec2 v1, Vec2 v2) {
			return new Vec2(v1.X - v2.X, v1.Y - v2.Y);
		}
		
		public static Vec2 operator *(Vec2 v, int scalar) {
			return new Vec2(v.X * scalar, v.Y * scalar);
		}
		
		public static Vec2 operator *(int scalar, Vec2 v) {
			return new Vec2(v.X * scalar, v.Y * scalar);
		}
		
		public static int Dist(Vec2 v1, Vec2 v2) {
			return v1.Dist(v2);
		}
		
		public int Dist(Vec2 v) {
			int x = X - v.X;
			int y = Y - v.Y;
			return (x * x) + (y * y);
		}

		public void Serialize(BinaryWriter writer) {
			writer.Write(X);
			writer.Write(Y);
		}

		public static Vec2 Deserialize(BinaryReader reader) {
			return new Vec2(reader.ReadInt32(), reader.ReadInt32());
		}
	}
}
