using System.Collections.Generic;
using UGUI.简单的背包系统.Scripts.Data;
using UnityEngine.UIElements;

namespace UGUI.简单的背包系统.Scripts.manager
{
    public class BackPackMgr
    {
        private static BackPackMgr _instance;
        public static BackPackMgr Instance => _instance ??= new BackPackMgr();

        // 记录背包道具数量
        private Dictionary<int, int> m_backItems = new Dictionary<int, int>();

        // 外部访问的道具字典（兼容旧代码）
        public Dictionary<int, int> propsDic => m_backItems;

        public void Init()
        {
            m_backItems = PropsDatabase.LoadData();
        }

        // 修改道具数量
        public void ChangePropCnt(int id, int deltaCnt)
        {
            if (m_backItems.ContainsKey(id))
            {
                m_backItems[id] += deltaCnt;
            }
            else
            {
                m_backItems[id] = deltaCnt;
            }
            if (m_backItems[id] < 0)
            {
                m_backItems[id] = 0;
            }
            // 写入数据到本地
            PropsDatabase.SaveData(m_backItems);
            // 抛出事件
            EventDispatcher.Instance.DispatchEvent(EventNameDef.EVENT_UPDATE_PROP_CNT, id, m_backItems[id]);
        }
    }
}
