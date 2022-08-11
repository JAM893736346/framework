using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
    //下一步怎么封装起来为一个独立功能？
/// </summary>
public class DoHealthBar : MonoBehaviour
{

    //闪烁快慢系数
    float AValue = 1;
    int mark = 1;
    //sin值
    float Sinvalue;
    //时间控制器
    float TimeParm;
    //协程变量
    Coroutine TimeAutoAddCoroutine;
    Coroutine Bloodcoroutine;
    Coroutine WaitTimecoroutine;
    //动画曲线amount
    AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

    Image backimg;
    Image frontimg;

    float MaxBlood;
    float Currentblood = 0;
    [Header("控制颜色闪烁开关")]
    [SerializeField] bool ISChangeAndflashing = false;
    [Header("控制血条动画状态")]
    [SerializeField] BloodState bloodState;


    UnityAction unityAction = delegate { };
    //分为 2种 第一类 带或者不带血条变色闪烁  第二类 血条缓动类型 不动，第一层动 ，二层动

    //调用用初始化
    public virtual void Initlaize(float maxblood)
    {
        backimg = transform.GetChild(0).GetComponent<Image>();
        frontimg = transform.GetChild(1).GetComponent<Image>();
        //初始化
        frontimg.fillAmount = 1;
        MaxBlood = maxblood;
        Currentblood = maxblood;
    }
    private void Update()
    {
        if (ISChangeAndflashing) ChangeAndflashing(frontimg);
    }
    //外部调用
    public void HealthBarFunc(float TargetBlood, float MaxBlood, float time)
    {
        switch (bloodState)
        {
            case BloodState.Empty:
                {
                    Emptyfillamount(frontimg, TargetBlood, MaxBlood);
                    break;
                }
            case BloodState.Frontfalsh:
                {
                    Slowfillamount(frontimg, TargetBlood, MaxBlood, time);
                    break;
                }
            case BloodState.Allflash:
                {
                    AllSlowamount(frontimg, backimg, TargetBlood, MaxBlood, time);
                    break;
                }
        }
    }

    //空动画类型
    void Emptyfillamount(Image front, float TargetBlood, float MaxBlood)
    {
        front.fillAmount = TargetBlood / MaxBlood;
    }

    //功能2血条缓动效果；
    void Slowfillamount(Image front, float Targetblood, float maxblood, float time)
    {
        //重置当前血量
        front.fillAmount = Currentblood / maxblood;
        if (Bloodcoroutine != null)
        {
            StopCoroutine(Bloodcoroutine);
        }
        Bloodcoroutine = StartCoroutine(LerpMethod(front, Currentblood, Targetblood, maxblood, time, curve));
        Currentblood = Targetblood;
    }

    /// <summary>
    /// 曲线差值血量
    /// </summary>
    /// <param name="front">前景照片</param>
    /// <param name="currentValue">当前值</param>
    /// <param name="TargetValue">目标值</param>
    /// <param name="MaxBlood">最大值</param>
    /// <param name="time">时间</param>
    /// <param name="cure">动画曲线</param>
    /// <returns></returns>
    IEnumerator LerpMethod(Image front, float currentValue, float TargetValue, float MaxBlood, float time, AnimationCurve curve)
    {
        float t = 0;
        while (t < 1)
        {
            //差值运算改变血条值
            front.fillAmount = Mathf.Lerp(currentValue / MaxBlood, TargetValue / MaxBlood, curve.Evaluate(t));
            t += Time.deltaTime / time;

            yield return null;
        }
    }
    //红到绿--大部分值只和fillAmount挂钩
    void ChangeAndflashing(Image front)
    {
        //开启计时协程
        if (TimeAutoAddCoroutine != null) StopCoroutine(TimeAutoAddCoroutine);
        TimeAutoAddCoroutine = StartCoroutine(nameof(TimeAutoAdd));
        /// 功能1--血条绿变红低于20闪烁
        AValue = Mathf.Clamp(1 / front.fillAmount, 1, 6);
        // 需求血量越小闪烁越快
        Sinvalue = front.fillAmount >= 0.5 ? 1 : (Mathf.Abs(Mathf.Sin(TimeParm * AValue)) + 1) / 2;
        front.color = new Color(1 - front.fillAmount, front.fillAmount, 0, Sinvalue);
    }

    /// <summary>
    /// 时间迂回
    /// </summary>
    /// <returns></returns>
    IEnumerator TimeAutoAdd()
    {
        while (true)
        {
            TimeParm += Time.deltaTime * mark;

            if (Mathf.Abs((int)TimeParm) == 10) mark *= -1;
            yield return null;
        }
    }

    ///功能3 血条前先先动后延迟效果
    void AllSlowamount(Image front, Image back, float Targetblood, float maxblood, float time)
    {
        Emptyfillamount(front, Targetblood, maxblood);

        Slowfillamount(back, Targetblood, maxblood, time);
        // if (WaitTimecoroutine != null) StopCoroutine(WaitTime(back,Targetblood,maxblood,time));
        // StartCoroutine(WaitTime(back,Targetblood,maxblood,time));

    }
    //动画等待时间
    IEnumerator WaitTime(Image back, float Targetblood, float maxblood, float time)
    {
        yield return new WaitForSeconds(2);
        Slowfillamount(back, Targetblood, maxblood, time);
    }
    //功能五：加血 中毒效果
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}

public enum BloodState
{
    //空动画类型
    Empty,
    //单层缓动
    Frontfalsh,
    //多层缓动
    Allflash
}
