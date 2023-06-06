using System;
using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Windows;
using UniRx;
using Input = UnityEngine.Input;

public class PlayerController : MonoBehaviour
{
    public float jump_y_interception = 20.0f;
    public float jump_x_interception = 2.0f;
    public float gravity = 16.0f;
    public int jumpLimits = 2;
    public EffectController playerEffects;
    public EffectController globalEffects;
    public Animator animator;
    [SerializeField] Material mat;
    [SerializeField] GameObject mainLight;

    public bool onGround;
    
    CharacterController myCharacterController;
    public PlayerMovementManager playerMovementManager;
    Player player;

    public float moveSpeed => player.statusDic.GetDic()["sta_move"].value;

    public bool canMove
    {
        get
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            return !info.IsTag("Attack")&&!info.IsName("Hit");
        }
    }

    public bool canJump
    {
        get
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            return !info.IsTag("Attack")&&!info.IsName("lending")&&!info.IsName("Break")&&!info.IsName("Hit");
        }
    }

    public bool canAttack
    {
        get
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            return !info.IsName("Break")&&!info.IsName("Hit");
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        myCharacterController = this.GetComponent<CharacterController>();
        playerMovementManager = new PlayerMovementManager(animator, this);
        this.player = this.GetComponent<Player>();
        ObservableStateMachineTrigger trigger = animator.GetBehaviour<ObservableStateMachineTrigger>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDir = Vector3.zero;
        float horizon = Input.GetAxis("Horizontal");
        float verti = Input.GetAxis("Vertical");

        moveDir += playerMovementManager.BasicMove(verti,horizon);

        moveDir += playerMovementManager.JumpMove(Input.GetKeyDown(KeyCode.I));

        playerMovementManager.AttackMove(Input.GetKeyDown(KeyCode.Q));

        myCharacterController.Move(moveDir);

        mat.SetVector("_ObjToLight", mainLight.transform.position - this.transform.position);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GameObject go = hit.gameObject;
        if(go.CompareTag("Plat"))
        {
            if(hit.moveDirection == new Vector3(0, -1, 0) && !onGround)
            {
                playerMovementManager.LendMove();
            }
            if (hit.moveDirection == new Vector3(0, 1, 0))
            {
                playerMovementManager.CeilingHit();
            }
        }
    }

    public Player GetPlayer()
    {
        return this.player;
    }

    public void Hit()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Jump"))
        {
            animator.SetTrigger("Hit");
        }
    }
}
