using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 通用红点角标组件（低耦合核心）。
/// 
/// 挂在任意 UI 物体（按钮/图标）上，通过 Inspector 配置一个红点路径，
/// 组件自动订阅该路径：值 &gt; 0 时显示角标和数字，= 0 时自动隐藏。
/// 
/// 低耦合体现：
///   1. 本组件【只】依赖 ReddotMananger 和路径字符串，不感知任何业务逻辑；
///   2. 业务模块增删红点时，本组件完全不需要改动；
///   3. 一个 UI 想加红点 = 挂一个组件 + 填一个路径，一行业务代码都不写。
/// </summary>
public class ReddotBadge : MonoBehaviour
{
    [Header("红点路径（填 ReddotDemoPaths 里的常量，或你自己的路径）")]
    [SerializeField] private string m_Path;

    [Header("角标根物体（圆点+数字的父物体，值为0时隐藏它）")]
    [SerializeField] private GameObject m_BadgeRoot;

    [Header("数字文本（显示当前红点值）")]
    [SerializeField] private Text m_NumText;

    /// <summary>当前订阅的路径</summary>
    public string Path
    {
        get { return m_Path; }
    }

    private bool m_Inited;

    // 获取红点角标对象和里面的text，如果脚本配置了m_path的值就直接初始化该节点，reddotmananger加入监听，立刻更新一次红点显示
    private void Awake()
    {
        if (m_BadgeRoot == null)
            m_BadgeRoot = gameObject;
        if (m_NumText == null)
            m_NumText = GetComponentInChildren<Text>(true);

        // 配置了路径就立即订阅；运行时动态创建的 UI 可调用 Init() 注入路径
        if (!string.IsNullOrEmpty(m_Path))
            Init(m_Path);
    }

    /// <summary>
    /// 运行时初始化（供代码动态创建 UI 时使用，Inspector 配置可忽略此调用）。
    /// </summary>
    public void Init(string path)
    {
        if (m_Inited)
            return;

        m_Path = path;
        m_Inited = true;

        // 订阅：值一变自动回调（业务完全不知道我们的存在）
        ReddotManager.Instance.AddListener(m_Path, OnReddotValueChange);

        // 立即用当前值刷一次，保证启动时就显示正确
        OnReddotValueChange(ReddotManager.Instance.GetValue(m_Path));
    }

    // UI组件销毁的时候需要移除所有回调方法
    private void OnDestroy()
    {
        if (!m_Inited || string.IsNullOrEmpty(m_Path))
            return;

        ReddotManager.Instance.RemoveListener(m_Path, OnReddotValueChange);
    }

    private void OnReddotValueChange(int value)
    {
        // 如果红点树返回这个节点有值就让红点角标显示，让text跟新维新的值。
        if (m_BadgeRoot != null)
            m_BadgeRoot.SetActive(value > 0);

        if (m_NumText != null)
            m_NumText.text = value > 99 ? "99+" : value.ToString();
    }
}
