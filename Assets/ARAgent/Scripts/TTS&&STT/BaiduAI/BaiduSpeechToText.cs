using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
[RequireComponent(typeof(BaiduSettings))]
public class BaiduSpeechToText : STT
{

    #region Baidu STT
    [SerializeField] private BaiduSettings m_Settings;


    private void Awake()
    {
        m_Settings = this.GetComponent<BaiduSettings>();
        m_SpeechRecognizeURL = "https://vop.baidu.com/server_api";
    }

    public override void SpeechToText(AudioClip _clip, Action<string> _callback)
    {
        StartCoroutine(GetBaiduRecognize(_clip, _callback));
    }


    private IEnumerator GetBaiduRecognize(AudioClip _audioClip, System.Action<string> _callback)
    {
        stopwatch.Restart();

        string asrResult = string.Empty;

        float[] samples = new float[_audioClip.samples];
        _audioClip.GetData(samples, 0);
        var samplesShort = new short[samples.Length];
        for (var index = 0; index < samples.Length; index++)
        {
            samplesShort[index] = (short)(samples[index] * short.MaxValue);
        }
        byte[] datas = new byte[samplesShort.Length * 2];

        Buffer.BlockCopy(samplesShort, 0, datas, 0, datas.Length);

        string url = string.Format(m_SpeechRecognizeURL + "?cuid={0}&token={1}", SystemInfo.deviceUniqueIdentifier, m_Settings.m_Token);

        WWWForm wwwForm = new WWWForm();
        wwwForm.AddBinaryData("audio", datas);

        using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, wwwForm))
        {
            unityWebRequest.SetRequestHeader("Content-Type", "audio/pcm;rate=16000");

            yield return unityWebRequest.SendWebRequest();

            if (string.IsNullOrEmpty(unityWebRequest.error))
            {
                asrResult = unityWebRequest.downloadHandler.text;
                RecogizeBackData _data = JsonUtility.FromJson<RecogizeBackData>(asrResult);
                if (_data.err_no == "0")
                {
                    _callback(_data.result[0]);
                }
                else
                {
                    _callback("Failed");
                }
            }
        }

        stopwatch.Stop();
        Debug.Log("Time：" + stopwatch.Elapsed.TotalSeconds);
    }

    #endregion


    [System.Serializable]
    public class RecogizeBackData
    {
        public string corpus_no = string.Empty;
        public string err_msg = string.Empty;
        public string err_no = string.Empty;
        public List<string> result;
        public string sn = string.Empty;
    }


    [System.Serializable]
    public class TokenInfo
    {
        public string access_token = string.Empty;
    }
}
