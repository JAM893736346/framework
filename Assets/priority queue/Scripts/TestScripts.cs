using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScripts : MonoBehaviour
{
    //面板中观察列表顺序
    public List<GameObject> objects = new List<GameObject>();
    SelectList<GameObject> list = new SelectList<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //初始化队列
        for (int i = 0; i < transform.childCount; i++)
        {
            list.Enqueue(transform.GetChild(i).gameObject);
        }
        //刷新列表
        refresh();
        StartCoroutine(ChangeEnemy());
    }

    // Update is called once per frame

    void refresh()
    {
        Debug.Log("顺序调整");
        objects.Clear();
        for (int i = 0; i < list.list.Count; i++)
        {
            objects.Add(list.list[i].Value);
        }
    }
    IEnumerator ChangeEnemy()
    {
        yield return new WaitForSeconds(1);
        list.ChangeData(3);
        list.ChangeData(4);
        list.ChangeData(5);
        list.Dequeue(list.list[0].Value);
        refresh();
        yield return null;
    }
}
