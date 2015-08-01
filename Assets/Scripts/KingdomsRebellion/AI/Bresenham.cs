using System;

namespace KingdomsRebellion.AI {
	public static class Bresenham {
		static int Normalize(int x) { return x < 0 ? -1 : x > 0 ? 1 : 0; }

		/// <summary>
		/// Apply f on all points on the line between (x0, y0) and (x1, y1)
		/// </summary>
		/// <param name="f">A function (if returns false, the algorithm stops early)</param>
		public static void Line(int x0, int y0, int x1, int y1, Func<int,int,bool> f) {
			int w = x1 - x0;
			int h = y1 - y0;
			int dx0 = Normalize(w), dy0 = Normalize(h), dx1 = Normalize(w), dy1 = 0;
			int longest = Math.Abs(w);
			int shortest = Math.Abs(h);

			if (!(longest > shortest)) {
				longest = Math.Abs(h);
				shortest = Math.Abs(w);
				dy1 = Normalize(h);
				dx1 = 0;
			}

			int numerator = longest >> 1;
			for (int i = 0; i < longest; ++i) {
				numerator += shortest;
				if (!(numerator < longest)) {
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