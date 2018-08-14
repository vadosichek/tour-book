using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorScreen : AppScreen {
    public delegate void OnProceed();
    public virtual event OnProceed Proceed;

    public delegate void OnCancel();
    public virtual event OnCancel Cancel;
}
