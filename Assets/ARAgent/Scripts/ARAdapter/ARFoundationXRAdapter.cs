using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections.LowLevel.Unsafe;
public class ARFoundationXRAdapter : MonoBehaviour, ARSDKAdapter
{

    private ARCameraManager m_CameraManager;
    private Texture2D currentFrame;
    private CameraIntrinisics cameraIntrinisics;

    private bool m_IntrinsicsGet;


    void Awake()
    {
    }

    public void init()
    {
        m_IntrinsicsGet = false;
        currentFrame = null;
        m_CameraManager = FindObjectOfType<ARCameraManager>();
        // m_session_origin = FindObjectOfType<ARSessionOrigin>();
        if (m_CameraManager == null)
        {
            Debug.LogError(GetType() + "/Awake()/ m_CameraManager is null!");
        }
    }

    public CameraIntrinisics GetCameraIntrinisics()
    {
        initIntrinsicsGet();
        return cameraIntrinisics;
    }

    public Texture2D GetFrameTexture()
    {
        initFrameData();
        return currentFrame;
    }

    void initIntrinsicsGet()
    {
        if (!m_IntrinsicsGet)
        {
            if (m_CameraManager.TryGetIntrinsics(out var intrinsics))
            {
                cameraIntrinisics = new CameraIntrinisics(intrinsics.focalLength, intrinsics.principalPoint, intrinsics.resolution);
                m_IntrinsicsGet = true;
            }
        }
    }

    unsafe public void initFrameData()
    {
        if (m_CameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            currentFrame = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false, false);
            XRCpuImage.Transformation m_Transformation = XRCpuImage.Transformation.MirrorY;
            var conversionParams = new XRCpuImage.ConversionParams(image, TextureFormat.RGBA32, m_Transformation);
            var rawTextureData = currentFrame.GetRawTextureData<byte>();

            image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
            currentFrame.Apply();

            image.Dispose();
        }
    }
}