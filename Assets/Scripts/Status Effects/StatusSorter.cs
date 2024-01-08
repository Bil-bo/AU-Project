using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple class for organising status effects
public class StatusSorter : MonoBehaviour
{

    private float CanvasWidth; 
    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        CanvasWidth = GetComponent<RectTransform>().rect.width;
    }
    public void OrganiseStatuses()
    { 
        float x = 0f;
        float y = 0f;

        foreach (Transform child  in transform)
        {
            RectTransform rectTransform = child.GetComponent<RectTransform>();

            if (x + rectTransform.rect.width > CanvasWidth)
            {
                // Move down to the next row
                x = 0f;
                y -= rectTransform.rect.height;
            }

            rectTransform.localPosition = new Vector2(x, y);

            x += rectTransform.rect.width;
        }




    }

}
