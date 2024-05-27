using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class AgentMovement : MonoBehaviour
    {
        public int m_PlayerNumber = 1;
        public float speed;
        private bool isControlEnabled = false; // Flag to enable/disable movement control
        private Animator animator;

        public bool m_NPC;                     // Is AI controlled?

        private Vector2 m_MovementInputValue;    // The current value of the movement input.
        private Vector2 m_AIMovement;            // The current value of the AI's movement input.
        private float MoveX;
        private float MoveY;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }


        private void Update()
        {
            if (m_NPC)
            {
                m_MovementInputValue = m_AIMovement;
            } else {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isControlEnabled = !isControlEnabled;
                }

                if (isControlEnabled)
                {
                    Vector2 dir = Vector2.zero;
                    if (Input.GetKey(KeyCode.A))
                    {
                        dir.x = -1;
                        animator.SetInteger("Direction", 3);
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        dir.x = 1;
                        animator.SetInteger("Direction", 2);
                    }

                    if (Input.GetKey(KeyCode.W))
                    {
                        dir.y = 1;
                        animator.SetInteger("Direction", 1);
                    }
                    else if (Input.GetKey(KeyCode.S))
                    {
                        dir.y = -1;
                        animator.SetInteger("Direction", 0);
                    }

                    dir.Normalize();
                    animator.SetBool("IsMoving", dir.magnitude > 0);

                    GetComponent<Rigidbody2D>().velocity = speed * dir;
                }
                else if (!isControlEnabled)
                {
                    // Disable movement when control is disabled
                    GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
            }
            
        }

        public void AIMove(float moveX, float moveY)
        {
            // Set m_MovementInputValue to the movement input received from AI
            MoveX = (moveX > 1) ? 1 : (moveX < -1) ? -1 : moveX;
            MoveY = (moveY > 1) ? 1 : (moveY < -1) ? -1 : moveY;
            m_AIMovement = new Vector2(MoveX, MoveY);

            // Determine animation direction based on movement direction
            if (moveX > 0)
            {
                animator.SetInteger("Direction", 2); // Right
            }
            else if (moveX < 0)
            {
                animator.SetInteger("Direction", 3); // Left
            }

            if (moveY > 0)
            {
                animator.SetInteger("Direction", 1); // Up
            }
            else if (moveY < 0)
            {
                animator.SetInteger("Direction", 0); // Down
            }

            m_AIMovement.Normalize();
            animator.SetBool("IsMoving", m_AIMovement.magnitude > 0);

            // Apply velocity
            GetComponent<Rigidbody2D>().velocity = speed * m_AIMovement;
        }



    }
}
