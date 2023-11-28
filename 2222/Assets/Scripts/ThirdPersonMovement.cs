using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
  //Essentials
  public CharacterController controller;
  public Transform cam;
  Animator animator;
  public float turnSmoothTime = 0.1f;
  float turnSmoothVelocity;
  //Movement
  public float walkspeed;
  public float runspeed;
  bool isRunning;
  public float trueSpeed;

  //jumping
  public float jumpHeight;
  public float gravity = -9.81f;
  bool isGrounded;
  public LayerMask groundMask;
  Vector3 velocity;


  void Start()
  {
    trueSpeed = walkspeed;
    animator = GetComponentInChildren<Animator>();
  }

  // Update is called once per frame
  void Update()
  {
    isGrounded = Physics.CheckSphere(transform.position, 0.2f, groundMask);

    if (isGrounded && velocity.y < 0)
    {
      velocity.y = -2f;
      animator.SetBool("isGrounded", true);
    }
    else
    {
      animator.SetBool("isGrounded", false);
    }

    if (Input.GetKey(KeyCode.LeftShift))
    {
      trueSpeed = runspeed;
      isRunning = true;
    }
    if (Input.GetKeyUp(KeyCode.LeftShift))
    {
      trueSpeed = walkspeed;
      isRunning = false;
    }
    animator.transform.localPosition = Vector3.zero;
    animator.transform.localEulerAngles = Vector3.zero;
    float horizontal = Input.GetAxisRaw("Horizontal");
    float vertical = Input.GetAxisRaw("Vertical");
    Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

    if (direction.magnitude >= 0.1f)
    {
      float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y; // find the angle in degrees
      float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); // smooth the angle
      transform.rotation = Quaternion.Euler(0f, angle, 0f); // rotate the player to the angle

      Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward; // move the player in the direction of the angle
      controller.Move(moveDir.normalized * trueSpeed * Time.deltaTime);

      if (isRunning == true)
      {
        animator.SetFloat("movementSpeed", 2); // set the animation to running
      }
      else
      {
        animator.SetFloat("movementSpeed", 1); // set the animation to walking
      }
    }
    else
    {
      animator.SetFloat("movementSpeed", 0); // set the animation to idle
    }


    //jumping
    if (Input.GetButtonDown("Jump") && isGrounded)
    {
      velocity.y = Mathf.Sqrt(jumpHeight);
      animator.SetBool("isJumping", true);
    }
    else
    {
      animator.SetBool("isJumping", false);
    }



    //gravity 
    velocity.y += gravity * Time.deltaTime;
    controller.Move(velocity * 7f * Time.deltaTime);

  }
}
