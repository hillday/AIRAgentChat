using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MillinAR;
using System;
using System.IO;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;


public class ChatAgent : MonoBehaviour
{
    /// <summary>
    /// Chat Setting
    /// </summary>
    [SerializeField] private ChatSetting m_ChatSettings;
    #region UI
    /// <summary>
    /// Audio Player
    /// </summary>
    [SerializeField] AudioPlayer m_AudioPlayer;

    /// <summary>
    /// Face Animation Player
    /// </summary>
    [SerializeField] FaceAnimationController m_FaceAnimationController;

    /// <summary>
    /// Need Play Face Animation
    /// </summary>
    [SerializeField] bool m_IsPlayFaceAnimation = true;

    /// <summary>
    /// VR Mode Space
    /// </summary>
    [SerializeField] GameObject m_ARSpace;

    /// <summary>
    /// Goto VR Mode Angle Of Phone X Rotate
    /// </summary>
    [SerializeField] float m_EnterVRAngle = 25.0f;

    /// <summary>
    /// VR Mode Pano Object
    /// </summary>
    [SerializeField] GameObject m_VRPano;

    /// <summary>
    /// Agent Of Avatar
    /// </summary>
    [SerializeField] TouristAgent m_TouristAgent;

    [SerializeField] ARFoundationXRAdapter m_ARSDKAdapter;
    #endregion

    #region Param
    Camera m_MainCamera;

    private float _lastEulerX = 0.0f;

    private string _CameraFuncCode = "F0001";

    private string _CameraFuncVoiceTips = "我正在拍照和理解中，请您耐心等等一下";
    #endregion
    private void Awake()
    {
        RegistButtonEvent();
    }

    private void Start()
    {
        m_MainCamera = Camera.main;
        FaceAnimationController.eventHandler += FinishAudioFacePlay;
        AudioPlayer.eventHandler += FinishAudioPlay;

        ChangeTourist("TouristGuide00");
        m_ARSDKAdapter.init();

    }


    void Update()
    {

        IsEnterVRSpace();
    }

    private void IsEnterVRSpace()
    {
        if (m_MainCamera.enabled)
        {
            Vector3 euler = m_MainCamera.transform.rotation.eulerAngles;
            if (euler.x > m_EnterVRAngle && euler.x < 360 - m_EnterVRAngle)
            {
                // check m_VRPano is activate
                if (!m_VRPano.activeSelf)
                    m_VRPano.SetActive(true);
                if (euler.x - _lastEulerX > 1)
                {
                    m_ARSpace.transform.rotation = Quaternion.Euler(euler.x, 0, 0);
                    _lastEulerX = euler.x;
                }
            }
            else
            {
                if (m_VRPano.activeSelf)
                    m_VRPano.SetActive(false);


                if (_lastEulerX > 0.0f)
                {
                    m_ARSpace.transform.rotation = Quaternion.Euler(0, 0, 0);
                    _lastEulerX = 0.0f;
                }
            }
        }
    }

    void OnDisable()
    {
        FaceAnimationController.eventHandler -= FinishAudioFacePlay;
        AudioPlayer.eventHandler -= FinishAudioPlay;
        UnRegistButtonEvent();
    }

    private void FinishAudioFacePlay(bool state)
    {
        Debug.Log("Face animation played：");
    }

    private void FinishAudioPlay(bool state)
    {
        Debug.Log("Audio played：");
        if (m_IsPlayFaceAnimation)
            m_FaceAnimationController.Stop();
        if (m_TouristAgent.GetAgentAnimator() != null)
            SetAnimator(false);

    }

    public void ChangeTourist(string name)
    {
        if (m_TouristAgent == null) return;

        m_TouristAgent.ChangeAvatar(name);
        m_IsPlayFaceAnimation = m_TouristAgent.GetHasFaceAnimation();
    }

    #region Send messgae

    public void SendData(string _postWord)
    {
        if (_postWord.Equals(""))
            return;

        Debug.Log("Sended message：" + _postWord);
        string _msg = _postWord;

        m_ChatSettings.m_ChatModel.PostMsg(_msg, CallBack);

    }

    private void CallBack(string _response)
    {
        _response = _response.Trim();
        _response = _response.Replace("*", "");

        Debug.Log("Reponse from AI：" + _response);
        if (_response.IndexOf(_CameraFuncCode) >= 0)
        {
            _response = _CameraFuncVoiceTips;
            StartCoroutine(ImageVision());
        }

        m_ChatSettings.m_TextToSpeech.Speak(_response, PlayVoice);
    }

    private IEnumerator ImageVision()
    {
        if (m_ARSDKAdapter == null)
        {
            Debug.Log("Error: ARSDKAdapte is null");
        }
        else
        {
            var imageTexture = m_ARSDKAdapter.GetFrameTexture();
            var encodedJPG = imageTexture.EncodeToJPG(90);
            //string path = "C:/Users/Surface/Downloads/Q04test01.jpg";
            // var encodedJPG = ReadImage(path);
            if (encodedJPG != null)
            {
                string encodedImage = Convert.ToBase64String(encodedJPG);
                encodedImage = "data:image/jpeg;base64," + encodedImage;
                m_ChatSettings.m_ChatModel.PostMsg(encodedImage, CallBack);
            }
        }


        yield return null;
    }

    public byte[] ReadImage(string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open, System.IO.FileAccess.Read);

        fileStream.Seek(0, SeekOrigin.Begin);

        byte[] binary = new byte[fileStream.Length];
        fileStream.Read(binary, 0, (int)fileStream.Length);

        fileStream.Close();

        fileStream.Dispose();

        fileStream = null;

        return binary;
    }

    #endregion

    #region Audio input
    /// <summary>
    /// Need Auto Send
    /// </summary>
    [SerializeField] private bool m_AutoSend = true;

    [SerializeField] private VoiceInputs m_VoiceInputs;
    [SerializeField] private GameObject m_UIDocument;

    private Button _voiceInputBotton;

    private void RegistButtonEvent()
    {
        if (m_UIDocument == null) return;
        var uiDocument = m_UIDocument.GetComponent<UIDocument>();

        _voiceInputBotton = uiDocument.rootVisualElement.Q("RecordBtn") as Button;
        _voiceInputBotton.clickable.activators.Clear();

        _voiceInputBotton.RegisterCallback<PointerDownEvent>(StartRecord);
        _voiceInputBotton.RegisterCallback<PointerUpEvent>(StopRecord);

    }

    private void UnRegistButtonEvent()
    {
        _voiceInputBotton.UnregisterCallback<PointerDownEvent>(StartRecord);
        _voiceInputBotton.UnregisterCallback<PointerUpEvent>(StopRecord);

    }


    public void StartRecord(PointerDownEvent evt)
    {
        var label = _voiceInputBotton.Q<Label>();
        label.text = "...";
        Debug.Log("ChatSample: Start record voice");
        m_VoiceInputs.StartRecordAudio();
    }

    public void StopRecord(PointerUpEvent evt)
    {
        var label = _voiceInputBotton.Q<Label>();
        label.text = "Ask";
        Debug.Log("ChatSample: End record voice");
        m_VoiceInputs.StopRecordAudio(AcceptClip);
    }

    private void AcceptData(byte[] _data)
    {
        if (m_ChatSettings.m_SpeechToText == null)
            return;

        m_ChatSettings.m_SpeechToText.SpeechToText(_data, DealingTextCallback);
    }


    private void AcceptClip(AudioClip _audioClip)
    {
        if (m_ChatSettings.m_SpeechToText == null)
            return;

        m_ChatSettings.m_SpeechToText.SpeechToText(_audioClip, DealingTextCallback);
    }

    private void DealingTextCallback(string _msg)
    {
        if (m_AutoSend)
        {
            SendData(_msg);
            return;
        }

    }

    #endregion

    #region Text to speech

    private void PlayVoice(AudioClip _clip, string _response)
    {
        SetAnimator(true);
        m_AudioPlayer.PlayAudio(_clip, AudioPlayStarted);
        if (m_IsPlayFaceAnimation)
            m_FaceAnimationController.PlayWithAutoAnimation(m_TouristAgent.GetSkinMeshes());

    }

    private void AudioPlayStarted(bool state, string msg)
    {
        Debug.Log("ARAssistant:Start audio state " + state + " msg " + msg);
        if (!state)
        {
            if (m_IsPlayFaceAnimation)
                m_FaceAnimationController.Stop();
            if (m_TouristAgent.GetAgentAnimator() != null)
                SetAnimator(false);
        }

    }

    #endregion

    private void SetAnimator(bool talking)
    {
        if (m_TouristAgent == null)
            return;

        if (talking)
        {
            m_TouristAgent.GetAgentAnimator().talk = true;
        }
        else
        {
            m_TouristAgent.GetAgentAnimator().idle = true;
        }

    }
}
