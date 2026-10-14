using UnityEngine;
using TMPro;

/// <summary>
/// 浮动提示语
/// </summary>
public class FlyTips : MonoBehaviour
{
    public TMP_Text tipText;
    public float showTime = 1.5f;

    private float m_timer;

    private void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= showTime)
        {
            Destroy(gameObject);
        }
    }

    public void SetTip(string text)
    {
        tipText.text = text;
    }

    /// <summary>
    /// 全局调用：显示一个提示
    /// </summary>
    public static void Show(string text)
    {
        var go = UIMgr.instance.ShowUi("prefabs/FlyTips");
        var tips = go.GetComponent<FlyTips>();
        if (tips != null)
        {
            tips.SetTip(text);
        }
    }
}
