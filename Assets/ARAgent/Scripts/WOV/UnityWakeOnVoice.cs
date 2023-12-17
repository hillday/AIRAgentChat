using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
#if UNITY_STANDALONE_WIN
using UnityEngine.Windows.Speech;
#endif
public class UnityWakeOnVoice : WOV
{

    [SerializeField]
    private string[] m_Keywords = { "玲玲" };

#if UNITY_STANDALONE_WIN
    private KeywordRecognizer m_Recognizer;
    // Use this for initialization
    void Start()
    {
        m_Recognizer = new KeywordRecognizer(m_Keywords);
        m_Recognizer.OnPhraseRecognized += OnPhraseRecognized;

    }
    
   
    public override void StartRecognizer()
    {
        if (m_Recognizer == null)
            return;

        m_Recognizer.Start();
    }
   
    public override void StopRecognizer()
    {
        if (m_Recognizer == null)
            return;

        m_Recognizer.Stop();
    }

   
    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendFormat("{0}", args.text);
        string _keyWord = builder.ToString();
        OnAwakeOnVoice(_keyWord);
    }
#endif
}
