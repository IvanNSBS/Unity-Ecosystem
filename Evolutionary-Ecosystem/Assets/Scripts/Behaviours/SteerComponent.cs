using UnityEngine;

public class SteerComponent : MonoBehaviour 
{
    private Rigidbody2D m_RigidBody = null;
    private Vector2 GetPosition() { return this.gameObject.transform.position; }
    private Vector2 GetVelocity() { return m_RigidBody.velocity; }
    public void ApplyForce(Vector2 force) { m_RigidBody.AddForce( force ); }
    public Vector2 Seek( Vector2 target_pos, float sight_radius, Genes agent_genes )
    {

        Vector2 desired = target_pos - GetPosition();
        float distance = desired.magnitude;
        desired = Vector2.ClampMagnitude(desired, agent_genes.m_MaxSpeed);

        if(distance < sight_radius)
        {
            Vector2 steer = desired - GetVelocity();
            steer = Vector2.ClampMagnitude(steer, agent_genes.m_MaxForce);

            Debug.Log(steer);
            return steer;
        }

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