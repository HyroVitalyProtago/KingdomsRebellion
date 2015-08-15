using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KingdomsRebellion.AI;
using KingdomsRebellion.Core.Map;
using KingdomsRebellion.Core.Math;
using UnityEngine;

namespace KingdomsRebellion.Core.Components {

	[RequireComponent(typeof(KRTransform))]
	public class KRMovement : KRBehaviour {

		[SerializeField]
		int _speed;

		public KRTransform Followee { get; private set; }

		public Vec2 Target { get; private set; }

		public int Speed { get { return _speed; } private set { _speed = value; } }

		KRTransform _krtransform;
		IEnumerable<Vec2> _waypoints;
		int _currentFrame;

		void Awake() {
			_krtransform = GetComponent<KRTransform>();
		}

		void Start() {
			_currentFrame = 0;
		}

		public void UpdateGame() {
			if (Followee != null) {
				Target = Followee.Pos;
			}
			if (Target == null || _krtransform.Pos == Target) {
				Target = null;
				return;
			}

			// NextFrame
			if (_currentFrame > 0) {
				--_currentFrame;
				return;
			} else {
				_currentFrame = Speed;
			}

			if (_waypoints == null) {
				_waypoints = KRFacade.FindPath(_krtransform.Pos, Target);
			}

			Vec2 nextWaypoint;
			nextWaypoint = !_waypoints.Any() ? Target : _waypoints.First();

			Vec2 nextPos = _krtransform.Pos;
			Bresenham.Line(_krtransform.Pos.X, _krtransform.Pos.Y, nextWaypoint.X, nextWaypoint.Y, delegate(int x, int y) {
				nextPos = new Vec2(x, y);
				return false;
			});

			if (nextPos != _krtransform.Pos && KRFacade.IsEmpty(nextPos)) {
				KRFacade.Remove(_krtransform);
				_krtransform.Pos = nextPos;
				KRFacade.Add(_krtransform);
			} else {
				_waypoints = null;
				Target = null;
			}
		}

		public bool HaveTarget() {
			return Target != null;
		}

		public void Move(Vec2 target) {
			if (_krtransform.Pos == target) {
				return;
			}
			if (Target == null) {
				_currentFrame = Speed;
			}
			Target = target;
		}

		public void Follow(KRTransform kt) {
			Followee = kt;
			Move(kt.Pos);
		}
	}
}
