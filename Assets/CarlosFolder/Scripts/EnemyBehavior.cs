using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private CircleCollider2D detectRadius;
    [SerializeField] private CircleCollider2D attackRadius;

    [SerializeField] private List<GameObject> pylonTargets;
    [SerializeField] private List<GameObject> playerTargets;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
