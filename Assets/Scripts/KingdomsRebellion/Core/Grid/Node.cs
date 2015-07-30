using System;
using KingdomsRebellion.Core.Math;
using UnityEngine;

namespace KingdomsRebellion.Core.Grid {

	public class Node : IComparable {

		bool _walkable;
		public bool Walkable { get { return _walkable && GameObject == null; } }
		public Vec2 Pos { get; private set; }
		int _gCost, _hCost;
		public int GCost { get { return _gCost; } set { update(); _gCost = value; } }
		public int HCost { get { return _hCost; } set { update(); _hCost = value; } }
		public int FCost { get { return _gCost + _hCost; } }
		public Node Parent { get; set; }
		public GameObject GameObject { get; set; }

		// TEST
		static Transform _parent = GameObject.Find("World/Dynamics/Tiles").transform;
		static GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		GameObject _self;

		public Node(Vec2 pos, bool walkable = true) {
			Pos = pos;
			_walkable = walkable;

			if (pos.X < 100 || pos.Y < 100 || pos.X > 150 || pos.Y > 150) return;
			_self = GameObject.Instantiate(cube);
			_self.transform.SetParent(_parent);
			_self.tag = "Ground";
			_self.transform.position = new Vector3(pos.X, -.5f, pos.Y);
		}

		// TEST
		void update() {
			if (_self) {
				_self.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
				_self.GetComponent<MeshRenderer>().material.color = new Color(
				(float)_gCost*.01f,
				(float)_hCost*.01f,
				.5f
			);
			}
		}

		public int CompareTo(object obj) {
			Node temp = obj as Node;
			if (temp == null) { throw new ArgumentException("Object is not a Node"); }

			int result = FCost.CompareTo(temp.FCost);
			if (result != 0) { return result; }

			return _hCost.CompareTo(temp._hCost);
		}
	}

}