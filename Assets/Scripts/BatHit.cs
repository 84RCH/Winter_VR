using UnityEngine;

public class BatHit : MonoBehaviour
{
    public float powerMultiplier = 1.5f;  // ボールに与える力の倍率

    private Rigidbody batRb;

    public AudioSource hitSound;          // 追加：衝突音
    public float minSpeedForSound = 1.0f; // 追加：弱いスイングでは音を鳴らさない

    private Vector3 lastPosition;
    private Vector3 batVelocity;

    void Start()
    {
        // Rigidbody（Kinematic）を取得
        batRb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        // バットの速度を計算（Kinematicでも動きは取れる）
        batVelocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        // ボールに当たったか判定
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
            if (ballRb == null) return;

            // バットの進行方向に力を与える
            Vector3 hitDirection = batVelocity.normalized;

            // 力の大きさ（スイング速度 × 倍率）
            float force = batVelocity.magnitude * powerMultiplier;

            // AddForce でボールを飛ばす
            ballRb.AddForce(hitDirection * force, ForceMode.Impulse);

            // 一定以上の速度なら衝突音を再生
            if (batVelocity.magnitude > minSpeedForSound)
            {
                hitSound?.Play();
            }
        }
    }
}
