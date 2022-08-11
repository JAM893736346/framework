using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 模拟抛物线 配合DoMove预测曲线终点
/// </summary>
public class Doparabola : MonoBehaviour
{
    //起始位置
    public Vector3 startPos;
    //终点位置
    public Vector3 endPos;
    //时间
    public float Time = 1;
    //飞行高度
    public float height = 25;
    //初始化
    public void init()
    {
        //设置路径点
        Vector3[] path1 = new Vector3[3];
        path1[0] = startPos;//起始点
        path1[1] = new Vector3((startPos.x + endPos.x) / 2, height, (startPos.z + endPos.z) / 2);//中间点
        path1[2] = endPos;//终点

        var tweenPath = transform.DOPath(path1, Time, PathType.CatmullRom).SetLookAt(0).SetEase(Ease.Linear);
        tweenPath.onComplete = () =>
        {
            // 完成函数
        };
    }
}
