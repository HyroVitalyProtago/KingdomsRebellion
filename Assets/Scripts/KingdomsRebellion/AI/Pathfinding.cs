using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.Core.Interfaces;

namespace KingdomsRebellion.AI {

	public static class Pathfinding<T> where T : IPos {

		public static IEnumerable<AbstractNode<T>> FindPath(AbstractNode<T> startNode, AbstractNode<T> targetNode) {
			IList<AbstractNode<T>> openSet = new List<AbstractNode<T>>();
			HashSet<AbstractNode<T>> closedSet = new HashSet<AbstractNode<T>>();
			openSet.Add(startNode);
			startNode.PathCost = 0;
			
			while (openSet.Count > 0) {
				AbstractNode<T> currentNode = openSet.Min();

				if (currentNode == targetNode) {
					return RetracePath(startNode,currentNode);
				}

				openSet.Remove(currentNode);
				closedSet.Add(currentNode);

				foreach (AbstractNode<T> neighbour in currentNode.Neighbours()) {
					if (!neighbour.IsFree() || closedSet.Contains(neighbour)) {
						if (neighbour == targetNode) { // abort if path can't join target
							return RetracePath(startNode, currentNode);
						}
						continue;
					}
					
					int newMovementCostToNeighbour = currentNode.PathCost + currentNode.GetDistance(neighbour);
					if (!openSet.Contains(neighbour) || newMovementCostToNeighbour < neighbour.PathCost) {
						neighbour.PathCost = newMovementCostToNeighbour;
						neighbour.EstimatedCost = neighbour.GetDistance(targetNode);
						neighbour.Parent = currentNode;

						if (!openSet.Contains(neighbour)) { openSet.Add(neighbour); }
					}
				}
			}

			return openSet;
		}
			
		static IEnumerable<AbstractNode<T>> RetracePath(AbstractNode<T> startNode, AbstractNode<T> endNode) {
			IList<AbstractNode<T>> path = new List<AbstractNode<T>>();
			AbstractNode<T> currentNode = endNode;

			while (currentNode != startNode) {
				path.Add(currentNode);
				currentNode = currentNode.Parent;
			}

			return path.Reverse();
		}

	}
}