using System;
using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class AzureTextToSpeech : TTS
{

    #region Param

    [SerializeField] private AzureSettings m_AzureSettings;

    public string voiceName = "zh-CN-XiaomoNeural";
    public string style = "chat";//chat  cheerful  angry  excited  sad

    #endregion
    private void Awake()
    {
        m_AzureSettings = this.GetComponent<AzureSettings>();
        m_PostURL = string.Format("https://{0}.tts.speech.microsoft.com/cognitiveservices/v1", m_AzureSettings.serviceRegion);
    }

    public override void Speak(string _msg, Action<AudioClip> _callback)
    {
        StartCoroutine(GetVoice(_msg, _callback));
    }

    public override void Speak(string _msg, Action<AudioClip, string> _callback)
    {
        StartCoroutine(GetVoice(_msg, _callback));
    }

    private IEnumerator GetVoice(string _msg, Action<AudioClip> _callback)
    {
        stopwatch.Restart();

        string textToSpeechRequestBody = GenerateTextToSpeech(m_AzureSettings.language, voiceName, style, 2, _msg);

        using (UnityWebRequest speechRequest = new UnityWebRequest(m_PostURL, "POST"))
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(textToSpeechRequestBody);
            speechRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
            speechRequest.downloadHandler = (DownloadHandler)new DownloadHandlerAudioClip(speechRequest.uri, AudioType.MPEG);

            speechRequest.SetRequestHeader("Ocp-Apim-Subscription-Key", m_AzureSettings.subscriptionKey);
            speechRequest.SetRequestHeader("X-Microsoft-OutputFormat", "audio-16khz-32kbitrate-mono-mp3");
            speechRequest.SetRequestHeader("Content-Type", "application/ssml+xml");

            yield return speechRequest.SendWebRequest();

            if (speechRequest.responseCode == 200)
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(speechRequest);
                _callback(audioClip);
            }
            else
            {
                Debug.LogError("Failed: " + speechRequest.error);
            }
        }

        stopwatch.Stop();
        Debug.Log("Azure take time：" + stopwatch.Elapsed.TotalSeconds);
    }

    private IEnumerator GetVoice(string _msg, Action<AudioClip, string> _callback)
    {
        stopwatch.Restart();

        string textToSpeechRequestBody = GenerateTextToSpeech(m_AzureSettings.language, voiceName, style, 2, _msg);

        using (UnityWebRequest speechRequest = new UnityWebRequest(m_PostURL, "POST"))
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(textToSpeechRequestBody);
            speechRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
            speechRequest.downloadHandler = (DownloadHandler)new DownloadHandlerAudioClip(speechRequest.uri, AudioType.MPEG);

            speechRequest.SetRequestHeader("Ocp-Apim-Subscription-Key", m_AzureSettings.subscriptionKey);
            speechRequest.SetRequestHeader("X-Microsoft-OutputFormat", "audio-16khz-32kbitrate-mono-mp3");
            speechRequest.SetRequestHeader("Content-Type", "application/ssml+xml");

            yield return speechRequest.SendWebRequest();

            if (speechRequest.responseCode == 200)
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(speechRequest);
                _callback(audioClip, _msg);
            }
            else
            {
                Debug.LogError("Failed: " + speechRequest.error);
            }
        }

        stopwatch.Stop();
        Debug.Log("Azure take time: " + stopwatch.Elapsed.TotalSeconds);
    }

    public string GenerateTextToSpeech(string lang, string name, string style, int styleDegree, string text)
    {
        string xml = string.Format(@"<speak version=""1.0"" xmlns=""http://www.w3.org/2001/10/synthesis""
            xmlns:mstts=""https://www.w3.org/2001/mstts"" xml:lang=""{0}"">
            <voice name=""{1}"">
                <mstts:express-as style=""{2}"" styledegree=""{3}"">
                    {4}
                </mstts:express-as>
            </voice>
        </speak>", lang, name, style, styleDegree, text);

        return xml;
    }

}
