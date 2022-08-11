using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFire : MonoBehaviour
{
    Doparabola myparabola;
    float time;
    [SerializeField] Transform targetPos;
    private void Start()
    {
        myparabola = GetComponent<Doparabola>();
        time = GetComponent<Doparabola>().Time;
        StartCoroutine(nameof(Fire));
    }
    //弓箭发射
    IEnumerator Fire()
    {
        yield return new WaitForSeconds(2f);
        //赋值初始位置
        myparabola.startPos = transform.position;
        if (targetPos.TryGetComponent<DoMove>(out DoMove doMove))
        {
            //赋值末位置
            myparabola.endPos = doMove.ForecastDir(time);
        }
        myparabola.init();
    }

}
