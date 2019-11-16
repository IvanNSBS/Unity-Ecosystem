using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Agent : MonoBehaviour {

    [Header("Components")]
    public Rigidbody2D m_RigidBody;
    public SteerComponent m_SteerBehavior;
    public LifeComponent m_LifeComponent;
    public Genes m_AgentGenes;
    [Header("General Info")]
    public AgentDiet m_Diet = AgentDiet.Vegetal;
    public AgentStateMachine m_FSM;
    [Header("Vision Parameters")]
    public List<GameObject> visible_food = new List<GameObject>();
    public List<GameObject> visible_predators = new List<GameObject>();
    public List<GameObject> visible_animals = new List<GameObject>();

    private Vector2? wander_point = null;


    private GameObject m_EatingFood = null, m_WaterDrinking = null;
    private void Start() {
        m_SteerBehavior = GetComponent<SteerComponent>();
        m_LifeComponent = GetComponent<LifeComponent>();
        m_FSM = new AgentStateMachine(this);
    }

    private void ConsumeFood()
    {
        if(m_FSM.state != AgentState.Eating)
            return;
        if(m_LifeComponent.m_CurrentHunger/m_LifeComponent.m_TimeToDeathByHunger > m_AgentGenes.m_NotHungry && m_EatingFood != null){
            waiting = true;
            m_EatingFood.GetComponent<FoodData>().Consume(gameObject, Time.deltaTime * 1.3f);
        }
        else{
            m_FSM.state = AgentState.Exploring;
            waiting = false;
            m_EatingFood = null;
        }
    }

    private void ConsumeWater()
    {

    }

    public void ResetWanderPoint() { wander_point = null; }
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
    private IEnumerator WaitSetState(float wait_time, AgentState nstate)
    {
        if(!waiting){
            var oldstate = m_FSM.state;
            waiting = true;
            m_FSM.state = nstate;

            yield return new WaitForSeconds(wait_time);
            m_FSM.state = oldstate;
            waiting = false;
            wander_point = null;
        }
    }

    public void Explore(ref Vector2 steer, ref bool arrived)
    {
        if(waiting)
            return;

        if(wander_point == null)
            wander_point = new Vector2( Random.Range(-5.7f, 5.7f), Random.Range(-3.6f, 3.6f) );
        
        Vector2[] pts = { (Vector2)wander_point };
        arrived = false;
        steer = m_SteerBehavior.SeekAndArrive(pts, 10000, 0.35f, m_AgentGenes, ref arrived);

        if( arrived ){
            // wander_point = null;
            StartCoroutine( WaitAfterArrive(2.0f) );
        }
    }

    public void SeekFood( ref Vector2 steer, ref bool arrived)
    {
        if(waiting)
            return;

        arrived = false;
        GameObject found = null;
        steer = m_SteerBehavior.SeekAndArrive(ref visible_food, m_AgentGenes.m_SightRadius, 0.15f, m_AgentGenes, ref arrived, ref found);
        if(arrived && m_FSM.state == AgentState.GoingToFood){
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            m_EatingFood = found;
            m_FSM.state = AgentState.Eating;
            // while( m_LifeComponent.m_CurrentHunger/m_LifeComponent.m_TimeToDeathByHunger > m_AgentGenes.m_NotHungry && found != null)
            // found.GetComponent<FoodData>().Consume(gameObject, 0.2f);
            // StartCoroutine( WaitSetState(2.0f, AgentState.Eating));
        }
    }

    public void EvadeAgents( ref Vector2 steer)
    {
        steer = m_SteerBehavior.Evade(visible_animals, m_AgentGenes.m_SightRadius, m_AgentGenes);
    }

    private void Update()
    {
        m_FSM.DecideNextState();
        var steer = m_FSM.GetStateSteer();
        m_SteerBehavior.ApplyForce(steer, m_AgentGenes.m_MaxSpeed);
        ConsumeFood();


        // Debug.Log("Update steer = " + steer);
        // Vector2 st = Vector2.zero; bool b = false;
        // if(visible_food.Count == 0 && visible_predators.Count == 0)
            // Explore(ref st, ref b);
        // else if(visible_predators.Count == 0)
        //     SeekFood();
        // else
        //     EvadeAgents();
    }
}