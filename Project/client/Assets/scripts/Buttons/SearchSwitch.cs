using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchSwitch : MonoBehaviour {
    
    public GameObject act;
    public GameObject inact;

    public void Switch(){
        act.transform.eulerAngles = new Vector3(0, 0, 0);
        inact.transform.eulerAngles = new Vector3(90,0,0);
    } 
}
