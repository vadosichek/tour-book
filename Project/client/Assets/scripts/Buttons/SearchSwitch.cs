using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchSwitch : MonoBehaviour {

    public GameObject users;
    public GameObject posts;

    public void Switch(){
        float posts_r = posts.transform.eulerAngles.x == 90 ? 0 : 90;
        posts.transform.eulerAngles = new Vector3(posts_r, 0, 0);

        float users_r = users.transform.eulerAngles.x == 90 ? 0 : 90;
        users.transform.eulerAngles = new Vector3(users_r,0,0);
    } 
}
