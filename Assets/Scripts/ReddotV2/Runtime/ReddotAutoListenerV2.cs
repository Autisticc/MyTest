using UnityEngine;

/// <summary>
/// 红点自动监听组件：解决"UI 销毁前忘记 RemoveListener 导致回调泄漏"的坑。
///
/// 用法：挂到任意 UI 物体上，运行时调用 Init(path, callback) 绑定一次，
/// 之后组件会跟随物体生命周期自动管理订阅：
///   - 物体启用（OnEnable）→ 自动 AddListener
///   - 物体禁用/销毁（OnDisable）→ 自动 RemoveListener
/// 无论 UI 如何被隐藏、销毁，回调都一定会被移除，不会泄漏。
/// **为什么 demo 里没用到**：demo 的角标组件 `ReddotBadgeV2` 已经自己实现了 `OnDestroy` 退订，两者职责重叠，所以 demo 用 `ReddotBadgeV2` 就够了，
/// `ReddotAutoListenerV2` 是留给**你自己项目**里的 UI 用的 —— 比如你有一个非角标类的 UI（弹窗、红点特效层）也想订阅红点，挂它 + 调一次 `Init` 就行
/// </summary>
public class ReddotAutoListenerV2 : MonoBehaviour
{
    [SerializeField] private string m_Path;
    private System.Action<int> m_Callback;
    private bool m_Inited;

    /// <summary>
    /// 初始化并绑定回调（组件激活后立即订阅）
    /// </summary>
    public void Init(string path, System.Action<int> callback)
    {
        m_Path = path;
        m_Callback = callback;
        m_Inited = true;

        if (isActiveAndEnabled)
        {
            Subscribe();
        }
    }

    private void OnEnable()
    {
        if (m_Inited)
        {
            Subscribe();
        }
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (string.IsNullOrEmpty(m_Path) || m_Callback == null)
        {
            return;
        }
        ReddotManagerV2.Instance.AddListener(m_Path, m_Callback);
    }

    private void Unsubscribe()
    {
        if (string.IsNullOrEmpty(m_Path) || m_Callback == null)
        {
            return;
        }
        ReddotManagerV2.Instance.RemoveListener(m_Path, m_Callback);
    }
}
