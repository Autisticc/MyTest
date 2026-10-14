using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HttpHelper : Singleton<HttpHelper>
{
    public const string WebUrl = "http://localhost:8080/";

    public void GetAllQuestion(Action<string> cb)
    {
        StartCoroutine(CoroutineHttpGet(WebUrl + "GetQuestionTypes", cb));
    }
    
    public void GetOneQuestionByType(string questionType, Action<string> cb)
    {
        questionType = UnityWebRequest.EscapeURL(questionType);
        StartCoroutine(CoroutineHttpGet(WebUrl + "GetOneQuestionByType?questionType=" + questionType, cb));
    }
    
    public void PostAddQuestion(string question, string questionType, string code, string answer, Action<string> cb)
    {
        WWWForm form = new WWWForm();
        form.AddField("question", question);
        form.AddField("question_type", questionType);
        form.AddField("code", code);
        form.AddField("answer", answer);
        StartCoroutine(CoroutineHttpPost(WebUrl + "PostAddQuestion", form, cb));
    }

    IEnumerator CoroutineHttpGet(string url, Action<string> cb)
    {
        // UnityWebRequest req = UnityWebRequest.Get(url);
        // yield return req.SendWebRequest();
        yield return new WaitForSeconds(2f);
        // if (!string.IsNullOrEmpty(req.error))
        // {
        //     Debug.Log(req.error);
        //     yield break;
        // }
        // 把请求响应的response内容放入回调事件
        // cb?.Invoke(req.downloadHandler.text);
        // req.Dispose();
        cb?.Invoke(url + "tetewljtglewjglewjg");
    }
    
    // WWWForm应该是封装好的表单类型
    IEnumerator CoroutineHttpPost(string url, WWWForm form, Action<string> cb)
    {
        UnityWebRequest req = UnityWebRequest.Post(url, form);
        yield return req.SendWebRequest();
        if (!string.IsNullOrEmpty(req.error))
        {
            Debug.Log(req.error);
            // cb?.Invoke(req.error);
            yield break;
        }
        cb?.Invoke(req.downloadHandler.text);
        
        req.Dispose();
    }
        
}
