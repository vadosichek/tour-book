using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FilePicker {
    /// <summary>
    /// opens file choosing dialog
    /// </summary>

    public static string PickImage(int maxSize){
        string result = null;
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) => {
            result = path;
        }, "Select a PNG image", "image/*", maxSize);
        return result;
    }
}
