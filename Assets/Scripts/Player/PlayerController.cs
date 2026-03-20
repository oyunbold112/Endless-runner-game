using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    public float forwardSpeed;
    public float maxSpeed;
    private Vector3 direction;
    private int desiredLane = 1; //0 = left 1 = middle 2 = right
    public float laneDistance; // the distance between two lanes
    public float jumpForce = 10f;
    public float Gravity = -20;
    public Animator animator;
    private bool isSliding = false;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerManager.isGameStarted) return;
        // increase speedo
        if (forwardSpeed < maxSpeed)
        {
            forwardSpeed += 0.1f * Time.deltaTime;
        }
        animator.SetBool("isGameStarted", true);
        direction.z = forwardSpeed;
        animator.SetBool("isGrounded", controller.isGrounded);
        if (controller.isGrounded)
        {
            if (SwipeManager.swipeUp)
            {
                Jump();
            }

        }
        else
        {
            direction.y += Gravity * Time.deltaTime;
        }
        if (SwipeManager.swipeDown && !isSliding)
        {
            StartCoroutine(Slide());
        }
        if (SwipeManager.swipeRight)
        {
            desiredLane++;
            if (desiredLane > 2) desiredLane = 2;
        }
        if (SwipeManager.swipeLeft)
        {
            desiredLane--;
            if (desiredLane < 0) desiredLane = 0;
        }
        Vector3 targetPosition = transform.position.z * Vector3.forward + Vector3.up * transform.position.y;
        if (desiredLane == 0) targetPosition += Vector3.left * laneDistance;
        else if (desiredLane == 2)
            targetPosition += Vector3.right * laneDistance;
        // 
        if (transform.position != targetPosition)
        {
            Vector3 diff = targetPosition - transform.position;
            Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;
            if (moveDir.sqrMagnitude < diff.sqrMagnitude)
                controller.Move(moveDir);
            else
                controller.Move(diff);
        }

        // Move player
        controller.Move(direction * Time.deltaTime);
    }

    private void Jump()
    {
        direction.y = jumpForce;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Obstacle")
        {
            PlayerManager.gameOver = true;
            FindObjectOfType<AudioManager>().PlaySound("Gameover");
        }
    }
    private IEnumerator Slide()
    {
        isSliding = true;
        animator.SetBool("isSliding", true);
        controller.center = new Vector3(0, -0.5f, 0);
        controller.height = 1;
        yield return new WaitForSeconds(1.3f);
        controller.center = new Vector3(0, 0, 0);
        controller.height = 2;

        animator.SetBool("isSliding", false);
        isSliding = false;
    }
}