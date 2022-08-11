using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CubeMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMoveX(50, 15).SetEase(Ease.Linear);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
