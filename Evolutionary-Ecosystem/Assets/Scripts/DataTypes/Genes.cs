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
    private float mutChance = .2f;
    private float maxMutAmount = .2f; 

    private float TryMutate(float val)
    {
        if(Random.Range(0.0f, 1.0f) < mutChance){
            float mut_amnt = Random.Range(-maxMutAmount, maxMutAmount);
            val = val + (val*mut_amnt);
            Debug.Log("Mutation happened");
        }
        return val;
    }
    public void RandomizeGenes(AnimalGenesRanges ranges)
    {
        random = new System.Random();
        m_MaxSpeed = Random.Range(ranges.min_speed, ranges.max_speed);
        m_MaxForce = Random.Range(ranges.min_force, ranges.max_force);
        m_MaxBrake = 3*m_MaxForce;
        m_SightRadius = Random.Range(ranges.min_sight_range, ranges.max_sight_range);
        m_ForgetRadius = Random.Range(ranges.min_forget_range, ranges.max_forget_range);
        m_IsMale = Random.Range(0.0f, 1.0f) > 0.5f;
        m_CriticalThirst = Random.Range(0.1f, 0.85f);
        m_CriticalHunger = Random.Range(0.1f, 0.85f);
        m_CriticalUrge = Random.Range(0.1f, 0.9f);
        m_Desirabilty = Random.Range(0.1f, 1.0f);
        m_GestationDuration = Random.Range(ranges.min_gestation_duration, ranges.max_gestation_duration);
        m_MaxOffsprings = Random.Range(1, ranges.max_offsprings);


        m_Agression = Random.Range(0.0f, 1.0f); 
        m_Altruism = Random.Range(0.0f, 1.0f); 
    }

    public void CrossOver(Genes father, Genes mother)
    {
        m_MaxSpeed = Random.Range(0f, 1f) > 0.5f ? father.m_MaxSpeed : mother.m_MaxSpeed;
        m_MaxForce = Random.Range(0f, 1f) > 0.5f ? father.m_MaxForce : mother.m_MaxForce;
        m_MaxSpeed = TryMutate(m_MaxSpeed);
        m_MaxForce = TryMutate(m_MaxForce);
        m_MaxBrake = 3*m_MaxForce;


        m_SightRadius = Random.Range(0f, 1f) > 0.5f ? father.m_SightRadius : mother.m_SightRadius;
        m_ForgetRadius = Random.Range(0f, 1f) > 0.5f ? father.m_ForgetRadius : mother.m_ForgetRadius;
        m_SightRadius = TryMutate(m_SightRadius);
        m_ForgetRadius = TryMutate(m_ForgetRadius);

        m_IsMale = Random.Range(0.0f, 1.0f) > 0.5f;


        m_CriticalThirst = Random.Range(0f, 1f) > 0.5f ? father.m_CriticalHunger : mother.m_CriticalHunger;
        m_CriticalHunger = Random.Range(0f, 1f) > 0.5f ? father.m_CriticalThirst : mother.m_CriticalThirst;
        m_CriticalUrge = Random.Range(0f, 1f) > 0.5f ? father.m_CriticalUrge : mother.m_CriticalUrge;
        m_CriticalThirst = TryMutate(m_CriticalThirst);
        m_CriticalHunger = TryMutate(m_CriticalHunger);
        m_CriticalUrge = TryMutate(m_CriticalUrge);


        m_Desirabilty = Random.Range(0f, 1f) > 0.5f ? father.m_Desirabilty : mother.m_Desirabilty;
        m_GestationDuration = Random.Range(0f, 1f) > 0.5f ? father.m_GestationDuration : mother.m_GestationDuration;
        m_Desirabilty = TryMutate(m_Desirabilty);
        m_GestationDuration = TryMutate(m_GestationDuration);


        m_MaxOffsprings = Random.Range(0f, 1f) > 0.5f ? father.m_MaxOffsprings : mother.m_MaxOffsprings;

        
        m_Agression = Random.Range(0f, 1f) > 0.5f ? father.m_Agression : mother.m_Agression; 
        m_Altruism = Random.Range(0f, 1f) > 0.5f ? father.m_Altruism : mother.m_Altruism;
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