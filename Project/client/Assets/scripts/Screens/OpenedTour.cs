using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenedTour : AppScreen {
    /// <summary>
    /// appScreen variation
    /// screen to view opened tour (panorams)
    /// </summary>

    //tour id
    public int id;

    //load override -- download tour
    public override void Load(){
        TourViewer.instance.View(id);
    }
}
