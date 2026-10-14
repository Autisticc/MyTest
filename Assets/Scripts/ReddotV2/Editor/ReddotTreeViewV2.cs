using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

/// <summary>
/// 红点树视图
/// </summary>
public class ReddotTreeViewV2 : TreeView
{
    private ReddotTreeViewItemV2 m_Root;

    private int m_Id;

    private System.Action<TreeNodeV2, int> m_RepaintHandler;

    public ReddotTreeViewV2(TreeViewState state) : base(state)
    {
        Reload();

        useScrollView = true;

        ReddotManagerV2.Instance.NodeNumChangeCallback += Reload;
        // 值变化只需重绘当前视图（回调参数在此场景用不到，忽略即可）
        m_RepaintHandler = (node, value) => Repaint();
        ReddotManagerV2.Instance.NodeValueChangeCallback += m_RepaintHandler;
    }

    protected override TreeViewItem BuildRoot()
    {
        m_Id = 0;

        m_Root = PreOrder(ReddotManagerV2.Instance.Root);
        m_Root.depth = -1;

        SetupDepthsFromParentsAndChildren(m_Root);

        return m_Root;
    }

    private ReddotTreeViewItemV2 PreOrder(TreeNodeV2 root)
    {
        if (root == null)
        {
            return null;
        }

        ReddotTreeViewItemV2 item = new ReddotTreeViewItemV2(m_Id++, root);

        if (root.ChildrenCount > 0)
        {
            foreach (TreeNodeV2 child in root.Children)
            {
                item.AddChild(PreOrder(child));
            }
        }

        return item;
    }

    public void OnDestroy()
    {
        ReddotManagerV2.Instance.NodeNumChangeCallback -= Reload;
        ReddotManagerV2.Instance.NodeValueChangeCallback -= m_RepaintHandler;
    }


}
