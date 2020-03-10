using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeammateAttack : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    private GameObject attack1;
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
        if (other.gameObject.tag == "Enemy")
        {
            attack1 = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        attack1 = null;
    }
}
