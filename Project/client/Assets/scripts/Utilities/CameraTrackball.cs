﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrackball : MonoBehaviour {
    /// <summary>
    /// rotates camera on draf
    /// </summary>

    public float speed;

    Vector3 FirstPoint;
    Vector3 SecondPoint;
    float xAngle;
    float yAngle;
    float xAngleTemp;
    float yAngleTemp;

    void Start(){
        ToDefault();
    }

    //reset position
    public void ToDefault(){
        xAngle = 0;
        yAngle = 0;
        this.transform.rotation = Quaternion.Euler(yAngle, xAngle, 0);
    }

    void Update(){
        if (Input.touchCount > 0){
            if (Input.GetTouch(0).phase == TouchPhase.Began){
                FirstPoint = Input.GetTouch(0).position;
                xAngleTemp = xAngle;
                yAngleTemp = yAngle;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved){
                SecondPoint = Input.GetTouch(0).position;
                xAngle = xAngleTemp + (SecondPoint.x - FirstPoint.x) * speed * 180 / Screen.width;
                yAngle = yAngleTemp + (SecondPoint.y - FirstPoint.y) * speed * 90 / Screen.height;
                transform.rotation = Quaternion.Euler(yAngle, -xAngle, 0.0f);
            }
        }
    }
}
