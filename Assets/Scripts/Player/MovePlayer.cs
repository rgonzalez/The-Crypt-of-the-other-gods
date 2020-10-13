using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class MovePlayer : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Maximum movement speed (in m/s).")]
    public float playerSpeed;

    [Tooltip("place a negative value to apply gravity, or positive value to 'float' the object")]
    public float gravityValue;

    // control variables to keep the status between frames
    private Vector3 moveDirection;
    private bool groundedPlayer;

    private Animator animator;
    // the vertical velocity for gravity for the object, 
    private Vector3 playerVelocity;
    private CharacterController controller;


    /// <summary>
    /// we set few animator parameters depending on the 
    /// player movement
    /// </summary>

    private void Animate()
    {

        // we need check if the player is moving to the mouse or backward
        // if the player has the mouse at the left of the character, and move left, is moving "forward", if
        // he moves right is moving "backward"
        bool facingMouse = true;

        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitdist = 0.0f;
        // If the ray is parallel to the plane, Raycast will return false.
        if (playerPlane.Raycast(ray, out hitdist))
        {
            // Get the point along the ray that hits the calculated distance.
            Vector3 targetPoint = ray.GetPoint(hitdist);
            // check if rotate right or left depeding on the gameobject.transform - raycast
            if (this.transform.position.x - targetPoint.x >= 0)
            {
                // facing LEFT 
               // this.transform.localScale = new Vector3(-1 * System.Math.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
                // if is running, is facing the camera?
                animator.SetBool("facingMouse",moveDirection.x <0);
            } else
            {
                // facing RIGHT
               // this.transform.localScale = new Vector3(System.Math.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
                // if is running, is facing the camera?
                animator.SetBool("facingMouse", moveDirection.x > 0);
            }
            // also we set the direction horizontal to look (so we can check in animator if < 0 ==> looking left)
            float horizontalLookPos =  targetPoint.x - this.transform.position.x;
            float verticalLookPos = targetPoint.z - this.transform.position.z;
            animator.SetFloat("horizontalLookPos", horizontalLookPos);
            animator.SetFloat("verticalLookPos", verticalLookPos);

            int verticalLook = 0; // 1 look top, -1 look down
            int horizontalLook = 0; // -1 look left, 1 look right
            // we extract the real position to look: if look down/right, the higher abs(value) wins
            // CHECK BETWEEN LEFT LOOK
            if (horizontalLookPos < 0)
            {
                // CHECK BETWEN LEFT/UP
                if (verticalLookPos > 0)
                {
                    if (Math.Abs(verticalLookPos) < Math.Abs(horizontalLookPos))
                    {
                        verticalLook = 0;
                        horizontalLook = -1;
                    } else
                    {
                        verticalLook = 1;
                        horizontalLook = 0;
                    }
                } else
                {
                    // CHECK BETWEEN LEFT/DOWN
                    if (Math.Abs(verticalLookPos) < Math.Abs(horizontalLookPos))
                    {
                        verticalLook = 0;
                        horizontalLook = -1;
                    }
                    else
                    {
                        verticalLook = -1;
                        horizontalLook = 0;
                    }
                }
            } else
            {
                //CHECK RIGHT
                if (verticalLookPos > 0)
                {
                    // CHECK BETWEN RIGHT/UP
                    if (Math.Abs(verticalLookPos) < Math.Abs(horizontalLookPos))
                    {
                        verticalLook = 0;
                        horizontalLook = 1;
                    }
                    else
                    {
                        verticalLook = 1;
                        horizontalLook = 0;
                    }
                }
                else
                {
                    // CHECK BETWEEN RIGHT/DOWN
                    if (Math.Abs(verticalLookPos) < Math.Abs(horizontalLookPos))
                    {
                        verticalLook = 0;
                        horizontalLook = 1;
                    }
                    else
                    {
                        verticalLook = -1;
                        horizontalLook = 0;
                    }
                }
            }

            animator.SetInteger("horizontalLook", horizontalLook);
            animator.SetInteger("verticalLook", verticalLook);
        }

        //set if the player is moving by controls, in boolean
        animator.SetBool("isMoving", (moveDirection != Vector3.zero));
        //now we set the parameters of the moving by axis
        animator.SetFloat("horizontalSpeed", moveDirection.x);
        animator.SetFloat("verticalSpeed", moveDirection.z);
        animator.SetFloat("fallSpeed", playerVelocity.y);

    }


    public virtual void Awake()
    { 
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
     
        // The actual player will only move in horizontal, but we extract the grounded in case

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(moveDirection * Time.deltaTime * playerSpeed);

        if (moveDirection != Vector3.zero)
        {
            gameObject.transform.forward = moveDirection;
        }

        // Changes the height position of the player..
        /* if (Input.GetButtonDown("Jump") && groundedPlayer)
         {
             moveDirection.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
         }*/

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        this.transform.rotation = new Quaternion(0f, 0f, 0f,0f);
   
    }

    public virtual void FixedUpdate()
    {
        Animate();
    }
}
