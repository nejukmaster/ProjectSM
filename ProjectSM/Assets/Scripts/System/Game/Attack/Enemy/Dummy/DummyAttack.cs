using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyAttack : Attack
{
    EnemyController controller => this.GetComponentInParent<EnemyController>();
    public void _BasicAttack()
    {
        Player.Instance.GetComponent<PlayerController>().Hit();
        base.OnAttack(Player.Instance);
    }
}
