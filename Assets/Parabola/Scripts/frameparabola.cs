using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frameparabola : MonoBehaviour
{
    [Header("目标点")]
    public Transform target;
    [Header("朝向目标点向量")]
    private Vector2 direct;
    [Header("横向路程")]
    private float xdistance;
    [Header("z向路程")]
    private float zdistance;
    [Header("横向移动速度,不确定因素")]
    private float xspeed;
    [Header("z向移动速度,不确定因素")]
    private float zspeed;
    [Header("纵向移动速度,不确定因素")]
    private float yspeed;
    [Header("运动到目标点总时间,确定因素")]
    [SerializeField] float alltime;
    [Header("子弹抛物线目前运动时间,确定因素")]
    private float time;
    [Header("物体实时在x轴方向")]
    private float xposition;
    [Header("物体实时在y轴方向")]
    private float yposition;
    [Header("物体实时在y轴方向")]
    private float zposition;


    [Header("重力加速度")]
    [SerializeField] float g;
    private Vector3 sourceposition;
    private Rigidbody2D rigi;
    void Start()
    {
        time = 0;
        alltime = 2;
        g = 9.8f;
        //记录物体开始时候的原始位置
        sourceposition = transform.position;     
    }

    
    void Update()
    {
        //-------------------------这段代码放在Update中可以让物体以抛物线实时跟踪砸到一个物体,如果将横轴纵轴距离固定在一个函数方法中求出,然后在下面求横纵速度和位置,就可以达到固定打到一个点的功能.
        //因为物体是可变的,所以距离也一直在变
        xdistance = target.position.x - sourceposition.x;

        zdistance = target.position.z - sourceposition.z;
        //获取横轴速度
        xspeed = xdistance / alltime;

        zspeed = zdistance / alltime;
        //获取纵轴速度,Vy=gt,这个t就是总时间的一半,
        //yspeed = g * alltime / 2f;

        //对上面公式进行强化,可以解决不同高度差时候的出现的不能通过目标点问题，公式是Vy=(H-0.5f*g*t^2)/t
        yspeed = ((target.position.y - sourceposition.y) + 0.5f * g * alltime * alltime) / alltime;
        //获取横轴位置
        xposition = xspeed * time;
        //获取z方向距离
        zposition = zspeed * time;
        //获取纵轴位置
        yposition = yspeed * time - 0.5f * g * time * time;

        //设置物体的位置，
        transform.position = sourceposition + new Vector3(xposition, yposition, zposition);
        //时间一直正常运行
        time += Time.deltaTime;
    }
}
