using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJoystick : MonoBehaviour
{
    // Start is called before the first frame update
    public ETCJoystick joystick;
    public Animator animThis;
    Rigidbody player;
    void Start()
    {
        player = GameObject.Find("PBRCharacter").GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = joystick.axisX.axisValue;
        float v = joystick.axisY.axisValue;
        if (Mathf.Abs(h) > 0.05f || (Mathf.Abs(v) > 0.05f)) //控制人物播放奔跑动画
        {
            animThis.SetBool("run", true);
        }
        else
        {
            try
            {
                animThis.SetBool("run", false);

            }
            catch { }
        }
    }
    private void FixedUpdate()
    {
        //player.velocity = Vector3.zero;
    }
}
