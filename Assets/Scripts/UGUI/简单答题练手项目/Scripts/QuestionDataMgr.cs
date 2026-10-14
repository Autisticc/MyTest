using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 题目数据结构
/// </summary>
[Serializable]
public class QuestionData
{
    public string question;
    public string code;
    public string answer;
}

/// <summary>
/// 静态题目数据管理器（模拟服务端，后续接真实服务器时替换这里即可）
/// </summary>
public class QuestionDataMgr
{
    private static QuestionDataMgr _instance;
    public static QuestionDataMgr instance => _instance ??= new QuestionDataMgr();

    // 所有题目类别
    public List<string> questionTypes = new List<string>
    {
        "C#基础",
        "Unity基础",
        "算法",
        "设计模式",
        "图形学与渲染"
    };

    // 按类别存储的题目库
    private Dictionary<string, List<QuestionData>> m_questionBank = new Dictionary<string, List<QuestionData>>();

    private QuestionDataMgr()
    {
        InitStaticData();
    }

    /// <summary>
    /// 初始化静态测试数据
    /// </summary>
    private void InitStaticData()
    {
        // C#基础
        m_questionBank["C#基础"] = new List<QuestionData>
        {
            new QuestionData
            {
                question = "请简述 private、public、protected、internal 的区别",
                code = "",
                answer = "public: 对任何类和成员都公开，无限制访问\nprivate: 仅对该类公开\nprotected: 对该类和其派生类公开\ninternal: 只能在包含该类的程序集中访问该类"
            },
            new QuestionData
            {
                question = "C#中的委托是什么？",
                code = "",
                answer = "C#所有的委托派生自System.Delegate类，委托是存有对某个方法的引用的一种引用类型变量，委托变量可以当作另一个方法的参数来进行传递，实现事件和回调方法。\n\n有点类似C++中的函数指针，但又是有所不同。\n在C++中，函数指针不是类型安全的，它指向的是内存中的某一个位置，我们无法判断这个指针实际指向什么。而C#的委托完全不同，它是类型安全的。"
            },
            new QuestionData
            {
                question = "C#中 interface 和 abstract class 的区别？",
                code = "",
                answer = "1. 抽象类可以有构造函数、成员变量、普通方法；接口只能有方法签名，不能有实现\n2. 一个类只能继承一个抽象类，但可以实现多个接口\n3. 抽象类适合用作模板，接口适合用作规范契约"
            }
        };

        // Unity基础
        m_questionBank["Unity基础"] = new List<QuestionData>
        {
            new QuestionData
            {
                question = "Unity中 deltaTime 是什么？",
                code = "",
                answer = "Time.deltaTime 表示从上一帧到当前帧的时间间隔（秒）。\n在Update中使用它，可以让移动、旋转等操作与帧率无关，保持不同设备上速度一致。"
            },
            new QuestionData
            {
                question = "Unity中协程的原理是什么？",
                code = "",
                answer = "协程（Coroutine）本质上是一个迭代器方法，通过 yield 关键字在特定时机挂起和恢复执行。\nUnity每帧会遍历所有正在运行的协程，检查其 yield 条件是否满足，满足则继续执行下一段逻辑。\n协程不是多线程，它仍然在主线程上分时执行。"
            },
            new QuestionData
            {
                question = "Vector3.Lerp 和 Vector3.MoveTowards 的区别？",
                code = "",
                answer = "Lerp 是按比例插值，速度会越来越慢（渐近）；\nMoveTowards 是按固定速度移动，不会减速，到达目标后停止。"
            }
        };

        // 算法
        m_questionBank["算法"] = new List<QuestionData>
        {
            new QuestionData
            {
                question = "快速排序的时间复杂度是多少？",
                code = "",
                answer = "平均时间复杂度 O(nlogn)，最坏 O(n²)（当数组已经有序时）。\n空间复杂度 O(logn) 递归栈。\n不稳定排序。"
            },
            new QuestionData
            {
                question = "哈希表如何解决哈希冲突？",
                code = "",
                answer = "1. 链地址法：每个桶维护一个链表，冲突元素追加到链表\n2. 开放寻址法：冲突后继续探查下一个空位置（线性探测、二次探测）\n3. 再哈希：用另一个哈希函数重新计算\n4. 建立公共溢出区"
            }
        };

        // 设计模式
        m_questionBank["设计模式"] = new List<QuestionData>
        {
            new QuestionData
            {
                question = "单例模式的作用和注意事项？",
                code = "",
                answer = "作用：保证一个类只有一个实例，并提供全局访问点。\n注意事项：\n1. 线程安全（多线程环境下加锁）\n2. 场景切换时单例对象是否销毁\n3. 懒汉式 vs 饿汉式\n4. 避免在单例中持有Unity对象引用导致内存泄漏"
            }
        };

        // 图形学与渲染
        m_questionBank["图形学与渲染"] = new List<QuestionData>
        {
            new QuestionData
            {
                question = "光照模型有哪些？",
                code = "",
                answer = "现实世界的光照是非常复杂的，若要准确模拟，整体计算量会非常大。一般光照模型会包含以下几个要素：自发光、环境光、漫反射、高光。\n\n常见的光照模型：\n1、Lambert模型：理想漫反射模型\n2、Blinn-Phong模型：在Phong基础上优化了高光计算\n3、PBR基于物理的渲染"
            }
        };
    }

    /// <summary>
    /// 获取所有题目类别
    /// </summary>
    public List<string> GetAllQuestionTypes()
    {
        return new List<string>(questionTypes);
    }

    /// <summary>
    /// 根据类别随机获取一道题
    /// </summary>
    public QuestionData GetRandomQuestion(string type)
    {
        if (!m_questionBank.ContainsKey(type) || m_questionBank[type].Count == 0)
            return null;

        var list = m_questionBank[type];
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    /// <summary>
    /// 添加一道新题
    /// </summary>
    public void AddQuestion(string type, QuestionData data)
    {
        if (!m_questionBank.ContainsKey(type))
        {
            m_questionBank[type] = new List<QuestionData>();
            questionTypes.Add(type);
        }
        m_questionBank[type].Add(data);
    }
}
