using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Components;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Network;

namespace KingdomsRebellion.UI {
    public class Target : KRBehaviour {

        IList<GameObject> _selected;
        GameObject _child;

        private void Start() {
            On("OnSelection");
            On("OnLeftClickUp");
            _selected = new List<GameObject>();
            _child = transform.GetChild(0).gameObject;
            _child.SetActive(false);
        }

        private void Update() {
            transform.Rotate(Vector3.up, 1);
            if (_selected.Count > 0) {
                Vec2 pos = null;
                foreach (var go in _selected) {
                    if (_selected.All(u => u.GetComponent<KRMovement>() != null
                        && u.GetComponent<KRTransform>().PlayerID == NetworkAPI.PlayerId)) {
                        Vec2 target = go.GetComponent<KRMovement>().Target;
                        if (pos == null && target != null) {
                            pos = target;
                        }
                        if (target != null && pos != target) return;
                    }
                    if (_selected.All(u => u.GetComponent<KRSpawn>() != null 
                        && u.GetComponent<KRTransform>().PlayerID == NetworkAPI.PlayerId)) {
                        Vec2 target = go.GetComponent<KRSpawn>().RallyPoint;
                        if (pos == null && target != null) {
                            pos = target;
                        }
                        if (target != null && pos != target) return;
                    }
                }
                if (pos != null) {
                    transform.position = new Vector3(pos.X, transform.position.y, pos.Y).Adjusted();
                    _child.SetActive(true);
                } else {
                    _child.SetActive(false);
                }
            } else {
                if (_child.activeInHierarchy) {
                    _child.SetActive(false);
                }
            }
        }

        void OnSelection(int playerID, IList<GameObject> selectedObj) {
            if (playerID == NetworkAPI.PlayerId) {
                _selected = selectedObj;
            }
        }

        void OnLeftClickUp(Vector3 mousePosition) {
            _selected.Clear();
        }
    }
}
