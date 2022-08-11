using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectData<T> : IComparable<SelectData<T>>
{
    //值类型
    public T Value
    {
        get { return Object; }
        set { Object = value; }

    }
    //优先级
    public int Priority
    {
        get { return priority; }
        set { priority = value; }

    }
    //对象
    T Object;
    //优先级
    int priority;
    //只有一个参数构造函数
    public SelectData(T Object)
    {
        this.Object = Object;
        priority = 0;
    }
    //构造函数
    public SelectData(T Object, int priority)
    {
        this.Object = Object;
        this.priority = priority;
    }
    //sort有三种结果 1,-1,0分别是大，小，相等。
    public int CompareTo(SelectData<T> other)
    {
        if (this.Priority > other.Priority)
        {
            return -1;
        }
        else if (this.Priority < other.Priority)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}

public class SelectList<T>
{
    //大小
    int Size;
    //储存所需队列
    public List<SelectData<T>> list;
    public SelectList() => list = new List<SelectData<T>>();
    /// <summary>
    /// 进队
    /// </summary>
    /// <param name="value"></param>T
    public void Enqueue(T value)
    {
        SelectData<T> select = new SelectData<T>(value);
        list.Add(select);
        Size++;
    }
    /// <summary>
    /// 出队
    /// </summary>
    /// <returns></returns>
    public T Dequeue(T value)
    {
        SelectData<T> temp = list[GetIndex(value)];
        list.Remove(temp);
        Size--;
        sortValue();
        return value;
    }
    /// <summary>
    /// 改变值
    /// </summary>
    /// <param name="InDex"></param>
    public void ChangeData(int InDex)
    {
        if (InDex > list.Count)
        {
            return;
        }
        list[InDex].Priority = 2;
    }
    /// <summary>
    /// 获得索引
    /// </summary>
    /// <param name="Obj"></param>
    public int GetIndex(T Obj)
    {
        for (int i = 0; i < Length(); i++)
        {
            if (list[i].Value.Equals(Obj))
            {
                return i;
            }
        }
        Debug.Log("不存在");
        return -1;
    }

    /// <summary>
    /// 判断相同 否
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Contains(T value)
    {
        for (int i = 0; i < Size; i++)
        {
            if (list[i].Value.Equals(value))
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 根据优先级排序
    /// </summary>
    void sortValue() => list.Sort();
    /// <summary>
    /// 获得第一个敌人
    /// </summary>
    /// <returns></returns>
    public T Peek() => list[0].Value;
    /// <summary>
    /// 长度
    /// </summary>
    /// <returns></returns>
    public int Length() => Size;
    /// <summary>
    /// 清空
    /// </summary>
    public void Clear()
    {
        Size = 0;
        list.Clear();
    }

}
