using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Grid;

namespace KingdomsRebellion.AI {
	public class Pathfinding {

		static AbstractGrid _grid;

		static Pathfinding() {
			_grid = KRFacade.GetGrid();
		}

		public static IEnumerable<Node> FindPath(Vec2 startPos, Vec2 targetPos) {
			Node startNode = _grid.NodeOf(startPos);
			Node targetNode = _grid.NodeOf(targetPos);
			
			IList<Node> openSet = new List<Node>();
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
			
			while (openSet.Count > 0) {
				Node currentNode = openSet.Min();
				
				if (currentNode == targetNode) {
					return RetracePath(startNode,targetNode);
				}

				openSet.Remove(currentNode);
				closedSet.Add(currentNode);
				
				foreach (Node neighbour in _grid.GetNeighbours(currentNode)) {
					if (!neighbour.Walkable || closedSet.Contains(neighbour)) {
						continue;
					}
					
					int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
					if (!openSet.Contains(neighbour) || newMovementCostToNeighbour < neighbour.GCost) {
						neighbour.GCost = newMovementCostToNeighbour;
						neighbour.HCost = GetDistance(neighbour, targetNode);
						neighbour.Parent = currentNode;
						
						if (!openSet.Contains(neighbour)) { openSet.Add(neighbour); }
					}
				}
			}

			return null; // empty IEnumerable
		}
		
		static IEnumerable<Node> RetracePath(Node startNode, Node endNode) {
			IList<Node> path = new List<Node>();
			Node currentNode = endNode;
			
			while (currentNode != startNode) {
				path.Add(currentNode);
				currentNode = currentNode.Parent;
			}

			return path.Reverse();
		}

		// D = D2 = 1 : Chebyshev distance
		static int D = 1; // D is the cost of moving horizontally or vertically
		static int D2 = 1; // D2 is the cost of moving diagonally

		// @see http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html
		static int GetDistance(Node node, Node goal) {
			int dx = Mathf.Abs(node.Pos.X - goal.Pos.X);
			int dy = Mathf.Abs(node.Pos.Y - goal.Pos.Y);

			if (dx > dy) { return 14*dy + 10*(dx-dy); }
			return 14*dx + 10*(dy-dx);

//			return D * (dx + dy) + (D2 - 2 * D) * Mathf.Min(dx, dy);
		}

	}
}