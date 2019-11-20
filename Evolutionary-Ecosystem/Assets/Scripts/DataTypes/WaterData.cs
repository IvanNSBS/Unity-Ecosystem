using UnityEngine;

public class WaterData : MonoBehaviour {
    public float m_nutrition = 0.4f;
    public void Consume(GameObject obj, float amount)
    {
        var agent = obj.GetComponent<Agent>();
        if(!agent)
            return;
        
        agent.m_LifeComponent.m_CurrentThirst -= m_nutrition*amount;
        agent.m_LifeComponent.m_CurrentThirst = Mathf.Clamp(agent.m_LifeComponent.m_CurrentThirst, 0.0f, Mathf.Infinity);
    }

}