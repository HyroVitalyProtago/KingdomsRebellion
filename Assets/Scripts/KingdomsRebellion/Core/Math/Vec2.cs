using System.IO;
using UnityEngine;

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

		public static bool operator ==(Vec2 v1, Vec2 v2) {
			if ((object)v1 == null && (object)v2 == null) return true;
			if ((object)v1 == null || (object)v2 == null) return false;
			return v1.X == v2.X && v1.Y == v2.Y;
		}
		
		public static bool operator !=(Vec2 v1, Vec2 v2) {
			return !(v1 == v2);
		}
		
		public static int Dist(Vec2 v1, Vec2 v2) {
			return v1.Dist(v2);
		}
		
		public int Dist(Vec2 v) {
			int x = X - v.X;
			int y = Y - v.Y;
			return (x * x) + (y * y);
		}

		public override string ToString() {
			return "(" + X + ", " + Y + ")";
		}

		public override bool Equals(object obj) {
			if ((object)obj == null || !(obj is Vec2)) return false;
			Vec2 v = obj as Vec2;
			return X == v.X && Y == v.Y;
		}

		public override int GetHashCode() {
			return X ^ Y;
		}

		public void Serialize(BinaryWriter writer) {
			writer.Write(X);
			writer.Write(Y);
		}
		
		public static Vec2 Deserialize(BinaryReader reader) {
			return new Vec2(reader.ReadInt32(), reader.ReadInt32());
		}
		
		public Vector3 ToVector3() {
			return new Vector3(this.X, 0, this.Y);
		}
		
		public static Vec2 FromVector3(Vector3 vector3) {
			return new Vec2(Mathf.FloorToInt(vector3.x), Mathf.FloorToInt(vector3.z));
		}
	}
}
