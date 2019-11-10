using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Agent : MonoBehaviour {
    public SteerComponent m_SteerBehavior;
    public Genes m_AgentGenes;
    [Header("Vision Parameters")]
    public List<GameObject> visible_food = new List<GameObject>();
    public List<GameObject> visible_predators = new List<GameObject>();


    private Vector2? wander_point = null;


    [Header("Life Parameters")]
    public float m_LifeTime = 500.0f;
    public float m_TimeToDeathByHunger = 150.0f;
    public float m_TimeToDeathByThirst = 80.0f;
    public float m_RemainingLifetime;
    public float m_CurrentHunger, m_CurrentThirst;
    private void Start() {
        m_SteerBehavior = GetComponent<SteerComponent>();
        m_RemainingLifetime = m_LifeTime;
        m_CurrentHunger = 0;
        m_CurrentThirst = 0;
    }

    private bool waiting = false;
    private IEnumerator WaitAfterArrive(float wait_time)
    {
        if(!waiting){
            waiting = true;
            yield return new WaitForSeconds(wait_time);
            waiting = false;
            wander_point = null;
        }
    }

    private void Explore()
    {
        if(wander_point == null)
            wander_point = new Vector2( Random.Range(-5.7f, 5.7f), Random.Range(-3.6f, 3.6f) );
        
        Vector2[] pts = { (Vector2)wander_point };
        bool arrived = false;
        var steer = m_SteerBehavior.SeekAndArrive(pts, 10000, 0.35f, m_AgentGenes, ref arrived);
        m_SteerBehavior.ApplyForce(steer, m_AgentGenes.m_MaxSpeed);
        if( arrived )
            StartCoroutine( WaitAfterArrive(1.0f) );
            // wander_point = null;
    }

    private void SeekFood()
    {
        wander_point = null;
        bool arrived = false;
        var steer = m_SteerBehavior.SeekAndArrive(ref visible_food, m_AgentGenes.m_SightRadius, 0.1f, m_AgentGenes, ref arrived);
        m_SteerBehavior.ApplyForce(steer, m_AgentGenes.m_MaxSpeed);
    }

    private void EvadeAgents()
    {
        wander_point = null;
        var steer = m_SteerBehavior.Evade(visible_predators, m_AgentGenes.m_SightRadius, m_AgentGenes);
        m_SteerBehavior.ApplyForce(steer, m_AgentGenes.m_MaxSpeed);
    }

    private void Update()
    {
        // Vector2 mouse = Camera.main.ScreenToWorldPoint( Input.mousePosition );
        // Vector2[] evade_list = { mouse };
        // var avoid = m_SteerBehavior.Evade(evade_list, 30.0f, m_AgentGenes);
        // if( avoid.magnitude > 0.0001f){
        //     wander_point = null;
        //     m_SteerBehavior.ApplyForce(avoid, m_AgentGenes.m_MaxSpeed);
        // }
        m_RemainingLifetime -= Time.deltaTime/m_LifeTime;
        m_CurrentHunger += Time.deltaTime/m_TimeToDeathByHunger;
        m_CurrentThirst += Time.deltaTime/m_TimeToDeathByThirst;

        var sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.Lerp(Color.red, Color.green, m_RemainingLifetime/m_LifeTime);

        if(visible_food.Count == 0 && visible_predators.Count == 0)
            Explore();
        else if(visible_predators.Count == 0)
            SeekFood();
        else
            EvadeAgents();

    }
}