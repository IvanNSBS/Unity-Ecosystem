using UnityEngine;

public class SteerComponent : MonoBehaviour 
{
    private Rigidbody2D m_RigidBody = null;
    private Vector2 GetPosition() { return this.gameObject.transform.position; }
    private Vector2 GetVelocity() { return m_RigidBody.velocity; }
    public void ApplyForce(Vector2 force) { m_RigidBody.velocity += force; }
    public Vector2 GetSteer( Vector2 target_pos, float tolerance, Genes agent_genes )
    {

        Vector2 desired = (target_pos - GetPosition()).normalized * agent_genes.m_MaxSpeed;
        float distance = desired.magnitude;

        if(distance < tolerance)
        {
            Vector2 steer = desired - GetVelocity();
            steer = Vector2.ClampMagnitude(steer, agent_genes.m_MaxForce);

            Debug.Log(steer);
            return steer;
        }

        return Vector2.zero;
    }
    
    public Vector2 Seek(Vector2[] targets, float tolerance, Genes genes)
    {
        Vector2? target_pos = null;
        float min_dist = Mathf.Infinity;
        foreach(var target in targets)
        {
            float distance = (target - GetPosition()).magnitude;
            if(distance < min_dist)
            {
                min_dist = distance;
                target_pos = target;
            }
        }
        if(target_pos != null)
            return GetSteer((Vector2)target_pos, tolerance, genes);

        return Vector2.zero;
    }

    private void Start()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
    }

    // private void Update()
    // {
    //     Vector2 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //     // Seek(new Vector2(10, 0), 10000);
    //     Seek(mouse_pos, 100000);
    // }
}