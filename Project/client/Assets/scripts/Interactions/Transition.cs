using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : Interaction {

    public int target_id;
    public Tour tour;

    public override void Open(){
        tour.Move(target_id);
    }
}
