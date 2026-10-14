using System.Collections;
using System.Collections.Generic;
using UGUI.简单的背包系统.Scripts.manager;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UGUI.简单的背包系统.Scripts.Config
{
    public class BackpackPanel : MonoBehaviour
    {

        private void Awake()
        {
            // 订阅事件
            EventDispatcher.Instance.Regist(EventNameDef.EVENT_UPDATE_PROP_CNT, OnEventUpdatePropCnt);
        }

        private void OnDestroy()
        {
            // 注销事件
            EventDispatcher.Instance.UnRegist(EventNameDef.EVENT_UPDATE_PROP_CNT, OnEventUpdatePropCnt);
        }

        private void Start()
        {
            propItemUi.SetActive(false);
            // 关闭按钮
            closeBtn.onClick.AddListener(() => { Destroy(gameObject); });
            // 使用道具按钮
            usePropBtn.onClick.AddListener(() =>
            {
                if (-1 == m_selectPropId) return;
                var cfg = PropCfg.Instance.GetCfg(m_selectPropId);
                TipsUi.Show("使用了" + cfg.name);
                BackPackMgr.Instance.ChangePropCnt(m_selectPropId, -1);
            });
            // 创建道具列表
            CreatePropList();
        }

        /// <summary>
        /// 道具数量发生变化
        /// </summary>
        /// <param name="args"></param>
        private void OnEventUpdatePropCnt(params object[] args)
        {
            int id = (int)args[0];
            int cnt = (int)args[1];

            if (cnt <= 0)
            {
                if (m_propUiDic.ContainsKey(id))
                {
                    Destroy(m_propUiDic[id].obj);
                    m_propUiDic.Remove(id);
                    if (id == m_selectPropId)
                        AutoSelectOneProp();
                }
            }
            else
            {
                if (m_propUiDic.ContainsKey(id))
                {
                    m_propUiDic[id].cnt.text = "x" + cnt;
                }
                else
                {
                    CreatePropItem(id, cnt);
                }
            }
        }

        /// <summary>
        /// 创建道具列表
        /// </summary>
        private void CreatePropList()
        {
            foreach (var item in BackPackMgr.Instance.propsDic)
            {
                CreatePropItem(item.Key, item.Value);
            }

            // 自动选择一个道具
            AutoSelectOneProp();
        }

        /// <summary>
        /// 自动选择一个道具
        /// </summary>
        private void AutoSelectOneProp()
        {
            var itor = m_propUiDic.GetEnumerator();
            if (itor.MoveNext())
            {
                var item = itor.Current.Value;
                item.tgl.isOn = true;
                itor.Dispose();
                OnPropItemSelected(item.propId);
            }
            else
            {
                OnPropItemSelected(-1);
            }
        }

        /// <summary>
        /// 创建一个道具item
        /// </summary>
        /// <param name="propId">道具id</param>
        /// <param name="cnt">道具数量</param>
        private void CreatePropItem(int propId, int cnt)
        {
            if (m_propUiDic.ContainsKey(propId)) return;
            if (cnt <= 0) return;
            PropItemUI ui = new PropItemUI();
            var obj = Instantiate(propItemUi);
            obj.SetActive(true);
            obj.transform.SetParent(propItemUi.transform.parent, false);

            // 修正 anchor 和 pivot，让 Grid Layout Group 能正确排列
            var rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0.5f, 0.5f);
            ui.obj = obj;
            ui.propId = propId;
            ui.icon = obj.transform.Find("Button/icon").GetComponent<Image>();
            ui.cnt = obj.transform.Find("Button/cnt").GetComponent<TMP_Text>();
            ui.tgl = obj.transform.Find("Button").GetComponent<Toggle>();
            ui.tgl.onValueChanged.AddListener((v) =>
            {
                if (ui.tgl.isOn) OnPropItemSelected(propId);
            });

            var cfg = PropCfg.Instance.getProCfgItemByID(propId);
            if (null != cfg)
            {
                var sprite = ResourceMgr.Instance.LoadSprite(cfg.sprite);
                ui.icon.sprite = sprite;
                //ui.icon.SetNativeSize();
            }

            ui.cnt.text = "x" + cnt;
            m_propUiDic[propId] = ui;

            // 强制重建布局，让 Grid Layout Group 重新排列
            LayoutRebuilder.ForceRebuildLayoutImmediate(propItemUi.transform.parent as RectTransform);
        }

        /// <summary>
        /// 道具被选中
        /// </summary>
        /// <param name="propId"></param>
        private void OnPropItemSelected(int propId)
        {
            m_selectPropId = propId;
            if (-1 == m_selectPropId)
            {
                // 没有道具被选中
                rightInfoRoot.SetActive(false);
            }
            else
            {
                rightInfoRoot.SetActive(true);
                var cfg = PropCfg.Instance.GetCfg(propId);
                nameText.text = cfg.name;
                descText.text = cfg.desc;
                icon.sprite = ResourceMgr.Instance.LoadSprite(cfg.sprite);
            }
        }

        public Button closeBtn;
        public GameObject propItemUi;
        public GameObject rightInfoRoot;

        public TMP_Text nameText;
        public TMP_Text descText;
        public Image icon;
        public Button usePropBtn;

        private int m_selectPropId;


        /// <summary>
        /// 道具列表ui缓存
        /// </summary>
        private Dictionary<int, PropItemUI> m_propUiDic = new Dictionary<int, PropItemUI>();

        private class PropItemUI
        {
            public int propId;
            public GameObject obj;
            public Image icon;
            public Toggle tgl;
            public TMP_Text cnt;

        }
    }
}

