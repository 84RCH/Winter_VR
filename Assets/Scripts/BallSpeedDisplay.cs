using UnityEngine;
using UnityEngine.UI;

public class BallSpeedDisplay : MonoBehaviour
{
    public Rigidbody ballRb;
    public Text speedText;  // UI Text

    void Update()
    {
        if (ballRb == null || speedText == null) return;

        float speed = ballRb.linearVelocity.magnitude;  // m/s
        float kmh = speed * 3.6f;                 // km/h に変換

        speedText.text = $"Speed: {kmh:F1} km/h";
    }
}