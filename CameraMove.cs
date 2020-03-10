using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform player;
    // Start is called before the first frame update
    public bool isAutoTranCamera=false;
    public Transform CammTran;
    private ETCJoystick playerjoy;
    private ETCJoystick hookjoy;
    private void Awake()
    {
        playerjoy = GameObject.Find("PlayerJoystick").GetComponent<ETCJoystick>();
        hookjoy = GameObject.Find("HookJoystick").GetComponent<ETCJoystick>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.position.z < 35)
        {
            onTeam1();  
                    }
        if (player.position.z > 35)
        {
            onTeam2();
        }
        if (isAutoTranCamera)
        {
            onAutoTranCamera();

        }
    }
    void onTeam1()
    {
        CammTran.position = new Vector3(player.position.x, CammTran.position.y, player.position.z - 27f);
        CammTran.rotation = Quaternion.Euler(41.8f, 0f, 0f);
        playerjoy.tmAdditionnalRotation = 0;
        hookjoy.tmAdditionnalRotation = 0;
    }
    void onTeam2()
    {
        CammTran.position = new Vector3(player.position.x, CammTran.position.y, player.position.z + 27f);
        CammTran.rotation = Quaternion.Euler(41.8f, 180f, 0f);
        playerjoy.tmAdditionnalRotation = 180;
        hookjoy.tmAdditionnalRotation = 180;

    }
    void onAutoTranCamera()
    {
        if (CammTran.position.z > 35)
        {

        }
    }
}
