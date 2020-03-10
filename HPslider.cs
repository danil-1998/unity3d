using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPslider : MonoBehaviour
{
    private Slider hpsLider;
    private RectTransform hpTran;

    // Start is called before the first frame update
    public Transform player;
    public Vector2 offsetpos;
    public float value=1000;
    public float maxValue=1000;
    void Start()
    {
        hpsLider = GetComponent<Slider>();
        hpTran = GetComponent<RectTransform>();
        value = maxValue;
        hpsLider.value = value;
        offsetpos.y = 80;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            return;
        Vector3 playerpos = player.transform.position;
        Vector2 pos = RectTransformUtility.WorldToScreenPoint(Camera.main, playerpos);
        hpTran.position = pos + offsetpos;
    }
}
