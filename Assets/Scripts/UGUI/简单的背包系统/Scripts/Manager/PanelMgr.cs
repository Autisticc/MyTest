using UnityEngine;

namespace UGUI.简单的背包系统.Scripts.manager
{
    public class PanelMgr
    {
        private static PanelMgr _instance;
        public static PanelMgr Instance => _instance ??= new PanelMgr();

        // 缓存Canvas
        private Transform m_canvasTrans;

        // 得到画布
        public void Init(Canvas canvas)
        {
            m_canvasTrans = canvas.transform;
        }

        // 显示界面
        public GameObject ShowPanel(string panelName)
        {
            var panel = Resources.Load<GameObject>("Panel/" + panelName);
            var obj = Object.Instantiate(panel, m_canvasTrans);
            return obj;
        }
    }
}

public class PanelName
{
    public const string BackpackPanel = "BackpackPanel";
    public const string PlazaPanel = "PlazaPanel";
    public const string TipsUi = "TipsUi";
}
