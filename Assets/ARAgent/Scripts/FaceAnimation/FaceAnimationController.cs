using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;


namespace MillinAR
{
    public class FaceAnimationController : MonoBehaviour
    {
        public delegate void EventDelegate(bool state);

        public static event EventDelegate eventHandler;

        [Header("Must Contain Key")]
        [SerializeField] protected string m_MustKey = "jawOpen";
        [Header("Animation Time Unit")]
        [SerializeField] protected float m_TimeScale = 1000.0f;

        [Header("BlendShape Unit")]
        [SerializeField] protected float m_BlendShapeScale = 100.0f;

        [Header("Mapping ARKit Keys")]
        [SerializeField] protected List<BlendShapeKeyMap> m_BlendShapeKeyMaps = new List<BlendShapeKeyMap>();


        [SerializeField] protected bool m_IsUseARKit = true;

        private float _startTime = -1.0f;
        private float _endTime = float.MaxValue;

        private float _animationTime = 0.0f;

        private bool _playSkinAnimation = false;

        private float _currentTime;

        private ARKitBlendshapeWeight _blendshapes;

        private List<HeadRotation> _rotations;

        private bool _isResetSkin = false;

        private float _recordDeltaTime = 0;

        private bool _isLoop = false;

        private List<GameObject> _skinMeshNodes;

        void Start()
        {

        }
        void Update()
        {
            if (_playSkinAnimation)
            {
                _currentTime = Time.time;
                if (_currentTime >= _startTime && _currentTime <= _endTime && _blendshapes != null)
                {

                    var playedTime = Mathf.InverseLerp(_startTime, _endTime, _currentTime);

                    int weightIndex = (int)Mathf.Lerp(0, _blendshapes.blendMap[m_MustKey].Length - 1, playedTime);
                    SetBlendShapeWeight(weightIndex, true);
                }
                else if (_currentTime >= _endTime && _blendshapes != null)
                {
                    if (!_isResetSkin)
                    {
                        SetBlendShapeWeight(0, false);
                        _isResetSkin = true;

                        if (!_isLoop)
                        {
                            Debug.Log("FaceAnimation: currentTime " + _currentTime + " endTime " + _endTime);
                            _playSkinAnimation = false;
                            eventHandler(true);
                        }
                    }

                    if (_isResetSkin)
                    {
                        if (_isLoop && _currentTime >= (_endTime + 10 * Time.deltaTime))
                        {
                            PlayFaceAnimation();
                        }
                    }
                }
            }
        }

        private void SetBlendShapeWeight(int value, bool fromIndex)
        {
            foreach (string name in _blendshapes.blendMap.Keys)
            {
                float[] blendshapeWeights = _blendshapes.blendMap[name];
                if (blendshapeWeights == null || blendshapeWeights.Length == 0)
                {
                    continue;
                }

                foreach (GameObject nodeObject in _skinMeshNodes)
                {

                    if (null != nodeObject)
                    {
                        SkinnedMeshRenderer skinMeshRenderer = nodeObject.GetComponent<SkinnedMeshRenderer>();
                        if (null != skinMeshRenderer)
                        {
                            Mesh sharedMesh = skinMeshRenderer.sharedMesh;

                            int index = FindIndexByBlendShapeName(sharedMesh, name);

                            if (fromIndex && index != -1)
                            {
                                float score = blendshapeWeights[value] * m_BlendShapeScale;
                                float scoreNext = blendshapeWeights[value + 1] * m_BlendShapeScale;
                                if (value < blendshapeWeights.Length - 1)
                                {
                                    if (skinMeshRenderer.GetBlendShapeWeight(index) == 0.0f)
                                        score = Mathf.Lerp(score, scoreNext, Time.deltaTime / _recordDeltaTime);
                                    else
                                        score = Mathf.Lerp(skinMeshRenderer.GetBlendShapeWeight(index), scoreNext, Time.deltaTime / _recordDeltaTime);

                                }
                                skinMeshRenderer.SetBlendShapeWeight(index, score);

                            }
                            else if (!fromIndex && index != -1)
                            {
                                skinMeshRenderer.SetBlendShapeWeight(index, value);
                            }
                        }

                    }

                }
            }
        }

        private int FindIndexByBlendShapeName(Mesh sharedMesh, string name)
        {
            for (int i = 0; i < sharedMesh.blendShapeCount; i++)
            {
                string blendShapeName = sharedMesh.GetBlendShapeName(i);
                if (!m_IsUseARKit)
                {

                    string avatarName = GetBlendShapeKey(name);

                    if (avatarName == null) return -1;
                    if (blendShapeName == avatarName)
                    {
                        return i;
                    }
                }
                else
                {
                    if (blendShapeName == name)
                    {
                        return i;
                    }
                }

            }
            return -1;
        }

        private string GetBlendShapeKey(string arkitName)
        {

            for (int i = 0; i < m_BlendShapeKeyMaps.Count; i++)
            {
                var map = m_BlendShapeKeyMaps[i];
                if (map.arkitName == arkitName)
                {
                    return map.avatarName;
                }
            }

            return null;

        }


        public void Play(List<GameObject> nodes, string faceAnimationLocalPath, bool loop = false)
        {


            if (File.Exists(faceAnimationLocalPath))
            {
                _skinMeshNodes = nodes;

                string jsonStr = File.ReadAllText(faceAnimationLocalPath);
                BlendRecord blendRecord = JsonUtility.FromJson<BlendRecord>(jsonStr);
                blendRecord.blendshapes.CreateMap();

                _blendshapes = blendRecord.blendshapes;
                _rotations = blendRecord.rotations;

                _animationTime = (blendRecord.endTime - blendRecord.startTime) / m_TimeScale;

                _isLoop = loop;

                PlayFaceAnimation();
            }

        }

        public void PlayWithAutoAnimation(List<GameObject> nodes, bool loop = true)
        {
            _skinMeshNodes = nodes;

            BlendRecord blendRecord = GenBlendRecord();
            blendRecord.blendshapes.CreateMap();

            _blendshapes = blendRecord.blendshapes;
            _rotations = blendRecord.rotations;

            _animationTime = (blendRecord.endTime - blendRecord.startTime) / m_TimeScale;

            _isLoop = loop;

            PlayFaceAnimation();
        }

        private BlendRecord GenBlendRecord()
        {
            BlendRecord blendRecord = new BlendRecord();
            blendRecord.startTime = 0;
            blendRecord.endTime = 10 * 1000;
            blendRecord.fps = 30;

            int dataSize = 30 * 10;

            ARKitBlendshapeWeight aRKitBlendshapeWeight = new ARKitBlendshapeWeight();
            aRKitBlendshapeWeight.jawOpen = new float[dataSize];
            aRKitBlendshapeWeight.eyeBlinkLeft = new float[dataSize];
            aRKitBlendshapeWeight.eyeBlinkRight = new float[dataSize];

            for (int i = 0; i < dataSize; i++)
            {
                float periodVal = GetPeriodicValue(i);
                aRKitBlendshapeWeight.jawOpen[i] = periodVal;
                aRKitBlendshapeWeight.eyeBlinkLeft[i] = periodVal;
                aRKitBlendshapeWeight.eyeBlinkRight[i] = periodVal;
            }

            blendRecord.blendshapes = aRKitBlendshapeWeight;
            blendRecord.rotations = null;

            return blendRecord;
        }

        private float GetPeriodicValue(int input)
        {
            double TwoPi = 2 * Math.PI;
            double scaledInput = input % 1000;
            double result = Math.Sin(scaledInput * TwoPi / 100);
            result = (result + 1) / 2;

            return (float)result / 2;
        }

        public void Stop()
        {
            _playSkinAnimation = false;
            SetBlendShapeWeight(0, false);
            _isResetSkin = true;
            _isLoop = false;
        }

        private void PlayFaceAnimation()
        {
            _startTime = Time.time;
            _endTime = _startTime + _animationTime;

            _recordDeltaTime = _animationTime / _blendshapes.blendMap[m_MustKey].Length;

            _isResetSkin = false;
            _playSkinAnimation = true;
        }

        [Serializable]
        public class BlendShapeKeyMap
        {
            [SerializeField] public string arkitName;
            [SerializeField] public string avatarName;

            public BlendShapeKeyMap() { }
            public BlendShapeKeyMap(string _arkitName, string _avatarName)
            {
                arkitName = _arkitName;
                avatarName = _avatarName;
            }
        }
    }



}