using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenedTour : AppScreen {
    public int id;

    public override void Load(){
        TourViewer.instance.View(id);
    }
}
