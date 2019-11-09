using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    [SerializeField] private float m_Acceleration;
    [SerializeField] private float m_Velocity;
    // Start is called before the first frame update
    private Rigidbody2D m_RigidBody = null;

    public void ApplyForce(Vector2 force)
    {
        m_RigidBody.AddForce(force);
    }
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
    }
}
