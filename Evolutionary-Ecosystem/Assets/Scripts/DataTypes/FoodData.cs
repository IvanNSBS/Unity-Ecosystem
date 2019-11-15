using UnityEngine;

public class FoodData : MonoBehaviour {
    public float m_nutrition = 1.0f;
    public float usages;
    public float max_usages = 2;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        usages = max_usages;
    }

    public void Eaten(GameObject obj)
    {
        var agent = obj.GetComponent<Agent>();
        if(!agent)
            return;
        
        agent.m_LifeComponent.m_CurrentHunger -= m_nutrition;
        usages = usages - 1.0f;
        Debug.Log("Food was eaten: " + usages);
        gameObject.transform.localScale -= Vector3.one * (1/max_usages);
        if(usages <= 0)
            Destroy(gameObject);
            
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        // if(!other.isTrigger)
        //     Destroy(this.gameObject);
    }
}