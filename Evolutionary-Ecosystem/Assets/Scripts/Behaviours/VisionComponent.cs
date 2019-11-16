
using UnityEngine;

public class VisionComponent : MonoBehaviour 
{
    Agent obj_agent;
    public bool m_IsForget = false;
    CircleCollider2D vision;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        obj_agent = gameObject.GetComponent<Agent>();
        vision = gameObject.AddComponent<CircleCollider2D>();
        vision.radius = !m_IsForget ? obj_agent.m_AgentGenes.m_SightRadius : obj_agent.m_AgentGenes.m_ForgetRadius;
        vision.isTrigger = true;
    }    

    void OnTriggerEnter2D(Collider2D other)
    {
        var agent = other.gameObject.GetComponent<Agent>();
        if(!agent){
            if(!m_IsForget && !obj_agent.visible_food.Contains(other.gameObject))
                obj_agent.visible_food.Add(other.gameObject);
        }

        else if(!m_IsForget && !obj_agent.visible_animals.Contains(other.gameObject) && !other.isTrigger){
            // Debug.Log("Adding " + other.gameObject);
            obj_agent.visible_animals.Add(other.gameObject);
        }    
    }
    private void OnTriggerExit2D(Collider2D other) {
        var agent = other.gameObject.GetComponent<Agent>();
        if(!agent){
            if(!m_IsForget && obj_agent.visible_food.Contains(other.gameObject))
                obj_agent.visible_food.Remove(other.gameObject);
        }
                
        else if(!m_IsForget && obj_agent.visible_animals.Contains(other.gameObject) && !other.isTrigger){
            obj_agent.visible_animals.Remove(other.gameObject);
        }
    }
}