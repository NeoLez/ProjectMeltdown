using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Root {
    public class MapPointsGen {
        public class Node
        {
            public int height;
            public int dist;
            public char line;
            public List<Node> InConnections = new();
            public List<Node> OutConnections = new();
            public Feature feature;
            

            public Node(int height, int dist)
            {
                this.height = height;
                this.dist = dist;
            }
            
            public bool CanConnectTo() {
                return feature == Feature.TUNNEL;
            }
        }

        public enum Feature {
            TUNNEL,
            TUNNEL_FORK_RIGHT,
            TUNNEL_FORK_LEFT,
            TUNNEL_JOIN,
            START,
            STATION,
            ABANDONED_STATION,
        }
        
        public class Map {
            private System.Random _random;
            public int height, width;
            public Node[,] nodes;

            /// <summary>
            /// Analyzes all nodes starting from the one at coordinates [<paramref name="startHeight"/>, <paramref name="startLength"/>] until a given <paramref name="maxDepth"/>>
            /// </summary>
            /// <returns>Nodes that satisfy the <paramref name="predicate"/> (Node nodeToAnalyze, int currentDepth) => valid/invalid as a list of tuples where the int value is the depth</returns>
            public List<ValueTuple<Node, int>> GetNodesThatMatch(int startHeight, int startLength, Func<Node, int, bool> predicate, int maxDepth) {
                if (startHeight < 0 || startLength < 0 || startHeight >= height || startLength >= width) return null;
                List<ValueTuple<Node, int>> results = new();
                Queue<Node> nodesToAnalyzeCurrentLevel = new();
                Queue<Node> nodesToAnalyzeNextLevel = new();
                HashSet<Node> visited = new();
                var startingNode = nodes[startHeight, startLength];
                nodesToAnalyzeCurrentLevel.Enqueue(startingNode);
                visited.Add(startingNode);
                
                int depth = 0;
                while (depth <= maxDepth) {
                    var node = nodesToAnalyzeCurrentLevel.Dequeue();
                    if(predicate(node, depth))
                        results.Add((node, depth));
                    
                    foreach (var outNode in node.OutConnections) {
                        if (depth < maxDepth && !visited.Contains(outNode)) {
                            nodesToAnalyzeNextLevel.Enqueue(outNode);
                            visited.Add(outNode);
                        }
                    }

                    if (nodesToAnalyzeCurrentLevel.Count == 0) {
                        if (nodesToAnalyzeNextLevel.Count == 0) break;
                        (nodesToAnalyzeCurrentLevel, nodesToAnalyzeNextLevel) = (nodesToAnalyzeNextLevel, nodesToAnalyzeCurrentLevel);
                        depth++;
                    }
                }

                return results;
            }

            public Feature GetFeature() {
                float chance = (float)_random.NextDouble();
            
                if (chance <= 0.6f) {
                    return Feature.TUNNEL;
                }
                if (chance <= 0.8f) 
                {
                    return Feature.ABANDONED_STATION;
                }
                else { return Feature.STATION; }
                
            }
            
            public Map(int height, int width, int seed) {
                _random = new System.Random(seed);
                this.height = height;
                this.width = width;
                nodes = new Node[height, width];

                for (int x = 0; x < height; x++) {
                    Node prevNode = null;
                    for (int y = 0; y < width; y++) {
                        nodes[x, y] = new Node(x, y);
                        if (y == 0) {
                            nodes[x, y].feature = Feature.START;
                        }
                        else {
                            nodes[x, y].feature = GetFeature();
                        }
                        
                        if (prevNode != null)
                        {
                            var tunnelNode = new Node(prevNode.height, y);
                            tunnelNode.feature = Feature.TUNNEL;
                            
                            prevNode.OutConnections.Add(tunnelNode);
                            tunnelNode.InConnections.Add(prevNode);
                            
                            tunnelNode.OutConnections.Add(nodes[x, y]);
                            nodes[x, y].InConnections.Add(tunnelNode);
                        }
                        prevNode = nodes[x, y];
                    }
                }
                
                float connectionChance = 1f;
                for (int y = 1; y < width - 1; y++) {
                    for (int x = 0; x < height; x++) {
                        if (!nodes[x, y].CanConnectTo()) {
                            continue;
                        }
                        
                        if (_random.NextDouble() <= connectionChance) {
                            int direction;
                            int targetX;
                            int targetY = y + 1;
                            
                            if (x == 0) {
                                direction = 1;
                            }else if (x == height - 1) {
                                direction = -1;
                            }
                            else {
                                direction = _random.Next(0, 2);
                                if (direction == 0) {
                                    direction = -1;
                                }
                            }
                            targetX = x + direction;

                            if (!nodes[targetX, targetY].CanConnectTo()) {
                                continue;
                            }
                            
                            var tunnelNode = new Node(x, y);
                            tunnelNode.feature = Feature.TUNNEL;
                            
                            tunnelNode.InConnections.Add(nodes[x, y]);
                            nodes[x, y].OutConnections.Add(tunnelNode);
                            
                            tunnelNode.OutConnections.Add(nodes[targetX, targetY]);
                            nodes[targetX, targetY].InConnections.Add(tunnelNode);
                            
                            nodes[x, y].feature = nodes[x,y].height > nodes[targetX, targetY].height ? Feature.TUNNEL_FORK_RIGHT : Feature.TUNNEL_FORK_LEFT;
                            nodes[targetX, targetY].feature = Feature.TUNNEL_JOIN;

                        }
                    }
                }
                
                for (int y = 1; y < width - 1; y++) {
                    for (int x = 0; x < height; x++)
                    {
                        while (nodes[x, y].feature == Feature.TUNNEL)
                        {
                            nodes[x, y].feature = GetFeature();
                        }
                    }
                }
            }
            
            

            public override string ToString() {
                StringBuilder builder = new();

                for (int x = 0; x < height; x++) {
                    for (int y = 0; y < width; y++) {
                        builder.Append($"[{x}.{y} {nodes[x, y].feature.ToString()} ");
                        
                        builder.Append(nodes[x, y].OutConnections.Count);
                        
                        builder.Append("]");
                    }

                    builder.Append("\n");
                }
                return builder.ToString();
            }
        }
    }
}