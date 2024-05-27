using System;
using UnityEngine;

namespace Complete
{
    [Serializable]
    public class AgentManager
    {

        public Boolean m_NPC;                                   // Is this AI controlled?
        public int m_Behaviour;                                 // Identifer for a behaviour tree

        [HideInInspector] public int m_PlayerNumber;
        [HideInInspector] public GameObject m_Instance;


        private AgentMovement m_Movement;                        
        private AgentAI m_AI;


        public void Setup()
        {
            // Get references to the components.
            m_Movement = m_Instance.GetComponent<AgentMovement>();
            m_AI = m_Instance.GetComponent<AgentAI>();

            // Set the player numbers to be consistent across the scripts.
            m_Movement.m_PlayerNumber = m_PlayerNumber;
            m_AI.m_PlayerNumber = m_PlayerNumber;

            // Let the agent scripts know if they are AI contolled
            m_Movement.m_NPC = m_NPC;

            // Let tank AI know which behaviour to run
            m_AI.m_Behaviour = m_Behaviour;


            // Get all of the renderers of the agent.
            MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        }

        public void AddTarget(GameObject target)
        {
            m_AI.AddTarget(target);
        }

        // Used during the phases of the game where the player/AI shouldn't be able to control their tank.
        public void DisableControl()
        {
            m_Movement.enabled = false;
            m_AI.enabled = false;
        }


        // Used during the phases of the game where the player should be able to control their tank.
        public void EnableControl()
        {
            if (m_NPC)
            {
                m_AI.enabled = true;
            }
            m_Movement.enabled = true;
        }
    }
}