using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Comment : Module {
    /// <summary>
    /// module variation
    /// comment UI-block
    /// </summary>

    //user name text object
    public Text name;
    //comment's text object
    public Text text;

    //variables to store data
    private string _name, _text;

    //set values to store
    public void SetValues(string __name, string __text){
        _name = __name + ":";
        _text = __text;
    }

    //override load -- loads stored data into text-objects
    public override void Load(){
        name.text = _name;
        text.text = _text;
    }
}
