using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject player;
    private GameObject attack1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        player.GetComponent<BotMove>().AttackObj = attack1;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            attack1 = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        attack1 = null;
    }
}
