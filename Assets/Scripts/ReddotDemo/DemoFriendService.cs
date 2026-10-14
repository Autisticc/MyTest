using UnityEngine;

/// <summary>
/// 模拟"好友系统"业务模块（低耦合演示）。
/// 
/// 关键点：本类【不】引用任何 UI 组件、不引用 ReddotBadge，
/// 只通过 ReddotMananger.ChangeValue(路径, 值) 修改红点数据。
/// </summary>
public class DemoFriendService
{
    private int m_ApplyCount;

    /// <summary>收到一条新的好友申请 → 申请+1</summary>
    public void ReceiveApply()
    {
        m_ApplyCount++;
        ReddotManager.Instance.ChangeValue(ReddotDemoPaths.FriendApply, m_ApplyCount);
    }

    /// <summary>处理一条好友申请 → 申请-1（最少为0）</summary>
    public void HandleOneApply()
    {
        m_ApplyCount = Mathf.Max(0, m_ApplyCount - 1);
        ReddotManager.Instance.ChangeValue(ReddotDemoPaths.FriendApply, m_ApplyCount);
    }
}
