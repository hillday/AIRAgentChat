using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class STT : MonoBehaviour
{


    [SerializeField] protected string m_SpeechRecognizeURL = String.Empty;
    [SerializeField] protected Stopwatch stopwatch = new Stopwatch();

    public virtual void SpeechToText(AudioClip _clip, Action<string> _callback)
    {

    }


    public virtual void SpeechToText(byte[] _audioData, Action<string> _callback)
    {

    }


}
