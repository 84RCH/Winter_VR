using UnityEngine;
public class BallLauncher : MonoBehaviour
{
    public GameObject ballPrefab; // ボールのPrefab
    public Transform spawnPoint; // ボールの発射位置

    public float ballLifeTime = 5f; // ボールの生存時間

    public float launchForce = 20f; // ボールの発射力
    public float spawnInterval = 3f; // ボールを発射する間隔

    private float timer = 0f; // 時間をカウントする変数

    private bool isStart=false; // 作動スイッチ

    void Update()
    {
        if (!isStart)
        {
            timer = spawnInterval;
            return;
        }

        timer += Time.deltaTime; // フレームごとに時間をカウント
        if (timer >= spawnInterval) // 一定時間ごとに発射
        {
            LaunchBall();
            timer = 0f; // タイマーをリセット
        }
    }
    void LaunchBall()
    {
        // ボールを生成
        GameObject ball = Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);
        // ボールに力を加える
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(spawnPoint.forward * launchForce, ForceMode.Impulse);
        }
        // 一定時間後にボールを削除
        Destroy(ball, ballLifeTime);
    }

    public void StartLauncher()
    {
        isStart = true;
    }
}