using System;
using UnityEngine;

interface ARSDKAdapter{
    void init();
    CameraIntrinisics GetCameraIntrinisics();

    Texture2D GetFrameTexture();
}