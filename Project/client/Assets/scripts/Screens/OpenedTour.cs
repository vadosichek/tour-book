using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenedTour : AppScreen {
    public int id;
    public TourViewer tour_viewer;

    public override void Load(){
        tour_viewer.View(id);
    }
}
