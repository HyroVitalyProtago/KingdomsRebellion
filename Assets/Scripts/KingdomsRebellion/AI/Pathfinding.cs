using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.AI {
	public static class Pathfinding {
		readonly static Heap<AbstractNode> OpenSet = new Heap<AbstractNode>(64);
		readonly static IList<AbstractNode> Nodes = new List<AbstractNode>();
		readonly static IList<Vec2> Empty = new List<Vec2>();

		public static IEnumerable<Vec2> FindPath(AbstractNode startNode, AbstractNode targetNode) {
			OpenSet.Clear();
			OpenSet.Push(startNode);
			Nodes.Add(startNode);
			startNode.Open();
			startNode.PathCost = 0;
			
			while (!OpenSet.IsEmpty()) {
				AbstractNode currentNode = OpenSet.Pop();

				if (currentNode == targetNode) {
					return RetracePath(startNode, currentNode);
				}
					
				currentNode.Close();

				foreach (AbstractNode neighbour in currentNode.Neighbours()) {
					if (!neighbour.IsFree() || neighbour.IsClosed()) {
						if (neighbour == targetNode) { // abort if path can't join target
							return RetracePath(startNode, currentNode);
						}
						continue;
					}
					
					int newMovementCostToNeighbour = currentNode.PathCost + currentNode.GetDistance(neighbour);
					if (!neighbour.IsOpened() || newMovementCostToNeighbour < neighbour.PathCost) {
						neighbour.PathCost = newMovementCostToNeighbour;
						neighbour.EstimatedCost = neighbour.GetDistance(targetNode);
						neighbour.Parent = currentNode;

						if (!neighbour.IsOpened()) {
							neighbour.Open();
							OpenSet.Push(neighbour);
							Nodes.Add(neighbour);
						}
					}
				}
			}

			Reset();
			return Empty;
		}

		static IEnumerable<Vec2> RetracePath(AbstractNode startNode, AbstractNode endNode) {
			IList<Vec2> path = new List<Vec2>();
			AbstractNode currentNode = endNode;

			while (currentNode != startNode) {
				path.Add(currentNode.Pos);
				currentNode = currentNode.Parent;
			}
			path.Add(currentNode.Pos);

			Reset();
			return path.Reverse();
		}

		static void Reset() {
			foreach (AbstractNode n in Nodes) {
				n.Reset();
			}
			Nodes.Clear();
		}
	}
}