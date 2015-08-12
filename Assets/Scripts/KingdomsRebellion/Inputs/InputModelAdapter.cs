using System;
using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Math;
using UnityEngine;
using KingdomsRebellion.Core.Components;

namespace KingdomsRebellion.Inputs {
	public static class InputModelAdapter {

		static event Action<Vec2> OnModelSelect;
		static event Action<Vec2,Vec2,Vec2> OnModelDrag;
		static event Action<Vec2> OnModelAction;

		static Vec2 beginDrag;

		public static Vec2 BeginDrag { get { return beginDrag; } }

		public static void Awake() {
			EventConductor.On(typeof(InputModelAdapter), "OnLeftClickDown");
			EventConductor.On(typeof(InputModelAdapter), "OnLeftClickUp");
			EventConductor.On(typeof(InputModelAdapter), "OnRightClick");

			EventConductor.Offer(typeof(InputModelAdapter), "OnModelSelect");
			EventConductor.Offer(typeof(InputModelAdapter), "OnModelDrag");
			EventConductor.Offer(typeof(InputModelAdapter), "OnModelAction");
		}

		public static Vector3 WorldPosition(Vector3 mousePosition) {
			Vector3 worldPosition = new Vector3(-1, -1, -1);
			
			Ray ray = Camera.main.ScreenPointToRay(mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
				if (hit.collider.gameObject.CompareTag("Selectable")) {
					worldPosition = hit.collider.GetComponent<KRTransform>().Pos.ToVector3();
				} else {
					worldPosition = hit.point;
				}
			}

			return worldPosition;
		}

		public static Vec2 ModelPosition(Vector3 mousePosition) {
			return Vec2.FromVector3(WorldPosition(mousePosition));
		}

		static void OnLeftClickDown(Vector3 mousePosition) {
			beginDrag = ModelPosition(mousePosition);
		}

		static void OnLeftClickUp(Vector3 mousePosition) {
			Vec2 modelPoint = ModelPosition(mousePosition);

			if (beginDrag.Dist(modelPoint) < 1) {
				if (OnModelSelect != null) {
					OnModelSelect(modelPoint);
				}
			} else {
				if (OnModelDrag != null) {
					Vec2 additionalPoint = ModelPosition(
						new Vector3(mousePosition.x, Camera.main.WorldToScreenPoint(beginDrag.ToVector3()).y, mousePosition.z)
					);
					OnModelDrag(beginDrag, modelPoint, additionalPoint);
				}
			}
		}

		static void OnRightClick(Vector3 mousePosition) {
			if (OnModelAction != null) {
				OnModelAction(ModelPosition(mousePosition));
			}
		}

	}
}