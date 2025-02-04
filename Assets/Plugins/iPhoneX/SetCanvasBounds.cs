using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SetCanvasBounds : MonoBehaviour
{
    private Rect GetSafeArea()
    {
#if UNITY_IOS
        return Screen.safeArea;
#elif UNITY_EDITOR
        float x, y, w, h;
        x = 0;
        y = 0;
        w = Screen.width;
        h = Screen.height;
        if (Screen.width == 1125 && Screen.height == 2436)
        {
            y = 102;
            h = 2202;
            return new Rect(x, y, w, h);
        }
        else if (Screen.width == 2436 && Screen.height == 1125)
        {
            x = 102;
            w = 2202;
            return new Rect(x, y, w, h);
        }
        return Rect.zero;
#else
        return Rect.zero;
#endif
    }

    public RectTransform canvas;
    public RectTransform panel;
    Rect lastSafeArea = new Rect(0, 0, 0, 0);

    // Use this for initialization
    void Start()
    {

    }

    void ApplySafeArea(Rect area)
    {
        var anchorMin = area.position;
        var anchorMax = area.position + area.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        panel.anchorMin = anchorMin;
        panel.anchorMax = anchorMax;

        lastSafeArea = area;
    }

    // Update is called once per frame
    void Update()
    {
        Rect safeArea = GetSafeArea(); // or Screen.safeArea if you use a version of Unity that supports it

        if (safeArea != lastSafeArea)
            ApplySafeArea(safeArea);
    }
}
