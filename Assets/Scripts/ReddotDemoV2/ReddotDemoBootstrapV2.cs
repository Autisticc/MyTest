using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 红点系统接入真实 UI 的演示装配器（独立场景版）。
///
/// 启动后自动构建一套"游戏主界面"样式的 UI：
///   顶部 4 个入口按钮（主界面/邮件/活动/好友），每个右上角带红点角标；
///   底部一排"模拟业务操作"按钮，点击后由业务模块修改红点值；
///   邮件/活动/好友入口的红点 = 各自叶子之和；主界面红点 = 三者之和（自动聚合）。
///
/// 使用方式：
///   1. 打开独立演示场景 Assets/Example/Scenes/ReddotDemoSceneV2.unity；
///   2. 本组件已挂载在场景 "ReddotDemoBootstrapV2" 物体上，直接点 Play 即可。
///      （其他场景不受影响，不会自动叠加本 Demo）
/// </summary>
public class ReddotDemoBootstrapV2 : MonoBehaviour
{
    // ---------- 业务模块（低耦合：UI 完全不认识它们，它们也不认识 UI） ----------
    private DemoMailServiceV2 m_Mail;
    private DemoActivityServiceV2 m_Activity;
    private DemoFriendServiceV2 m_Friend;

    private Canvas m_Canvas;

    private void Awake()
    {
        m_Mail = new DemoMailServiceV2();
        m_Activity = new DemoActivityServiceV2();
        m_Friend = new DemoFriendServiceV2();

        BuildCanvas();
        BuildEntryButtons();
        BuildControlButtons();
    }

    // 红点树刷新已由 ReddotDriverV2 自动每帧驱动（无需手动调用 Update）

    // ================= UI 构建 =================

    private void BuildCanvas()
    {
        GameObject canvasGo = new GameObject("ReddotDemoCanvasV2");
        canvasGo.transform.SetParent(transform, false);

        m_Canvas = canvasGo.AddComponent<Canvas>();
        m_Canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGo.AddComponent<GraphicRaycaster>();

        // 保证按钮可点击：场景没有 EventSystem 就创建一个
        if (EventSystem.current == null)
        {
            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<StandaloneInputModule>();
        }
    }

    private void BuildEntryButtons()
    {
        // 标题
        CreateText("标题", new Vector2(0f, 260f), new Vector2(800f, 60f), "红点系统 · 低耦合 UI 集成 Demo", 32);

        // 主界面入口（订阅 Main 路径：自动 = 邮件 + 活动 + 好友 之和）
        CreateEntryButton("主界面", "主界面总红点", ReddotDemoPathsV2.Main, new Vector2(-450f, 120f), new Vector2(360f, 110f));

        // 三个业务入口（订阅父节点路径，值自动聚合）
        CreateEntryButton("邮件", "邮件入口", ReddotDemoPathsV2.Mail, new Vector2(-60f, 120f), new Vector2(280f, 110f));
        CreateEntryButton("活动", "活动入口", ReddotDemoPathsV2.Activity, new Vector2(270f, 120f), new Vector2(280f, 110f));
        CreateEntryButton("好友", "好友入口", ReddotDemoPathsV2.Friend, new Vector2(600f, 120f), new Vector2(280f, 110f));

        // 说明文字：父节点自动聚合
        CreateText("聚合说明", new Vector2(0f, 40f), new Vector2(1400f, 36f),
            "父节点红点 = 子节点之和（自动计算）：主界面 = 邮件 + 活动 + 好友，任意一个变化主界面自动刷新", 20);
    }

    private void BuildControlButtons()
    {
        CreateText("操作区标题", new Vector2(0f, -120f), new Vector2(600f, 40f), "模拟业务操作（点击后观察各入口红点）", 24);

        // 第一行：增加类操作
        CreateControlButton("收到新邮件", new Vector2(-600f, -210f), () => m_Mail.ReceiveNewMail());
        CreateControlButton("活动上新奖励", new Vector2(-200f, -210f), () => m_Activity.AddClaimableReward());
        CreateControlButton("收到好友申请", new Vector2(200f, -210f), () => m_Friend.ReceiveApply());

        // 第二行：减少类操作
        CreateControlButton("读掉一封邮件", new Vector2(-600f, -300f), () => m_Mail.ReadOneMail());
        CreateControlButton("领取一个奖励", new Vector2(-200f, -300f), () => m_Activity.ClaimOneReward());
        CreateControlButton("处理一条申请", new Vector2(200f, -300f), () => m_Friend.HandleOneApply());

        // 第三行：全清零
        CreateControlButton("全部清零", new Vector2(200f, -390f), ResetAll, new Color(0.75f, 0.35f, 0.35f, 1f));

        CreateText("低耦合说明", new Vector2(0f, -480f), new Vector2(1600f, 32f),
            "业务模块只调 ChangeValue(路径, 值)；UI 角标只调 AddListener(路径) —— 双方互不认识，通过路径字符串解耦", 18);
    }

    /// <summary>把三个业务红点全部清零（业务侧驱动，UI 自动跟随）</summary>
    private void ResetAll()
    {
        ReddotManagerV2.Instance.ChangeValue(ReddotDemoPathsV2.MailUnread, 0);
        ReddotManagerV2.Instance.ChangeValue(ReddotDemoPathsV2.ActivityReward, 0);
        ReddotManagerV2.Instance.ChangeValue(ReddotDemoPathsV2.FriendApply, 0);
    }

    // ================= UI 创建工具 =================

    /// <summary>创建一个带红点角标的入口按钮</summary>
    private void CreateEntryButton(string buttonName, string label, string reddotPath, Vector2 pos, Vector2 size)
    {
        Button btn = CreateButton(buttonName, label, pos, size, new Color(0.25f, 0.42f, 0.7f, 1f));

        // 在按钮右上角创建红点角标（红色圆点 + 数字）
        GameObject badge = CreateRect("Badge_" + buttonName, btn.transform, new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(-8f, 8f), new Vector2(52f, 52f));

        Image badgeImg = badge.AddComponent<Image>();
        badgeImg.color = new Color(0.9f, 0.2f, 0.2f, 1f);

        // 角标里的数字
        Text numText = CreateText("Num_" + buttonName, new Vector2(0f, 0f), new Vector2(52f, 52f), "0", 26);
        numText.transform.SetParent(badge.transform, false);
        RectTransform numRt = numText.rectTransform;
        numRt.anchorMin = Vector2.zero;
        numRt.anchorMax = Vector2.one;
        numRt.offsetMin = Vector2.zero;
        numRt.offsetMax = Vector2.zero;

        // 挂 ReddotBadgeV2：值>0 显示角标，=0 隐藏
        ReddotBadgeV2 badgeComp = badge.AddComponent<ReddotBadgeV2>();
        badgeComp.Init(reddotPath);
    }

    /// <summary>创建一个普通按钮</summary>
    private Button CreateButton(string name, string label, Vector2 pos, Vector2 size, Color color)
    {
        GameObject go = CreateRect(name, m_Canvas.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, size);

        Image img = go.AddComponent<Image>();
        img.color = color;

        Button btn = go.AddComponent<Button>();

        Text txt = CreateText("Label_" + name, Vector2.zero, size, label, 26);
        txt.transform.SetParent(go.transform, false);
        RectTransform txtRt = txt.rectTransform;
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.offsetMin = Vector2.zero;
        txtRt.offsetMax = Vector2.zero;

        return btn;
    }

    /// <summary>创建一个绑定业务操作的按钮</summary>
    private void CreateControlButton(string label, Vector2 pos, UnityEngine.Events.UnityAction onClick, Color? color = null)
    {
        Color c = color ?? new Color(0.3f, 0.34f, 0.42f, 1f);
        Button btn = CreateButton("Btn_" + label, label, pos, new Vector2(300f, 72f), c);
        btn.onClick.AddListener(onClick);
    }

    private GameObject CreateRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        return go;
    }

    private Text CreateText(string name, Vector2 pos, Vector2 size, string content, int fontSize)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        if (m_Canvas != null)
            go.transform.SetParent(m_Canvas.transform, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Text txt = go.AddComponent<Text>();
        txt.text = content;
        txt.fontSize = fontSize;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.font = GetFont();
        txt.raycastTarget = false;
        return txt;
    }

    private static Font GetFont()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        return font;
    }
}
