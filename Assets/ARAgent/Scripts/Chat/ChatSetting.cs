using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChatSetting
{

    [SerializeField] public LLM m_ChatModel;

    public TTS m_TextToSpeech;

    public STT m_SpeechToText;
}
