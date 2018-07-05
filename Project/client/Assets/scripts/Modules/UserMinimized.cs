using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UserMinimized : Module {
    public Image pic;
    public Text login;
    public Text name;
}

[Serializable]
public struct UserMinimizedJSON{
    public int id;
    public string login;
    public string name;
};
