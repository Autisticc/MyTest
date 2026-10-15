using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSafeArea : MonoBehaviour
{
    public Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        var safeArea = Screen.safeArea;
        Debug.Log(safeArea);
        Debug.Log(Screen.width + " "  + Screen.height);
        
        var canvasRect = canvas.GetComponent<RectTransform>();
        var screenScale = canvasRect.rect.width / Screen.width;
        var max = Mathf.Max(safeArea.x, Screen.width - (safeArea.x + safeArea.width)) * screenScale;

        var rect = this.GetComponent<RectTransform>();
        rect.offsetMin = new Vector2(max,rect.offsetMin.y);
        rect.offsetMax = new Vector2(-max,rect.offsetMax.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
