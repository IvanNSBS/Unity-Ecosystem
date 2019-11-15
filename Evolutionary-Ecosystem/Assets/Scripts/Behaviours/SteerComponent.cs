using UnityEngine;
using System.Collections.Generic;

public class SteerComponent : MonoBehaviour 
{
    private Rigidbody2D m_RigidBody = null;
    private Vector2 GetPosition() { return this.gameObject.transform.position; }
    private Vector2 GetVelocity() { return m_RigidBody.velocity; }
    public void ApplyForce(Vector2 force, float max_force) { 
        m_RigidBody.velocity += force;
        m_RigidBody.velocity = Vector2.ClampMagnitude(m_RigidBody.velocity, max_force);
    }

    private Vector2 SetMagnitude(Vector2 vector, float magnitude){
        return vector.normalized * magnitude;
    }
    public Vector2 GetSteer( Vector2 target_pos, float tolerance, Genes genes, bool lerp=false )
    {

        Vector2 desired = (target_pos - GetPosition()).normalized * genes.m_MaxSpeed;// * invert;
        float distance = (target_pos-GetPosition()).magnitude;

        if(distance < tolerance)
        {
            if(lerp)
                desired = SetMagnitude(desired, genes.m_MaxSpeed*distance/tolerance);
            Vector2 steer = desired - GetVelocity();
            steer = Vector2.ClampMagnitude(steer, lerp ? genes.m_MaxBrake : genes.m_MaxForce );
            return steer;
        }

        return Vector2.zero;
    }
    
    public void GetClosest(Vector2[] targets, out Vector2? target_pos, out float min_dist){
        target_pos = null;
        min_dist = Mathf.Infinity;
        foreach(var target in targets)
        {
            float distance = (target - GetPosition()).magnitude;
            if(distance < min_dist)
            {
                min_dist = distance;
                target_pos = target;
            }
        }
    }
    public void GetClosest(ref List<GameObject> targets, out GameObject target_obj, out float min_dist){
        target_obj = null;
        min_dist = Mathf.Infinity;
        for(int i = targets.Count -1; i >= 0; i--)
        {
            var target = targets[i];
            if(target == null){
                targets.RemoveAt(i);
                continue;
            }
            Vector2 pos = new Vector2(target.transform.position.x, target.transform.position.y);
            float distance = (pos - GetPosition()).magnitude;
            if(distance < min_dist)
            {
                min_dist = distance;
                target_obj = target;
            }
        }
    }

    public Vector2 SeekAndArrive(Vector2[] targets, float seek_tol, float arrive_tol, Genes genes, ref bool arrived)
    {
        Vector2? target_pos;
        float min_dist;
        GetClosest(targets, out target_pos, out min_dist);

        if(target_pos != null)
        {
            if(min_dist < arrive_tol){
                var steer =  GetSteer((Vector2)target_pos, seek_tol, genes, lerp: true);
                arrived = steer.magnitude <= 0.0001f;
                return steer;
            }

            return GetSteer((Vector2)target_pos, seek_tol, genes);
        }

        return Vector2.zero;
    }

    public Vector2 SeekAndArrive(ref List<GameObject> targets, float seek_tol, float arrive_tol, Genes genes, ref bool arrived)
    {
        GameObject target_pos;
        float min_dist;
        GetClosest(ref targets, out target_pos, out min_dist);

        if(target_pos != null)
        {
            Vector2 pos = new Vector2(target_pos.transform.position.x, target_pos.transform.position.y);
            if(min_dist < arrive_tol){
                var steer =  GetSteer(pos, seek_tol, genes, lerp: true);
                arrived = min_dist <= arrive_tol;
                targets.Remove(target_pos);
                return steer;
            }

            return GetSteer(pos, seek_tol, genes);
        }

        return Vector2.zero;
    }

    public Vector2 Evade(List<GameObject> targets, float sight_radius, Genes genes)
    {
        Vector2 sum = Vector2.zero;
        int count = 0;
        foreach(var target in targets)
        {
            Vector2 pos = new Vector2(target.transform.position.x, target.transform.position.y);
            float d = (GetPosition() - pos).magnitude;
            if( d > 0 && d <= sight_radius){
                // Vector2 diff = (GetPosition() - pos).normalized;
                Vector2 diff = -target.GetComponent<SteerComponent>().GetVelocity();
                diff /= d;
                sum += diff;
                count++;
            }
        }
        if(count > 0){
            sum /= count;
            sum = SetMagnitude(sum, genes.m_MaxSpeed);
            Vector2 steer = sum - GetVelocity();
            steer = Vector2.ClampMagnitude(steer, genes.m_MaxForce);   
            return steer;
        }
        return Vector2.zero;
    }

    private void Start()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
    }
}