﻿using UnityEngine;
using System.Collections;

public class CameraSript : MonoBehaviour
{
    public static float offsetX;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(BirdScript.instance != null) {
            if (BirdScript.instance.isAlive) {
                MoveTheCamera();
            }
        }

    }

    void MoveTheCamera() {
        Vector3 temp = transform.position;
        temp.x = BirdScript.instance.GetPositionX() + offsetX;
        transform.position = temp;
    }
}