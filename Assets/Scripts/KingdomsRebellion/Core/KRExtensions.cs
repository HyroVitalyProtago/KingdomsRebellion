using UnityEngine;
using System.IO;

namespace KingdomsRebellion.Core {
	public static class KRExtensions {
		static Vector3 offset = new Vector3(.5f,0,.5f);
		public static Vector3 Adjusted(this Vector3 v) {
			return v + offset;
		}
	}
}