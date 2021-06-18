using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatUpDown : MonoBehaviour
{
    public float Amplitude = 0.5f;
    public float speed = 2f;
    public float CurrHeight;
    public float FuckTime = 0f;
    Vector3 StartPos;

    private void Start()
    {
        StartPos = this.transform.position;
    }
    void Update()
    {
        FuckTime += Time.deltaTime;
        CurrHeight = (Mathf.Sin(FuckTime*speed) * Amplitude);
        this.transform.position = ((Vector3.up * CurrHeight) + StartPos);
    }
}
