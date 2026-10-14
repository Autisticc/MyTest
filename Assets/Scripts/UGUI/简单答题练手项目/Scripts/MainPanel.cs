using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainPanel : MonoBehaviour
{
    [Header("UI引用")]
    public TMP_Dropdown questionTypeDropdown;
    public TMP_Text questionText;
    public Button inputQuestionBtn;
    public Button answerBtn;
    public Button nextBtn;

    // 当前题目
    private QuestionData m_currentQuestion;
    // 是否已经显示答案
    private bool m_showAnswer;

    private void Awake()
    {
        inputQuestionBtn.onClick.AddListener(OnClickInputQuestion);
        answerBtn.onClick.AddListener(OnClickAnswer);
        nextBtn.onClick.AddListener(OnClickNext);
        questionTypeDropdown.onValueChanged.AddListener(OnTypeChanged);
    }

    private void Start()
    {
        RefreshDropdown();
        OnClickNext();
    }

    /// <summary>
    /// 刷新下拉框选项
    /// </summary>
    private void RefreshDropdown()
    {
        questionTypeDropdown.ClearOptions();
        var types = QuestionDataMgr.instance.GetAllQuestionTypes();
        questionTypeDropdown.AddOptions(types);
    }

    /// <summary>
    /// 类别切换
    /// </summary>
    private void OnTypeChanged(int index)
    {
        OnClickNext();
    }

    /// <summary>
    /// 录入新题按钮
    /// </summary>
    private void OnClickInputQuestion()
    {
        UIMgr.instance.ShowUi("prefabs/QuestionInputPanel");
    }

    /// <summary>
    /// 看答案按钮
    /// </summary>
    private void OnClickAnswer()
    {
        m_showAnswer = true;
        RefreshQuestionText();
    }

    /// <summary>
    /// 下一题按钮
    /// </summary>
    private void OnClickNext()
    {
        m_showAnswer = false;
        string type = questionTypeDropdown.options[questionTypeDropdown.value].text;
        m_currentQuestion = QuestionDataMgr.instance.GetRandomQuestion(type);
        RefreshQuestionText();
    }

    /// <summary>
    /// 刷新题目文本显示
    /// </summary>
    private void RefreshQuestionText()
    {
        if (m_currentQuestion == null)
        {
            questionText.text = "暂无题目";
            return;
        }

        if (m_showAnswer)
        {
            string codePart = string.IsNullOrEmpty(m_currentQuestion.code)
                ? "" : $"\n代码:\n{m_currentQuestion.code}\n";
            questionText.text = $"题目:\n{m_currentQuestion.question}\n{codePart}\n解答:\n{m_currentQuestion.answer}";
        }
        else
        {
            questionText.text = $"题目:\n{m_currentQuestion.question}";
        }
    }
}
