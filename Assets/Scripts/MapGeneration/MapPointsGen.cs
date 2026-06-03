using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Root {
    public class MapPointsGen {
        public class Node
        {
            public int height;
            public char line;
            public List<Node> InConnections = new();
            public List<Node> OutConnections = new();
            public Feature feature;

            public Node(int height)
            {
                this.height = height;
            }
            
            public bool CanConnectTo() {
                return feature == Feature.TUNNEL;
            }
        }

        public enum Feature {
            TUNNEL,
            TUNNEL_FORK,
            TUNNEL_JOIN,
            START,
            STATION,
            ABANDONED_STATION,
        }

        public static Feature GetFeature() {
            float chance = Random.value;
            if (chance <= 0.5f) {
                return  Feature.TUNNEL;
            }

            if (chance <= 0.8f) {
                return Feature.ABANDONED_STATION;
            }

            return Feature.STATION;
        }
        
        public class Map {
            public int height, width;
            public Node[,] nodes;

            public Map(int height, int width) {
                this.height = height;
                this.width = width;
                nodes = new Node[height, width];

                for (int x = 0; x < height; x++) {
                    Node prevNode = null;
                    for (int y = 0; y < width; y++) {
                        nodes[x, y] = new Node(x);
                        if (y == 0) {
                            nodes[x, y].feature = Feature.START;
                        }
                        else {
                            nodes[x, y].feature = GetFeature();
                        }
                        
                        if (prevNode != null)
                        {
                            var tunnelNode = new Node(prevNode.height);
                            tunnelNode.feature = Feature.TUNNEL;
                            
                            prevNode.OutConnections.Add(tunnelNode);
                            tunnelNode.InConnections.Add(prevNode);
                            
                            tunnelNode.OutConnections.Add(nodes[x, y]);
                            nodes[x, y].InConnections.Add(tunnelNode);
                        }
                        prevNode = nodes[x, y];
                    }
                }
                
                float connectionChance = 0.5f;
                for (int y = 1; y < width - 1; y++) {
                    for (int x = 0; x < height; x++) {
                        if (!nodes[x, y].CanConnectTo()) {
                            continue;
                        }
                        
                        if (Random.value <= connectionChance) {
                            int direction;
                            int targetX;
                            int targetY = y + 1;
                            
                            if (x == 0) {
                                direction = 1;
                            }else if (x == height - 1) {
                                direction = -1;
                            }
                            else {
                                direction = Random.Range(0, 2);
                                if (direction == 0) {
                                    direction = -1;
                                }
                            }
                            targetX = x + direction;

                            if (!nodes[targetX, targetY].CanConnectTo()) {
                                continue;
                            }
                            
                            var tunnelNode = new Node(x);
                            tunnelNode.feature = Feature.TUNNEL;
                            
                            tunnelNode.InConnections.Add(nodes[x, y]);
                            nodes[x, y].OutConnections.Add(tunnelNode);
                            
                            tunnelNode.OutConnections.Add(nodes[targetX, targetY]);
                            nodes[targetX, targetY].InConnections.Add(tunnelNode);
                            
                            nodes[x, y].feature = Feature.TUNNEL_FORK;
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