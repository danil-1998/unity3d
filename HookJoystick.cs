using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HookJoystick : MonoBehaviour
{
    // Start is called before the first frame update
    private ETCJoystick hookjoystick;
    private ETCJoystick playerjoystick;
    private ETCButton attackbutton;
    public Rigidbody HookClone;//钩子克隆体
    public Rigidbody Hook;//钩子预制体
    public Transform FPonit;//钩子发射位置
    public Transform player;//玩家
    private Animator anim;//玩家动画
    public Transform hookplan;//技能指示器
    public Transform Hookback;//钩子回归目标位置
    public GameObject HookObj;//钩中的对象
    public GameObject AttackObj;
    private float Speed = 40;//钩子速度
    private float Hooktime = 2f;//钩子最大飞行时间
    private float Hooktimer;
    public float Hp = 1000;
    public float HookHurt = 400;//角色的钩子伤害
    public float AttackHurt = 100;//角色射击伤害
    public float AttackTime = 0.8f;//角色的射击间隔
    private float MinDistance = 15;//钩回敌人与自身距离小于这个值时，停止移动
    public bool HookIsOut = false;//钩子是否发出
    public bool HookIsUp = false;//是否钩中敌人
    public bool HookIsBack = false;//钩子是否回收
    public bool isDie = false;
    public bool isShootReady = true;
    public bool isdrop = false;
    public Slider slider;
    private Collider[] HookCollection;           //发射中碰撞到的物体
    private void Awake()
    {
        anim = player.GetComponent<Animator>();
        hookjoystick = GetComponent<ETCJoystick>();
        playerjoystick = GameObject.Find("PlayerJoystick").GetComponent<ETCJoystick>();
        attackbutton = GameObject.Find("AttackButton").GetComponent<ETCButton>();
        
    }
    void Start()
    {
        UnityEngine.Events.UnityAction listener1 = OnMoveStart;
        UnityEngine.Events.UnityAction listener2 = OnMoveEnd;
        UnityEngine.Events.UnityAction listener3 = OnAttackStart;
        hookjoystick.onMoveStart.AddListener(listener1);
        hookjoystick.onMoveEnd.AddListener(listener2);
        attackbutton.onUp.AddListener(listener3);
        Hooktimer = Hooktime;
        slider = player.Find("Canvas/Slider").GetComponent<Slider>();
        
    }
    void Update()
    {
        //Debug.Log(HookClone.gameObject.name); 
        if (slider.value == 0)
        {
            if (isDie == false)
            {
                OnDie();
            }
        }
        else
        {
            if (player.tag=="OnDrop")
            {
                isdrop = true;
                if (isdrop)
                {
                    OnDrop();
                    isdrop =false;
                }
            }
            if (player.tag == "DropOver")
            {
                player.tag = "Player";
                DropOver();
            }
        }
        if (HookIsOut)
        {
            HookCollider();
            if (HookIsUp)
            {
                HookIsBack = true;
                HookIsOut = false;
            }
            Hooktimer -= Time.deltaTime;
            if (Hooktimer < 0)
            {
                HookIsBack = true;
                HookIsOut = false;
                Hooktimer = Hooktime;
            }

            /*if (Vector3.Distance(HookClone.position, Hookback.transform.position) > MaxDistance)
            {
                HookIsBack = true;
            }*/
        }
        if (HookIsBack)
        {
            if (HookIsUp == false)
            {
                HookCollider();
            }
            OnHookBack();
        }
        
    }
    void OnDie()
    {
        anim.SetBool("die", true);
        playerjoystick.activated = false;
        hookjoystick.activated = false;
        attackbutton.activated = false;
        isDie = true;
        Invoke("OnDestroyPlayer", 1f);
    }
    void OnDestroyPlayer()
    {
        player.gameObject.SetActive(false);
        Invoke("OnResurrection", 5);
    }
    void OnResurrection()
    {
        slider.value = 1000;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.position = new Vector3(-136, 2, 7);
        player.gameObject.SetActive(true);
        playerjoystick.activated = true;
        hookjoystick.activated = true;
        attackbutton.activated = true;
        isDie = false;
    }
    void OnAttackStart()
    {
        if (AttackObj && isDie == false && isShootReady == true)
        {
            anim.SetBool("shoot", true);
            AttackObj.transform.Find("Canvas/Slider").GetComponent<Slider>().value -= AttackHurt;
            playerjoystick.isTurnAndMove = false;
            player.LookAt(AttackObj.transform);
            isShootReady = false;
            Invoke("AttackReady", AttackTime);
            Invoke("OnShootEnd", 0.7f);
        }
    }
    void AttackReady()
    {
        isShootReady = true;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Debug.Log(other.gameObject.name);
            AttackObj = other.gameObject;
        }
    }

    void HookCollider()
    {
        //对钩子进行球形检测，返回所有碰到或者在球范围内的碰撞体数组     
        //注意将人称控制器及其子物体的Layer修改为不是Default的一个层，否则钩子会检测到自身的碰撞体
        if (HookClone)
        {
            HookCollection = Physics.OverlapSphere(HookClone.position, 1.5f, 1 << LayerMask.NameToLayer("Player"));
        }
        if (HookCollection.Length > 0)
        {
            foreach (Collider item in HookCollection)
            {
                //将敌人的tag设置为“Enemy”

                if (item.gameObject.tag == player.gameObject.tag)
                {
                    if (item.gameObject.name == player.name)
                    {
                        Destroy(HookClone.gameObject);
                        hookjoystick.activated = true;
                        HookIsOut = false;
                    }
                    else
                    {
                        item.transform.SetParent(HookClone.transform);
                        item.gameObject.layer = 8;
                        item.tag = "OnDrop";
                        HookObj = item.gameObject;
                        HookIsUp = true;
                    }
                }
                else if (item.gameObject.tag.Equals("Enemy"))
                {
                    item.transform.SetParent(HookClone.transform);
                    item.gameObject.layer = 8;
                    item.tag = "OnDrop";
                    item.gameObject.GetComponent<BotMove>().slider.value -= HookHurt;
                    HookObj = item.gameObject;
                    HookIsUp = true;
                }
            }

        }
    }
    void OnMoveStart()//钩子摇杆开始移动时
    {
        hookplan.gameObject.SetActive(true);
    }
    void OnMoveEnd()//当松开钩子摇杆
    {
        if (isDie==false)
        {
            anim.SetBool("shoot", true);
            playerjoystick.isTurnAndMove = false;
            player.rotation = hookplan.rotation;
            hookplan.rotation = player.rotation;
            Invoke("OnShootEnd", 0.5f);

            HookClone = (Rigidbody)Instantiate(Hook, FPonit.position, FPonit.rotation);
            HookClone.velocity = player.transform.TransformDirection(Vector3.forward * Speed);
            HookClone.name = player.name;
            HookIsOut = true;
            hookjoystick.activated = false;
        }
        hookplan.gameObject.SetActive(false);
    }
    void OnHookBack()//当钩子返回时
    {

        HookClone.velocity = transform.TransformDirection(Vector3.forward * 0);
        HookClone.transform.LookAt(Hookback.transform.position);
        HookClone.transform.Translate(Vector3.forward);
        if (Vector3.Distance(HookClone.position, Hookback.transform.position) < MinDistance)
        {
            OnHookDestroy();
        }
    }
    void OnHookDestroy()
    {
        if (HookIsUp)
        {
            HookIsOut = false;
            HookIsBack = false;
            HookIsUp = false;
            HookObj.transform.SetParent(null);
            HookObj.tag = "DropOver";
            HookObj.layer = 10;
            Destroy(HookClone.gameObject);

            if (30 < HookObj.transform.position.z && HookObj.transform.position.z < 40)
            {
                HookObj.transform.position = player.position;
            }
            //diren.rotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            HookIsOut = false;
            HookIsBack = false;
            Destroy(HookClone.gameObject);
        }
        Hooktimer = Hooktime;
        hookjoystick.activated = true;
    }
    void OnShootEnd()
    {
        anim.SetBool("shoot", false);
        playerjoystick.isTurnAndMove = true;
    }
    // Update is called once per frame
    void OnDrop()
    {
        playerjoystick.activated = false;
        hookjoystick.activated = false;
        attackbutton.activated = false;
    }
    void DropOver()
    {
        playerjoystick.activated = true;
        attackbutton.activated = true;
        if (!HookIsOut && !HookIsBack)
        {
            hookjoystick.activated = true;
        }
    }
}
