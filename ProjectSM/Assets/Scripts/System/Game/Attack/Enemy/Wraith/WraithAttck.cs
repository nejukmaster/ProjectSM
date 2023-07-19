using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WraithAttack : Attack
{
    EnemyController controller => this.GetComponentInParent<EnemyController>();

    public void _Charging()
    {
        EffectController.globalEffectController.PlayEffect("Wraith_Charging", this.transform.position + new Vector3(0f,.2f,0f));
    }
    public void _BasicAttack()
    {
        EffectController.globalEffectController.StopEffect("Wraith_Charging");
        Player.Instance.GetComponent<PlayerController>().Hit();
        base.OnAttack(Player.Instance);
    }
}
