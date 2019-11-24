using UnityEngine;

public class LifeComponent : MonoBehaviour {

    private float m_EnergyUsage = 0.1f;
    [HideInInspector] public Rigidbody2D m_RigidBody;
    [Header("Life Parameters")]
    public float m_LifeTime = 500.0f;
    public float m_TimeToDeathByHunger = 250.0f;
    public float m_TimeToDeathByThirst = 180.0f;
    public float m_TotalHealth = 1.0f;
    public float m_TotalEnergy = 100.0f;
    public float m_TotalReproductionUrge = 100.0f;
    public float m_RemainingLifetime;
    public float m_CurrentHealth, m_CurrentEnergy;
    public float m_CurrentHunger, m_CurrentThirst;
    public float m_CurrentReproductionUrge;
    public float m_StomachSize = 10.0f;
    public float m_MaxDayFoodEnergy = 70.0f;
    public float m_Weight = 1.0f, max_Weight = 15.0f, timeToGrow = 0.5f;
    float timeToAdulthood = 50.0f;
    public float curTimeToAdulthood = 0.0f;
    [HideInInspector] public Genes m_AgentGenes;

    public void SubtractTimeToAdulthood(float val) { timeToAdulthood-=val; }
    private void Awake() {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_AgentGenes = GetComponent<Agent>().m_AgentGenes;
        ResetLifeStatus();
    }

    public void ResetLifeStatus()
    {
        m_RemainingLifetime = m_LifeTime;
        m_CurrentHunger = 0;
        m_CurrentThirst = 0;
        m_CurrentEnergy = m_TotalEnergy;
        m_CurrentReproductionUrge = 0;
        timeToAdulthood -= m_AgentGenes.m_GestationDuration;
    }

    Vector3 pointsix = new Vector3(0.6f, 0.6f, 0.6f);
    private void Update() {
        m_RemainingLifetime -= Time.deltaTime/m_LifeTime;
        m_CurrentEnergy -= Time.deltaTime*Mathf.Exp(m_RigidBody.velocity.magnitude)*m_EnergyUsage;
        m_CurrentHunger += Time.deltaTime/m_TimeToDeathByHunger;
        m_CurrentThirst += Time.deltaTime/m_TimeToDeathByThirst;
        m_CurrentReproductionUrge += Time.deltaTime/m_TotalReproductionUrge;

        m_CurrentThirst = Mathf.Clamp01(m_CurrentThirst);
        m_CurrentHunger = Mathf.Clamp01(m_CurrentHunger);
        m_CurrentReproductionUrge = Mathf.Clamp01(m_CurrentReproductionUrge);
        m_CurrentEnergy = Mathf.Clamp(m_CurrentEnergy, 0.0f, m_CurrentThirst);
        if(curTimeToAdulthood < 1.0f)
            curTimeToAdulthood += Time.deltaTime/timeToAdulthood;

        gameObject.transform.localScale = Vector3.Lerp(pointsix, Vector3.one, curTimeToAdulthood);
        // gameObject.transform.localScale = Vector3.ClampMagnitude(gameObject.transform.localScale, 1.0f);
    }
}