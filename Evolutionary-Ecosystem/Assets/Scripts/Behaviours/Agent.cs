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
    public Color materialColor;
    [SerializeField] AnimalGenesRanges genes_ranges;
    public string tag;
    [Header("Vision Parameters")]
    public List<GameObject> visible_food = new List<GameObject>();
    public List<GameObject> visible_water = new List<GameObject>();
    public List<GameObject> visible_predators = new List<GameObject>();
    public List<GameObject> visible_animals = new List<GameObject>();
    public List<GameObject> unimpressed_females = new List<GameObject>();
    private Vector2? wander_point = null;
    private GameObject m_EatingFood = null, m_WaterDrinking = null;
    public GameObject m_MateTarget = null;
    private float m_ReproductionTime = 2.0f;
    private float m_reproducing = 0.0f, m_gestating;
    [SerializeField] private GameObject m_OffspringPrefab;


    public bool IsWanderNull() { return wander_point == null; }
    void Awake()
    {
        m_AgentGenes.RandomizeGenes(genes_ranges);     
        var sprite = GetComponent<SpriteRenderer>();
        var mat = sprite.material;
        if(m_AgentGenes.m_IsMale){
            float g = (1.0f-m_AgentGenes.m_Desirabilty);
            float r = 0.23f*g;
            var color = new Color(0.7f-r, 0.43f*g, 0.0f, 1.0f);
            mat.color = color;
        }
        else
            mat.color = Color.gray;
    }
    private void Start() {
        m_SteerBehavior = GetComponent<SteerComponent>();
        m_LifeComponent = GetComponent<LifeComponent>();
        m_FSM = new AgentStateMachine(this);
    }

    public void Die(CauseOfDeath cause)
    {
        //register death;

        //add back to pool;
        // gameObject.SetActive(false);
        Debug.Log("Rabbit died of: " + cause);
        ObjectPooler.Instance.AddToPool(tag, gameObject);
        if (tag == "rabbit")
            World.instance.actualRabbitAmount--;
        if (tag == "wolf")
            World.instance.actualWolfAmount--;
        World.instance.UpdateUI();
    }

    public void ResetAgent(bool cross_over = false, Genes father = null, Genes mother = null)
    {
        if(!cross_over)
            m_AgentGenes.RandomizeGenes(genes_ranges);
        else
            m_AgentGenes.CrossOver(father, mother);     
        visible_food.Clear();
        visible_predators.Clear();
        visible_animals.Clear();
        unimpressed_females.Clear();
        wander_point = null;
        m_EatingFood = null;
        m_WaterDrinking = null;
        m_MateTarget = null;
        m_reproducing = 0.0f;

        var sprite = GetComponent<SpriteRenderer>();
        var mat = sprite.material;
        if(m_AgentGenes.m_IsMale){
            float g = (1.0f-m_AgentGenes.m_Desirabilty);
            float r = 0.23f*g;
            var color = new Color(0.7f-r, 0.43f*g, 0.0f, 1.0f);
            mat.color = color;
        }
        else
            mat.color = Color.gray;
    }

    private void ConsumeFood()
    {
        if(m_FSM.state != AgentState.Eating)
            return;
        if(m_LifeComponent.m_CurrentHunger > m_AgentGenes.m_NotHungry && m_EatingFood != null){
            waiting = true;
            var food_data = m_EatingFood.GetComponent<FoodData>();
            if(food_data)
                food_data.Consume(gameObject, Time.deltaTime);
            else{
                // Debug.Log("eating prey!!");
                m_LifeComponent.m_CurrentHunger -= 0.08f;
                m_EatingFood.GetComponent<Agent>().Die(CauseOfDeath.Eaten);
                m_EatingFood = null;
            }

        }
        else{
            m_FSM.state = AgentState.Exploring;
            waiting = false;
            m_EatingFood = null;
        }
    }
    private void ConsumeWater()
    {
        if(m_FSM.state != AgentState.Drinking)
            return;
        if(m_LifeComponent.m_CurrentThirst > m_AgentGenes.m_NotThirsty && m_WaterDrinking != null){
            waiting = true;
            m_WaterDrinking.GetComponent<WaterData>().Consume(gameObject, Time.deltaTime);
        }
        else{
            m_FSM.state = AgentState.Exploring;
            waiting = false;
            m_WaterDrinking = null;
        }
    }


    private void Reproduce()
    {
        if(m_FSM.state != AgentState.Reproducing)
            return;
        
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        m_reproducing += Time.deltaTime;
        if(m_reproducing >= m_ReproductionTime){
            m_reproducing = 0.0f;
            m_FSM.state = AgentState.Exploring;
            var father_genes = m_MateTarget.GetComponent<Agent>().m_AgentGenes;
            m_MateTarget = null;
            m_LifeComponent.m_CurrentReproductionUrge -= 0.35f;
            m_LifeComponent.m_CurrentReproductionUrge = Mathf.Clamp(m_LifeComponent.m_CurrentReproductionUrge, 0.0f, 1.0f);
            if (!m_AgentGenes.m_IsMale)
                StartCoroutine(Gestate(father_genes));
        }
    }

    IEnumerator Gestate(Genes father)
    {
        m_gestating += Time.deltaTime;
        yield return new WaitForSeconds(Time.deltaTime);
        if(m_gestating >= m_AgentGenes.m_GestationDuration){
            StartCoroutine(SpawnOffsprings(0, Random.Range(1, m_AgentGenes.m_MaxOffsprings), father));
            m_gestating = 0.0f;
        }
        else
            StartCoroutine(Gestate(father));
    }
    IEnumerator SpawnOffsprings(int cur_depth, int n_offsprings, Genes father)
    {
        if(cur_depth >= n_offsprings)
            yield break;
        
        ObjectPooler.Instance.SpawnFromPool(tag, gameObject.transform.position, father, m_AgentGenes);
        yield return new WaitForSeconds(0.75f);
        StartCoroutine(SpawnOffsprings(cur_depth+1, n_offsprings, father));
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

        if(wander_point == null){
            Vector3 tryWanderPoint;
            int counter = 0;
            do
            {
                Vector3 r_point = Random.insideUnitCircle.normalized * Random.Range(0.5f, 3.0f);
                tryWanderPoint = this.transform.position + r_point;
                var output = Physics2D.OverlapCircleAll(tryWanderPoint, 0.5f);
                counter++;
                if (counter >= 50)
                {
                    this.Die(CauseOfDeath.Age);
                    return;
                }
            } while (World.instance.GetTileAt((int)tryWanderPoint.x, (int)tryWanderPoint.y).type == WorldTile.Type.Water);
            wander_point = tryWanderPoint;
        }
        
        Vector2[] pts = { (Vector2)wander_point };
        arrived = false;
        steer = m_SteerBehavior.SeekAndArrive(pts, 10000, 0.35f, m_AgentGenes, ref arrived);

        if( arrived ){
            StartCoroutine( WaitAfterArrive(1.2f) );
        }
    }

    public void SeekFood( ref Vector2 steer, ref bool arrived)
    {
        if(waiting){
            return;
        }

        arrived = false;
        GameObject found = null;
        steer = m_SteerBehavior.SeekAndArrive(ref visible_food, m_AgentGenes.m_SightRadius, 1f, m_AgentGenes, ref arrived, ref found);
        if(arrived && m_FSM.state == AgentState.GoingToFood){
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            m_EatingFood = found;
            m_FSM.state = AgentState.Eating;
        }
    }
    public void SeekWater(ref Vector2 steer, ref bool arrived)
    {
        if(waiting){
            return;
        }
        bool debug = tag == "wolf" && AgentState.GoingToWater == m_FSM.state;
        arrived = false;
        GameObject found = null;
        steer = m_SteerBehavior.SeekAndArrive(ref visible_water, m_AgentGenes.m_SightRadius, 2f, m_AgentGenes, ref arrived, ref found, remove:false, debug);
        if(arrived && m_FSM.state == AgentState.GoingToWater){
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            m_WaterDrinking = found;
            m_FSM.state = AgentState.Drinking;
        }
    }

    public void EvadeAgents( ref Vector2 steer)
    {
        steer = m_SteerBehavior.Evade(visible_predators, m_AgentGenes.m_ForgetRadius, m_AgentGenes);
    }

    public void GoToMate(ref Vector2 steer, ref bool arrived)
    {
        if (!m_MateTarget)
        {
            m_FSM.state = AgentState.Exploring;
            return;
        }
        arrived = false;
        GameObject obj = null;
        List<GameObject> mate = new List<GameObject>{ m_MateTarget };
        steer = m_SteerBehavior.SeekAndArrive(ref mate, m_AgentGenes.m_SightRadius, 0.4f, m_AgentGenes, ref arrived, ref obj);
    }

    public void ScanForPartner()
    {
        if(!m_AgentGenes.m_IsMale || m_LifeComponent.m_CurrentReproductionUrge < m_AgentGenes.m_CriticalUrge){
            return;
        }
        if(m_MateTarget == null)
        {
            foreach(var potential_mate in visible_animals)
            {
                bool isFemale = !potential_mate.GetComponent<Agent>().m_AgentGenes.m_IsMale;
                if(isFemale && !unimpressed_females.Contains(potential_mate)){
                    if(PotentialMateFound(potential_mate))
                        break;
                }
            }
        }
        // else
            // Debug.Log("Partner is: " + m_MateTarget);
    }

    public bool PotentialMateFound(GameObject female)
    {
        bool accepted = female.GetComponent<Agent>().RequestMate(gameObject);
        if(accepted){
            m_MateTarget = female;
            m_FSM.state = AgentState.GoingToPartner;
        }
        else{
            unimpressed_females.Add(female);
            StartCoroutine(ForgetRegection(female));
        }
        return accepted;
    }

    IEnumerator ForgetRegection(GameObject female)
    {
        yield return new WaitForSeconds(10.0f);
        unimpressed_females.Remove(female);
    }

    public bool RequestMate(GameObject male)
    {
        float chance_notcritical = Mathf.Lerp(0.1f, 1f, m_LifeComponent.m_CurrentReproductionUrge/m_AgentGenes.m_CriticalUrge);
        if(Random.Range(0.0f, 1.0f) > chance_notcritical){
            Debug.Log(male + "tried, but not critical reproduction");
            return false;
        }
        
        float male_chance = male.GetComponent<Agent>().m_AgentGenes.m_Desirabilty;
        float chance = Mathf.Lerp(0.9999f, 1.0f, male_chance);
        if(Random.Range(0.0f, 1.0f) > chance)
            return false;
            
        m_MateTarget = male;
        m_FSM.state = AgentState.WaitingForPartner;


        // if (!visible_animals.Contains(m_MateTarget))
        // {
        //     m_MateTarget.GetComponent<Agent>().m_MateTarget = null;
        //     m_MateTarget.GetComponent<Agent>().m_FSM.state = AgentState.Exploring;
        //     m_FSM.state = AgentState.Exploring;
        //     m_MateTarget = null;
        //     Debug.Log("Not contains, reproduction failed");
        //     return false;
        // }


        return true;
    }

    private void Update()
    {
        if(m_LifeComponent.m_CurrentHunger >= 1.0f)
            Die(CauseOfDeath.Hunger);
        else if(m_LifeComponent.m_CurrentThirst >= 1.0f)
            Die(CauseOfDeath.Thirst);
        else if(m_LifeComponent.m_RemainingLifetime <= 0.0f)
            Die(CauseOfDeath.Age);

        m_FSM.DecideNextState();
        var steer = m_FSM.GetStateSteer();
        m_SteerBehavior.ApplyForce(steer, m_AgentGenes.m_MaxSpeed * gameObject.transform.localScale.magnitude*Time.timeScale);
        ConsumeFood();
        ConsumeWater();
        ScanForPartner();
        Reproduce();
    }
}