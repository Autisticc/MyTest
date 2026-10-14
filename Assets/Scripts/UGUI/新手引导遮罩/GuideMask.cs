using UnityEngine;
using UnityEngine.UI;

public class GuideMask : MaskableGraphic, ICanvasRaycastFilter
{
    public static GuideMask Self;
    private RectTransform _target;
    private Bounds _targetBounds;

    // 这个点击的位置是否无效，true无效，实现ICanvasRaycastFilter接口的方法
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (_target == null) return true;
        // RectangleContainsScreenPoint,判断这个点是否在_target上, 在就可以点不在就不能点。
        return !RectTransformUtility.RectangleContainsScreenPoint(_target, sp, eventCamera);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Play(RectTransform target)
    {
        gameObject.SetActive(true);
        target.gameObject.SetActive(true);
        _target = target;
        RefreshView();
    }

    public void Init()
    {
        Self = this;
        Close();
    }

    // 围出中间镂空的高亮区, 画出引导区域，重写MaskableGraphic的方法
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        toFill.Clear();
        
        var rect = rectTransform.rect;
        var min = (Vector2)_targetBounds.min;
        var max = (Vector2)_targetBounds.max;
        
        var left = rect.xMin;
        var right = rect.xMax;
        var top = rect.yMax;
        var bottom = rect.yMin;
        
        // 这是GuideMask的四个∠
        var outLt = new Vector2(left, top);
        var outRt = new Vector2(right, top);
        var outLb = new Vector2(left, bottom);
        var outRb = new Vector2(right, bottom);
        //这个targetArea的四个∠, 左上右上左下右下，top，bottom
        var inLt = new Vector2(min.x, max.y);
        var inRt = max;
        var inLb = min;
        var inRb = new Vector2(max.x, min.y);
        // var inLt = new Vector2(_targetMin.x, _targetMax.y);
        // var inRt = new Vector2(_targetMax.x, _targetMax.y);
        // var inLb = new Vector2(_targetMin.x, _targetMin.y);
        // var inRb = new Vector2(_targetMax.x, _targetMin.y);

        // 上、下、左、右四个矩形，围出中间镂空的高亮区，AddQuad画矩形的方法
        AddQuad(toFill, outLt, outRt, inRt, inLt);
        AddQuad(toFill, outLb, outRb, inRb, inLb);
        AddQuad(toFill, outLt, inLt, inLb, outLb);
        AddQuad(toFill, inRt, outRt, outRb, inRb);
    }

    private void AddQuad(VertexHelper toFill, Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        var index = toFill.currentVertCount;
        toFill.AddVert(a, color, Vector2.zero);
        toFill.AddVert(b, color, Vector2.zero);
        toFill.AddVert(c, color, Vector2.zero);
        toFill.AddVert(d, color, Vector2.zero);
        toFill.AddTriangle(index, index + 1, index + 2);
        toFill.AddTriangle(index, index + 2, index + 3);
    }

    private void LateUpdate()
    {
        RefreshView();
    }

    private void RefreshView()
    {
        var newBounds = _target != null && _target.gameObject.activeSelf
            ? RectTransformUtility.CalculateRelativeRectTransformBounds(transform, _target)
            : new Bounds();

        if (_targetBounds != newBounds)
        {
            _targetBounds = newBounds;
            // SetAllDirty() 会告诉 Unity：“这个图形脏了，需要重新绘制。” Unity 就会再次调用OnPopulateMesh.
            SetAllDirty();
        }
    }
}
