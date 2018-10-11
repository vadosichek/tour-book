using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorScreen : AppScreen {
    /// <summary>
    /// appScreen variation
    /// base class for editor screens
    /// </summary>

    //event to call when edits done
    public delegate void OnProceed();
    public virtual event OnProceed Proceed;

    //event to call when edits cancelled
    public delegate void OnCancel();
    public virtual event OnCancel Cancel;
}