# 红点系统 · 低耦合 UI 集成指南

> 适用项目：Unity 2023.2.22f1 · 红点框架（ReddotMananger / TreeNode / RangeString）
> 配套 Demo：`Assets/Example/Scripts/ReddotDemo/`

---

## 一、这个 Demo 长什么样

打开独立演示场景 `Assets/Example/Scenes/ReddotDemoScene.unity` 后点 Play，屏幕中央会出现这套 UI（其他场景不会自动叠加）：

```
┌────────────────────────────────────────────────────────────┐
│                红点系统 · 低耦合 UI 集成 Demo                  │
│                                                              │
│   ┌──────────────┐  ┌─────────┐  ┌─────────┐  ┌─────────┐  │
│   │  主界面总红点  │  │  邮件   │  │  活动   │  │  好友   │  │
│   │      (2)      │  │   (1)   │  │   (1)   │  │   (0)   │  │
│   └──────────────┘  └─────────┘  └─────────┘  └─────────┘  │
│   ┌──────────────────────────────────────────────────────┐  │
│   │ 父节点红点 = 子节点之和：主界面 = 邮件+活动+好友，自动刷新  │  │
│   └──────────────────────────────────────────────────────┘  │
│   ┌─────────┐  ┌─────────┐  ┌─────────┐                      │
│   │收到新邮件│  │活动上新奖│  │收到好友申│  ← 模拟业务操作按钮    │
│   └─────────┘  └─────────┘  └─────────┘                      │
│   ┌─────────┐  ┌─────────┐  ┌─────────┐                      │
│   │读掉一封  │  │领取一个  │  │处理一条  │  ┌─────────┐         │
│   └─────────┘  └─────────┘  └─────────┘  │ 全部清零 │         │
│                                          └─────────┘         │
│   ┌──────────────────────────────────────────────────────┐  │
│   │ 业务模块只调 ChangeValue(路径,值)；UI 角标只调          │  │
│   │ AddListener(路径) —— 双方互不认识，靠路径字符串解耦      │  │
│   └──────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────┘
```

**重点观察**：点"收到新邮件" → 邮件入口角标变 1，同时**主界面总红点也变 1**（父节点自动聚合，无需任何代码）。

---

## 二、低耦合是怎么做到的

```
┌──────────────┐   ChangeValue(路径, 值)   ┌────────────────┐   AddListener(路径)   ┌─────────────┐
│  业务模块      │ ───────────────────────► │ ReddotMananger  │ ◄──────────────────── │  UI 组件     │
│ DemoMailService│                          │  (红点树/脏标记)  │                       │ ReddotBadge  │
│ DemoActivity…  │                          └────────────────┘                       └─────────────┘
│ DemoFriend…    │                              ▲  只认"路径字符串"                    ▲
└──────────────┘                                └──────────────┬───────────────────┘
                                                        唯一的耦合点
```

**三个原则**：

| 原则 | 体现 |
|---|---|
| 业务不感知 UI | 业务模块只调 `ChangeValue(path, value)`，**完全不知道**有没有 UI 在显示这个红点 |
| UI 不感知业务 | `ReddotBadge` 只调 `AddListener(path)`，**完全不知道**是哪个业务在改值 |
| 唯一约定 = 路径字符串 | 双方只共享 `ReddotDemoPaths.cs` 里的字符串常量，增删红点只改这一个文件 |

**后果**：邮件模块上线/下线、邮件 UI 换皮肤、加一个新的业务入口……任何一方改动都不会牵连另一方。

---

## 三、集成到自己项目的步骤

### 第 1 步：确认框架文件在位

需要这三个核心文件（本项目已存在，直接复用）：

| 文件 | 职责 |
|---|---|
| `Assets/Reddot/Runtime/ReddotMananger.cs` | 单例管理器：路径→节点、监听注册、脏节点轮询 |
| `Assets/Reddot/Runtime/TreeNode.cs` | 树节点：值、父子关系、聚合计算 |
| `Assets/Reddot/Runtime/RangeString.cs` | 路径切分（避免字符串分配的性能优化） |

### 第 2 步：定义你的红点路径常量

新建一个静态类，集中管理路径（**业务和 UI 共用**）：

```csharp
public static class MyReddotPaths
{
    // 叶子：业务真正会改的
    public const string MailUnread = "Main/Mail/Unread";
    // 父节点：自动聚合，UI 订阅它拿汇总
    public const string Mail = "Main/Mail";
}
```

> 路径规则：`/` 分层；父节点值自动 = 子节点之和；叶子节点才允许直接赋值。

### 第 3 步：业务侧 —— 值变了就通知（1 行代码）

你的业务模块里，数据变化处调用：

```csharp
// 例如邮件系统收到新邮件
ReddotMananger.Instance.ChangeValue(MyReddotPaths.MailUnread, 未读数量);
```

**业务侧到此结束**。不需要知道任何 UI 组件。

### 第 4 步：UI 侧 —— 挂角标组件（1 行配置）

把 `ReddotBadge.cs`（Demo 里的通用组件）挂到你 UI 的任意物体上：

1. 在 Hierarchy 里选中你的按钮/图标物体
2. Inspector → Add Component → 搜索 `ReddotBadge`
3. 配置两个字段：

```
Reddot Path   =  Main/Mail            ← 填要订阅的路径（父节点拿汇总数字）
Badge Root    =  （角标根物体，可为空=当前物体）
Num Text      =  （数字文本，可为空=自动找子物体）
```

4. 运行即可。值 > 0 显示角标，= 0 自动隐藏。

> 代码动态创建 UI 时用 `badge.Init("Main/Mail")`，见 Demo 的 `ReddotDemoBootstrap.cs`。

### 第 5 步：驱动刷新（保证 Update 被调用）

红点树的"脏节点结算"在 `ReddotMananger.Instance.Update()` 里。保证每帧调用一次即可（Demo 里由 Bootstrap 的 Update 驱动；你项目里可以是任意常驻脚本）：

```csharp
void Update()
{
    ReddotMananger.Instance.Update();
}
```

> 多脚本重复调用也无害（内部有空集合快速返回）。

### 第 6 步：验证

1. 运行场景 → 点业务按钮 → UI 角标变化
2. 父节点（如"主界面"）自动 = 子节点之和
3. 把子红点清零 → 父节点自动归零、角标隐藏

---

## 四、文件清单

```
Assets/Example/Scripts/ReddotDemo/
├── ReddotDemoPaths.cs        # 路径常量（业务与 UI 的唯一约定）
├── ReddotBadge.cs            # ★ 通用红点角标组件（可复用到你任何 UI）
├── DemoMailService.cs        # 模拟邮件业务（低耦合示例）
├── DemoActivityService.cs    # 模拟活动业务（低耦合示例）
├── DemoFriendService.cs      # 模拟好友业务（低耦合示例）
└── ReddotDemoBootstrap.cs    # 装配器：自动构建 UI + 每帧驱动 Update
```

---

## 五、常见问题

**Q：角标不显示？**
检查：路径是否填对；`ReddotMananger.Instance.Update()` 是否每帧被调用；叶子值是否 > 0。

**Q：想加一个新红点（比如"背包"）？**
1. `MyReddotPaths` 加一行常量（如 `Bag = "Main/Bag"`）
2. 业务改值时 `ChangeValue("Main/Bag", n)`
3. UI 上挂一个 `ReddotBadge` 填 `Main/Bag`
4. 若挂在 `Main` 下，主界面总红点自动带上它

**Q：父节点能直接赋值吗？**
不能，会抛异常（设计如此）。父节点值永远由子节点自动求和。

**Q：Demo UI 和我的其他场景重合了？**
不会。Demo 已改为独立场景：只有打开 `Assets/Example/Scenes/ReddotDemoScene.unity` 才会显示 Demo UI，其他场景（含 ExampleScene）运行不受影响，不会自动叠加。

**Q：想把 Demo 接到自己的场景里？**
把 `ReddotDemoBootstrap.cs` 挂到你场景任意空物体上（或复制 `ReddotBadge.cs` + `ReddotDemoPaths.cs` 到你的 UI 上按上文步骤集成），然后删掉 Demo 目录即可。
