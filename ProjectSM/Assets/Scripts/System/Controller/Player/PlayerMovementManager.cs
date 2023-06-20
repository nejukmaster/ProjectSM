using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementManager
{
    PlayerController controller;
    Transform transform => controller.transform;

    IEnumerator jumpEnumerator;
    int jumpCount = 0;
    Coroutine currentCo;
    EffectController globalEffects;

    Animator animator;
    public PlayerMovementManager(Animator animator, PlayerController controller){
        this.animator = animator;
        this.controller = controller;
        globalEffects = EffectController.globalEffectController;
    }

    public Vector3 JumpMove(bool p_keyDown)
    {
        Vector3 r = Vector3.zero;
        if(p_keyDown && jumpCount < controller.jumpLimits)
        {
            if (controller.canJump)
            {
                jumpEnumerator = JumpCo();
                controller.onGround = false;
                jumpCount++;

                animator.SetTrigger("Jump");
                controller.playerEffects.StopEffect("lending");

                globalEffects.PlayEffect("JumpEffect",new Vector3(controller.transform.position.x,controller.transform.position.y,controller.transform.position.z));
            }
        }
        if(jumpEnumerator != null)
        {
            if (jumpEnumerator.MoveNext())
            {
                r.y += (float)jumpEnumerator.Current * Time.deltaTime;
            }
            else
            {
                jumpEnumerator = null;
            }
        }
        return r;
    }

    public void AttackMove(bool p_keyDown)
    {
        if (p_keyDown && controller.canAttack)
        {
            Dictionary<string, Status> _dict = controller.GetPlayer().statusDic.GetDic();
            int _asp = _dict["sta_asp"].value;
            animator.SetFloat("AttackSpeed", (float)_asp/100);
            animator.SetTrigger("Attack");
        }
    }
    
    public Vector3 BasicMove(float verti, float horizon){
        Vector3 r = Vector3.zero;
        r.y += -1 * controller.gravity * Time.deltaTime;

        if(controller.canMove){
         
            r.z += horizon * controller.moveSpeed * Time.deltaTime;

            if (Mathf.Abs(horizon) > 0)
            {
                if (horizon > 0)
                    this.transform.rotation = Quaternion.Euler(0, 0, 0);
                else if (horizon < 0)
                    this.transform.rotation = Quaternion.Euler(0, 180f, 0);
                animator.SetBool("Running", true);
            }
            else
            {
                animator.SetBool("Running", false);
            }
        }
        return r;
    }

    public void LendMove()
    {
        controller.onGround = true;
        jumpCount = 0;

        animator.SetTrigger("Lend");
        controller.playerEffects.PlayEffect("lending");
    }

    public void CeilingHit()
    {
        jumpEnumerator = null;
    }
    IEnumerator JumpCo()
    {
        float s_y = transform.position.y;
        float ypos = transform.position.y;
        float t = 0;
        while (JumpGraph(controller.jump_y_interception, controller.jump_x_interception, t) >= 0)
        {
            t += Time.deltaTime;
            ypos += JumpGraph(controller.jump_y_interception, controller.jump_x_interception, t) * Time.deltaTime;
            yield return JumpGraph(controller.jump_y_interception, controller.jump_x_interception, t);
        }
    }

    float JumpGraph(float height, float speed, float progress)
    {
        return -height * (progress / speed) + height;
    }
}
