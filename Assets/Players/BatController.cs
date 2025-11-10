using UnityEngine;

public class BatController : MonoBehaviour
{
    [Header("Refs")]
    public Transform sweetSpot;           // 甜区参考（其 forward 作为“击球平面法线”）
    public Rigidbody batRb;               // 棒子的刚体（可 Kinematic）

    [Header("Launch Tuning")]
    [Range(0f, 2f)] public float restitution = 0.95f;    // 弹性
    public float minExitSpeed = 20f;                      // m/s
    public float maxExitSpeed = 65f;                      // 0=不限制
    [Range(0f, 90f)] public float minLaunchAngle = 10f;   // 相对水平的仰角下限（度）
    [Range(0f, 90f)] public float maxLaunchAngle = 35f;   // 相对水平的仰角上限（度）
    public float powerScale = 1.0f;                       // 力量缩放（乘到相对速度上）

    [Header("Spin Tuning")]
    public float radiusBall = 0.0366f;                    // 3.66 cm
    public float sideSpinScale = 120f;                    // 横向偏差→侧旋（调手感）
    public float topSpinScale = 80f;                     // 纵向偏差→上/下旋
    public float sprayDegrees = 2f;                      // 极轻微随机散布

    [Header("Safety")]
    public float sameBallCooldown = 0.04f;                // 防多次触发
    private GameObject lastBall;
    private float lastBallTime;

    private void Reset()
    {
        batRb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball")) return;
        if (other.attachedRigidbody == null) return;
        if (sweetSpot == null || batRb == null) return;

        // 冷却
        if (other.gameObject == lastBall && Time.time - lastBallTime < sameBallCooldown) return;
        lastBall = other.gameObject; lastBallTime = Time.time;

        Rigidbody ball = other.attachedRigidbody;

        // 命中点（用最近点逼近；若你有 Contact 点可用之）
        Vector3 hitPoint = other.ClosestPoint(sweetSpot.position);

        // 定义“稳定”的法线：甜区 forward
        Vector3 n = sweetSpot.forward.normalized;

        // 接触点速度
        Vector3 vBat = batRb.GetPointVelocity(hitPoint);
        Vector3 vBall = ball.GetPointVelocity(hitPoint);
        Vector3 vRel = vBall - vBat;

        // —— 镜面反射（忽略 Unity 接触法线）——
        // 反射：v' = v - 2*(v·n)*n
        Vector3 vRelReflected = vRel - 2f * Vector3.Dot(vRel, n) * n;
        Vector3 vRelOut = restitution * vRelReflected;

        // 出射速度强度
        float targetSpeed = Mathf.Max(minExitSpeed, vRel.magnitude * powerScale);
        vRelOut = vRelOut.normalized * targetSpeed;

        // 轻微随机散布（可关）
        if (sprayDegrees > 0f)
        {
            vRelOut = Quaternion.AngleAxis(Random.Range(-sprayDegrees, sprayDegrees), Random.onUnitSphere) * vRelOut;
        }

        // —— 把仰角夹在范围内 ——（相对世界水平）
        {
            Vector3 horiz = Vector3.ProjectOnPlane(vRelOut, Vector3.up);
            float speed = vRelOut.magnitude;
            if (horiz.sqrMagnitude > 1e-6f && speed > 1e-3f)
            {
                float currentDeg = Vector3.SignedAngle(horiz, vRelOut, Vector3.Cross(horiz, Vector3.up));
                float clampedDeg = Mathf.Clamp(currentDeg, minLaunchAngle, maxLaunchAngle);
                float delta = clampedDeg - currentDeg;
                vRelOut = Quaternion.AngleAxis(delta, Vector3.Cross(horiz, Vector3.up)) * vRelOut;
            }
        }

        // 叠加棒子的接触点速度 → 世界出射速度
        Vector3 vOut = vRelOut + vBat;
        if (maxExitSpeed > 0f && vOut.magnitude > maxExitSpeed)
            vOut = vOut.normalized * maxExitSpeed;

        // 赋给球
        ball.linearVelocity = vOut;

        // —— 自旋（由甜区偏差决定，稳定且直观）——
        // 计算命中点在甜区局部坐标的偏差（X 左右、Y 上下）
        Vector3 localHit = sweetSpot.InverseTransformPoint(hitPoint);
        float side = localHit.x;   // 横偏：正→给右曲（以你坐标系为准）
        float vert = localHit.y;   // 竖偏：正→上旋，负→下旋

        Vector3 addSpin = Vector3.zero;
        // 侧旋轴：世界 Up 附近，或用 n 与水平向量叉乘
        Vector3 sideAxis = Vector3.up;
        // 上/下旋轴：速度方向的右侧（或 n × vOut）
        Vector3 topAxis = Vector3.Cross(vOut.normalized, Vector3.up).normalized;

        addSpin += sideAxis * (side * sideSpinScale);
        addSpin += topAxis * (vert * topSpinScale);

        ball.maxAngularVelocity = Mathf.Max(ball.maxAngularVelocity, 200f);
        ball.angularVelocity += addSpin;
    }
}
