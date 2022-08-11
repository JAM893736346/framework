using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 预判敌人位置脚本
/// </summary>
public class DoMove : MonoBehaviour
{
    //移动方向
    public Vector3 movedir;
    //上一次位置
    public Vector3 lastpos;
    //移动速度
    public float moveSpeed;
    //最终位置（敌人受击位置）
     Transform endPos;


    void Start()
    {
        lastpos = transform.position;
        StartCoroutine(nameof(testmovedir));
        endPos = transform;
    }
    //每隔0.2s检测一下各个变量
    IEnumerator testmovedir()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.15f);
            movedir = transform.position - lastpos;
            moveSpeed = Vector3.Magnitude(movedir) / 0.2f;
            movedir = movedir.normalized;
            lastpos = transform.position;
        }
    }
    /// <summary>
    /// 计算预测位置
    /// </summary>
    /// <param name="time">时间</param>
    /// <returns>计算好的位置</returns>
    public Vector3 ForecastDir(float time)
    {
        Vector3 resultPos = endPos.position + movedir * time * moveSpeed;
        return resultPos;
    }
    private void OnDestroy()
    {
        StopCoroutine(nameof(testmovedir));
    }
}
