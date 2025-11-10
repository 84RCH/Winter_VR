using UnityEngine;

public class BallController : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bat"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 hitDirection = (transform.position - collision.transform.position).normalized;
            rb.linearVelocity = hitDirection * 10f;
        }
    }
}