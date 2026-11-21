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
            SHOP,
            CHARGER,
            DELIVERY,
        }
        
        public class Map {
            private System.Random _random;
            public int height { get; private set; } 
            public int width { get; private set; }
            [SerializeReference] private Node[,] nodes;
            private bool[,] _accessible;
            private Dictionary<int, int> firstDistAccesible = new();
            private Queue<int> nextLineToProcess = new();
            private float forkFrequency = 4.2f;
            public Vector2Int
                StartPosition;

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
            
            public Map(int height, int width, int seed) {
                _random = new Random(seed);
                this.height = height;
                this.width = width;
                nodes = new Node[height, width];
                _accessible = new bool[height, width];
                
                for (int i = 0; i < height; i++)
                    firstDistAccesible[i] = int.MaxValue;
                
                StartPosition = new Vector2Int(new Random(GameManager.seed).Next(0, height), 0);

                //Initializing nodes and connecting them with tunnels
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

                //Creating first fork right after the start to maximize player options
                SetLineAsAccessible(StartPosition.x, 0);
                int direction = StartPosition.x == 0 ? 1 : StartPosition.x == this.height - 1 ? -1 : (_random.Next(0, 2) == 0 ? -1 : 1);

                int targetY = StartPosition.x + direction;
                int targetX = 2;
                SetLineAsAccessible(targetY, targetX);
                ConnectFork(nodes[StartPosition.x, 1], nodes[targetY, targetX]);
                firstDistAccesible[StartPosition.x] = 2;
                nextLineToProcess.Enqueue(StartPosition.x);
                firstDistAccesible[targetY] = 2;
                nextLineToProcess.Enqueue(targetY);

                //Process line queue to generate forks
                while(nextLineToProcess.Count > 0) {
                    int lineToProcess = nextLineToProcess.Dequeue();
                    int forkAmount = (int)((width - firstDistAccesible[lineToProcess]) / forkFrequency);
                    CreateForks(forkAmount, lineToProcess, firstDistAccesible[lineToProcess] + 1);
                }
                

                //Place fixed features
                PlaceFeature(Feature.DELIVERY);
                PlaceFeature(Feature.DELIVERY);
                PlaceFeature(Feature.SHOP);
                PlaceFeature(Feature.SHOP);
                PlaceFeature(Feature.CHARGER);
                PlaceFeature(Feature.CHARGER);

                for (int y = 0; y < height; y++) {
                    for (int x = 1; x < width - 1; x++) {
                        var node = nodes[y, x];
                        if(node.feature == Feature.UNINITIALIZED)
                            nodes[y, x].feature = Feature.ABANDONED_STATION;
                    }
                }
            }

            public void PlaceFeature(Feature feature)
            {
                int attempts = 0;
                while (attempts < 1000)
                {
                    var node = nodes[_random.Next(0, height), _random.Next(1, width - 1)];
                    attempts++;
                    if(!_accessible[node.height, node.dist] || node.feature != Feature.UNINITIALIZED ||
                       IsFeature(nodes[node.height, node.dist + 1].feature) || IsFeature(nodes[node.height, node.dist - 1].feature)) continue;
                    node.feature = feature;
                    break;
                }
            }

            private bool IsFeature(Feature feature)
            {
                return feature == Feature.SHOP || feature == Feature.CHARGER || feature == Feature.DELIVERY;
            }

            private void CreateForks(int forkAmount, int height, int startDist)
            {
                int forksMade = 0;
                for (int attempt = 0; attempt < 50 && forksMade < forkAmount; attempt++) {
                    int x = _random.Next(startDist, width - 3);
                    if (!nodes[height, x].CanConnectTo()) continue;

                    //There should be no other forks at a distance of 1 from the location of the new one
                    var left = nodes[height, x - 1].feature;
                    var right = nodes[height, x + 1].feature;
                    if (left == Feature.TUNNEL_FORK_LEFT || left == Feature.TUNNEL_FORK_RIGHT) continue;
                    if (right == Feature.TUNNEL_FORK_LEFT || right == Feature.TUNNEL_FORK_RIGHT) continue;

                    int targetX = x + 1;
                    int direction;
                    if (height == 0) direction = 1;
                    else if (height == this.height - 1) direction = -1;
                    else
                    {
                        var acc1 = _accessible[height - 1, targetX];
                        var acc2 = _accessible[height + 1, targetX];
                        //If both directions are accessible or both ar inaccessible, just generate one at random, otherwise always pick the inaccessible one so the player can go to all lines
                        if (acc1 == acc2) direction = _random.Next(0, 2) == 0 ? -1 : 1;
                        else if (!acc1) direction = -1;
                        else direction = 1;
                    }
                    
                    int targetY = height + direction;
                    
                    if (!nodes[targetY, targetX].CanConnectTo()) continue;

                    ConnectFork(nodes[height, x], nodes[targetY, targetX]);
                    
                    SetLineAsAccessible(targetY, targetX);

                    if (!firstDistAccesible.TryGetValue(targetY, out var v) || v > targetX)
                    {
                        firstDistAccesible[targetY] = targetX;
                        if(nextLineToProcess.Count == 0 || nextLineToProcess.Peek() != targetY) nextLineToProcess.Enqueue(targetY);
                    }

                    forksMade++;
                }
                
            }

            private void SetLineAsAccessible(int height, int width)
            {
                for (int x = width; x < this.width; x++)
                    _accessible[height, x] = true;
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
                        if(_accessible[y,x])builder.Append("V");
                        
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