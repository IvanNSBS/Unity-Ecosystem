using UnityEngine;

public class SteerBehaviorComponent : MonoBehaviour 
{
    private MovementComponent m_MovementComponent;

    private void Start()
    {
        m_MovementComponent = GetComponent<MovementComponent>();
    }

    private void Update()
    {
        
    }
}