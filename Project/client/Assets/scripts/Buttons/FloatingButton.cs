using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingButton : MonoBehaviour {
    /// <summary>
    /// Round floating button class to use on almost every screen
    /// </summary>

    //parent of multiple buttons
    public GameObject action_buttons; 

    //opens/closes list of action_buttons
    public void Open(){ 
        action_buttons.SetActive(!action_buttons.active);
    }
}
