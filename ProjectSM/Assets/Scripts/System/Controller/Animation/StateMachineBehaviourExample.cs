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
        // MonoBehaviour.OnAnimatorIK ���Ŀ� ����
    }

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        // ��ũ��Ʈ�� ������ ���� ���� ��ȯ�� ������ ����
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        // ��ũ��Ʈ�� ������ ���� ��迡�� �������ö� ����
    }
}
