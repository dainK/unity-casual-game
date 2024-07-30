using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameHeaven
{
    public class UISpriteAnimation : MonoBehaviour
    {
         Image _image;

        [SerializeField] List<Sprite> _sprites;
        [SerializeField] float _speed = 0.02f;

        public List<Sprite> Sprites => _sprites;

        int _index = 0;
        Coroutine _coroutineAnim;
        bool _isLoop = false;
        private Tween _delayedCall = null;

        private void Start()
        {
            _image = GetComponent<Image>();
            _image.raycastTarget = false;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _delayedCall?.Kill();
            _delayedCall = null;
        }

        public void ChangeSprite(List<Sprite> sprites)
        {
            _sprites = sprites;
        }

        public void PlayAnimationOnce()
        {
            gameObject.SetActive(true);
            if (_coroutineAnim != null)
            {
                StopCoroutine(_coroutineAnim);
            }
            _coroutineAnim = StartCoroutine(PlayAnimUI());
        }

        public void PlayAnimationLoop(float duration)
        {
            gameObject.SetActive(true);
            _index = 0; 
            _isLoop = true;
            if (_coroutineAnim != null)
            {
                StopCoroutine(_coroutineAnim);
            }
            _coroutineAnim = StartCoroutine(PlayAnimUI());

            if (duration > 0f) {
                _delayedCall?.Kill();
                _delayedCall = null;
                _delayedCall = DOVirtual.DelayedCall(duration, StopAnimation);
            }
        }

        public void StopAnimation()
        {
            _isLoop  = false;
            if (_coroutineAnim != null)
            {
                StopCoroutine(_coroutineAnim);
                _coroutineAnim = null;
            }
            gameObject.SetActive(false);
        }

        private IEnumerator PlayAnimUI()
        {
            while (true)
            {
                yield return new WaitForSeconds(_speed);

                _image.sprite = _sprites[_index];
                _index += 1;

                if (_index >= _sprites.Count)
                {
                    _index = 0;
                    if(!_isLoop )
                    {
                        StopAnimation();
                    }
                }

            }
        }
    }
}
