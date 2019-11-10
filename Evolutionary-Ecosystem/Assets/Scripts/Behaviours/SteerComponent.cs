using UnityEngine;

public class SteerComponent : MonoBehaviour 
{
    private Rigidbody2D m_RigidBody = null;
    private Vector2 GetPosition() { return this.gameObject.transform.position; }
    private Vector2 GetVelocity() { return m_RigidBody.velocity; }
    public void ApplyForce(Vector2 force, float max_force) { m_RigidBody.velocity += force; m_RigidBody.velocity = Vector2.ClampMagnitude(m_RigidBody.velocity, max_force); }

    private Vector2 SetMagnitude(Vector2 vector, float magnitude){
        return vector.normalized * magnitude;
    }
    public Vector2 GetSteer( Vector2 target_pos, float tolerance, Genes genes, bool lerp=false )
    {

        Vector2 desired = (target_pos - GetPosition()).normalized * genes.m_MaxSpeed;
        float distance = desired.magnitude;

        if(distance < tolerance)
        {
            if(lerp)
                desired = SetMagnitude(desired, genes.m_MaxSpeed*distance/tolerance);
            Vector2 steer = desired - GetVelocity();
            steer = Vector2.ClampMagnitude(steer, lerp ? genes.m_MaxBrake : genes.m_MaxForce );

            Debug.Log(steer);
            return steer;
        }

        return Vector2.zero;
    }
    
    public Vector2 SeekAndArrive(Vector2[] targets, float seek_tol, float arrive_tol, Genes genes)
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
        {
            if(min_dist < arrive_tol)
                return GetSteer((Vector2)target_pos, seek_tol, genes, lerp: true);

            return GetSteer((Vector2)target_pos, seek_tol, genes);
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