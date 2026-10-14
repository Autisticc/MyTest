using UnityEngine;

/// <summary>
/// 模拟"活动系统"业务模块（低耦合演示）。
/// 
/// 关键点：本类【不】引用任何 UI 组件、不引用 ReddotBadge，
/// 只通过 ReddotMananger.ChangeValue(路径, 值) 修改红点数据。
/// </summary>
public class DemoActivityService
{
    private int m_ClaimableCount;

    /// <summary>活动上架一个新的可领取奖励 → 可领+1</summary>
    public void AddClaimableReward()
    {
        m_ClaimableCount++;
        ReddotManager.Instance.ChangeValue(ReddotDemoPaths.ActivityReward, m_ClaimableCount);
    }

    /// <summary>领取一个奖励 → 可领-1（最少为0）</summary>
    public void ClaimOneReward()
    {
        m_ClaimableCount = Mathf.Max(0, m_ClaimableCount - 1);
        ReddotManager.Instance.ChangeValue(ReddotDemoPaths.ActivityReward, m_ClaimableCount);
    }
}
