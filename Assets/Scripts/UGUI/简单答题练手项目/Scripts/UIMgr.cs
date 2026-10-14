using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr : Singleton<UIMgr>
{
    private Transform m_canvasTrans;
    
    public void Init()
    {
        m_canvasTrans = GameObject.Find("Canvas").transform;
    }

    public GameObject ShowUi(string resPath)
    {
        var prefab = ResMgr.instance.GetRes(resPath);
        if (!prefab) return prefab;
        // 示例化时直接设置parent
        var uiObj = Object.Instantiate(prefab, m_canvasTrans,false);
        return uiObj;
    }
}
