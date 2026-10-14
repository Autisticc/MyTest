using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class MainUI : MonoBehaviour
{
    public static MainUI Self;

    public RectTransform Target1;
    public RectTransform Target2;
    public RectTransform Target3;
    public RectTransform GuideEnd;

    public void Awake()
    {
        Self = this;
        FindFirstObjectByType<GuideMask>().Init();
    }

    public void Start()
    {
        var targets = new[] { Target1, Target2, Target3, GuideEnd };
        for (int i = 0; i < targets.Length; i++)
        {
            var t = targets[i];
            t.gameObject.SetActive(false);
            t.GetComponent<Button>().onClick.AddListener(() => OnClick(t));
        }
    }
    
    public void StartGuide()
    {
        GuideMask.Self.Play(Target1);
    }

    private void OnClick(RectTransform t)
    {
        t.gameObject.SetActive(false);
        if (t == Target1) GuideMask.Self.Play(Target2);
        else if (t == Target2) GuideMask.Self.Play(Target3);
        else if (t == Target3) GuideMask.Self.Play(GuideEnd);
        else GuideMask.Self.Close();
    }
    
    public void OnGUI()
    {
        if (GUILayout.Button("=============  Close GuideMask  ============="))
        {
            GuideMask.Self.Close();
        }
        if (GUILayout.Button("=============  start guide  ============="))
        {
            StartGuide();
        }
    }
}
