using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(CanvasRenderer))]
public class RoundedImage : Image
{
    [Header("圆角半径")]
    [SerializeField] private float radius = 10f;

    [Header("四角统一控制")]
    [SerializeField] private bool uniformRadius = true;

    [Header("四角单独半径(左下,右下,左上,右上)")]
    [SerializeField] private Vector4 cornerRadius = new(10, 10, 10, 10);

    private static readonly int RectSizeID = Shader.PropertyToID("_RectSize");
    private static readonly int CornerRadiusID = Shader.PropertyToID("_CornerRadius");

    private Material _instanceMaterial;

    // 访问实际渲染用的材质实例，避免直接改到磁盘上的共享 .mat 资源
    private Material GetRenderMaterial()
    {
        if (_instanceMaterial == null)
        {
            var shader = Shader.Find("Custom/UI/RoundedRect");
            if (shader == null) return null;
            _instanceMaterial = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
        }
        return _instanceMaterial;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        // 每次启用时强制用我们自己的材质实例
        var mat = GetRenderMaterial();
        if (mat != null)
        {
            m_Material = mat;
            UpdateMaterialProperties();
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        UpdateMaterialProperties();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        UpdateMaterialProperties();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        SetVerticesDirty();
        SetMaterialDirty();
    }
#endif

    private void UpdateMaterialProperties()
    {
        var mat = GetRenderMaterial();
        if (mat == null) return;

        var rect = rectTransform.rect;
        mat.SetVector(RectSizeID, new Vector4(rect.width, rect.height, 0, 0));

        if (uniformRadius)
        {
            mat.SetVector(CornerRadiusID, new Vector4(radius, radius, radius, radius));
        }
        else
        {
            mat.SetVector(CornerRadiusID, cornerRadius);
        }
    }

    protected override void OnDestroy()
    {
        if (Application.isPlaying)
        {
            if (_instanceMaterial != null) Destroy(_instanceMaterial);
        }
        base.OnDestroy();
    }
}
