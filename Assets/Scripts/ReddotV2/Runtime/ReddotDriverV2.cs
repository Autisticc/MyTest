using UnityEngine;

/// <summary>
/// 红点系统自动驱动组件。
///
/// 解决原框架"必须手动每帧调用 ReddotManagerV2.Instance.Update()"的接入坑：
/// 本组件随任意场景自动创建（不创建任何 UI、不改变场景内容），
/// 每帧自动驱动红点树结算，业务代码不再需要关心 Update 调用。
///
/// 若想手动控制驱动时机，删除本文件的 [RuntimeInitializeOnLoadMethod] 方法，
/// 改为把本组件手动挂到你的常驻物体上即可。
/// </summary>
public class ReddotDriverV2 : MonoBehaviour
{
    private static ReddotDriverV2 s_Instance;
    
    // 让被标记的静态方法在游戏运行时自动调用一次，不需要把脚本挂到任何 GameObject 上，也不需要场景里存在这个对象。
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoCreate()
    {
        if (s_Instance != null)
        {
            return;
        }

        GameObject go = new GameObject("ReddotDriverV2");
        s_Instance = go.AddComponent<ReddotDriverV2>();
        DontDestroyOnLoad(go);
    }

    private void Update()
    {
        // Update() 内部幂等：脏节点为空时直接返回，重复调用无副作用
        ReddotManagerV2.Instance.Update();
    }
}
