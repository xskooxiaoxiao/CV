using UnityEngine;
using NPBehave;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Complete
{
    /*
    A simple AI based on behaviour trees.
    */
    public class AgentAI : MonoBehaviour
    {
        public int m_PlayerNumber = 1;      // Used to identify which tank belongs to which player.  This is set by this tank's manager.
        public int m_Behaviour = 0;         // Used to select an AI behaviour in the Unity Inspector

        private AgentMovement m_Movement;    // Reference to tank's movement script, used by the AI to control movement.
        private List<GameObject> m_Targets; // List of enemy targets for this tank
        private Root tree;                  // The tank's behaviour tree
        private Blackboard blackboard;      // The tank's behaviour blackboard

        private float fleeTimer = 0f;
        private float stationaryTimer = 0f;

        // Initialisation
        private void Awake()
        {
            m_Targets = new List<GameObject>();
        }

        // Start behaviour tree
        private void Start()
        {
            Debug.Log("Initialising AI player " + m_PlayerNumber);
            m_Movement = GetComponent<AgentMovement>();

            tree = CreateBehaviourTree();
            blackboard = tree.Blackboard;
            #if UNITY_EDITOR
            Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
            debugger.BehaviorTree = tree;
            #endif

            tree.Start();
        }

        private Root CreateBehaviourTree()
        {
            
            switch (m_Behaviour)
            {
                // N=1
                case 1:
                    return TrackAndRandomMove();
                // N=2
                case 2:
                    return FleeAndRandomMove();
                case 3:
                    return RandomMove();
               
                default:
                    return Stop();
            }
        }

        private void Move(float moveX, float moveY)
        {
            m_Movement.AIMove(moveX, moveY);
        }

        private void UpdatePerception()
        {
            Vector3 targetPos = TargetTransform().position;
            Vector3 localPos = this.transform.InverseTransformPoint(targetPos);
            Vector3 heading = localPos.normalized;
            blackboard["targetDistance"] = localPos.magnitude;
            blackboard["targetOnRight"] = localPos.x > 0;
            blackboard["targetOnLeft"] = localPos.x < 0;
            blackboard["targetAbove"] = localPos.y > 0;
            blackboard["targetBelow"] = localPos.y < 0;
            blackboard["targetOffCentre"] = Mathf.Abs(heading.x);
        }

        public void AddTarget(GameObject target)
        {
            m_Targets.Add(target);
        }

        private Transform TargetTransform()
        {
            if (m_Targets.Count > 0)
            {
                return m_Targets[0].transform;
            }
            else
            {
                return null;
            }
        }

        // case Stop
        private Root Stop()
        {
            return new Root(new Action(() => Move(0, 0)));
        }

        // case1
        private Root TrackAndRandomMove()
        {
            // Create a sequence node to execute child nodes in order
            return new Root(
                new Sequence(
                    // Update perception information
                    new Action(UpdatePerception),
                    // Check distance to target and move accordingly
                    new Action(() =>
                    {
                        // Get the target's position
                        Transform targetTransform = TargetTransform();
                        if (targetTransform != null)
                        {
                            // Calculate the distance to the target
                            float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);
                            // If distance is less than 15, move towards the target; otherwise, move randomly
                            if (distanceToTarget < 15f)
                            {
                                // Move towards the target
                                // Calculate the direction towards the target relative to the agent
                                Vector3 targetDirection = (targetTransform.position - transform.position).normalized;
                                // Call the Move method to move the agent towards the target direction
                                Move(targetDirection.x, targetDirection.y);
                            }
                            else
                            {
                                // Move randomly
                                // Generate random direction for movement
                                float randomX = UnityEngine.Random.Range(-1f, 1f);
                                float randomY = UnityEngine.Random.Range(-1f, 1f);
                                // Call the Move method to move the agent randomly
                                Move(randomX, randomY);
                            }
                        }
                    }),
                    // Delay between random movements
                    new Wait(UnityEngine.Random.Range(1f, 3f))
                )
            );
        }




        // case2
        private Root FleeAndRandomMove()
        {
            // Create a sequence node to execute child nodes in order
            return new Root(
                new Sequence(
                    // Update perception information
                    new Action(UpdatePerception),
                    // Flee from the target
                    new Action(() =>
                    {
                        stationaryTimer += Time.deltaTime; // Increment stationary timer
                        Transform targetTransform = TargetTransform();
                        if (targetTransform != null)
                        {
                            float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);
                            if (distanceToTarget < 10f)
                            {
                                Vector3 fleeDirection = (transform.position - targetTransform.position).normalized;
                                if (stationaryTimer >= 6f) // Check if it's time to change flee direction
                                {
                                    // Generate a new random flee direction
                                    float randomOffsetX = UnityEngine.Random.Range(-0.2f, 0.2f);
                                    float randomOffsetY = UnityEngine.Random.Range(-0.2f, 0.2f);
                                    fleeDirection = new Vector3(randomOffsetX, randomOffsetY, 0f).normalized;

                                    stationaryTimer = 0f; // Reset stationary timer
                                }
                                Move(fleeDirection.x, fleeDirection.y);
                            }
                            else
                            {
                                float randomX = UnityEngine.Random.Range(-1f, 1f);
                                float randomY = UnityEngine.Random.Range(-1f, 1f);

                                Move(randomX, randomY);
                                stationaryTimer = 0f; // Reset stationary timer
                            }
                        }
                    }),
                    new Wait(UnityEngine.Random.Range(1f, 3f))
                )
            );
        }

        // case3
        private Root RandomMove()
        {
            // Generate a random duration for movement between 1 and 3 seconds
            float randomDuration = UnityEngine.Random.Range(1f, 3f);

            // Create a root node for the behavior tree
            return new Root(
                // Create a sequence node to execute child nodes in order
                new Sequence(
                    // Action node for random movement
                    new Action(() =>
                    {
                        // Generate random movement values
                        float randomMoveX = UnityEngine.Random.Range(-1f, 1f);
                        float randomMoveY = UnityEngine.Random.Range(-1f, 1f);
                        // Call the Move method to move the agent randomly
                        Move(randomMoveX, randomMoveY);
                    }),
                    // Wait for the random duration before proceeding
                    new Wait(randomDuration)
                )
            );
        }
        

    }
}