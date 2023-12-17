using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using UnityEngine.UI;
using System.Globalization;
using System.IO;
using System;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;


namespace MillinAR
{
    public class TouristAgent : MonoBehaviour
    {



        [SerializeField] private GameObject m_NavAgent;


        [SerializeField] private string m_AvatarNodeName = "TouristGuide01";

        private AnimateController _agentAnimater;


        [SerializeField] private List<AvatarConfig> m_Avatars = new List<AvatarConfig>();

        public delegate void EventDelegate(bool state);
        public static event EventDelegate eventHandler;


        void Awake()
        {

        }

        private void Start()
        {

        }


        public AnimateController GetAgentAnimator()
        {
            return _agentAnimater;
        }

        public List<GameObject> GetSkinMeshes()
        {
            AvatarConfig avatarConfig = GetAvatarConfig(m_AvatarNodeName);
            if (avatarConfig == null) return null;

            List<GameObject> faceObjects = new List<GameObject>();
            foreach (string node in avatarConfig.skinMeshs)
            {
                GameObject nodeObject = m_NavAgent.transform.Find(m_AvatarNodeName + "/" + node).gameObject;
                faceObjects.Add(nodeObject);
            }

            return faceObjects;
        }

        public bool GetHasFaceAnimation()
        {
            AvatarConfig avatarConfig = GetAvatarConfig(m_AvatarNodeName);
            if (avatarConfig == null) return false;

            return avatarConfig.hasFaceAnimation;
        }


        public Vector3 GetAgentPosition()
        {
            return m_NavAgent.transform.position;
        }


        public void ChangeAvatar(string name)
        {

            foreach (AvatarConfig node in m_Avatars)
            {
                GameObject nodeObject = m_NavAgent.transform.Find(node.name).gameObject;
                nodeObject.SetActive(false);
            }

            GameObject currentNode = m_NavAgent.transform.Find(name).gameObject;
            currentNode.SetActive(true);

            m_AvatarNodeName = name;
            _agentAnimater = currentNode.GetComponent<AnimateController>();
        }

        private AvatarConfig GetAvatarConfig(string name)
        {
            foreach (AvatarConfig node in m_Avatars)
            {
                if (node.name == name) return node;
            }

            return null;
        }


        void Update()
        {

        }

        [Serializable]
        public class AvatarConfig
        {
            [SerializeField] public string name;
            [SerializeField] public List<string> skinMeshs;
            [SerializeField] public bool hasFaceAnimation;

            public AvatarConfig() { }
            public AvatarConfig(string _name, List<string> _skinMeshs, bool _hasFaceAnimation)
            {
                name = _name;
                skinMeshs = _skinMeshs;
                hasFaceAnimation = _hasFaceAnimation;
            }
        }
    }


}