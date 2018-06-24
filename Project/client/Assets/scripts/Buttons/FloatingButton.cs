using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingButton : MonoBehaviour {
    public GameObject action_buttons;

    public void Open(){
        action_buttons.SetActive(!action_buttons.active);
    }
}
