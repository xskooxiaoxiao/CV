using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // This is the movement speed
    public static float moveSpeed = 5f;
    // This is the jump speed
    public static float jumpSpeed = 5f;
    // This is the current able jump times
    public static int curJumpTimes = 1;
    // This is the max jump times
    public static int maxJumpTimes = 2;
    // This is the player's x length
    public static float playerXLength = 1.0f;
    // This is the player's z length
    public static float playerZLength = 1.0f;
    // This is the player's height
    public static float playerHeight = 1.0f;

    public bool ifOnGround = true;

    // Fire five ray to determine if the player is standing on the ground or in the air.
    public bool IfGrounded() {
        RaycastHit hit;
        // 
        if(Physics.Raycast(this.transform.position,Vector3.down,out hit,0.1f))
            return true;
        else
            return false;
    }


    // Use WASD to move the player, and use Space to jump, and use LeftShift to sprint.
    public bool MovePlayer(LayerMask groundLayer) {

        bool move = false;

        //----------------------------- 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit, 30, groundLayer.value))
        {
            Vector3 mouse_world_point = hit.point;
            mouse_world_point.y = transform.position.y;
            transform.LookAt(mouse_world_point);

        }


        //-----------------------------
        ifOnGround = IfGrounded();
        // This is the movement part
        //if (Input.GetKey(KeyCode.W)) {
        //    this.transform.position = new Vector3(this.transform.position.x + moveSpeed * Time.deltaTime, this.transform.position.y, this.transform.position.z);
        //    //this.transform.rotation = Quaternion.Euler(0, 0, 0);
        //    move = true;
        //}
        //if (Input.GetKey(KeyCode.S)) {
        //    this.transform.position = new Vector3(this.transform.position.x - moveSpeed * Time.deltaTime, this.transform.position.y, this.transform.position.z);
        //    //this.transform.rotation = Quaternion.Euler(0, 180, 0);
        //    move = true;
        //}
        //if (Input.GetKey(KeyCode.A)) {
        //    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + moveSpeed * Time.deltaTime);
        //    //this.transform.rotation = Quaternion.Euler(0, 270, 0);
        //    move = true;
        //}
        //if (Input.GetKey(KeyCode.D)) {
        //    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - moveSpeed * Time.deltaTime);
        //    //this.transform.rotation = Quaternion.Euler(0, 90, 0);
        //    move = true;
        //}

        move = MoveAxis();
        // This is the jump part
        if (Input.GetKeyDown(KeyCode.Space) && (ifOnGround || curJumpTimes > 0)) {
            if (ifOnGround)
                curJumpTimes = maxJumpTimes - 1;
            else
                curJumpTimes--;
            this.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }
        return move;
    }

    bool MoveAxis()
    {
        
        Vector3 loc = transform.position;
        float hor = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float ver = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        if(hor != 0 || ver!= 0)
        {
            loc.x += hor;
            loc.z += ver;


            loc.x = Mathf.Clamp(loc.x, 6, 34);
            loc.z = Mathf.Clamp(loc.z, 6, 34);
            this.transform.position = loc;
            return true;
        }
        else
        {
            return false;
        }
    }


    // This is the initialization part
    void Init() {
        curJumpTimes = maxJumpTimes;
        this.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
    }


    void Start()
    {
        Init();
    }


    //void Update()
    //{
    //    // Debug.DrawRay(this.transform.position + new Vector3(0, -0.5f * playerHeight, 0), Vector3.down, Color.green);
    //    MovePlayer();
    //}
}
