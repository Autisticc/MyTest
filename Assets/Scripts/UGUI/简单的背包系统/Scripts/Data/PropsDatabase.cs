using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace UGUI.简单的背包系统.Scripts.Data
{
    public class PropsDatabase
    {
        
        // 从本地读取数据
        public static Dictionary<int, int> LoadData()
        {
            var jsonStr = PlayerPrefs.GetString("props", "{}");
            Debug.Log("LoadData: \n" + jsonStr);
            var result = JsonMapper.ToObject<Dictionary<string, int>>(jsonStr);
            
            Dictionary<int, int> data = new Dictionary<int, int>();
            foreach (var item in result)
            {
                data[int.Parse(item.Key)] = item.Value;
            }
            return data;
        }
        
        // 写入数据到本地
        public static void SaveData(Dictionary<int, int> data)
        {
            var jsonStr = JsonMapper.ToJson(data);
            PlayerPrefs.SetString("props", jsonStr);
        }
        
    }
}