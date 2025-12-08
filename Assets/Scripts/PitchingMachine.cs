using UnityEngine;

public class PitchingMachine : MonoBehaviour
{
    public GameObject ballPrefab;     // 投げるボール
    public Transform spawnPoint;      // 発射位置
    public float pitchInterval = 2.0f;
    public float pitchSpeed = 12.0f;  // ボール初速（m/s）

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > pitchInterval)
        {
            PitchBall();
            timer = 0f;
        }
    }

    void PitchBall()
    {
        GameObject ball = Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(spawnPoint.up * pitchSpeed, ForceMode.VelocityChange);
        }
    }
}
