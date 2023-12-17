using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MillinAR
{
    public class AnimateController : MonoBehaviour
    {
        public bool walk
        {
            get { return _walk; }
            set
            {
                _walk = value;
                _animator?.SetBool("Walking", value);
                if (_walk)
                {
                    _animator?.SetBool("Talking", false);
                    _animator?.SetBool("Idle", false);
                }
            }
        }

        public bool talk
        {
            get { return _talk; }
            set
            {
                _talk = value;
                _animator?.SetBool("Talking", value);
                if (_talk)
                {
                    _animator?.SetBool("Walking", false);
                    _animator?.SetBool("Idle", false);
                }
            }
        }

        public bool idle
        {
            get { return _idle; }
            set
            {
                _idle = value;
                _animator?.SetBool("Idle", value);
                if (_idle)
                {
                    _animator?.SetBool("Walking", false);
                    _animator?.SetBool("Talking", false);
                }
            }
        }

        Animator _animator;

        bool _walk = false;
        bool _talk = false;
        bool _idle = false;

        void Awake()
        {
            _animator = GetComponent<Animator>();
            idle = true;
        }

        public float GetPlayedTime(string name)
        {
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(name)) return -1.0f;
            return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime * 100;
        }

    }
}
