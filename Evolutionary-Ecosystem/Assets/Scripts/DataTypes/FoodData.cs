using UnityEngine;

public class FoodData : MonoBehaviour {
    public float m_nutrition = 1.0f;
    public int usages = 1;

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