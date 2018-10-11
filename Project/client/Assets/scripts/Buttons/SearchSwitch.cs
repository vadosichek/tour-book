using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchSwitch : MonoBehaviour {
    /// <summary>
    /// Controls mode of the search
    /// </summary>

    //object to set active
    public GameObject act;

    //object to inactivate
    public GameObject inact;

    //switch-action
    public void Switch(){
        act.transform.eulerAngles = new Vector3(0, 0, 0);
        inact.transform.eulerAngles = new Vector3(90,0,0);
    } 
}
