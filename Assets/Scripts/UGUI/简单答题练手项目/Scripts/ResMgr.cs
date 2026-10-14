using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResMgr : Singleton<ResMgr>
{
    private Dictionary<string, GameObject> m_prefabs = new Dictionary<string, GameObject>();

    public GameObject GetRes(string resPath)
    {
        // 如果存入了字典直接返回，没有就在Resources加载一下
        if (m_prefabs.ContainsKey(resPath))
        {
            return m_prefabs[resPath];
        }
        var prefab = Resources.Load<GameObject>(resPath);
        return prefab != null ? (m_prefabs[resPath] = prefab) : null;
    }
    
}
