using UnityEngine;

public class BallLife : MonoBehaviour
{
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
