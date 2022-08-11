using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 预判抛物线（版本二）
/// </summary>
public class DoMethod2 : MonoBehaviour
{
    //默认自身为初始位置可以改
    public Vector3 endpos;
    // public Transform startPoint;
    //public Transform startPos;
    public Transform endPos;
    public float heightPos;
    public float time;

    private GameObject target;

    private float acceleration;
    private float speedX;//速度水平x分量
    private float speedZ;//速度水平z分量
    private float speedY;//垂直方向分量
    // Use this for initialization
    private void Awake()
    {
        //协程禁用也能执行
        StartCoroutine(acticeself());
        this.enabled=false;
    }
    IEnumerator acticeself()
    {
        // gameObject.SetActive(false);
        yield return new WaitForSeconds(2);
        this.enabled=true;
    }
    void Start()
    {
        endpos = endPos.position;
        target = endPos.gameObject;
        if (target.TryGetComponent<DoMove>(out DoMove doMove))
        {
            endpos = doMove.ForecastDir(time);
        }


        float height = heightPos - transform.position.y;
        float distanceX = endpos.x - transform.position.x;
        float distanceZ = endpos.z - transform.position.z;

        speedX = distanceX / time;
        speedZ = distanceZ / time;

        speedY = 4 * height / time;
        acceleration = speedY / (0.5f * time);

        transform.rotation = Quaternion.LookRotation(new Vector3(speedX, speedY, speedZ), Vector3.up);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ((transform.position - endpos).magnitude > 0.1f)
        {
            speedY -= acceleration * Time.deltaTime;
            //transform.rotation = Quaternion.Euler(speedX, speedY, speedZ);
            transform.rotation = Quaternion.LookRotation(new Vector3(speedX, speedY, speedZ), Vector3.up);
            //Debug.Log(transform.forward);
            //Debug.LogWarning(new Vector3(speedX, speedY, speedZ).normalized);
            float speed = (new Vector3(speedX, speedY, speedZ)).magnitude;
            transform.Translate(Vector3.forward * speed * Time.deltaTime); //Vector3.forward使用局部坐标的Z方向
        }
    }

    private void DestroySelf()
    {
        GameObject.Destroy(this);
    }
}