using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 引导遮罩·不规则图形 Demo
/// 
/// 把本脚本挂到场景任意空物体上（或使用菜单 Tools → 搭建引导遮罩不规则图形Demo），
/// 运行即可看到：一个黄色五角星按钮，被全屏半透明遮罩盖住，只有五角星形状内部镂空高亮。
/// 点击五角星 → 遮罩消失。
///
/// 原理：五角星按钮用 UI/StencilWrite 材质，把自身形状写入模板缓冲(Ref=1)；
///       全屏遮罩用 UI/HoleMask 材质，只在模板值 != 1 处画黑，于是五角星区域天然镂空。
///       镂空形状 = 按钮图片的任意形状，无需手动算顶点。
/// </summary>
public class IrregularGuideMask : MonoBehaviour
{
    [Header("按钮")]
    [SerializeField] private float buttonSize = 300f;
    [SerializeField] private Color starColor = Color.yellow;

    [Header("遮罩")]
    [SerializeField] private Color maskColor = new Color(0f, 0f, 0f, 0.6f);

    private Image _mask;
    private Image _star;

    private void Start()
    {
        Build();
    }

    // 点一下：遮罩消失；再点一下：遮罩回来（方便观察镂空是否贴合五角星）
    private void OnStarClicked()
    {
        if (_mask != null) _mask.gameObject.SetActive(!_mask.gameObject.activeSelf);
    }

    private void Build()
    {
        // ---------- Canvas ----------
        var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // ---------- EventSystem（没有才建） ----------
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            var esGo = new GameObject("EventSystem",
                typeof(EventSystem), typeof(StandaloneInputModule));
            esGo.transform.SetParent(null);
        }

        // ---------- 五角星 Sprite（运行时程序化生成） ----------
        var starSprite = CreateStarSprite(256);

        // ---------- 五角星按钮（StencilWrite 材质，把形状写进模板） ----------
        var btnGo = new GameObject("StarButton",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        btnGo.transform.SetParent(canvasGo.transform, false);
        btnGo.transform.SetAsFirstSibling(); // 让按钮先渲染，先写模板

        var btnRect = btnGo.GetComponent<RectTransform>();
        btnRect.anchorMin = btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(buttonSize, buttonSize);

        _star = btnGo.GetComponent<Image>();
        _star.sprite = starSprite;
        _star.color = starColor;
        _star.material = new Material(Shader.Find("UI/StencilWrite"));

        btnGo.GetComponent<Button>().onClick.AddListener(OnStarClicked);

        // ---------- 全屏遮罩（HoleMask 材质，反向模板抠洞） ----------
        var maskGo = new GameObject("GuideMask",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        maskGo.transform.SetParent(canvasGo.transform, false);

        var maskRect = maskGo.GetComponent<RectTransform>();
        maskRect.anchorMin = Vector2.zero;
        maskRect.anchorMax = Vector2.one;
        maskRect.offsetMin = Vector2.zero;
        maskRect.offsetMax = Vector2.zero;

        _mask = maskGo.GetComponent<Image>();
        _mask.material = new Material(Shader.Find("UI/HoleMask")) { color = maskColor };
        _mask.raycastTarget = false; // 关键：遮罩不挡点击，让点击穿透到按钮
    }

    // ==================== 五角星 ====================

    private static Sprite CreateStarSprite(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { filterMode = FilterMode.Bilinear };

        float cx = size * 0.5f, cy = size * 0.5f;
        float R = size * 0.5f - 2f;                 // 外圆半径
        float r = R * 0.382f;                       // 内圆半径（黄金分割，标准五角星）
        var poly = StarPoints(cx, cy, R, r);

        var colors = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool inside = PointInPolygon(new Vector2(x + 0.5f, y + 0.5f), poly);
                colors[y * size + x] = inside ? Color.white : Color.clear;
            }
        }
        tex.SetPixels(colors);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    // 五角星 10 个顶点：外顶点 5 个 + 内顶点 5 个，交替排列，一个角朝上
    private static Vector2[] StarPoints(float cx, float cy, float R, float r)
    {
        var pts = new Vector2[10];
        for (int i = 0; i < 10; i++)
        {
            float radius = i % 2 == 0 ? R : r;
            float ang = (90f - i * 36f) * Mathf.Deg2Rad; // i=0 时在正上方
            pts[i] = new Vector2(cx + radius * Mathf.Cos(ang), cy + radius * Mathf.Sin(ang));
        }
        return pts;
    }

    // 射线法：判断点是否在任意多边形内
    private static bool PointInPolygon(Vector2 p, Vector2[] poly)
    {
        bool inside = false;
        for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
        {
            if ((poly[i].y > p.y) != (poly[j].y > p.y) &&
                p.x < (poly[j].x - poly[i].x) * (p.y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x)
            {
                inside = !inside;
            }
        }
        return inside;
    }
}
