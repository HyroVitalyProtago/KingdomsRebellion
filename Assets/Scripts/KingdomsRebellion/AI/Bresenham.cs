using System;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.AI {

	/// <summary>
	/// A Tool class for trace line in discrete world
	/// </summary>
	public static class Bresenham {
		static int Normalize(int x) { return x < 0 ? -1 : x > 0 ? 1 : 0; }

		/// <summary>
		/// Apply f on all (except the first) points on the line between orig and dest
		/// </summary>
		/// <param name="orig">Origin point</param>
		/// <param name="dest">Destination point</param>
		/// <param name="f">A function (if returns false, the algorithm stops early)</param>
		public static void Line(Vec2 orig, Vec2 dest, Func<int,int,bool> f) {
			Line(orig.X, orig.Y, dest.X, dest.Y, f);
		}

		/// <summary>
		/// Apply f on all (except the first) points on the line between (x0, y0) and (x1, y1)
		/// </summary>
		/// <param name="x0">Abscissa of the origin point</param>
		/// <param name="y0">Ordinate of the origin point</param>
		/// <param name="x1">Abscissa of the destination point</param>
		/// <param name="y1">Ordinate of the destination point</param>
		/// <param name="f">A function (if returns false, the algorithm stops early)</param>
		public static void Line(int x0, int y0, int x1, int y1, Func<int,int,bool> f) {
			int w = x1 - x0;
			int h = y1 - y0;
			int dx0 = Normalize(w), dy0 = Normalize(h), dx1 = Normalize(w), dy1 = 0;
			int longest = Math.Abs(w);
			int shortest = Math.Abs(h);

			if (longest <= shortest) {
				longest = Math.Abs(h);
				shortest = Math.Abs(w);
				dy1 = Normalize(h);
				dx1 = 0;
			}

			int numerator = longest >> 1;
			for (int i = 0; i < longest; ++i) {
				numerator += shortest;
				if (numerator >= longest) {
					numerator -= longest;
					x0 += dx0;
					y0 += dy0;
				} else {
					x0 += dx1;
					y0 += dy1;
				}
				if (!f(x0,y0)) { return; }
			}
		}
	}
}