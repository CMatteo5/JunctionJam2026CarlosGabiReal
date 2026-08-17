using UnityEngine;

public class BrokenPylon : MonoBehaviour
{
    [SerializeField] private float lifetime = 20f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}