using System.IO;

namespace KingdomsRebellion.Core.Math {

	//
	// Light implementation of Vector3 with integers
	//
	public class Vec3 {

		readonly int _x, _y, _z;

		public Vec3(int x, int y, int z) {
			_x = x; 
			_y = y;
			_z = z;
		}
			
		public static Vec3 operator -(Vec3 v) {
			return new Vec3(-v._x, -v._y, -v._z); 
		}
			
		public static Vec3 operator +(Vec3 v1, Vec3 v2) { 
			return new Vec3(v1._x + v2._x, v1._y + v2._y, v1._z + v2._z); 
		}
			
		public static Vec3 operator -(Vec3 v1, Vec3 v2) {
			return new Vec3(v1._x - v2._x, v1._y - v2._y, v1._z - v2._z);
		}
			
		public static Vec3 operator *(Vec3 v, int scalar) {
			return new Vec3(v._x * scalar, v._y * scalar, v._z * scalar);
		}
			
		public static Vec3 operator *(int scalar, Vec3 v) {
			return new Vec3(v._x * scalar, v._y * scalar, v._z * scalar); 
		}

		public static int Dist(Vec3 v1, Vec3 v2) {
			return v1.Dist(v2);
		}

		public int Dist(Vec3 v) {
			int x = _x - v._x;
			int y = _y - v._y;
			int z = _z - v._z;
			return (x * x) + (y * y) + (z * z);
		}

		public void Serialize(BinaryWriter writer) {
			writer.Write(_x);
			writer.Write(_y);
			writer.Write(_z);
		}

		public static Vec3 Deserialize(BinaryReader reader) {
			return new Vec3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
		}
	}

}