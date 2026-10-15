using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 默认滚动列表类
/// @Author B站：码农小徐 
/// 如有疑问或定制需求可私信联系作者 https://space.bilibili.com/304648178
/// </summary>
public class NormalBagUI : MonoBehaviour
{
    public GameObject bagItemGo;

    public GameObject contentGo;

    public int itemNum = 100;
    // Start is called before the first frame update
    void Start()
    {
        //模拟服务器数据
        for (int i = 0; i < itemNum; i++)
        {
            var go = GameObject.Instantiate(bagItemGo);
            go.transform.SetParent(contentGo.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.name = $"BagItem{i}";

            go.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();
            var image = go.transform.Find("Icon");
            image.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Icon/icon{i % 51 + 1}");

            var index = i;
            go.GetComponentInChildren<Button>().onClick.AddListener(() => print($"点击的是第{index}个"));

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
