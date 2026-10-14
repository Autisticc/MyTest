using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionInputPanel : MonoBehaviour
{
    [Header("UI引用")]
    public TMP_Dropdown questionTypeDropdown;
    public TMP_InputField questionInput;
    public TMP_InputField codeInput;
    public TMP_InputField answerInput;
    public Button submitBtn;
    public Button closeBtn;

    private void Awake()
    {
        submitBtn.onClick.AddListener(OnClickSubmit);
        closeBtn.onClick.AddListener(OnClickClose);
    }

    private void Start()
    {
        RefreshDropdown();
    }

    private void RefreshDropdown()
    {
        questionTypeDropdown.ClearOptions();
        var types = QuestionDataMgr.instance.GetAllQuestionTypes();
        questionTypeDropdown.AddOptions(types);
    }

    /// <summary>
    /// 提交新题
    /// </summary>
    private void OnClickSubmit()
    {
        string type = questionTypeDropdown.options[questionTypeDropdown.value].text;
        string question = questionInput.text;
        string code = codeInput.text;
        string answer = answerInput.text;

        if (string.IsNullOrEmpty(question))
        {
            Debug.LogWarning("题目不能为空");
            return;
        }
        if (string.IsNullOrEmpty(answer))
        {
            Debug.LogWarning("答案不能为空");
            return;
        }

        QuestionDataMgr.instance.AddQuestion(type, new QuestionData
        {
            question = question,
            code = code,
            answer = answer
        });

        Debug.Log($"录入成功：[{type}] {question}");

        // 显示提示
        FlyTips.Show("录入成功！");

        // 清空输入
        questionInput.text = "";
        codeInput.text = "";
        answerInput.text = "";

        // 关闭面板
        OnClickClose();
    }

    private void OnClickClose()
    {
        Debug.Log("close btn");
        Destroy(gameObject);
    }
}
