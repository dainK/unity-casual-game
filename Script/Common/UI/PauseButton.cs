using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameHeaven
{
    public class PauseButton : MonoBehaviour
    {
        [SerializeField] Sprite _pause;
        [SerializeField] Sprite _play;

        Image _image;
        bool _paused;
        public bool IsPaused => _paused;
        // Start is called before the first frame update
        void Start()
        {
            _image = GetComponent<Image>();
            //SetPause(false);
        }

        public void SetPause(bool paused)
        {
            _paused = paused;
            if(_paused)
            {
                _image.sprite = _play;
            }
            else
            {
                _image.sprite = _pause;
            }
        }
    }
}