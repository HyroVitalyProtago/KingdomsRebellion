using System;
using KingdomsRebellion.Core.Interfaces;
using KingdomsRebellion.Core.Math;
using UnityEngine;

namespace KingdomsRebellion.Core.Components {

	public class KRTransform : KRBehaviour, IPos, ISize {
    	public int __playerID, __sizeX, __sizeY; // just for Unity Editor

		event Action<GameObject> OnBirth;

		public int PlayerID { get; set; }
		public Vec2 Pos {
			get {
				return _pos;
			}
			set {
				_pos = value;
				Vector3 v = _pos.ToVector3().Adjusted();
				transform.position = new Vector3(v.x, transform.position.y, v.z);
			}
		}
		public Vec2 Size { get; private set; }

		Vec2 _pos;

		void Awake() {
			PlayerID = __playerID;
			Pos = Vec2.FromVector3(transform.position);
			Size = new Vec2(__sizeX, __sizeY);

			KRFacade.Add(this);
			Offer("OnBirth");
		}

		void Start() {
			if (OnBirth != null) { OnBirth(gameObject); }
		}
    }
}