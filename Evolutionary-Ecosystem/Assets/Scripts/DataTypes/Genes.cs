using UnityEngine;


[System.Serializable]
public class Genes
{
    [SerializeField] public float m_MaxSpeed = 5.0f;
    [SerializeField] public float m_MaxForce = 5.0f;
    [SerializeField] public float m_MaxBrake = 20.0f;
    public float m_SightRadius = 2.0f;
    public float m_ForgetRadius = 4.0f;
    [SerializeField] public bool m_IsMale = true;
    public float m_CriticalThirst = 0.3f, m_NotThirsty = 0.1f;
    public float m_CriticalHunger = 0.2f, m_NotHungry = 0.05f;
    public float m_CriticalEnergy = 0.35f, m_NotTired = 0.85f;
    public float m_CriticalUrge = 0.85f;
    public float m_Desirabilty = 0.3f, m_GestationDuration = 20.0f;
    public int m_MaxOffsprings = 5;
    public float m_Agression = 0.5f;
    public float m_Altruism = 0.5f;


    
    // Genes()
    // {
    //     // m_IsMale = Random.Range(0.0f, 1.0f) < 0.5;
    //     m_MaxSpeed = 5.0f;
    //     m_MaxForce = 5.0f;
    // }
}