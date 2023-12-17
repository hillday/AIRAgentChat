using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR;

[RequireComponent(typeof(BaiduSettings))]
public class BaiduTextToSpeech : TTS
{
    #region Param

    [SerializeField] private BaiduSettings m_Settings;

    [SerializeField] private PostDataSetting m_Post_Setting;
    #endregion

    private void Awake()
    {
        m_Settings = this.GetComponent<BaiduSettings>();
        m_PostURL = "http://tsn.baidu.com/text2audio";
    }

    #region Public Method



    public override void Speak(string _msg, Action<AudioClip, string> _callback)
    {
        StartCoroutine(GetSpeech(_msg, _callback));
    }

    #endregion

    #region Private Method


    private IEnumerator GetSpeech(string _msg, Action<AudioClip, string> _callback)
    {
        stopwatch.Restart();
        var _url = m_PostURL;
        var _postParams = new Dictionary<string, string>();
        _postParams.Add("tex", _msg);
        _postParams.Add("tok", m_Settings.m_Token);
        _postParams.Add("cuid", SystemInfo.deviceUniqueIdentifier);
        _postParams.Add("ctp", m_Post_Setting.ctp);
        _postParams.Add("lan", m_Post_Setting.lan);
        _postParams.Add("spd", m_Post_Setting.spd);
        _postParams.Add("pit", m_Post_Setting.pit);
        _postParams.Add("vol", m_Post_Setting.vol);
        _postParams.Add("per", SetSpeeker(m_Post_Setting.per));
        _postParams.Add("aue", m_Post_Setting.aue);

        int i = 0;
        foreach (var item in _postParams)
        {
            _url += i != 0 ? "&" : "?";
            _url += item.Key + "=" + item.Value;
            i++;
        }


        using (UnityWebRequest _speech = UnityWebRequestMultimedia.GetAudioClip(_url, AudioType.WAV))
        {
            yield return _speech.SendWebRequest();
            if (_speech.error == null)
            {
                var type = _speech.GetResponseHeader("Content-Type");
                if (type.Contains("audio"))
                {
                    var _clip = DownloadHandlerAudioClip.GetContent(_speech);
                    _callback(_clip, _msg);
                }
                else
                {
                    var _response = _speech.downloadHandler.data;
                    string _msgBack = System.Text.Encoding.UTF8.GetString(_response);
                    UnityEngine.Debug.LogError(_msgBack);
                }

            }

            stopwatch.Stop();
            UnityEngine.Debug.Log("Time:" + stopwatch.Elapsed.TotalSeconds);
        }

    }

    private string SetSpeeker(SpeekerRole _role)
    {
        if (_role == SpeekerRole.度小宇) return "1";
        if (_role == SpeekerRole.度小美) return "0";
        if (_role == SpeekerRole.度逍遥) return "3";
        if (_role == SpeekerRole.度丫丫) return "4";
        if (_role == SpeekerRole.JP度小娇) return "5";
        if (_role == SpeekerRole.JP度逍遥) return "5003";
        if (_role == SpeekerRole.JP度小鹿) return "5118";
        if (_role == SpeekerRole.JP度博文) return "106";
        if (_role == SpeekerRole.JP度小童) return "110";
        if (_role == SpeekerRole.JP度小萌) return "111";
        if (_role == SpeekerRole.JP度米朵) return "5";

        return "0";
    }

    #endregion

    #region Data




    [System.Serializable]
    public class PostDataSetting
    {
        public string ctp = "1";

        public string lan = "zh";

        public string spd = "5";

        public string pit = "5";

        public string vol = "5";

        public SpeekerRole per = SpeekerRole.度小美;

        public string aue = "6";
    }

    public enum SpeekerRole
    {
        度小宇,
        度小美,
        度逍遥,
        度丫丫,
        JP度逍遥,
        JP度小鹿,
        JP度博文,
        JP度小童,
        JP度小萌,
        JP度米朵,
        JP度小娇
    }


    public class SpeechResponse
    {
        public int error_index;
        public string error_msg;
        public string sn;
        public int idx;
        public bool Success
        {
            get { return error_index == 0; }
        }
        public AudioClip clip;
    }


    #endregion

}
