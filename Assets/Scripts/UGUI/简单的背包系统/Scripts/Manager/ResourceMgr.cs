using System.Collections.Generic;
using UnityEngine;

namespace UGUI.简单的背包系统.Scripts.manager
{
    public class ResourceMgr
    {
        private static ResourceMgr _instance;
        public static ResourceMgr Instance => _instance ??= new ResourceMgr();

        /// <summary>
        /// 精灵资源
        /// </summary>
        private Dictionary<string, Sprite> m_spritesDic = new Dictionary<string, Sprite>();
        /// <summary>
        /// 界面资源
        /// </summary>
        private Dictionary<string, GameObject> m_panelsDic = new Dictionary<string, GameObject>();

        // 加载精灵图
        public Sprite LoadSprite(string name)
        {
            if (m_spritesDic.ContainsKey(name))
            {
                return m_spritesDic[name];
            }
            var obj = Resources.Load<GameObject>("Sprite/" + name);

            var sprite = obj.GetComponent<SpriteRenderer>().sprite;
            m_spritesDic.Add(name, sprite);
            return sprite;
        }

        // 加载预设界面
        public GameObject LoadPanel(string name)
        {
            if (m_panelsDic.ContainsKey(name))
                return m_panelsDic[name];
            var prefab = Resources.Load<GameObject>("Panel/" + name);
            m_panelsDic.Add(name, prefab);
            return prefab;
        }
    }
}
