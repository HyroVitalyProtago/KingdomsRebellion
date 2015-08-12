using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.Core.Interfaces;

namespace KingdomsRebellion.AI {

	public static class Pathfinding<T> where T : IPos {

		static Heap<AbstractNode<T>> openSet = new Heap<AbstractNode<T>>(64);
		static IList<AbstractNode<T>> nodes = new List<AbstractNode<T>>();

		static void Reset() {
			foreach (AbstractNode<T> n in nodes) {
				n.Reset();
			}
			nodes.Clear();
		}

		public static IEnumerable<AbstractNode<T>> FindPath(AbstractNode<T> startNode, AbstractNode<T> targetNode) {
			openSet.Clear();
			openSet.Push(startNode);
			nodes.Add(startNode);
			startNode.Open();
			startNode.PathCost = 0;
			
			while (!openSet.IsEmpty()) {
				AbstractNode<T> currentNode = openSet.Pop();

				if (currentNode == targetNode) {
					return RetracePath(startNode, currentNode);
				}
					
				currentNode.Close();

				foreach (AbstractNode<T> neighbour in currentNode.Neighbours()) {
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
							openSet.Push(neighbour);
							nodes.Add(neighbour);
						}
					}
				}
			}

			Reset();
			return openSet;
		}
			
		static IEnumerable<AbstractNode<T>> RetracePath(AbstractNode<T> startNode, AbstractNode<T> endNode) {
			IList<AbstractNode<T>> path = new List<AbstractNode<T>>();
			AbstractNode<T> currentNode = endNode;

			while (currentNode != startNode) {
				path.Add(currentNode);
				currentNode = currentNode.Parent;
			}

			Reset();
			return path.Reverse();
		}

	}
}