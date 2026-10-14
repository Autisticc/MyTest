using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 红点管理器
/// </summary>
public class ReddotManagerV2
{
    private static ReddotManagerV2 m_Instance;

    public static ReddotManagerV2 Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new ReddotManagerV2();
            }
            return m_Instance;
        }
    }

    /// <summary>
    /// 所有节点集合
    /// </summary>
    private Dictionary<string, TreeNodeV2> m_AllNodes;

    /// <summary>
    /// 脏节点集合
    /// </summary>
    private HashSet<TreeNodeV2> m_DirtyNodes;

    /// <summary>
    /// 临时脏节点集合
    /// </summary>
    private List<TreeNodeV2> m_TempDirtyNodes;

    

    /// <summary>
    /// 节点数量改变回调
    /// </summary>
    public Action NodeNumChangeCallback;

    /// <summary>
    /// 节点值改变回调
    /// </summary>
    public Action<TreeNodeV2,int> NodeValueChangeCallback;

    /// <summary>
    /// 路径分隔字符
    /// </summary>
    public char SplitChar
    {
        get;
        private set;
    }

    /// <summary>
    /// 缓存的StringBuild
    /// </summary>
    public StringBuilder CachedSb
    {
        get;
        private set;
    }

    /// <summary>
    /// 红点树根节点
    /// </summary>
    public TreeNodeV2 Root
    {
        get;
        private set;
    }


    public ReddotManagerV2()
    {
        SplitChar = '/';
        m_AllNodes = new Dictionary<string, TreeNodeV2>();
        Root = new TreeNodeV2("Root");
        m_DirtyNodes = new HashSet<TreeNodeV2>();
        m_TempDirtyNodes = new List<TreeNodeV2>();
        CachedSb = new StringBuilder();
    }

    /// <summary>
    /// 添加节点值监听（路径不存在时会自动创建节点，属于"声明式订阅"）
    /// </summary>
    public TreeNodeV2 AddListener(string path,Action<int> callback)
    {
        if (callback == null)
        {
            return null;
        }

        TreeNodeV2 node = GetTreeNodeV2(path);
        node.AddListener(callback);

        return node;
    }

    /// <summary>
    /// 移除节点值监听（只查不建，路径不存在则直接忽略）
    /// </summary>
    public void RemoveListener(string path,Action<int> callback)
    {
        if (callback == null)
        {
            return;
        }

        TreeNodeV2 node = TryGetTreeNodeV2(path);
        if (node == null)
        {
            return;
        }
        node.RemoveListener(callback);
    }

    /// <summary>
    /// 移除所有节点值监听（只查不建，路径不存在则直接忽略）
    /// </summary>
    public void RemoveAllListener(string path)
    {
        TreeNodeV2 node = TryGetTreeNodeV2(path);
        if (node == null)
        {
            return;
        }
        node.RemoveAllListener();
    }

    /// <summary>
    /// 改变节点值（路径不存在时自动创建，创建后即可赋值）
    /// </summary>
    public void ChangeValue(string path,int newValue)
    {
        TreeNodeV2 node = GetTreeNodeV2(path);
        node.ChangeValue(newValue);
    }

    /// <summary>
    /// 获取节点值（只读，路径不存在返回 0，不会隐式创建节点）
    /// </summary>
    public int GetValue(string path)
    {
        TreeNodeV2 node = TryGetTreeNodeV2(path);
        if (node == null)
        {
            return 0;
        }

        return node.Value;
    }

    /// <summary>
    /// 获取节点：路径不存在则沿路径逐段创建（建树入口）
    /// </summary>
    public TreeNodeV2 GetTreeNodeV2(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new Exception("路径不合法，不能为空");
        }

        if (m_AllNodes.TryGetValue(path,out TreeNodeV2 node))
        {
            return node;
        }

        TreeNodeV2 cur = Root;
        int length = path.Length;

        int startIndex = 0;

        for (int i = 0; i < length; i++)
        {
            if (path[i] == SplitChar)
            {
                if (i == length - 1)
                {
                    throw new Exception("路径不合法，不能以路径分隔符结尾：" + path);
                }

                int endIndex = i - 1;
                if (endIndex < startIndex)
                {
                    throw new Exception("路径不合法，不能存在连续的路径分隔符或以路径分隔符开头：" + path);
                }

                TreeNodeV2 child = cur.GetOrAddChild(new RangeStringV2(path,startIndex,endIndex));

                //更新startIndex
                startIndex = i + 1;

                cur = child;
            }
        }

        //最后一个节点 直接用length - 1作为endIndex
        TreeNodeV2 target = cur.GetOrAddChild(new RangeStringV2(path, startIndex, length - 1));

        m_AllNodes.Add(path, target);

        return target;


    }

    /// <summary>
    /// 只读获取节点：路径不存在返回 null，不创建任何节点（无副作用）
    /// </summary>
    public TreeNodeV2 TryGetTreeNodeV2(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        m_AllNodes.TryGetValue(path, out TreeNodeV2 node);
        return node;
    }

    /// <summary>
    /// 移除节点（含其整棵子树：递归清空 m_AllNodes 索引，避免幽灵节点残留）
    /// </summary>
    public bool RemoveTreeNodeV2(string path)
    {
        TreeNodeV2 node = TryGetTreeNodeV2(path);
        if (node == null)
        {
            return false;
        }

        RemoveSubtreeIndex(node);
        m_AllNodes.Remove(path);
        return node.Parent.RemoveChild(new RangeStringV2(node.Name, 0, node.Name.Length - 1));
    }

    /// <summary>
    /// 递归清除指定节点子树在 m_AllNodes 中的所有索引
    /// </summary>
    private void RemoveSubtreeIndex(TreeNodeV2 node)
    {
        if (node.Children == null)
        {
            return;
        }

        foreach (TreeNodeV2 child in node.Children)
        {
            RemoveSubtreeIndex(child);
            m_AllNodes.Remove(child.FullPath);
        }
    }

    /// <summary>
    /// 移除所有节点
    /// </summary>
    public void RemoveAllTreeNodeV2()
    {
        Root.RemoveAllChild();
        m_AllNodes.Clear();
    }

    /// <summary>
    /// 管理器轮询
    /// </summary>
    public void Update()
    {
        if (m_DirtyNodes.Count == 0)
        {
            return;
        }

        m_TempDirtyNodes.Clear();
        foreach (TreeNodeV2 node in m_DirtyNodes)
        {
            m_TempDirtyNodes.Add(node);
        }
        m_DirtyNodes.Clear();

        //处理所有脏节点
        for (int i = 0; i < m_TempDirtyNodes.Count; i++)
        {
            m_TempDirtyNodes[i].ChangeValue();
        }
    }

    /// <summary>
    /// 标记脏节点
    /// </summary>
    public void MarkDirtyNode(TreeNodeV2 node)
    {
        if (node == null || node.Name == Root.Name)
        {
            return;
        }

        m_DirtyNodes.Add(node);
    }

}
