using System;
using System.Collections.Generic;
using _project.scripts.grid;
using UnityEngine;

//This is an explanation of the algorithm A* : 
// https://www.geeksforgeeks.org/a-search-algorithm/

//TODO : - Better gCost assignation based one the tile speed (sand, dirt, beton, ...) => CalcultateGCost()

namespace _project.scripts.pathfinding
{
    public static class AStar
    {
        private static Vector2 gridSize;
        
        private class Node
        {
            public Vector2Int position;
            public Node parent;
            public int gCost;
            public int hCost;
            
            public int fCost => gCost + hCost;
            public bool isObstacle;

            public Node(Vector2Int position, Node parent, int gCost, int hCost, bool isObstacle)
            {
                this.position = position;
                this.parent = parent;
                this.gCost = gCost;
                this.hCost = hCost;
                this.isObstacle = isObstacle;
            }
        }

        public static List<Vector2> FindPath(Vector2 from, Vector2 to, Vector2 gridCellSize,
            System.Func<Vector2, bool> isObstacle)
        {
            gridSize = gridCellSize;

            Vector2Int fromGridPos = GridUtils.WorldToGrid(from);
            Vector2Int toGridPos = GridUtils.WorldToGrid(to);

            List<Node> openList = new List<Node>();
            HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

            Node fromNode = new Node(fromGridPos, null, 0, CalculateHCost(fromGridPos, toGridPos), false);
            openList.Add(fromNode);
            
            while (openList.Count > 0)
            {
                Node currentNode = GetLowestFCostNode(openList);

                openList.Remove(currentNode);
                closedSet.Add(currentNode.position);

                if (currentNode.position == toGridPos)
                {
                    return GeneratePath(currentNode);
                }

                foreach (Vector2Int adjacentPosition in GetAdjacentPositions(currentNode.position))
                {
                    if (closedSet.Contains(adjacentPosition) || isObstacle(GridUtils.GridToWorld(adjacentPosition)))
                    {
                        continue;
                    }

                    int gCost = currentNode.gCost + CalculateGCost(currentNode.position, adjacentPosition);
                    int hCost = CalculateHCost(adjacentPosition, fromGridPos);

                    Node adjacentNode = new Node(adjacentPosition, currentNode, gCost, hCost, false);

                    int index = openList.FindIndex(node => node.position == adjacentNode.position);
                    if (index != -1)
                    {
                        if (gCost < openList[index].gCost)
                        {
                            openList[index].parent = currentNode;
                            openList[index].gCost = gCost;
                        }
                    }
                    else
                    {
                        openList.Add(adjacentNode);
                    }
                }
            }

            return null;
        }

        private static int CalculateGCost(Vector2Int currentPosition, Vector2Int adjacentPosition)
        {
            return Mathf.CeilToInt(Vector2Int.Distance(currentPosition, adjacentPosition));
        }

        private static Node GetLowestFCostNode(List<Node> nodes)
        {
            Node lowestCostNode = nodes[0];

            for (int i = 1; i < nodes.Count; i++)
            {
                if (nodes[i].fCost < lowestCostNode.fCost || (nodes[i].fCost == lowestCostNode.fCost && nodes[i].hCost < lowestCostNode.hCost))
                {
                    lowestCostNode = nodes[i];
                }
            }

            return lowestCostNode;
        }

        private static int CalculateHCost(Vector2Int current, Vector2Int target)
        {
            return Mathf.Abs(current.x - target.x) + Mathf.Abs(current.y - target.y);
        }

        private static List<Vector2> GeneratePath(Node endNode)
        {
            List<Vector2> path = new List<Vector2>();
            Node currentNode = endNode;

            while (currentNode != null)
            {
                path.Add(GridUtils.GridToWorld(currentNode.position));
                currentNode = currentNode.parent;
            }
            path.Reverse();
            return path;
        }
        
        private static List<Vector2Int> GetAdjacentPositions(Vector2Int position)
        {
            return new List<Vector2Int>()
            {
                position + Vector2Int.up,
                position + Vector2Int.right,
                position + Vector2Int.down,
                position + Vector2Int.left,
                position + Vector2Int.up + Vector2Int.right,
                position + Vector2Int.up + Vector2Int.left,
                position + Vector2Int.down + Vector2Int.right,
                position + Vector2Int.down + Vector2Int.left,
            };
        }
    }
}