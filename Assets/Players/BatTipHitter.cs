using UnityEngine;

/// 放在棒子上，绑定棒尖TipTracker与一个小触发器（放在棒尖附近）
/// 触发到“Ball”时，把 Tip 的速度/方向转化为球的出射速度 + 自旋
public class BatTipHitter : MonoBehaviour
{
    [Header("Refs")]
    public BatTipTracker tip;          // 棒尖Tip（上面那个脚本）
    public Collider hitTrigger;        // 放在棒尖处的小触发器（IsTrigger = true）

    [Header("Launch")]
    [Tooltip("把 Tip 的速度放大到球的出射速度（1=等速，>1 更猛）。")]
    public float powerScale = 1.3f;
    [Tooltip("最低出射速度（m/s）。")]
    public float minExitSpeed = 18f;
    [Tooltip("最高出射速度（0=不限制）。")]
    public float maxExitSpeed = 60f;

    [Tooltip("给出射方向一个上扬权重（0=不用上扬，1=强烈朝上）。")]
    [Range(0f, 1f)] public float upBias = 0.25f;
    [Tooltip("把出射方向朝 Tip.forward 收敛（0=不收敛，1=完全跟随forward）。")]
    [Range(0f, 1f)] public float forwardAssist = 0.2f;
    [Tooltip("限制仰角范围（相对水平）。")]
    public Vector2 launchAngleClamp = new Vector2(8f, 40f);

    [Header("Spin")]
    [Tooltip("命中点相对棒尖的左右/上下偏差转成侧旋/上旋的强度。")]
    public float sideSpinScale = 120f;
    public float topSpinScale = 80f;

    [Header("Safety")]
    public float sameBallCooldown = 0.04f;

    public AudioSource audioSource;
    public AudioClip audioClip;

    private GameObject lastBall;
    private float lastBallTime;

    private void Reset()
    {
        if (hitTrigger == null)
            hitTrigger = GetComponentInChildren<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitTrigger == null || tip == null) return;
        if (!other.CompareTag("Ball")) return;
        if (other.attachedRigidbody == null) return;

        // 冷却：同一颗球短时间只触发一次
        if (other.gameObject == lastBall && Time.time - lastBallTime < sameBallCooldown) return;
        lastBall = other.gameObject; lastBallTime = Time.time;

        Rigidbody ball = other.attachedRigidbody;

        // 1) 取棒尖的速度向量
        Vector3 vTip = tip.TipVelocity;
        if (vTip.sqrMagnitude < 1e-6f) return; // 没有有效挥棒

        // 2) 计算初始出射方向与速度大小
        Vector3 dir = vTip.normalized;

        // 2.1 轻微“上扬”与“朝forward收敛”，可让方向更可控
        dir = Vector3.Slerp(dir, Vector3.up, upBias);                 // 上扬
        dir = Vector3.Slerp(dir, tip.transform.forward, forwardAssist); // 朝棒尖forward微收敛

        // 2.2 仰角限制（相对世界水平）
        Vector3 horiz = Vector3.ProjectOnPlane(dir, Vector3.up);
        if (horiz.sqrMagnitude > 1e-6f)
        {
            float current = Vector3.SignedAngle(horiz, dir, Vector3.Cross(horiz, Vector3.up));
            float clamped = Mathf.Clamp(current, launchAngleClamp.x, launchAngleClamp.y);
            float delta = clamped - current;
            dir = Quaternion.AngleAxis(delta, Vector3.Cross(horiz, Vector3.up)) * dir;
        }

        float speed = Mathf.Max(minExitSpeed, vTip.magnitude * powerScale);
        Vector3 vOut = dir * speed;

        if (maxExitSpeed > 0f && vOut.magnitude > maxExitSpeed)
            vOut = vOut.normalized * maxExitSpeed;

        // 3) 给球赋速度
        ball.linearVelocity = vOut;
        audioSource.PlayOneShot(audioClip);
        //PlaySE();

        // 4) 自旋：命中点相对棒尖本地偏差 → 侧旋/上旋
        Vector3 hitPoint = other.ClosestPoint(tip.transform.position);
        Vector3 localHit = tip.transform.InverseTransformPoint(hitPoint);
        float side = localHit.x;  // 左右偏差 → 侧旋
        float vert = localHit.y;  // 上下偏差 → 上/下旋

        Vector3 sideAxis = Vector3.up;                                       // 侧旋轴：世界Up（可替换成依据dir的法线）
        Vector3 topAxis = Vector3.Cross(dir.normalized, Vector3.up).normalized; // 上旋轴：速度右侧

        Vector3 addSpin = sideAxis * (side * sideSpinScale)
                        + topAxis * (vert * topSpinScale);

        ball.maxAngularVelocity = Mathf.Max(ball.maxAngularVelocity, 200f);
        ball.angularVelocity += addSpin;
    }

    void PlaySE()
    {
        GameObject SEPlayer = new GameObject();
        SEPlayer.AddComponent<AudioSource>();
        SEPlayer.GetComponent<AudioSource>().PlayOneShot(audioClip);

        Destroy(SEPlayer, audioClip.length + 0.1f);
    }
}
