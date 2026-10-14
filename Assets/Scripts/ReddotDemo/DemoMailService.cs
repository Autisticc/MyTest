using UnityEngine;

/// <summary>
/// 模拟"邮件系统"业务模块（低耦合演示）。按钮事件触发的时候触发。
/// 
/// 关键点：本类【不】引用任何 UI 组件、不引用 ReddotBadge，
/// 只通过 ReddotMananger.ChangeValue(路径, 值) 修改红点数据。
/// UI 侧是否有人监听、有几个入口显示这个红点，本类一概不知。
/// </summary>
public class DemoMailService
{
    private int m_UnreadCount;

    /// <summary>收到一封新邮件 → 未读+1</summary>
    public void ReceiveNewMail()
    {
        m_UnreadCount++;
        ReddotManager.Instance.ChangeValue(ReddotDemoPaths.MailUnread, m_UnreadCount);
    }

    /// <summary>读掉一封邮件 → 未读-1（最少为0）</summary>
    public void ReadOneMail()
    {
        m_UnreadCount = Mathf.Max(0, m_UnreadCount - 1);
        ReddotManager.Instance.ChangeValue(ReddotDemoPaths.MailUnread, m_UnreadCount);
    }
}
