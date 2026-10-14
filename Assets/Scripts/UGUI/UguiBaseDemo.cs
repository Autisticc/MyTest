using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UGUI全部基础组件统一Demo脚本
/// 说明：Panel不是C#类！右键创建的Panel物体本质是挂载Image组件的GameObject，所以字段类型改为Image
/// 使用方法：新建空物体UguiDemoManager，挂载此脚本；在Inspector面板，把场景中创建好的UI物体拖拽到对应字段
/// </summary>
public class UguiBaseDemo : MonoBehaviour
{
    [Header("===== 所有UI组件拖拽引用到Inspector =====")]

    /*
     组件：Canvas 画布
     作用：UGUI所有UI元素的根容器，控制UI渲染模式，配套CanvasScaler分辨率适配、GraphicRaycaster射线检测
     Unity操作步骤：Hierarchy右键 -> UI -> Canvas
         自动生成Canvas + EventSystem。
         选中Canvas，Inspector修改RenderMode切换渲染模式；
         CanvasScaler（这是canvas里面的组件）设置参考分辨率1920×1080，UI Scale Mode选Scale With Screen Size
     面试题：Canvas三种RenderMode分别是什么，各自适用场景？CanvasScaler三种缩放模式区别？
     </summary>
    */
    public Canvas canvas;

    /// <summary>
    /// 组件：Panel 面板
    /// 作用：UI分组容器，本质是挂载Image组件的物体，用来放背景底板，批量管理一组UI，整体显示/隐藏
    /// Unity操作步骤：右键Canvas -> UI -> Panel；Inspector的Image修改颜色/背景图；把Text、Button等UI拖入Panel作为子物体；将Panel物体拖入此字段
    /// 面试题：UGUI的Panel是不是独立组件？Panel本质是什么？一般用来做什么？
    /// </summary>
    public Image panel;

    /// <summary>
    /// 组件：Text 文本(旧UGUI文本，新项目推荐TMP)
    /// 作用：展示静态文字，控制字体、字号、颜色、对齐方式
    /// Unity操作步骤：右键Canvas -> UI -> Text；Inspector修改文字内容、字号、颜色；把Text物体拖入此字段
    /// 面试题：UGUI Text 和 TextMeshPro(TMP) 的区别，TMP有什么优势？
    /// </summary>
    public Text text;

    /// <summary>
    /// 组件：Image UI精灵图片
    /// 作用：显示Sprite类型UI图片，支持Sliced九宫格拉伸，用于图标、背景、按钮图，可以打进图集
    /// Unity操作步骤：右键Canvas -> UI -> Image；
    ///     导入图片，图片的Texture Type设置为Sprite(2D and UI>需要按提示安装Sprite2D包)；
    ///     图片的Sprite mode设置为single
    ///     图片拖入Source Image；Image Type选择Simple/Sliced/Tiled/Filled
    /// 面试题：Image四种Type分别是什么？九宫格Sliced原理？Image和RawImage最大区别？
    /// </summary>
    public Image image;

    /// <summary>
    /// 组件：RawImage 原始纹理图片
    /// 作用：直接渲染Texture2D、RenderTexture，不需要转为Sprite，**不能打入图集**；适合相机渲染贴图、视频画面贴UI
    /// Unity操作步骤：右键Canvas -> UI -> Raw Image；贴图保持默认Texture类型，不改成Sprite；贴图拖入RawImage的Texture字段
    /// 面试题：RawImage为什么不能打图集？什么场景优先选用RawImage？
    /// </summary>
    public RawImage rawImage;

    /// <summary>
    /// 组件：Button 按钮
    /// 作用：点击交互组件，自带Normal/Hover/Pressed/Disabled四种状态，点击触发回调事件
    /// Unity操作步骤：右键Canvas -> UI -> Button；自带子物体Text，修改显示文字；把Button物体拖入此字段
    /// 面试题：Button点击底层事件流程，GraphicRaycaster、EventSystem、IPointerClickHandler的作用？interactable属性作用？
    /// </summary>
    public Button button;

    /// <summary>
    /// 组件：Toggle 复选框
    /// 作用：布尔开关，勾选/取消勾选；搭配ToggleGroup组件，实现一组单选互斥
    /// Unity操作步骤：右键Canvas -> UI -> Toggle（属性is on控制是否选择）；自带背景、勾选图片、Text子物体；拓展单选：新建空物体挂载ToggleGroup，多个Toggle作为其子物体
    /// 面试题：ToggleGroup如何实现互斥单选，底层原理？
    /// </summary>
    public Toggle toggle;

    /// <summary>
    /// 组件：Slider 滑动条
    /// 作用：拖动滑块，获取0~最大值之间浮点数，用于血量条、音量、进度条
    /// Unity操作步骤：右键Canvas -> UI -> Slider；Inspector设置MinValue最小值、MaxValue最大值、Value当前值，Direction修改滑动方向
    /// 面试题：Slider的onValueChanged拖动时每一帧都会触发吗？
    /// </summary>
    public Slider slider;

    /// <summary>
    /// 组件：Scrollbar 滚动条
    /// 作用：取值范围0~1，代表可视区域占总内容的比例；一般不单独使用，ScrollView自动管理
    /// Unity操作步骤：右键Canvas -> UI -> Scrollbar；Size代表滑块长度占总条比例，Value是滑块位置
    /// 面试题：Scrollbar 和 Slider 的区别是什么？
    /// </summary>
    public Scrollbar scrollbar;

    /// <summary>
    /// 组件：Dropdown 下拉选择框
    /// 作用：下拉菜单，点击展开选项列表，选中选项返回选项索引
    /// Unity操作步骤：右键Canvas -> UI -> Dropdown；Inspector面板Options列表可以直接添加选项文本,using TMPro;
    /// 面试题：Dropdown底层原理，大量选项时存在什么性能问题？
    /// </summary>
    public TMP_Dropdown dropdown;

    /// <summary>
    /// 组件：InputField 输入框
    /// 作用：接收玩家键盘输入文字，账号、密码、聊天输入
    /// Unity操作步骤：右键Canvas -> UI -> Input Field；自带Text子物体；Content Type可以设置普通文本 / Password密码模式
    /// 面试题：InputField的onEndEdit 和 onValueChanged 的区别？
    /// </summary>
    public TMP_InputField inputField;

    /// <summary>
    /// 组件：ScrollView滚动视图，代码内取ScrollRect组件
    /// 作用：带遮罩的滚动容器，背包、长列表；结构：ScrollView -> Viewport(Mask遮罩) -> Content(存放列表子物体)
    /// Unity操作步骤：右键Canvas -> UI -> Scroll View；选中Content调整高度，在Content下放置多个UI子物体，Content尺寸大于Viewport才可以滚动
    /// 面试题：ScrollView里面Viewport和Content作用？Mask遮罩原理？ScrollRect滚动卡顿常见原因？
    /// </summary>
    public ScrollRect scrollRect;


    void Start()
    {
        // ========== Canvas 演示 ==========
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            Debug.Log($"【Canvas】渲染模式：{canvas.renderMode}");
        }

        // ========== Text 演示 ==========
        if (text != null)
        {
            text.text = "按下按钮显示\n和隐藏背景板";
            text.fontSize = 16;
            text.color = Color.red;
        }

        // ========== Image 演示 ==========
        if (image != null)
        {
            image.color = Color.white;
            image.type = Image.Type.Tiled;
        }

        // ========== RawImage 演示 ==========
        if (rawImage != null)
        {
            rawImage.color = Color.white;
        }

        // ========== Button 注册点击事件 ==========
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }

        // ========== Toggle 注册事件 ==========
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(OnToggleChanged);
        }

        // ========== Slider 注册事件 ==========
        if (slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = 100;
            slider.value = 50;
            slider.onValueChanged.AddListener(OnSliderChanged);
        }

        // ========== Scrollbar 注册事件 ==========
        if (scrollbar != null)
        {
            scrollbar.size = 0.3f;
            scrollbar.onValueChanged.AddListener(OnScrollbarChanged);
        }

        // ========== Dropdown 初始化选项 ==========
        if (dropdown != null)
        {
            dropdown.options.Clear();
            dropdown.options.Add(new TMP_Dropdown.OptionData("选项A"));
            dropdown.options.Add(new TMP_Dropdown.OptionData("选项B"));
            dropdown.options.Add(new TMP_Dropdown.OptionData("选项C"));
            dropdown.RefreshShownValue();
            dropdown.onValueChanged.AddListener(OnDropdownChanged);
        }

        // ========== InputField 注册事件 ==========
        if (inputField != null)
        {
            inputField.onEndEdit.AddListener(OnInputEndEdit);
        }

        // ========== ScrollView(ScrollRect) 注册滚动事件 ==========
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.AddListener(OnScrollChanged);
        }
    }

    #region 各个组件回调方法
    /// <summary>
    /// Button点击回调：点击按钮切换Panel面板显示隐藏
    /// </summary>
    void OnButtonClick()
    {
        Debug.Log("【Button】按钮被点击");
        // 点击按钮示例：切换面板显隐
        if(panel != null)
        {
            panel.gameObject.SetActive(!panel.gameObject.activeSelf);
        }
    }

    /// <summary>
    /// Toggle勾选状态变化回调
    /// </summary>
    void OnToggleChanged(bool isOn)
    {
        Debug.Log($"【Toggle】勾选状态：{isOn}");
    }

    /// <summary>
    /// Slider滑动值变化回调，联动修改Text文本
    /// </summary>
    void OnSliderChanged(float val)
    {
        Debug.Log($"【Slider】滑动值：{val:F2}");
        // 联动：滑动条修改Text文字
        if(text != null)
        {
            text.text = $"滑动值 = {val:F0}";
        }
    }

    /// <summary>
    /// Scrollbar滚动条位置变化回调
    /// </summary>
    void OnScrollbarChanged(float val)
    {
        Debug.Log($"【Scrollbar】滚动条位置：{val:F2}");
    }

    /// <summary>
    /// Dropdown下拉选项选中回调，index为选中选项索引
    /// </summary>
    void OnDropdownChanged(int index)
    {
        Debug.Log($"【Dropdown】选中选项索引：{index}");
    }

    /// <summary>
    /// InputField输入完成回调，结束编辑（回车/失去焦点触发）
    /// </summary>
    void OnInputEndEdit(string content)
    {
        Debug.Log($"【InputField】输入完成：{content}");
    }

    /// <summary>
    /// ScrollRect滚动位置变化回调，pos为滚动偏移Vector2
    /// </summary>
    void OnScrollChanged(Vector2 pos)
    {
        Debug.Log($"【ScrollView】滚动位置：{pos}");
    }
    #endregion
}