using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetPositionVector()
    {
        return transform.position;
    }

    public Transform GetTransform()
    {
        return transform;
    }


}
