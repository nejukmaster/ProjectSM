using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[System.Serializable]
public struct EnemyAttribute
{
    [Tooltip("Enemy의 애니메이터")]
    public Animator animator;
    [Tooltip("Enemy가 받는 중력의 크기")]
    public float gravity;
    [Tooltip("Enemy의 머티리얼")]
    public Material mat;
    [Tooltip("Enemy가 피격시 빛날 색깔")]
    public Color hitColor;
    [Tooltip("Enemy가 피격시 빛날 시간")]
    public float hitBlinkSec;
    [Tooltip("Enemy를 쉐이딩할 주 광원")]
    public GameObject mainLight;
    [Tooltip("Enemy의 적 감지 범위: x,y")]
    public Vector2 dectectingRange;
}
public class EnemyController : MonoBehaviour
{

    [SerializeField] EnemyAttribute attr;
    Coroutine currentCo;
    Color matColor;
    Enemy enemy;
    NavMeshAgent navAgent;

    bool canMove = true;
    // Start is called before the first frame update
    void Start()
    {
        matColor = attr.mat.GetColor("_Color");
        this.enemy = GetComponent<Enemy>();
        navAgent = GetComponent<NavMeshAgent>();

        ObservableStateMachineTrigger trigger = attr.animator.GetBehaviour<ObservableStateMachineTrigger>();
        IDisposable exitState = trigger.OnStateExitAsObservable().Subscribe(onStateInfo =>
        {
            AnimatorStateInfo info = onStateInfo.StateInfo;
            if (info.IsName("Base Layer.Beatdown"))
            {
                enemy.Despawn();
            }
            if (info.IsName("Base Layer.Attack"))
            {
                canMove = true;
            }
        }).AddTo(this);
        IDisposable enterState = trigger.OnStateEnterAsObservable().Subscribe(onStateInfo =>
        {
            AnimatorStateInfo info = onStateInfo.StateInfo;
            if(info.IsName("Base Layer.Attack"))
            {

            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Vector3 moveDir = Vector3.zero;

        moveDir.y -= attr.gravity * Time.deltaTime;

        attr.mat.SetVector("_ObjToLight", attr.mainLight.transform.position - this.transform.position);

        float disTplayer = player.transform.position.z - this.transform.position.z;
        if (Mathf.Abs(disTplayer) < attr.dectectingRange.x && Mathf.Abs(disTplayer) > enemy.statusDic.GetDic()["sta_ran"].value)
        {
            if (canMove)
            {
                if(disTplayer * this.transform.forward.z < 0)
                {
                    navAgent.enabled = false;
                    this.transform.rotation = Quaternion.Euler(0, 90f * this.transform.forward.z + 90, 0);
                    navAgent.enabled = true;
                }
                navAgent.SetDestination(player.transform.position);
                navAgent.speed = enemy.statusDic.GetDic()["sta_move"].value;
                navAgent.stoppingDistance = enemy.statusDic.GetDic()["sta_ran"].value;
                attr.animator.SetBool("Walking", true);
            }
        }
        else if(Mathf.Abs(disTplayer) <= enemy.statusDic.GetDic()["sta_ran"].value)
        {
            attr.animator.SetBool("Walking", false);
            if (!attr.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                canMove = false;
                attr.animator.SetTrigger("Attack");
                if (disTplayer * this.transform.forward.z < 0)
                {
                    navAgent.enabled = false;
                    this.transform.rotation = Quaternion.Euler(0,90f * this.transform.forward.z + 90,0);
                    navAgent.enabled = true;
                }
                navAgent.SetDestination(this.transform.position);
                navAgent.velocity = Vector3.zero;
                navAgent.speed = 0;
            }
        }
    }
    
    public void Hit(int dmg)
    {
        if (currentCo != null)
        {
            StopCoroutine(currentCo);
        }
        currentCo = StartCoroutine(HitCo());
        if (attr.animator.GetCurrentAnimatorStateInfo(0).IsName("Hit")|| attr.animator.GetCurrentAnimatorStateInfo(0).IsName("Hit0"))
        {
            navAgent.velocity = Vector3.zero;
            navAgent.speed = 0;
            attr.animator.SetTrigger("Hit");

        }
        else attr.animator.SetTrigger("Hit");
        enemy.statusDic.GetDic()["sta_hp"].value -= dmg;
        if (enemy.statusDic.GetDic()["sta_hp"].value <= 0)
        {
            attr.animator.SetTrigger("BeatDown");
        }
    }

    IEnumerator HitCo()
    {
        Color _color = matColor;
        attr.mat.SetColor("_Color",attr.hitColor);
        yield return new WaitForSeconds(attr.hitBlinkSec);
        attr.mat.SetColor("_Color", matColor);
    }
}
