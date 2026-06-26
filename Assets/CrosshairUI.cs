using Root.Controller;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    private Image _rawImage;

    private void Awake()
    {
        _rawImage = GetComponent<Image>();
    }

    private void Update()
    {
        _rawImage.enabled = MouseHandler.ShowCrosshair;
    }
}