using UnityEngine;

public class WaterData : MonoBehaviour {
    public float m_nutrition = 0.1f;
    public void Consume(GameObject obj, float amount)
    {
        var agent = obj.GetComponent<Agent>();
        if(!agent)
            return;
        
        agent.m_LifeComponent.m_CurrentHunger -= m_nutrition*amount;
        agent.m_LifeComponent.m_CurrentHunger = Mathf.Clamp(agent.m_LifeComponent.m_CurrentHunger, 0.0f, Mathf.Infinity);
    }

}