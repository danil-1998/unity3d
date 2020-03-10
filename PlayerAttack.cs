using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private GameObject attack1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            GameObject.Find("HookJoystick").GetComponent<HookJoystick>().AttackObj = attack1;
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
