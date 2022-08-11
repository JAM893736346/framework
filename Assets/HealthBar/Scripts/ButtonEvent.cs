using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEvent : MonoBehaviour
{
    Button button;
    public DoHealthBar doHealth;
    public float maxblood = 100;
    public float Damagevale = 10;
    public float Currentblood = 100;
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Damage);
        doHealth.Initlaize(maxblood);

    }

    // Update is called once per frame
    void Update()
    {

    }
    void Damage()
    {
        Currentblood -= Damagevale;
        doHealth.HealthBarFunc(Currentblood, maxblood, 1);
    }
}
