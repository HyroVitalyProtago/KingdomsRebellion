using System.IO;

namespace KingdomsRebellion.Core.Math {

	/// <summary>
	/// Light implementation of Vector3 with integers.
	/// </summary>
	public class Vec3 {

		public readonly int X, Y, Z;

		public Vec3(int x, int y, int z) {
			X = x; 
			Y = y;
			Z = z;
		}
			
		public static Vec3 operator -(Vec3 v) {
			return new Vec3(-v.X, -v.Y, -v.Z); 
		}
			
		public static Vec3 operator +(Vec3 v1, Vec3 v2) { 
			return new Vec3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z); 
		}
			
		public static Vec3 operator -(Vec3 v1, Vec3 v2) {
			return new Vec3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
		}
			
		public static Vec3 operator *(Vec3 v, int scalar) {
			return new Vec3(v.X * scalar, v.Y * scalar, v.Z * scalar);
		}
			
		public static Vec3 operator *(int scalar, Vec3 v) {
			return new Vec3(v.X * scalar, v.Y * scalar, v.Z * scalar); 
		}

		public static int Dist(Vec3 v1, Vec3 v2) {
			return v1.Dist(v2);
		}

		public int Dist(Vec3 v) {
			int x = X - v.X;
			int y = Y - v.Y;
			int z = Z - v.Z;
			return (x * x) + (y * y) + (z * z);
		}

		public void Serialize(BinaryWriter writer) {
			writer.Write(X);
			writer.Write(Y);
			writer.Write(Z);
		}

		public static Vec3 Deserialize(BinaryReader reader) {
			return new Vec3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
		}
	}

}