using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// For assisting in targeting a character
public class ArrowPointer : MonoBehaviour
{

    [SerializeField]
    private Image StartPoint;

    [SerializeField]
    private Image ControlPoint;

    [SerializeField]
    private List<Image> MiddleSegments = new List<Image>();

    [SerializeField]
    private Image EndPoint;


    private RectTransform CanvasRect;

    // Start is called before the first frame update
    void Start()
    {
        CanvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        StartPoint.gameObject.SetActive(false);

        ControlPoint.gameObject.SetActive(false);
        ShowArrow(false);

    }

    // Set to the card you want to select
    public void SetStartPos(RectTransform cardPos) 
    {
        Vector2 targetPos = cardPos.position;
        Vector2 targetSize = cardPos.sizeDelta;

        targetPos.y += targetSize.y / 2f;

        StartPoint.rectTransform.anchoredPosition = FindLocalPoint(targetPos);
        ControlPoint.rectTransform.anchoredPosition = StartPoint.rectTransform.anchoredPosition;
    }


    // Make pointer visible
    public void ShowArrow(bool show)
    {
        EndPoint.gameObject.SetActive(show);

        foreach (Image middle in MiddleSegments)
        {
            middle.gameObject.SetActive(show);

        }
    }


    // Move arrow
    public void DrawArrow(Vector2 mousePos)
    { 
        
        EndPoint.rectTransform.anchoredPosition = FindLocalPoint(mousePos);
        ControlPoint.rectTransform.anchoredPosition = new Vector2(StartPoint.rectTransform.anchoredPosition.x, Mathf.Clamp(EndPoint.rectTransform.anchoredPosition.y, StartPoint.rectTransform.anchoredPosition.y, 0f));
        DrawQuadraticBezierCurve(StartPoint.rectTransform.anchoredPosition, ControlPoint.rectTransform.anchoredPosition, EndPoint.rectTransform.anchoredPosition);
    }



    //Chat GPT code
    // Basically just creating a smooth curve of points on a line
    private void DrawQuadraticBezierCurve(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        for (int i = 0; i < MiddleSegments.Count; i++)
        {
            float t = (i + 1) / (float)(MiddleSegments.Count + 1);
            Vector2 lerpedPosition = QuadraticBezierLerp(p0, p1, p2, t);
            MiddleSegments[i].rectTransform.anchoredPosition = lerpedPosition;
        }
    }

    private Vector2 QuadraticBezierLerp(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector2 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }

    private Vector2 FindLocalPoint(Vector2 initPos)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRect, initPos, null, out Vector2 localPoint)) { return localPoint; }
        return Vector2.zero;
    }


}
