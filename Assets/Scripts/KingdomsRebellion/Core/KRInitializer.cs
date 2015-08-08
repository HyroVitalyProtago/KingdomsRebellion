using System;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace KingdomsRebellion.Core {
	public class KRInitializer : KRBehaviour {

		/// <summary>
		/// Call Awake (static) method on all classes in the Assembly
		/// </summary>
		void Awake() {
			Assembly
				.GetExecutingAssembly()
				.GetExportedTypes()
				.Where(t => t.IsClass)
				.ToList()
				.ForEach(delegate(Type c) {
					var m = c.GetMethod("Awake", BindingFlags.Static | BindingFlags.Public);
					if (m != null) {
						Debug.Log(c.Name+".Awake()");
						m.Invoke(null, null);
					}
				});
		}
	}
}