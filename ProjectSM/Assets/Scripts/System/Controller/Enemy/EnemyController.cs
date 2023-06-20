using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEditor.Rendering;
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
    [Tooltip("Enemy의 자유배회 범위")]
    public float loiteringRange;
}
public class EnemyController : MonoBehaviour
{
    [SerializeField] EnemyAttribute attr;
    Coroutine currentCo;
    Color matColor;
    Enemy enemy;
    float detectRange => enemy.statusDic.GetDic()["sta_ran"].value * 3f;
    Vector3 destination;
    CharacterController characterController;

    bool canMove = true;
    // Start is called before the first frame update
    void Start()
    {
        matColor = attr.mat.GetColor("_Color");
        this.enemy = GetComponent<Enemy>();
        characterController = GetComponent<CharacterController>();

        ObservableStateMachineTrigger trigger = attr.animator.GetBehaviour<ObservableStateMachineTrigger>();
        IDisposable exitState = trigger.OnStateExitAsObservable().Subscribe(onStateInfo =>
        {
            AnimatorStateInfo info = onStateInfo.StateInfo;
            if (info.IsName("Base Layer.Beatdown"))
            {
                enemy.Despawn();
                Stage.Instance.CharacterDeathHook(enemy);
            }
            if (info.IsName("Base Layer.Attack"))
            {
                canMove = true;
            }
            if (info.IsName("Base Layer.Walking"))
            {
                canMove= false;
            }
        }).AddTo(this);
        IDisposable enterState = trigger.OnStateEnterAsObservable().Subscribe(onStateInfo =>
        {
            AnimatorStateInfo info = onStateInfo.StateInfo;
            if(info.IsName("Base Layer.Attack"))
            {

            }
            if(info.IsName("Base Layer.Hit") || info.IsName("Base Layer.Hit0"))
            {
                canMove = false;
            }
            if(info.IsName("Base Layer.Walking"))
            {
                canMove = true;
            }
            if(info.IsName("Base Layer.BeatDown"))
            {
                canMove = false;
            }
        }).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.OnReady && GameManager.instance.onGame)
        {
            GameObject player = GameObject.FindWithTag("Player");
            Vector3 moveDir = Vector3.zero;

            moveDir.y -= attr.gravity * Time.deltaTime;

            //attr.mat.SetVector("_ObjToLight", attr.mainLight.transform.position - this.transform.position);

            float disTplayer = player.transform.position.z - this.transform.position.z;

            if (Math.Abs(destination.z - this.transform.position.z) > 0.1f && canMove)
            {
                moveDir += new Vector3(0, 0, this.transform.forward.z * enemy.statusDic.GetDic()["sta_move"].value * Time.deltaTime);
            }

            //자유 배회
            if (Mathf.Abs(disTplayer) > detectRange)
            {
                attr.animator.SetBool("Walking", true);
                if (Math.Abs(destination.z - this.transform.position.z) <= 0.1f)
                {
                    Vector3 dest = transform.position - new Vector3(0f, 0f, attr.loiteringRange * transform.forward.z);
                    this.transform.rotation = Quaternion.Euler(0, 90f * this.transform.forward.z + 90, 0);
                    destination = dest;
                }
            }
            //감지 범위 내 진입시 추적
            if (Math.Abs(disTplayer) <= detectRange && Math.Abs(disTplayer) > enemy.statusDic.GetDic()["sta_ran"].value)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), new Vector3(0, 0, transform.forward.z), out hit, 50f) && hit.collider.gameObject.CompareTag("Player"))
                {
                    if (disTplayer * this.transform.forward.z < 0)
                    {
                        this.transform.rotation = Quaternion.Euler(0, 90f * this.transform.forward.z + 90, 0);
                    }
                    destination = player.transform.position;
                    attr.animator.SetBool("Walking", true);
                }
                else
                {
                    attr.animator.SetBool("Walking", true);
                    if (Math.Abs(destination.z - this.transform.position.z) <= 0.1f)
                    {
                        Vector3 dest = transform.position - new Vector3(0f, 0f, attr.loiteringRange * transform.forward.z);
                        this.transform.rotation = Quaternion.Euler(0, 90f * this.transform.forward.z + 90, 0);
                        destination = dest;
                    }
                }
            }
            //사거리내 진입시 공격
            else if (Mathf.Abs(disTplayer) <= enemy.statusDic.GetDic()["sta_ran"].value && canMove)
            {
                attr.animator.SetBool("Walking", false);
                if (!attr.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    attr.animator.SetTrigger("Attack");
                    if (disTplayer * this.transform.forward.z < 0)
                    {
                        this.transform.rotation = Quaternion.Euler(0, 90f * this.transform.forward.z + 90, 0);
                    }
                }
            }

            characterController.Move(moveDir);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GameObject go = hit.gameObject;
        if (go.CompareTag("Obstacle"))
        {
            Vector3 dest = transform.position - new Vector3(0f, 0f, attr.loiteringRange * transform.forward.z);
            this.transform.rotation = Quaternion.Euler(0, 90f * this.transform.forward.z + 90, 0);
            destination = dest;
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
