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
    public GameObject m_MateTarget = null;
    private float m_ReproductionTime = 2.0f;
    private float m_reproducing = 0.0f, m_gestating;

    void Awake()
    {
        m_AgentGenes.RandomizeGenes();     
        // m_AgentGenes = new Genes(1.0f);   
    }
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

    private void Reproduce()
    {
        if(m_FSM.state != AgentState.Reproducing)
            return;
        
        m_reproducing += Time.deltaTime;
        if(m_reproducing >= m_ReproductionTime){
            control = true;
            m_reproducing = 0.0f;
            m_FSM.state = AgentState.Exploring;
            m_MateTarget = null;
            m_LifeComponent.m_CurrentReproductionUrge = Mathf.Clamp(m_LifeComponent.m_CurrentReproductionUrge-0.35f, 0.0f, 1.0f);
            if(!m_AgentGenes.m_IsMale)
                StartCoroutine(Gestate());
        }
    }

    IEnumerator Gestate()
    {
        m_gestating += Time.deltaTime;
        yield return new WaitForSeconds(Time.deltaTime);
        if(m_gestating >= m_AgentGenes.m_GestationDuration){
            StartCoroutine(SpawnOffsprings(0));
            m_gestating = 0.0f;
        }
        else
            StartCoroutine(Gestate());
    }
    IEnumerator SpawnOffsprings(int depth)
    {
        if(depth >= m_AgentGenes.m_MaxOffsprings)
            yield break;
        
        Instantiate(gameObject);
        yield return new WaitForSeconds(0.75f);
        StartCoroutine(SpawnOffsprings(depth+1));
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
        }
    }

    public void EvadeAgents( ref Vector2 steer)
    {
        steer = m_SteerBehavior.Evade(visible_animals, 0.8f, m_AgentGenes);
    }

    public void GoToMate(ref Vector2 steer, ref bool arrived)
    {
        arrived = false;
        GameObject obj = null;
        List<GameObject> mate = new List<GameObject>{ m_MateTarget };
        steer = m_SteerBehavior.SeekAndArrive(ref mate, m_AgentGenes.m_SightRadius, 0.25f, m_AgentGenes, ref arrived, ref obj);
    }

    public void ScanForPartner()
    {
        if(!m_AgentGenes.m_IsMale)
            return;
        if(m_MateTarget == null)
        {
            foreach(var potential_mate in visible_animals)
            {
                Debug.Log("scanning...");
                bool isFemale = !potential_mate.GetComponent<Agent>().m_AgentGenes.m_IsMale;
                if(isFemale){
                    if(PotentialMateFound(potential_mate))
                        break;
                }
            }
        }
        else
            Debug.Log("Partner is: " + m_MateTarget);
    }

    bool control = false;
    public bool PotentialMateFound(GameObject female)
    {
        if(control)
            return false;
        bool accepted = female.GetComponent<Agent>().RequestMate(gameObject);
        if(accepted){
            m_MateTarget = female;
            m_FSM.state = AgentState.GoingToPartner;
            control = true;
        }
        return accepted;
    }

    public bool RequestMate(GameObject male)
    {
        m_MateTarget = male;
        m_FSM.state = AgentState.WaitingForPartner;
        return true;
    }

    private void Update()
    {
        m_FSM.DecideNextState();
        var steer = m_FSM.GetStateSteer();
        m_SteerBehavior.ApplyForce(steer, m_AgentGenes.m_MaxSpeed);
        ConsumeFood();
        // ScanForPartner();
        // Reproduce();
    }
}