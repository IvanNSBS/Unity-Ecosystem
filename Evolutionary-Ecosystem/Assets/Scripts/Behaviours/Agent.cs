using UnityEngine;
using System.Collections;

public class Agent : MonoBehaviour {
    public SteerComponent m_SteerBehavior;
    public Genes m_AgentGenes;

    private Vector2? wander_point = null;
    private bool waiting = false;
    private void Start() {
        m_SteerBehavior = GetComponent<SteerComponent>();
    }

    private IEnumerator WaitAfterArrive(float wait_time)
    {
        if(!waiting){
            waiting = true;
            yield return new WaitForSeconds(wait_time);
            waiting = false;
            wander_point = null;
        }
    }

    private void Wander()
    {
        if(wander_point == null)
            wander_point = new Vector2( Random.Range(-4.0f, 8.0f), Random.Range(0.0f, 4.0f) );
        
        Vector2[] pts = { (Vector2)wander_point };
        bool arrived = false;
        var steer = m_SteerBehavior.SeekAndArrive(pts, 10000, 0.35f, m_AgentGenes, ref arrived);
        m_SteerBehavior.ApplyForce(steer, m_AgentGenes.m_MaxSpeed);
        if( arrived )
            StartCoroutine( WaitAfterArrive(2.0f) );
    }

    private void Update()
    {
        // Vector2 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Vector2[] targets = {mouse_pos};
        // bool arrived = false;
        // var s = m_SteerBehavior.SeekAndArrive(targets, 100000, 0.35f, m_AgentGenes, ref arrived);
        // m_SteerBehavior.ApplyForce(s, m_AgentGenes.m_MaxSpeed);

        Wander();
    }
}