using UGUI.简单的背包系统.Scripts.manager;
using UnityEngine;
using UnityEngine.UI;


namespace UGUI.简单的背包系统.Scripts.Config
{
    /// <summary>
    /// 大厅界面
    /// </summary>
    public class PlazaPanel : MonoBehaviour
    {
        public Button packpackBtn;
        public Button addPropBtn;

        void Start()
        {
            // 背包按钮
            packpackBtn.onClick.AddListener(() => 
            {
                PanelMgr.Instance.ShowPanel(PanelName.BackpackPanel);
            });
            // 加道具按钮
            addPropBtn.onClick.AddListener(() => {
                // 随机加一个道具
                var propId = Random.Range(1, 8);
                BackPackMgr.Instance.ChangePropCnt(propId, 1);
                var cfg = PropCfg.Instance.GetCfg(propId);
                TipsUi.Show("恭喜获得" + cfg.name + "x1");
            });
        }
    }
}