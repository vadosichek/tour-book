using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Comment : Module {
    
    public Text name;
    public Text text;

    private string _name, _text;

    public void SetValues(string __name, string __text){
        _name = __name + ":";
        _text = __text;
    }

    public override void Load(){
        name.text = _name;
        text.text = _text;
    }
}
