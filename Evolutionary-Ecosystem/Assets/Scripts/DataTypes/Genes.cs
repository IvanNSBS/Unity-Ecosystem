using UnityEngine;
// using System;

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

    private System.Random random;


    // public Genes(float rng)
    // {
    //     random = new System.Random();
    //     Debug.Log("Genes was constructed!!");
    //     var speed = RandomInRange(0.3f, 1.3f);
    //     Debug.Log("Speed was: " + speed);
    //     m_MaxSpeed = speed;
    //     m_MaxForce = RandomInRange(0.4f, 1.0f);
    //     m_MaxBrake = 3*m_MaxForce;
    //     m_SightRadius = RandomInRange(0.5f, 10.0f);
    //     m_ForgetRadius = RandomInRange(3f, 15.0f);
    //     m_IsMale = RandomInRange(0.0f, 1.0f) > 0.5f;
    //     m_CriticalThirst = RandomInRange(0.1f, 0.85f);
    //     m_CriticalHunger = RandomInRange(0.1f, 0.85f);
    //     m_CriticalUrge = RandomInRange(0.1f, 0.9f);
    //     m_Desirabilty = RandomInRange(0.1f, 1.0f);
    //     m_GestationDuration = RandomInRange(5.0f, 20.0f);
    //     m_MaxOffsprings = random.Next(0, 8);
    //     m_Agression = RandomInRange(0.0f, 1.0f); 
    //     m_Altruism = RandomInRange(0.0f, 1.0f); 
    // }


    public void RandomizeGenes()
    {
        random = new System.Random();
        var speed = Random.Range(0.3f, 1.3f);
        m_MaxSpeed = speed;
        m_MaxForce = Random.Range(0.8f, 2.0f);
        m_MaxBrake = 3*m_MaxForce;
        m_SightRadius = Random.Range(0.5f, 3.0f);
        m_ForgetRadius = Random.Range(3f, 5.0f);
        m_IsMale = Random.Range(0.0f, 1.0f) > 0.5f;
        m_CriticalThirst = Random.Range(0.1f, 0.85f);
        m_CriticalHunger = Random.Range(0.1f, 0.85f);
        m_CriticalUrge = Random.Range(0.1f, 0.9f);
        m_Desirabilty = Random.Range(0.1f, 1.0f);
        m_GestationDuration = Random.Range(5.0f, 20.0f);
        m_MaxOffsprings = Random.Range(1, 5);
        m_Agression = Random.Range(0.0f, 1.0f); 
        m_Altruism = Random.Range(0.0f, 1.0f); 
    }

    public void CrossOver(Genes father, Genes mother)
    {

    }

    float RandomInRange(float min, float max)
    {
        return Mathf.Lerp(min, max, (float)random.NextDouble());
    }

    // Genes()
    // {
    //     // m_IsMale = Random.Range(0.0f, 1.0f) < 0.5;
    //     m_MaxSpeed = 5.0f;
    //     m_MaxForce = 5.0f;
    // }
}