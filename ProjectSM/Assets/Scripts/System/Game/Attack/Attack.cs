using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] Character attacker;

    public void OnAttack(Character victim)
    {
        Stage.Instance.CharacterHitOtherHook(attacker, victim);
    }
}
