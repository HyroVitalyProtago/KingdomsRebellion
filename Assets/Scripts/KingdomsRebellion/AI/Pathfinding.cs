using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.Core.Interfaces;

namespace KingdomsRebellion.AI {
	public static class Pathfinding<T> where T : IPos {
		readonly static Heap<AbstractNode<T>> OpenSet = new Heap<AbstractNode<T>>(64);
		readonly static IList<AbstractNode<T>> Nodes = new List<AbstractNode<T>>();

		public static IEnumerable<AbstractNode<T>> FindPath(AbstractNode<T> startNode, AbstractNode<T> targetNode) {
			OpenSet.Clear();
			OpenSet.Push(startNode);
			Nodes.Add(startNode);
			startNode.Open();
			startNode.PathCost = 0;
			
			while (!OpenSet.IsEmpty()) {
				AbstractNode<T> currentNode = OpenSet.Pop();

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
							OpenSet.Push(neighbour);
							Nodes.Add(neighbour);
						}
					}
				}
			}

			Reset();
			return OpenSet;
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

		static void Reset() {
			foreach (AbstractNode<T> n in Nodes) {
				n.Reset();
			}
			Nodes.Clear();
		}
	}
}