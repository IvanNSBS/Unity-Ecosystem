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
        m_SteerWeights.Add(AgentState.Fleeing, new[]{ 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f } );
        m_SteerWeights.Add(AgentState.Exploring, new[]{ 1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f } );
        m_SteerWeights.Add(AgentState.GoingToFood, new[]{ 1.0f, 0.0f, 1.0f, 0.0f, 0.7f, 0.0f } );
        m_SteerWeights.Add(AgentState.GoingToWater, new[]{ 1.0f, 0.0f, 0.0f, 0.0f, 0.7f, 1.0f } );
        m_SteerWeights.Add(AgentState.GoingToPartner, new[]{ 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f} );
        m_SteerWeights.Add(AgentState.WaitingForPartner, new[]{ 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f } );
        m_SteerWeights.Add(AgentState.Eating, new[]{ 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f } );
        m_SteerWeights.Add(AgentState.Drinking, new[]{ 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f } );
        m_SteerWeights.Add(AgentState.SearchingForMate, new[]{1.0f, 0.0f, 0.0f, 1.0f, 0.7f, 0.0f});
        m_SteerWeights.Add(AgentState.Reproducing, new[]{1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f});
    }

    public void DecideNextState()
    {
        if( m_Owner.visible_predators.Count == 0 && 
           (state == AgentState.Eating  || state == AgentState.Drinking ||
            state == AgentState.Resting || state == AgentState.SearchingForMate ||
            state == AgentState.Reproducing) )
            return;

        float hunger = m_Owner.m_LifeComponent.m_CurrentHunger;
        float thirst = m_Owner.m_LifeComponent.m_CurrentThirst;
        float urge = m_Owner.m_LifeComponent.m_CurrentReproductionUrge;
        float energy = m_Owner.m_LifeComponent.m_CurrentEnergy;

        float hunger_pct = hunger;
        float thirst_pct = thirst;
        float urge_pct = urge;
        float energy_pct = energy/m_Owner.m_LifeComponent.m_TotalEnergy;

        // Priority = Flee --> DrinkWater -->  Eat --> Reproduce --> Rest
        if(m_Owner.visible_predators.Count > 0)
            state = AgentState.Fleeing;
        else if(m_Owner.m_MateTarget != null && m_Owner.m_MateTarget.activeSelf){
            if(m_Owner.m_AgentGenes.m_IsMale)
                state = AgentState.GoingToPartner;
            else{
                m_Owner.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                state = AgentState.WaitingForPartner;
            }
        }
        else if(thirst_pct > m_Owner.m_AgentGenes.m_CriticalThirst && m_Owner.visible_water.Count > 0)
            state = AgentState.GoingToWater;
        else if(hunger_pct > m_Owner.m_AgentGenes.m_CriticalHunger && m_Owner.visible_food.Count > 0)
            state = AgentState.GoingToFood;
        else
            state = AgentState.Exploring;

        if(state != AgentState.Exploring)
            m_Owner.ResetWanderPoint();
    }

    public Vector2 GetStateSteer()
    {
        Vector2 steer_seek = Vector2.zero;
        Vector2 steer_water = Vector2.zero;
        Vector2 steer_explore = Vector2.zero;
        Vector2 steer_avoid = Vector2.zero;
        Vector2 steer_reproduction = Vector2.zero;

        bool explore_arrive = false, seek_arrive = false, water_arrive = false;
        bool reproduction_arrive = false;

        m_Owner.EvadeAgents(ref steer_avoid);
        m_Owner.SeekFood(ref steer_seek, ref seek_arrive);
        m_Owner.Explore(ref steer_explore, ref explore_arrive);
        m_Owner.SeekWater(ref steer_water, ref water_arrive);
        if(m_Owner.m_AgentGenes.m_IsMale)
            m_Owner.GoToMate(ref steer_reproduction, ref reproduction_arrive);

        steer_water *= m_SteerWeights[state][GameplayStatics.w_watersearch];
        steer_explore *= m_SteerWeights[state][GameplayStatics.w_exploring];
        steer_avoid *= m_SteerWeights[state][GameplayStatics.w_flee];
        steer_seek *= m_SteerWeights[state][GameplayStatics.w_resourcesearch];
        if(m_Owner.m_AgentGenes.m_IsMale){
            steer_reproduction *= m_SteerWeights[state][GameplayStatics.w_matesearch];
        }

        Vector2 sum = steer_seek + steer_avoid + steer_explore + steer_reproduction + steer_water;
        // if(state == AgentState.GoingToPartner && !reproduction_arrive){
        //     Debug.Log("seek food = " + steer_seek );
        //     Debug.Log("seek water = " + steer_water );
        //     Debug.Log("avoid = " + steer_avoid );
        //     Debug.Log("explore = " + steer_explore );
        //     Debug.Log("reproduction = " + steer_reproduction );
        // }
        if(state == AgentState.GoingToPartner && reproduction_arrive){
            state = AgentState.Reproducing;
            m_Owner.m_MateTarget.GetComponent<Agent>().m_FSM.state = AgentState.Reproducing;
        }
        // sum = steer_avoid;
        // sum = steer_seek;
        return sum;
    }

}