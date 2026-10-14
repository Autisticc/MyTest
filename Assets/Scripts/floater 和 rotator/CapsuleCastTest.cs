using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleCastTest : MonoBehaviour
{
    [Header("Cast Settings")]
    public float maxDistance = 10f;

    [Header("Animation Settings")]
    public float travelTime = 0.4f; // 飞行耗时
    public float holdTime = 0.5f;   // 到达后停留时间
    public bool loop = true;        // 是否循环播放

    // 缓存的检测数据
    private Vector3 m_debugP1;
    private Vector3 m_debugP2;
    private float m_debugRadius;
    private Vector3 m_debugDirection;
    private RaycastHit m_debugHit;
    private bool m_debugHasHit;
    private float m_debugStopDistance;

    // 动画计时
    private float m_animTimer;
    private float m_totalTime;

    void Start()
    {
        m_totalTime = travelTime + holdTime;
    }
    void Update()
    {
        // ---- 1. 执行 CapsuleCast ----
        CharacterController charContr = GetComponent<CharacterController>();
        Vector3 p1 = transform.position + charContr.center + Vector3.up * -charContr.height * 0.5F;
        Vector3 p2 = p1 + Vector3.up * charContr.height;

        bool hasHit = Physics.CapsuleCast(
            p1, p2, charContr.radius, transform.forward,
            out RaycastHit hit, maxDistance);

        // ---- 2. 缓存数据 ----
        m_debugP1 = p1;
        m_debugP2 = p2;
        m_debugRadius = charContr.radius;
        m_debugDirection = transform.forward;
        m_debugHit = hit;
        m_debugHasHit = hasHit;
        m_debugStopDistance = hasHit ? hit.distance : maxDistance;

        // ---- 3. 驱动动画进度 ----
        m_animTimer += Time.deltaTime;
        if (m_animTimer >= m_totalTime)
        {
            m_animTimer = loop ? 0f : m_totalTime;
        }
    }

 void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        if (m_debugRadius <= 0f) return;

        // ---- 1. 起始胶囊：绿色 ----
        Gizmos.color = new Color(0f, 1f, 0f, 0.4f);
        DrawWireCapsule(m_debugP1, m_debugP2, m_debugRadius);

        // ---- 2. 计算飞行进度 ----
        float t = Mathf.Clamp01(m_animTimer / travelTime);
        float easedT = Mathf.SmoothStep(0f, 1f, t); // 缓动，看起来更顺
        float currentDistance = m_debugStopDistance * easedT;

        Vector3 offset = m_debugDirection * currentDistance;
        Vector3 movingP1 = m_debugP1 + offset;
        Vector3 movingP2 = m_debugP2 + offset;

        // ---- 3. 飞行中的胶囊：颜色随进度变化 ----
        // 飞行中为蓝色，命中停住后为红色
        bool arrived = t >= 1f;
        Gizmos.color = arrived && m_debugHasHit ? Color.red : Color.cyan;
        DrawWireCapsule(movingP1, movingP2, m_debugRadius);

        // ---- 4. 轨迹线：从起点到当前胶囊位置 ----
        Gizmos.color = new Color(1f, 1f, 0f, 0.6f);
        Gizmos.DrawLine(m_debugP1, movingP1);
        Gizmos.DrawLine(m_debugP2, movingP2);

        // ---- 5. 命中点：红色球 + 法线 ----
        if (m_debugHasHit && arrived)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_debugHit.point, 0.12f);
            Gizmos.DrawLine(m_debugHit.point, m_debugHit.point + m_debugHit.normal * 0.5f);
        }
    }

    // 绘制完整的线框胶囊
    void DrawWireCapsule(Vector3 p1, Vector3 p2, float radius)
    {
        Gizmos.DrawWireSphere(p1, radius);
        Gizmos.DrawWireSphere(p2, radius);

        Vector3 axis = (p2 - p1).normalized;
        if (axis.sqrMagnitude < 0.0001f) return; // 高度为 0 时只画球

        Vector3 right = Vector3.Cross(axis, Vector3.up).normalized;
        if (right.sqrMagnitude < 0.0001f)
            right = Vector3.Cross(axis, Vector3.right).normalized;

        int segments = 16;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 dir = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axis) * right;
            Gizmos.DrawLine(p1 + dir * radius, p2 + dir * radius);
        }
    }
}

