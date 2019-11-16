using UnityEngine;

public class FoodData : MonoBehaviour {
    public float m_nutrition = 10.0f;
    public float usages;
    public float max_usages = 2;
    private Vector3 start_scale;

    
    void Start()
    {
        usages = max_usages;
        start_scale = gameObject.transform.localScale;
    }

    public void Consume(GameObject obj, float amount)
    {
        var agent = obj.GetComponent<Agent>();
        if(!agent)
            return;
        
        agent.m_LifeComponent.m_CurrentHunger -= m_nutrition*amount;
        agent.m_LifeComponent.m_CurrentHunger = Mathf.Clamp(agent.m_LifeComponent.m_CurrentHunger, 0.0f, Mathf.Infinity);
        usages -= amount;
        if(usages <= 0)
            Destroy(gameObject);
        else    
            gameObject.transform.localScale = start_scale * (usages/max_usages);
    }

    private void Update() {
        usages += Time.deltaTime * 0.1f;
        usages = Mathf.Clamp(usages, 0.0f, max_usages);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // if(!other.isTrigger)
        //     Destroy(this.gameObject);
    }
}