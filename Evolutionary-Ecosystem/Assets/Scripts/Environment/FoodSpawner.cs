using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public int m_InitialAmount = 20;
    public float m_SpawnPct = 0.05f;
    public GameObject m_FoodPrefab;
    void Start()
    {
        for(int i = 0; i < m_InitialAmount; i++)
        {
            var obj = Instantiate(m_FoodPrefab);
            obj.gameObject.transform.position = new Vector2( Random.Range(-4.0f, 8.0f), Random.Range(0.0f, 4.0f) );
        }        
    }    

    private void Update() {
        if(Random.Range(0.0f, 1.0f) < m_SpawnPct){
            var obj = Instantiate(m_FoodPrefab);
            obj.gameObject.transform.position = new Vector2( Random.Range(-4.0f, 8.0f), Random.Range(0.0f, 4.0f) );
        }

    }
}