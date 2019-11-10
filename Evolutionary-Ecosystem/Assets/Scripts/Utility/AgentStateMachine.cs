using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AgentStateMachine
{
    Dictionary<AgentState, float[]> m_SteerWeights = new Dictionary<AgentState, float[]>();
    public AgentState state;
    [HideInInspector] public Agent m_Owner;
    public AgentStateMachine( Agent owner )
    {
        m_Owner = owner;    
        m_SteerWeights.Add(AgentState.Fleeing, new[]{ 1.0f, 0.0f, 0.0f, 0.0f, 0.4f } );
        m_SteerWeights.Add(AgentState.Exploring, new[]{ 1.0f, 1.0f, 0.0f, 0.0f, 0.7f } );
        m_SteerWeights.Add(AgentState.GoingToFood, new[]{ 1.0f, 1.0f, 1.0f, 1.0f, 0.7f } );
        m_SteerWeights.Add(AgentState.GoingToWater, new[]{ 1.0f, 1.0f, 1.0f, 1.0f, 0.7f } );
        m_SteerWeights.Add(AgentState.GoingToNest, new[]{ 1.0f, 1.0f, 1.0f, 1.0f, 0.7f } );
        m_SteerWeights.Add(AgentState.Eating, new[]{ 1.0f, 0.0f, 0.0f, 0.0f, 0.0f } );
        m_SteerWeights.Add(AgentState.Drinking, new[]{ 1.0f, 0.0f, 0.0f, 0.0f, 0.0f } );
        m_SteerWeights.Add(AgentState.Resting, new[]{ 0.0f, 0.0f, 0.0f, 0.0f, 0.0f } );
        m_SteerWeights.Add(AgentState.SearchingForMate, new[]{1.0f, 0.0f, 0.0f, 1.0f, 0.7f});
    }

    public void DecideNextState()
    {
        float hunger = m_Owner.m_LifeComponent.m_CurrentHunger;
        float thirst = m_Owner.m_LifeComponent.m_CurrentThirst;
        float urge = m_Owner.m_LifeComponent.m_CurrentReproductionUrge;
        float energy = m_Owner.m_LifeComponent.m_CurrentEnergy;

        float hunger_pct = hunger/m_Owner.m_LifeComponent.m_TimeToDeathByHunger;
        float thirst_pct = thirst/m_Owner.m_LifeComponent.m_TimeToDeathByThirst;
        float urge_pct = urge/m_Owner.m_LifeComponent.m_TotalReproductionUrge;
        float energy_pct = energy/m_Owner.m_LifeComponent.m_TotalEnergy;

        // Priority = Flee --> DrinkWater -->  Eat --> Reproduce --> Rest
        if(m_Owner.visible_predators.Count > 0)
            state = AgentState.Fleeing;
        if(thirst_pct > hunger_pct && thirst_pct >= m_Owner.m_AgentGenes.m_CriticalThirst)
            state = AgentState.GoingToWater;
        else if( hunger_pct >= m_Owner.m_AgentGenes.m_CriticalHunger)
            state = AgentState.GoingToFood;
        else
            state = AgentState.Exploring;

        if(state != AgentState.Exploring)
            m_Owner.ResetWanderPoint();
    }

    public Vector2 GetStateSteer()
    {
        Vector2 steer_seek = Vector2.zero;
        Vector2 steer_explore = Vector2.zero;
        Vector2 steer_avoid = Vector2.zero;

        bool explore_arrive = false, seek_arrive = false;

        m_Owner.EvadeAgents(ref steer_avoid);
        m_Owner.SeekFood(ref steer_seek, ref seek_arrive);
        m_Owner.Explore(ref steer_explore, ref explore_arrive);

        // Debug.Log("State steer explore = " + steer_explore);
        // return steer_explore;

        steer_explore *= m_SteerWeights[state][GameplayStatics.w_exploring];
        steer_avoid *= m_SteerWeights[state][GameplayStatics.w_avoid];
        steer_seek *= m_SteerWeights[state][GameplayStatics.w_resourcesearch];

        Vector2 sum = steer_seek + steer_avoid + steer_explore;
        return sum;
    }

}