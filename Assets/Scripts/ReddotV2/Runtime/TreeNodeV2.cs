using System;
using System.Collections.Generic;


/// <summary>
/// 树节点
/// </summary>
public class TreeNodeV2
{

    /// <summary>
    /// 子节点
    /// </summary>
    private Dictionary<RangeStringV2, TreeNodeV2> m_Children;

    /// <summary>
    /// 节点值改变回调
    /// </summary>
    private Action<int> m_ChangeCallback;

    /// <summary>
    /// 完整路径
    /// </summary>
    private string m_FullPath;

    /// <summary>
    /// 子树节点总数（缓存：不含自身，增删节点时维护，O(1) 读取）
    /// </summary>
    private int m_ChildrenCount;

    /// <summary>
    /// 节点名
    /// </summary>
    public string Name
    {
        get;
        private set;
    }

    /// <summary>
    /// 完整路径
    /// </summary>
    public string FullPath
    {
        get
        {
            if (string.IsNullOrEmpty(m_FullPath))
            {
                if (Parent == null || Parent == ReddotManagerV2.Instance.Root)
                {
                    m_FullPath = Name;
                }
                else
                {
                    m_FullPath = Parent.FullPath + ReddotManagerV2.Instance.SplitChar + Name;
                }
            }

            return m_FullPath;
        }
    }

    /// <summary>
    /// 节点值
    /// </summary>
    public int Value
    {
        get;
        private set;
    }

    /// <summary>
    /// 父节点
    /// </summary>
    public TreeNodeV2 Parent
    {
        get;
        private set;
    }

    /// <summary>
    /// 子节点
    /// </summary>
    public Dictionary<RangeStringV2, TreeNodeV2>.ValueCollection Children
    {
        get
        {
            return m_Children?.Values;
        }
    }

    /// <summary>
    /// 子节点数量（子树节点总数，O(1) 读取）
    /// </summary>
    public int ChildrenCount
    {
        get
        {
            return m_ChildrenCount;
        }
    }

    public TreeNodeV2(string name)
    {
        Name = name;
        Value = 0;
        m_ChangeCallback = null;
    }

    public TreeNodeV2(string name, TreeNodeV2 parent) : this(name)
    {
        Parent = parent;
    }

    /// <summary>
    /// 添加节点值监听
    /// </summary>
    public void AddListener(Action<int> callback)
    {
        m_ChangeCallback += callback;
    }

    /// <summary>
    /// 移除节点值监听
    /// </summary>
    public void RemoveListener(Action<int> callback)
    {
        m_ChangeCallback -= callback;
    }

    /// <summary>
    /// 移除所有节点值监听
    /// </summary>
    public void RemoveAllListener()
    {
        m_ChangeCallback = null;
    }

    /// <summary>
    /// 改变节点值（使用传入的新值，只能在叶子节点上调用）
    /// </summary>
    public void ChangeValue(int newValue)
    {
        if (m_Children != null && m_Children.Count != 0)
        {
            throw new Exception("不允许直接改变非叶子节点的值：" + FullPath);
        }

        InternalChangeValue(newValue);
    }

    /// <summary>
    /// 改变节点值（根据子节点值计算新值，只对非叶子节点有效）
    /// </summary>
    public void ChangeValue()
    {
        int sum = 0;

        if (m_Children != null && m_Children.Count != 0)
        {
            foreach (KeyValuePair<RangeStringV2, TreeNodeV2> child in m_Children)
            {
                sum += child.Value.Value;
            }
        }

        InternalChangeValue(sum);
    }

    /// <summary>
    /// 获取子节点，如果不存在则添加
    /// </summary>
    public TreeNodeV2 GetOrAddChild(RangeStringV2 key)
    {
        TreeNodeV2 child = GetChild(key);
        if (child == null)
        {
            child = AddChild(key);
        }
        return child;
    }

    /// <summary>
    /// 获取子节点
    /// </summary>
    public TreeNodeV2 GetChild(RangeStringV2 key)
    {

        if (m_Children == null)
        {
            return null;
        }

        m_Children.TryGetValue(key, out TreeNodeV2 child);
        return child;
    }

    /// <summary>
    /// 添加子节点
    /// </summary>
    public TreeNodeV2 AddChild(RangeStringV2 key)
    {
        if (m_Children == null)
        {
            m_Children = new Dictionary<RangeStringV2, TreeNodeV2>();
        }
        else if (m_Children.ContainsKey(key))
        {
            throw new Exception("子节点添加失败，不允许重复添加：" + FullPath);
        }

        TreeNodeV2 child = new TreeNodeV2(key.ToString(), this);
        m_Children.Add(key, child);
        NotifyChildrenCountChanged(1);
        ReddotManagerV2.Instance.NodeNumChangeCallback?.Invoke();
        return child;
    }

    /// <summary>
    /// 移除子节点
    /// </summary>
    public bool RemoveChild(RangeStringV2 key)
    {
        if (m_Children == null || m_Children.Count == 0)
        {
            return false;
        }

        TreeNodeV2 child = GetChild(key);

        if (child != null)
        {
            //子节点被删除 需要进行一次父节点刷新
            ReddotManagerV2.Instance.MarkDirtyNode(this);

            m_Children.Remove(key);
            NotifyChildrenCountChanged(-(child.ChildrenCount + 1));

            ReddotManagerV2.Instance.NodeNumChangeCallback?.Invoke();

            return true;
        }

        return false;
    }

    /// <summary>
    /// 移除所有子节点
    /// </summary>
    public void RemoveAllChild()
    {
        if (m_Children == null || m_Children.Count == 0)
        {
            return;
        }

        int removed = m_ChildrenCount;
        m_Children.Clear();
        NotifyChildrenCountChanged(-removed);

        ReddotManagerV2.Instance.MarkDirtyNode(this);
        ReddotManagerV2.Instance.NodeNumChangeCallback?.Invoke();
    }

    /// <summary>
    /// 沿父链向上维护子树节点数缓存,就是你增加删除节点，通知你的所有父节点，让他们也减。
    /// </summary>
    private void NotifyChildrenCountChanged(int delta)
    {
        m_ChildrenCount += delta;
        if (Parent != null)
        {
            Parent.NotifyChildrenCountChanged(delta);
        }
    }

    public override string ToString()
    {
        return FullPath;
    }

    /// <summary>
    /// 改变节点值
    /// </summary>
    private void InternalChangeValue(int newValue)
    {
        if (Value == newValue)
        {
            return;
        }

        Value = newValue;
        m_ChangeCallback?.Invoke(newValue);
        ReddotManagerV2.Instance.NodeValueChangeCallback?.Invoke(this, Value);

        //标记父节点为脏节点
        ReddotManagerV2.Instance.MarkDirtyNode(Parent);
    }
}
