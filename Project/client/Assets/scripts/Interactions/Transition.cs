using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : Interaction {
    /// <summary>
    /// interaction variation
    /// button to move from one panorama to another
    /// </summary>

    //transit to (panorama id)
    public int target_id;

    //tour object
    public Tour tour;

    //call tour transition method on open
    public override void Open(){
        tour.Move(target_id);
    }
}
