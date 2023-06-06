using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachineBehaviourExample : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // MonoBehaviour.OnAnimatorIK 직후에 실행
    }

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        // 스크립트가 부착된 상태 기계로 전환이 왔을때 실행
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        // 스크립트가 부착된 상태 기계에서 빠져나올때 실행
    }
}
