using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace XScrollView
{
    /// <summary>
    /// 滚动列表类
    /// @Author B站：码农小徐 
    /// 如有疑问或定制需求可私信联系作者 https://space.bilibili.com/304648178
    /// </summary>
    public class UIXScrollView : MonoBehaviour
    {
        //scrollView info
        private ScrollRect scrollRect;
        private float height;
        private float width;
        
        [Header("=== 滚动列表的content ===")]
        public Transform content;
        private RectTransform contentRect;

        //cell info
        public BagScrollViewItem cell;
        [Header("=== 背包列表物品水平垂直间隔 ===")]
        public float offsetX = 2f;
        public float offsetY = 10f;
        
        private float cellHeight;
        private float cellWidth;


        //列表能显示的行列
        private int row;

        private int column;

        //列表最多显示的cell个数
        private int maxShowCellNum;

        //列表显示的cell列表就是重复利用的空位，用于设置位置,只存了0-显示的个数这么多
        private List<BaseXScrollViewItem> showCellItems;

        //总行数
        private int totalRow;

        //上次显示的节点序号
        private int preStartIndex = 0;


        //总数，这个就是背包物品的总数，但是值在BagUI里面控制了
        public int totalNum = 100;
        
        //事件相关
        public UnityAction<BaseXScrollViewItem, int> updateItemAction;
        
        private UnityAction<int> clickItemAction;
        

        private void OnDestroy()
        {
            updateItemAction = null;
            clickItemAction = null;
        }
        
        // 
        public void InitXScrollView(int num)
        {
            totalNum = num;
            
            // 整个滚动列表的高度和宽度
            scrollRect = GetComponent<ScrollRect>();
            var rect = this.GetComponent<RectTransform>().rect;
            height = rect.height;
            width = rect.width;

            // 里面物体的高度和宽度
            var cellRect = cell.GetComponent<RectTransform>().rect;
            cellHeight = cellRect.height;
            cellWidth = cellRect.width;
            
            // ==========确定整个列表可以显示的物体数量
            //列, 滚动列表宽/物品框+间隔, 向下取整，变小
            column = Mathf.FloorToInt(width / (cellWidth + offsetX));
            //行，滚动列表的高/物品高和间隔，向上取整，变大
            row = Mathf.CeilToInt(height / (cellHeight + offsetY));
            
            //最多只创建屏幕显示的个数这么多，这个列乘行就是整个列表可以显示的物体数量。
            maxShowCellNum = column * (row + 1);
            
            // ==========确定conten的高度有多高
            // 先得到总行数，总数除以列数就是了
            totalRow = Mathf.CeilToInt((float)totalNum / column);

            //隐藏模版cell
            cell.gameObject.SetActive(false);

            //设置content的大小
            contentRect = content.GetComponent<RectTransform>();
            var contentHeight = totalRow * (cellHeight + offsetY); //向上取整，并且乘以每一行的高
            var contentWidth = contentRect.sizeDelta.x;// 宽度已经和列表保持一致，这里只是获取，可能只能这样更新吧
            var contentSize = new Vector2(contentWidth, contentHeight);
            contentRect.sizeDelta = contentSize;


            //开始创建节点
            StartCoroutine(CreateCell());

            //滚动事件
            scrollRect.onValueChanged.AddListener(ScrollViewOnValueChanged);

        }


        /// <summary>
        /// 添加列表项更新事件
        /// </summary>
        /// <param name="updateAction"></param>
        public void AddUpdateCellAction(UnityAction<BaseXScrollViewItem, int> updateAction)
        {
            updateItemAction += updateAction;
        }
        
        
        /// <summary>
        /// 添加列表项点击事件
        /// </summary>
        /// <param name="clickAction"></param>
        public void AddCellClickAction(UnityAction<int> clickAction)
        {
            clickItemAction += clickAction;
        }


        /// <summary>
        /// 创建格子
        /// </summary>
        /// <returns></returns>
        IEnumerator CreateCell()
        {
            //最多只创建屏幕显示的个数这么多，如果总数少于最大显示数去小的
            var showCell = Mathf.Min(maxShowCellNum, totalNum);
            
            showCellItems = new List<BaseXScrollViewItem>(showCell);
            
            for (int i = 0; i < showCell; i++)
            {
                yield return null;
                
                var index = i;

                var go = GameObject.Instantiate(cell.gameObject, content);
                go.name = $"Cell_{index}";
                var scrollItem = go.GetComponent<BaseXScrollViewItem>();
                
                UpdateCell(scrollItem, index);
                scrollItem.AddButtonClickListener(clickItemAction);
                go.SetActive(true);

                showCellItems.Add(scrollItem);
                
            }
        }


        /// <summary>
        /// 更新节点
        /// </summary>
        /// <param name="scrollItem"></param>
        /// <param name="index"></param>
        void UpdateCell(BaseXScrollViewItem scrollItem, int index)
        {
            // 更新这个物品的位置，也就是让他找到一个cell↙，GetCellPos就是根据索引找到他的位置
            // 更新cell的位置每个cell都挂载了BaseXScrollViewItem
            scrollItem.UpdateCellPos(GetCellPos(index));
            
            updateItemAction?.Invoke(scrollItem, index);
        }



        /// <summary>
        /// 获取节点位置
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Vector2 GetCellPos(int index)
        {
            var curColumn = index % column; //当前的列
            var curX = curColumn * (cellWidth + offsetX); //当前的x坐标

            var curRow = index / column; //当前的行
            var curY = -curRow * (cellHeight + offsetY); //当前的y坐标

            var pos = new Vector2(curX, curY);
            Debug.Log(index+"位置：" + pos);
            return pos;
        }

      

        /// <summary>
        /// 滚动事件
        /// </summary>
        /// <param name="v"></param>
        void ScrollViewOnValueChanged(Vector2 v)
        {
            UpdateScrollView();
        }

        // 需要重绑定刷新就把forceUpdate = true传入这个方法
        public void UpdateScrollView(bool forceUpdate = false)
        {
            // content的y
            var y = contentRect.anchoredPosition.y;
            //可能会小于0，小于0归0，因为conten的锚点在顶端
            y = Mathf.Max(0, y);
            //拖动的距离，超过一行时，则顶部的一行移动到底部显示，根据往下拖的距离算出移动了多少行
            var moveRow = Mathf.FloorToInt(y / (cellHeight + offsetY));
            //视图范围内,移动的行数+视图显示的行数<=总行数
            if (moveRow >= 0 && (moveRow + row) <= totalRow)
            {
                // 序号是从0开始所以不用+1，移动的行数乘以总列数就是移动了多少格，每次计算都是从0开始移动的
                var startIndex = moveRow * column; //起始序号

                //========== 差量更新（环形复用，只搬边界行，中间格子不动） ==========
                //滚动的行差：正=向下滚，负=向上滚
                var deltaRow = (startIndex - preStartIndex) / column;

                //1. 位置对齐：只要跨行了，就先搬运边界行，保证格子位置正确（与是否强制刷新无关）
                if (deltaRow != 0)
                {
                    RecycleBoundaryCells(deltaRow);
                }

                //2. 数据刷新：强制刷新时，无论是否跨行，都全量重绑所有格子数据，说是可能物品信息更新。
                if (forceUpdate)
                {
                    for (int i = 0; i < showCellItems.Count; i++)
                    {
                        var item = showCellItems[i];
                        if (item.index < totalNum)
                        {
                            item.gameObject.SetActive(true);
                            updateItemAction?.Invoke(item, item.index);
                        }
                    }
                }

                preStartIndex = startIndex;

                //========== 原全量刷新逻辑（注释保留，供对比） ==========
                //和上次的起始序号不同才进行刷新
                //if (!forceUpdate && startIndex == preStartIndex)
                //{
                //    return;
                //}

                //print(startIndex + ":" + preStartIndex);

                //*更新所有cell，从开始的位置到能显示的最大个数，遍历每个cell格子位置，刷新这些位置，比如你有36个位置就刷新36个
                //for (int i = startIndex; i < startIndex + maxShowCellNum; i++)
                //{
                //    var index = i;
                //    ScrollUpdateCell(index, startIndex);
                //}

                //preStartIndex = startIndex;
                //========== 原全量刷新逻辑结束 ==========
            }
        }

        /// <summary>
        /// 差量更新：滚动跨行时，只把滚出视口的边界行搬到另一头，中间格子完全不动
        /// </summary>
        /// <param name="deltaRow">行差，正=向下滚，负=向上滚</param>
        void RecycleBoundaryCells(int deltaRow)
        {
            var deltaCell = deltaRow * column; //换算成格子数（含正负）
            var newStart = preStartIndex + deltaCell; //本次列表起点序号

            int head, tail, add;
            if (deltaRow > 0)
            {
                //向下滚：滚出的是池头 index 最小的一批，搬到池尾（index 加满池容量）
                head = preStartIndex;
                tail = newStart;
                add = maxShowCellNum;
            }
            else
            {
                //向上滚：滚出的是池尾 index 最大的一批，搬回池头（index 减满池容量）
                head = newStart + maxShowCellNum;
                tail = preStartIndex + maxShowCellNum;
                add = -maxShowCellNum;
            }
            Debug.Log("showCellItems.Count:" + showCellItems.Count);
            for (int g = 0; g < showCellItems.Count; g++)
            {
                var item = showCellItems[g];
                var idx = item.index;
                //命中滚出范围才搬运，其余中间格子不动
                if (idx >= head && idx < tail)
                {
                    var newIdx = idx + add;
                    item.index = newIdx;

                    //超出总数的不显示
                    if (newIdx >= totalNum)
                    {
                        item.gameObject.SetActive(false);
                        continue;
                    }

                    item.gameObject.SetActive(true);
                    UpdateCell(item, newIdx);
                }
            }
        }

        /// <summary>
        /// 更新节点信息
        /// </summary>
        /// <param name="index"></param>
        /// <param name="startIndex"></param>
        void ScrollUpdateCell(int index, int startIndex)
        {
            // 比如我传12进来，9宫格拉3行开始就是12，scrollViewItem[0]就是第12个物品的坑位
            var scrollViewItem = showCellItems[index - startIndex];
            //超出总数的不显示
            if (index >= totalNum)
            {
                scrollViewItem.gameObject.SetActive(false);
                return;
            }

            scrollViewItem.gameObject.SetActive(true);

            UpdateCell(scrollViewItem, index);
        }

      

    }
}
