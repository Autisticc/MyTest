using UnityEngine;

/// <summary>
/// 启动入口：挂在场景中任意物体上即可
/// </summary>
public class Bootstrap : MonoBehaviour
{
    void Awake()
    {
        // 确保管理器存在
        if (FindObjectOfType<UIMgr>() == null)
            new GameObject("UIMgr").AddComponent<UIMgr>();
        if (FindObjectOfType<ResMgr>() == null)
            new GameObject("ResMgr").AddComponent<ResMgr>();
    }

    void Start()
    {
        UIMgr.instance.Init();
        UIMgr.instance.ShowUi("prefabs/MainPanel");
    }
}
