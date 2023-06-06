using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    PlayerController controller => this.GetComponentInParent<PlayerController>();
    public void _BasicAttack()
    {
        Dictionary<string, Status> _dict = controller.GetPlayer().statusDic.GetDic();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            int _range = _dict["sta_ran"].value;
            int _hitrate = _dict["sta_hit"].value;
            Vector3 pTe = enemies[i].transform.position - controller.transform.position;
            if (Vector3.Dot(controller.transform.forward, pTe) > 0 && pTe.magnitude < _range * _range)
            {
                if (UnityEngine.Random.Range(0, 100) < _hitrate)
                {
                    if (UnityEngine.Random.Range(0, 100) >= _dict["sta_criper"].value)
                    {
                        enemies[i].GetComponent<EnemyController>().Hit(_dict["sta_att"].value);
                    }
                    else
                    {
                        enemies[i].GetComponent<EnemyController>().Hit(_dict["sta_att"].value + (int)(_dict["sta_att"].value * (float)(_dict["sta_cridam"].value) / 100f));
                    }
                }
            }
        }
    }
}
