using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace UGUI.简单的背包系统.Scripts.Config
{
    public class PropCfgItem
    {
        public int id;
        public string name;
        public string sprite;
        public string desc;
    }

    // 属性配置, 读取配置表
    public class PropCfg
    {
        private static PropCfg _instance;
        public static PropCfg Instance => _instance ??= new PropCfg();

        private Dictionary<int, PropCfgItem> m_cfgDic = new Dictionary<int, PropCfgItem>();

        // 加载配置，初始化字典序号对配置
        public void LoadCfg()
        {
            var cfgJson = Resources.Load<TextAsset>("PropCfg");
            if (cfgJson == null)
            {
                Debug.LogError("Resources 下没有找到 PropCfg 配置文件");
                return;
            }
            var cfg = JsonMapper.ToObject<List<PropCfgItem>>(cfgJson.text);
            foreach (var propCfgItem in cfg)
            {
                m_cfgDic[propCfgItem.id] = propCfgItem;
            }
        }

        public PropCfgItem GetCfg(int id)
        {
            return m_cfgDic.ContainsKey(id) ? m_cfgDic[id] : null;
        }

        public PropCfgItem getProCfgItemByID(int id)
        {
            return GetCfg(id);
        }
    }
}
