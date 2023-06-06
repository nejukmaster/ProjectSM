using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyAttack : MonoBehaviour
{
    [SerializeField] Player player;
    public void _BasicAttack()
    {
        player.GetComponent<PlayerController>().Hit();
    }
}
