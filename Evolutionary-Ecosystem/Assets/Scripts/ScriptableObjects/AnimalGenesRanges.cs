using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "Data", menuName = "Genes", order = 1)]
public class AnimalGenesRanges : ScriptableObject
{   


    [Header("gest")]
    public int max_offsprings = 5;
    public float min_gestation_duration = 5.0f;
    public float max_gestation_duration = 30.0f;



    [Header("Vision")]
    public float min_sight_range = 1.0f;
    public float max_sight_range = 5.0f;
    public float min_forget_range = 3.0f;
    public float max_forget_range = 5.0f;

    
    [Header("Velocity")]
    public float min_speed = 0.5f;
    public float max_speed = 2.0f;
    public float min_force = 1.0f;
    public float max_force = 3.0f;

}