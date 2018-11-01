using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    [Header("Controller vars")]
    public float moveSpeed = 6f;
    public float jumpForce = 15f;
    public float secondJumpForce = 15f;
    public float gravity = 20f;
    public float rotSpeed = 90f;
    private bool canJump = false;
    private int jumpCounter = 0;
    private AudioSource aSource;

    [Header("UI vars")]
    public Text scoreTxt;
    public AudioClip coinSfx;
    private int score;

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;

    [Header("Animator vars")]
    public Animator anim;
    private

	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController>();
        aSource = GetComponent<AudioSource>();
        EnableAnimatorState("idle");
    }
	
	// Update is called once per frame
	void Update () {
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection = moveDirection * moveSpeed;
            if (Input.GetAxis("Vertical") == 0)
            {
                EnableAnimatorState("idle");
            }
            else
            {
                EnableAnimatorState("run");
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);
        controller.Move(moveDirection * Time.deltaTime);
        controller.transform.Rotate(0, Input.GetAxis("Horizontal") * rotSpeed*Time.deltaTime, 0);

    }

    private void Jump()
    {

        if (controller.isGrounded)
        {
            moveDirection.y = jumpForce;
            canJump = true;
            jumpCounter = 0;
            EnableAnimatorState("jump");

        }
        else if (canJump && jumpCounter < 1)
        {
            moveDirection.y = secondJumpForce;
            canJump = false;
            jumpCounter++;
            EnableAnimatorState("backflip");
        }

    }

    private void DisableAnimatorStates(Animator _anim ,string _stateName)
    {
        foreach (AnimatorControllerParameter param in _anim.parameters)
        {
            if (param.name != _stateName)
            {
                _anim.SetBool(param.name, false);
            }
        }
    }

    private void EnableAnimatorState(string _stateName)
    {
        DisableAnimatorStates(anim, _stateName);
        anim.SetBool(_stateName, true);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Coins")
        {
            aSource.clip = coinSfx;
            aSource.Play();
            score++;
            scoreTxt.text = "" + score;
            Destroy(hit.gameObject);
        }
    }
}
