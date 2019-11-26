using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "RaceData", menuName = "LifeParameters", order = 1)]
public class RaceLifeParameters : ScriptableObject
{   


    [Header("Parameters")]
    public float m_LifeTime = 500.0f;
    public float m_TimeToDeathByHunger = 250.0f;
    public float m_TimeToDeathByThirst = 180.0f;
    public float m_TotalEnergy = 100.0f;
    public float m_TotalReproductionUrge = 100.0f;
    public float timeToAdulthood = 50.0f;

}