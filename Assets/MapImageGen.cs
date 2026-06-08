using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Root
{
    public class MapImageGen : MonoBehaviour {
        [SerializeField] private RenderTexture renderTexture;
        [SerializeField] private GameObject dot;
        [SerializeField] private GameObject line;
        [SerializeField] private float dotRadius;
        [SerializeField] private Vector2 size;

        [SerializeField] private List<NodeImage> images;
        [SerializeField] private Texture arrowTexture; 
        private float xSpacing;
        private float ySpacing;
        [SerializeField] private bool calc;

        [Serializable]
        private class NodeImage {
            public MapPointsGen.Feature feature;
            public Texture Texture;
        }

        private Texture GetSprite(MapPointsGen.Feature feature) {
            foreach (var image in images) {
                if (image.feature == feature) {
                    return image.Texture;
                }
            }

            return null;
        }
        
        private void Update() {
            if (calc) {
                calc = false;
                Start();
            }
        }

        private void Start() {
            var map = GameManager.MapGeneration.map;
            
            xSpacing =  size.x / (map.width + 1);
            ySpacing =  size.y / (map.height + 1);
            
            for (int y = 0; y < map.height; y++) {
                for (int x = 0; x < map.width; x++) {
                    var obj = Instantiate(dot, transform);
                    obj.name = y + "-" + x;
                    obj.transform.position = GetCoords(y, x).Swizzle_xy0() +  Vector3.forward * transform.position.z;
                    obj.transform.localScale = Vector3.one * dotRadius;
                    var r = obj.GetComponent<Renderer>();
                    var mat = new Material(r.material);
                    r.material = mat;
                    mat.mainTexture = GetSprite(map.nodes[y, x].feature);

                    foreach (var outNode in map.nodes[y,x].OutConnections) {
                        if (outNode.feature != MapPointsGen.Feature.TUNNEL) {
                            Debug.LogWarning("Unexpected feature type");
                            continue;
                        }
                        DrawLine(y, x, outNode.OutConnections[0].height, outNode.OutConnections[0].dist);
                    }
                }
            }

            var currentNode = GameManager.MapGeneration.GetCurrentNode();
            var arr = Instantiate(dot, transform);
            arr.transform.position = GetCoords(currentNode.height, 0).Swizzle_xy0() +  Vector3.forward * transform.position.z;
            arr.transform.position -= dotRadius * Vector3.right;
            arr.transform.localScale = Vector3.one * dotRadius;
            var rend = arr.GetComponent<Renderer>();
            var mate = new Material(rend.material);
            rend.material = mate;
            mate.mainTexture = arrowTexture;
            
            OneShotRenderSystem.Instance.Render(renderTexture);
        }

        private Vector2 GetCoords(int y, int x) {
            return new Vector2(x * xSpacing + xSpacing - size.x/2, y * ySpacing + ySpacing - size.y/2);
        }

        private void DrawLine(int y1, int x1, int y2, int x2) {
            Vector2 p1 = GetCoords(y1, x1);
            Vector2 p2 = GetCoords(y2, x2);
            var obj = Instantiate(line, transform);
            obj.name = $"({y1}-{x1})({y2}-{x2})";
            obj.transform.position = ((p1 + p2) / 2).Swizzle_xy0() + Vector3.forward  *transform.position.z;
            obj.transform.right = (p2 - p1);
            obj.transform.localScale = new Vector3((p2 - p1).magnitude - dotRadius, obj.transform.localScale.y, obj.transform.localScale.z);
        }
    }
}
