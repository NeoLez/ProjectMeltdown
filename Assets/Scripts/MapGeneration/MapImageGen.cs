using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Root
{
    public class MapImageGen : MonoBehaviour {
        [SerializeField] private RenderTexture renderTexture;
        [SerializeField] private GameObject dot;
        [SerializeField] private GameObject line;
        [SerializeField] private Canvas text;
        [SerializeField] private float distanceTextVerticalOffset;
        [SerializeField] private float lineTextHorizontalOffset;
        [SerializeField] private float dotRadius;
        [SerializeField] private Vector2 size;
        [SerializeField] private List<Color> lineColors;

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
                    mat.color = lineColors[y];
                    r.material = mat;
                    mat.mainTexture = GetSprite(map.GetNode(y, x).feature);

                    foreach (var outNode in map.GetNode(y, x).OutConnections) {
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
            mate.color = Color.black;
            rend.material = mate;
            mate.mainTexture = arrowTexture;

            DrawText(map);
            
            OneShotRenderSystem.Instance.Render(renderTexture);
            gameObject.SetActive(false);
        }

        private Vector2 GetCoords(int y, int x) {
            return new Vector2(x * xSpacing + xSpacing - size.x/2, y * ySpacing + ySpacing - size.y/2);
        }

        private void DrawLine(int y1, int x1, int y2, int x2) {
            Vector2 p1 = GetCoords(y1, x1);
            Vector2 p2 = GetCoords(y2, x2);
            var obj = Instantiate(line, transform);
            var rend = obj.GetComponent<Renderer>();
            var mate = new Material(rend.material);
            mate.color = lineColors[y1];
            rend.material = mate;
            obj.name = $"({y1}-{x1})({y2}-{x2})";
            obj.transform.position = ((p1 + p2) / 2).Swizzle_xy0() + Vector3.forward  *transform.position.z;
            obj.transform.right = (p2 - p1);
            obj.transform.localScale = new Vector3((p2 - p1).magnitude - dotRadius, obj.transform.localScale.y, obj.transform.localScale.z);
        }

        private void DrawText(MapPointsGen.Map map) {
            for (int x = 0; x < map.width; x++) {
                var obj = Instantiate(text, transform);
                obj.GetComponentInChildren<TMP_Text>().text = $"{x}";
                obj.transform.position = GetCoords(0, x).Swizzle_xy0() + Vector3.forward * transform.position.z + Vector3.up * distanceTextVerticalOffset;
            }
            
            for (int y = 0; y < map.height; y++) {
                var obj = Instantiate(text, transform);
                obj.GetComponentInChildren<TMP_Text>().text = $"{map.GetNode(y, 0).line}";
                obj.transform.position = GetCoords(y, map.width - 1).Swizzle_xy0() + Vector3.forward * transform.position.z + Vector3.right * lineTextHorizontalOffset;
            }
        }
    }
}
