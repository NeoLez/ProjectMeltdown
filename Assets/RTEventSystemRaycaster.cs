using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RTEventSystemRaycaster : BaseRaycaster
{
    [SerializeField] private RawImage rtDisplay;
    [SerializeField] private Camera rtCamera;
    [SerializeField] private Vector2 rtSize = new(640, 360);

    public override Camera eventCamera => rtCamera;

    public override void Raycast(
        PointerEventData eventData,
        List<RaycastResult> resultAppendList)
    {
        if (rtCamera == null || rtDisplay == null)
            return;

        if (!RemapToRT(eventData.position, out Vector2 rtPos))
            return;

        Ray ray = rtCamera.ScreenPointToRay(rtPos);

        if (Physics.Raycast(ray, out RaycastHit hit3D, 100f))
        {
            resultAppendList.Add(new RaycastResult
            {
                gameObject = hit3D.collider.gameObject,
                module = this,
                distance = hit3D.distance,
                worldPosition = hit3D.point,
                worldNormal = hit3D.normal,
                screenPosition = eventData.position,
                index = resultAppendList.Count
            });
        }

        RaycastWorldSpaceCanvas(
            ray,
            eventData,
            resultAppendList
        );
    }

    private void RaycastWorldSpaceCanvas(
        Ray ray,
        PointerEventData eventData,
        List<RaycastResult> results)
    {
        foreach (var graphic in FindObjectsByType<Graphic>())
        {
            Canvas canvas = graphic.canvas;

            if (canvas == null)
                continue;

            if (canvas.renderMode != RenderMode.WorldSpace)
                continue;

            if (!graphic.raycastTarget)
                continue;

            RectTransform rectTransform = graphic.rectTransform;

            Plane plane = new Plane(
                rectTransform.forward,
                rectTransform.position
            );

            if (!plane.Raycast(ray, out float enter))
                continue;

            Vector3 worldHit = ray.GetPoint(enter);
            Vector2 localHit = rectTransform.InverseTransformPoint(worldHit);

            if (!rectTransform.rect.Contains(localHit))
                continue;

            results.Add(new RaycastResult
            {
                gameObject = graphic.gameObject,
                module = this,
                distance = enter,
                worldPosition = worldHit,
                screenPosition = eventData.position,
                index = results.Count,
                sortingLayer = canvas.sortingLayerID,
                sortingOrder = canvas.sortingOrder
            });
        }

        results.Sort(
            (a, b) => a.distance.CompareTo(b.distance)
        );
    }

    private bool RemapToRT(
        Vector2 screenPos,
        out Vector2 rtPos)
    {
        rtPos = Vector2.zero;

        RectTransform rect = rtDisplay.rectTransform;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            screenPos,
            null,
            out Vector2 localPoint))
        {
            return false;
        }

        Vector2 rectSize = rect.rect.size;

        if (localPoint.x < -rectSize.x * 0.5f ||
            localPoint.x > rectSize.x * 0.5f ||
            localPoint.y < -rectSize.y * 0.5f ||
            localPoint.y > rectSize.y * 0.5f)
        {
            return false;
        }

        Vector2 normalized = new Vector2(
            (localPoint.x + rectSize.x * 0.5f) / rectSize.x,
            (localPoint.y + rectSize.y * 0.5f) / rectSize.y
        );

        rtPos = new Vector2(
            normalized.x * rtSize.x,
            normalized.y * rtSize.y
        );

        return true;
    }
}