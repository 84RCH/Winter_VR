using UnityEngine;

/// 挂在棒尖Tip（空物体）上：负责估算 Tip 的世界速度/方向
public class BatTipTracker : MonoBehaviour
{
    [Header("Sampling")]
    [Tooltip("用多少帧差分估算速度（越大越稳，越小越灵敏）。")]
    public int samples = 4;
    [Tooltip("最低速度阈值（m/s），低于此视为无挥棒。")]
    public float minValidSpeed = 2f;

    private Vector3[] posBuf;
    private float[] timeBuf;
    private int idx;
    private bool filled;

    public Vector3 TipVelocity { get; private set; }   // 世界速度（m/s）
    public Vector3 TipDirection =>
        TipVelocity.sqrMagnitude < 1e-6f ? transform.forward : TipVelocity.normalized;

    private void Awake()
    {
        samples = Mathf.Max(2, samples);
        posBuf = new Vector3[samples];
        timeBuf = new float[samples];
        posBuf[0] = transform.position;
        timeBuf[0] = Time.fixedTime;
        idx = 0;
    }

    private void FixedUpdate()
    {
        idx = (idx + 1) % samples;
        posBuf[idx] = transform.position;
        timeBuf[idx] = Time.fixedTime;

        int oldest = filled ? (idx + 1) % samples : 0;
        filled = filled || idx == samples - 1;

        Vector3 dp = posBuf[idx] - posBuf[oldest];
        float dt = timeBuf[idx] - timeBuf[oldest];
        if (dt <= 1e-6f) { TipVelocity = Vector3.zero; return; }

        TipVelocity = dp / dt;

        // 过滤极小速度（避免抖动误触发）
        if (TipVelocity.magnitude < minValidSpeed) TipVelocity = Vector3.zero;
    }
}
