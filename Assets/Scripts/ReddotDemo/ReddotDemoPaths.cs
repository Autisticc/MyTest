/// <summary>
/// 红点路径常量：业务模块与 UI 模块之间【唯一】的耦合约定。
/// 
/// 低耦合设计核心：
///   业务侧只调用 ReddotMananger.ChangeValue(路径, 值)
///   UI 侧只调用 ReddotMananger.AddListener(路径, 回调)
///   双方互不认识，都只认识这个字符串路径 —— 增删红点只改这里。
/// 
/// 树结构（路径用 / 分层）：
///   Main
///   ├── Mail
///   │   └── Unread      （叶子：业务直接赋值）
///   ├── Activity
///   │   └── Reward      （叶子：业务直接赋值）
///   └── Friend
///       └── Apply       （叶子：业务直接赋值）
///   父节点（Mail/Activity/Friend/Main）的值 = 子节点之和，由框架自动计算。
/// </summary>
public static class ReddotDemoPaths
{
    // 叶子节点（业务真正会改的）
    public const string MailUnread = "Main/Mail/Unread";
    public const string ActivityReward = "Main/Activity/Reward";
    public const string FriendApply = "Main/Friend/Apply";

    // 父节点（自动聚合，UI 订阅这些就能拿到汇总数字）
    public const string Mail = "Main/Mail";
    public const string Activity = "Main/Activity";
    public const string Friend = "Main/Friend";
    public const string Main = "Main";
}
