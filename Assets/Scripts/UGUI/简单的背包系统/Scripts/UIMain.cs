using UGUI.简单的背包系统.Scripts.Config;
using UGUI.简单的背包系统.Scripts.manager;
using UnityEngine;

namespace UGUI.简单的背包系统.Scripts
{
    public class UIMain : MonoBehaviour
    {
        public Canvas canvas;

        private void Awake()
        {
            PropCfg.Instance.LoadCfg();
            BackPackMgr.Instance.Init();
            canvas = GetComponent<Canvas>();
            PanelMgr.Instance.Init(canvas);
        }

        void Start()
        {
            PanelMgr.Instance.ShowPanel(PanelName.PlazaPanel);
        }
    }
}