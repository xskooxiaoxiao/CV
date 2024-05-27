using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MyMap;
using System.Collections.Generic;

namespace Complete
{
    public class GameManager : MonoBehaviour
    {


        public GameObject m_AgentPrefab1;
        public GameObject m_AgentPrefab2;
        public AgentManager[] m_Agent1;
        public AgentManager[] m_Agent2;


        private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.

        private void Start()
        {
            StartCoroutine(Wait());
            StartCoroutine(RoundStarting());
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(0.2f);
            SpawnAllAgents();
        }

        private void SpawnAllAgents()
        {

            Vector2Int randomPosition1 = Map.validAreas[Random.Range(0, Map.validAreas.Count)];
            Vector2Int randomPosition2;

            do
            {
                randomPosition2 = Map.validAreas[Random.Range(0, Map.validAreas.Count)];
            } while (randomPosition2 == randomPosition1);

            // ... create them, set their player number and references needed for control.
            for (int i = 0; i < m_Agent1.Length; i++)
            {
                m_Agent1[i].m_Instance = Instantiate(m_AgentPrefab1, new Vector3(randomPosition1.x, randomPosition1.y, 0), Quaternion.identity) as GameObject;
                m_Agent1[i].m_PlayerNumber = 1;
                m_Agent1[i].Setup();
            }

            for (int i = 0; i < m_Agent2.Length; i++)
            {
                m_Agent2[i].m_Instance = Instantiate(m_AgentPrefab2, new Vector3(randomPosition2.x, randomPosition2.y, 0), Quaternion.identity) as GameObject;
                m_Agent2[i].m_PlayerNumber = 1;
                m_Agent2[i].Setup();
            }

            foreach (AgentManager agent1 in m_Agent1)
            {
                foreach (AgentManager agent2 in m_Agent2)
                {
                    agent1.AddTarget(agent2.m_Instance);
                }
            }

            foreach (AgentManager agent2 in m_Agent2)
            {
                foreach (AgentManager agent1 in m_Agent1)
                {
                    agent2.AddTarget(agent1.m_Instance);
                }
            }
        }

        private IEnumerator RoundStarting()
        {
            // As soon as the round starts reset the tanks and make sure they can't move.
            DisableAgentControl();

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return m_StartWait;
        }

        private void EnableAgentControl()
        {
            for (int i = 0; i < m_Agent1.Length; i++)
            {
                m_Agent1[i].EnableControl();
            }
            for (int i = 0; i < m_Agent2.Length; i++)
            {
                m_Agent2[i].EnableControl();
            }
        }


        private void DisableAgentControl()
        {
            for (int i = 0; i < m_Agent1.Length; i++)
            {
                m_Agent1[i].DisableControl();
            }
            for (int i = 0; i < m_Agent2.Length; i++)
            {
                m_Agent2[i].DisableControl();
            }
        }
    }
}