using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotMove : MonoBehaviour
{
    // Start is called before the first frame update
    private string MyTeam;
    private float randx;
    private float randz;
    private Vector3 randV;
    private Transform PlayerTran;
    private Rigidbody PlayerRigi;
    private float Movetime=1f;//每隔多少秒进行变换位置
    private float Movetimer = 1f;
    private float Attacktime = 2f;
    private float Attacktimer = 1f;
    private float Speed = 20f;//BOT移动速度
    private float HookSpeed = 40f;
    public float Hp = 1000;
    public float HookHurt = 400;
    public float AttackHurt = 100;
    public bool isShootReady = false;//是否可以射击
    private bool isMoveReady = true;
    private bool isTeam1 = false;//是否处在队伍一
    private Animator anim;
    public Rigidbody HookClone;//钩子克隆体
    public Rigidbody Hook;//钩子预制体
    public Transform FPonit;//钩子发射位置
    public Transform Hookback;//钩子回归目标位置
    public GameObject HookObj;//钩中的目标
    public GameObject AttackObj;//攻击目标
    private float HookBackTime = 2f;//钩子发出后多长时间开始返回
    private float HookBackTimer;
    private float HookAttackTime = 2f;//收回钩子后几秒继续发出钩子
    private float HookAttackTimer;
    float MinDistance = 15;//钩回敌人与自身距离小于这个值时，停止移动
    float MaxDistance = 200;//钩子最长距离
    public bool HookIsOut = false;//钩子是否发出
    public bool HookIsUp = false;//是否钩中敌人
    public bool HookIsBack = false;//钩子是否回收
    public bool isdie = false;
    public Slider slider;
    private Collider[] colCollection;           //发射中碰撞到的物体
    void Start()
    {
        PlayerTran = this.GetComponent<Transform>();
        PlayerRigi = this.GetComponent<Rigidbody>();
        randx = Random.Range(-205, -68);
        randz = Random.Range(48, 82);
        randV = new Vector3(randx, 2, randz);
        anim = this.GetComponent<Animator>();
        Invoke("OnShoot", 2f);
        HookBackTimer = HookBackTime;
        HookAttackTimer = HookAttackTime;
        slider = PlayerTran.Find("Canvas/Slider").GetComponent<Slider>();
        MyTeam = PlayerTran.tag;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerTran.position.z > 35)
        {
            isTeam1 = false;
        }
        else
        {
            isTeam1 = true;
        }
        if (PlayerTran.tag == "DropOver")
        {
            PlayerTran.tag = MyTeam;
        }

        if (slider.value == 0)
        {
            if (isdie == false)
            {
                OnDie();
                isdie = true;
            }
        }
        else
        {
            if (PlayerTran.tag=="OnDrop")
            {
                OnDrag();
            }
            else if (isShootReady)
            {
                HookAttackTimer -= Time.deltaTime;
                if (HookAttackTimer < 0)
                {
                    HookAttackTimer = HookAttackTime;
                    OnShoot();
                }
               
            }
            else if (isMoveReady)
            {
                MoveRand();
            }
            if (AttackObj)
            {
                AttackRand();
            }
        }
        if (HookIsOut)
        {
            CheckCollider();
            if (HookIsUp)
            {
                HookIsBack = true;
                HookIsOut = false;
            }
            HookBackTimer -= Time.deltaTime;
            if (HookBackTimer < 0)
            {
                HookIsBack = true;
                HookIsOut = false;
                HookBackTimer = HookBackTime;
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
                CheckCollider();
            }
            OnHookBack();
        }


    }
    void CheckCollider()
    {
        //对钩子进行球形检测，返回所有碰到或者在球范围内的碰撞体数组     
        //注意将人称控制器及其子物体的Layer修改为不是Default的一个层，否则钩子会检测到自身的碰撞体
        if (HookClone)
        {
            colCollection = Physics.OverlapSphere(HookClone.position, 2f, 1 << LayerMask.NameToLayer("Player"));
        }
        if (colCollection.Length > 0)
        {
            foreach (Collider item in colCollection)
            {
                //将敌人的tag设置为“Enemy”
                if (item.gameObject.tag == PlayerTran.gameObject.tag)
                {
                    if (item.gameObject.name == PlayerTran.name)
                    {
                        Destroy(HookClone.gameObject);
                        HookIsOut = false;
                        Invoke("OnShoot", 2f);
                    }
                    else
                    {
                        item.transform.SetParent(HookClone.transform);
                        item.tag = "OnDrop";
                        item.gameObject.layer = 8;
                        HookObj = item.gameObject;
                        HookIsUp = true;
                    }
                }
                else if (item.gameObject.tag.Equals("Player"))
                {
                    item.transform.Find("Canvas/Slider").GetComponent<Slider>().value -= HookHurt;
                    item.transform.SetParent(HookClone.transform);
                    item.tag = "OnDrop";
                    item.gameObject.layer = 8;
                    HookObj = item.gameObject;
                    HookIsUp = true;
                
                }
            }

        }
    }
    void OnHookBack()
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
                HookObj.transform.position = PlayerTran.position;
            }
            //diren.rotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            HookIsOut = false;
            HookIsBack = false;
            Destroy(HookClone.gameObject);
        }
        isShootReady = true;
        HookAttackTimer = HookAttackTime;
    }
    void OnDie()
    {
        if (HookIsUp)
        {
            HookObj.transform.SetParent(null);
            HookObj.layer = 10;
            HookObj.tag = "DropOver";
            Destroy(HookClone.gameObject);
        }
        else if(HookIsOut)
        {
            Destroy(HookClone.gameObject);
            HookIsOut = false;
        }
        anim.SetBool("die", true);
        Invoke("DestroyPlayer", 1.5f);
        //PlayerTran.GetComponent<BotMove>().enabled = false;
    }
    void DestroyPlayer()
    {
        PlayerTran.gameObject.SetActive(false);
        Invoke("OnResurrection", 5);
    }
    void OnResurrection()
    {
        if (PlayerTran.tag == ("Enemy"))
        {
            PlayerTran.position = new Vector3(-140, 2, 66);
        }
        else
        {
            PlayerTran.position = new Vector3(-140, 2, 7);
        }
        slider.value = 1000;
        PlayerRigi.velocity = Vector3.zero;
        PlayerTran.gameObject.SetActive(true);
        isdie = false;
        isShootReady = true;
        HookAttackTimer = HookAttackTime;
    }
    void OnDrag()
    {
        anim.SetBool("run", false);
        PlayerRigi.velocity = Vector3.zero;
    }
    void OnShoot()
    {
        isMoveReady = false;
        if (!(AttackObj == null))
        {
            PlayerTran.LookAt(AttackObj.transform);
        }
        else
        {
            if (isTeam1)
            {
                RandomNum1();
            }
            else
            {
                RandomNum2();
            }
        }
        PlayerRigi.velocity = Vector3.zero;
        anim.SetBool("shoot", true);
        HookClone = (Rigidbody)Instantiate(Hook, FPonit.position, FPonit.rotation);
        HookClone.velocity = PlayerTran.TransformDirection(Vector3.forward * HookSpeed);
        HookIsOut = true;
        HookClone.name = PlayerTran.name+"Hook";
        isShootReady = false;
        Invoke("OnShootEnd", 0.5f);
    }
    void OnShootEnd()
    {
        isMoveReady =true;
        anim.SetBool("shoot", false);
    }
    void OnMove()
    {
        PlayerRigi.velocity = PlayerRigi.transform.TransformDirection(Vector3.forward * Speed);
        anim.SetBool("run", true);
    }
    void OnAttack()
    {
        isMoveReady = false;
        PlayerRigi.velocity = Vector3.zero;
        AttackObj.transform.Find("Canvas/Slider").GetComponent<Slider>().value -= AttackHurt;
        PlayerTran.LookAt(AttackObj.transform);
        anim.SetBool("shoot", true);
        Invoke("OnShootEnd", 0.5f);
    }
    void AttackRand()
    {
        if (Attacktimer == 0)
        {
            Attacktimer = Attacktime;
            OnAttack();
        }
        if (Attacktimer > 0)
        {
            Attacktimer -= Time.deltaTime;
        }
        if (Attacktimer < 0)
        {
            Attacktimer = 0;
        }
    }
    void MoveRand()
    {
        if (Movetimer == 0)
        {
            Movetimer = Movetime;
            if (isTeam1)
            {
                RandomNum2();
                OnMove();
            }
            else
            {
                RandomNum1();
                OnMove();
            }
        }
        if (Movetimer > 0)
        {
            Movetimer -= Time.deltaTime;
        }
        if (Movetimer < 0)
        {
            Movetimer = 0;
        }
    }
    void RandomNum1()
    {
        randx = Random.Range(-205, -68);
        randz = Random.Range(48, 82);
        randV = new Vector3(randx, 2, randz);
        PlayerTran.LookAt(randV);

    }
    void RandomNum2()
    {
        randx = Random.Range(-205, -68);
        randz = Random.Range(-6, 25);
        randV = new Vector3(randx, 2, randz);
        PlayerTran.LookAt(randV);

    }
}
