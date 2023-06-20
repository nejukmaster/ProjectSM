using System;
using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Windows;
using UniRx;
using Input = UnityEngine.Input;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public class PressedKey
    {
        public KeyCode key;
        public float time;
        public PressedKey(KeyCode key)
        {
            this.key = key;
            this.time = 2.0f;
        }
    }

    [Tooltip("점프 고도 그래프의 y절편")]
    public float jump_y_interception = 20.0f;
    [Tooltip("점프 고도 그래프의 x절편")]
    public float jump_x_interception = 2.0f;
    [Tooltip("중력의 세기")]
    public float gravity = 16.0f;
    [Tooltip("연속점프 가능 횟수")]
    public int jumpLimits = 2;
    [Tooltip("플레이어 이펙트 매니저")]
    public EffectController playerEffects;
    [Tooltip("글로벌 이펙트 매니저")]
    public EffectController globalEffects;
    [Tooltip("플레이어 애니메이터")]
    public Animator animator;
    [Tooltip("플레이어 머티리얼")]
    [SerializeField] Material mat;
    [Tooltip("플레이어가 피격시 빛날 색깔")]
    public Color hitColor;
    [Tooltip("플레이어가 피격시 빛날 시간")]
    public float hitBlinkSec;

    [Tooltip("플레이어가 땅에 붙어 있는지를 표시합니다.")]
    public bool onGround;
    
    CharacterController myCharacterController;
    public PlayerMovementManager playerMovementManager;
    Player player;
    Color matColor;
    Coroutine currentCo;
    public List<PressedKey> pressedKeys = new List<PressedKey>();

    GameObject mainLight;



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
        matColor = mat.GetColor("_Color");
        mainLight = RenderSettings.sun.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.OnReady && GameManager.instance.onGame)
        {
            Vector3 moveDir = Vector3.zero;
            float horizon = Input.GetAxis("Horizontal");
            float verti = Input.GetAxis("Vertical");

            moveDir += playerMovementManager.BasicMove(verti, horizon);

            moveDir += playerMovementManager.JumpMove(Input.GetKeyDown(KeyCode.I));

            if (Input.anyKeyDown)
            {
                playerMovementManager.AttackMove(Input.GetKeyDown(KeyCode.Q));

                if (CheckKeyDown() != KeyCode.None)
                    pressedKeys.Add(new PressedKey(CheckKeyDown()));
                for (int i = 0; i < Commands.commands.Count; i++)
                {
                    if (CheckCommand(Commands.commands.ElementAt(i).Key))
                    {
                        moveDir += Commands.commands.ElementAt(i).Value();
                    }
                }
            }

            for (int i = 0; i < pressedKeys.Count; i++)
            {
                pressedKeys[i].time -= Time.deltaTime;
                if (pressedKeys[i].time <= 0)
                {
                    pressedKeys.RemoveAt(i);
                    i--;
                }
            }

            myCharacterController.Move(moveDir);



            mat.SetVector("_ObjToLight", mainLight.transform.position - this.transform.position);
        }
    }

    private KeyCode CheckKeyDown()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
            return KeyCode.RightArrow;
        else if(Input.GetKeyDown(KeyCode.DownArrow))
            return KeyCode.DownArrow;
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
            return KeyCode.LeftArrow;
        else if(Input.GetKeyDown(KeyCode.UpArrow))
            return KeyCode.UpArrow;
        else if(Input.GetKeyDown(KeyCode.I))
            return KeyCode.I;
        else if (Input.GetKeyDown(KeyCode.Q))
            return KeyCode.Q;
        else
            return KeyCode.None;
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
        if (currentCo != null)
        {
            StopCoroutine(currentCo);
        }
        currentCo = StartCoroutine(HitCo());
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Jump"))
        {
            animator.SetTrigger("Hit");
            StartCoroutine(HitCo());
        }
    }

    public bool CheckCommand(KeyCode[] cmds)
    {
        bool r = false;
        for(int i = 0; i < pressedKeys.Count; i++)
        {
            if(pressedKeys.Count - i >= cmds.Length)
            {
                bool _r = true;
                for(int j = 0; j < cmds.Length; j++)
                {
                    if (pressedKeys[i+j].key != cmds[j])
                    {
                        _r = false;
                        break;
                    }
                }
                if (_r)
                {
                    r = true;
                    break;
                }
            }
        }

        return r;
    }

    IEnumerator HitCo()
    {
        Color _color = matColor;
        mat.SetColor("_Color", hitColor);
        yield return new WaitForSeconds(hitBlinkSec);
        mat.SetColor("_Color", matColor);
    }
}
