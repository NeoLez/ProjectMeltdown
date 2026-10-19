using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace Root {
    public class MapPointsGen {
        [Serializable] public class Node
        {
            public int height;
            public int dist;
            public char line;
            [NonSerialized] public List<Node> InConnections = new();
            [NonSerialized] public List<Node> OutConnections = new();
            public Feature feature;
            public Map Map;
            

            public Node(int height, int dist, Map map)
            {
                this.height = height;
                this.dist = dist;
                Map = map;
                line = (char)(height+65);//shouldn't have more than 27 lines :v
            }
            
            public bool CanConnectTo() {
                return feature == Feature.TUNNEL || feature == Feature.UNINITIALIZED;
            }
            
            /// <summary>
            /// Analyzes all accessible nodes until a given <paramref name="maxDepth"/>>
            /// </summary>
            /// <returns>Nodes that satisfy the <paramref name="predicate"/> (Node nodeToAnalyze, int currentDepth) => valid/invalid as a list of tuples where the int value is the depth. SORTED BY DEPTH</returns>
            public List<ValueTuple<Node, int>> GetNodesThatMatch(Func<Node, int, bool> predicate, int maxDepth) {
                List<ValueTuple<Node, int>> results = new();
                Queue<Node> nodesToAnalyzeCurrentLevel = new();
                Queue<Node> nodesToAnalyzeNextLevel = new();
                HashSet<Node> visited = new();
                nodesToAnalyzeCurrentLevel.Enqueue(this);
                visited.Add(this);
                
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

            public override string ToString() {
                return $"{line}{dist}:{feature.ToString()}";
            }
        }

        public enum Feature {
            UNINITIALIZED = 0,
            TUNNEL,
            TUNNEL_FORK_RIGHT,
            TUNNEL_FORK_LEFT,
            TUNNEL_JOIN,
            START,
            STATION,
            ABANDONED_STATION,
            END,
        }
        
        public class Map {
            private System.Random _random;
            public int height { get; private set; } 
            public int width { get; private set; }
            [SerializeReference] private Node[,] nodes;

            public Node GetNode(int y, int x) {
                return nodes[y, x];
            }
            
            /// <summary>
            /// Analyzes all nodes starting from the one at coordinates [<paramref name="startHeight"/>, <paramref name="startLength"/>] until a given <paramref name="maxDepth"/>>
            /// </summary>
            /// <returns>Nodes that satisfy the <paramref name="predicate"/> (Node nodeToAnalyze, int currentDepth) => valid/invalid as a list of tuples where the int value is the depth. SORTED BY DEPTH</returns>
            public List<ValueTuple<Node, int>> GetNodesThatMatch(int startHeight, int startLength, Func<Node, int, bool> predicate, int maxDepth) {
                if (startHeight < 0 || startLength < 0 || startHeight >= height || startLength >= width) return null;
                return nodes[startHeight, startLength].GetNodesThatMatch(predicate, maxDepth);
            }

            public Feature GetFeature() {
                float chance = (float)_random.NextDouble();
                
                if (chance <= 0.5f) 
                {
                    return Feature.ABANDONED_STATION;
                }
                else { return Feature.STATION; }
                
            }
            
            public Map(int height, int width, int seed) {
                _random = new Random(seed);
                this.height = height;
                this.width = width;
                nodes = new Node[height, width];

                for (int y = 0; y < height; y++) {
                    Node prevNode = null;
                    for (int x = 0; x < width; x++) {
                        nodes[y, x] = new Node(y, x, this);
                        if (x == 0) {
                            nodes[y, x].feature = Feature.START;
                        } else if (x == width - 1) {
                            nodes[y, x].feature = Feature.END;
                        }
                        if (prevNode != null)
                        {
                            ConnectWithTunnel(prevNode, nodes[y, x]);
                        }
                        prevNode = nodes[y, x];
                    }
                }

                int forkAmount = (_random.Next(1, 3) + _random.Next(1, 2)) / 2;
                for (int y = 0; y < height; y++) {
                    int forksMade = 0;
                    for (int attempt = 0; attempt < 50 && forksMade <= forkAmount; attempt++) {
                        int x = _random.Next(1, width - 3);
                        if (!nodes[y, x].CanConnectTo()) continue;

                        var left = nodes[y, x - 1].feature;
                        if (left == Feature.TUNNEL_FORK_LEFT || left == Feature.TUNNEL_FORK_RIGHT) continue;

                        int direction = y == 0 ? 1
                            : y == height - 1 ? -1
                            : (_random.Next(0, 2) == 0 ? -1 : 1);

                        int targetY = y + direction;
                        int targetX = x + 1;
                        if (!nodes[targetY, targetX].CanConnectTo()) continue;

                        ConnectFork(nodes[y, x], nodes[targetY, targetX]);
                        forksMade++;
                    }
                }

                for (int x = 1; x < width - 1; x++) {
                    for (int y = 0; y < height; y++) {
                        var node = nodes[y, x];
                        if(node.feature == Feature.UNINITIALIZED)
                            nodes[y, x].feature = GetFeature();
                    }
                }
            }
            
            private void ConnectWithTunnel(Node a, Node b) {
                var tunnelNode = new Node(a.height, a.dist, this);
                tunnelNode.feature = Feature.TUNNEL;
                            
                a.OutConnections.Add(tunnelNode);
                tunnelNode.InConnections.Add(a);
                            
                tunnelNode.OutConnections.Add(b);
                b.InConnections.Add(tunnelNode);
            }
            
            private void ConnectFork(Node a, Node b) {
                var tunnelNode = new Node(a.height, a.dist, this);
                tunnelNode.feature = Feature.TUNNEL;
                            
                tunnelNode.InConnections.Add(a);
                a.OutConnections.Add(tunnelNode);
                            
                tunnelNode.OutConnections.Add(b);
                b.InConnections.Add(tunnelNode);
                            
                a.feature = a.height > b.height ? Feature.TUNNEL_FORK_RIGHT : Feature.TUNNEL_FORK_LEFT;
                b.feature = Feature.TUNNEL_JOIN;
            }
            

            public override string ToString() {
                StringBuilder builder = new();

                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        builder.Append($"[{y}.{x} {nodes[y, x].feature.ToString()} ");
                        
                        builder.Append(nodes[y, x].OutConnections.Count);
                        
                        builder.Append("]");
                    }

                    builder.Append("\n");
                }
                return builder.ToString();
            }
        }
    }
}