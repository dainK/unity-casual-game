using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameHeaven
{
    namespace GameMemory
    {

        public class Card : MonoBehaviour
        {
            [SerializeField] Image _frontImage;  // 카드 앞면
            [SerializeField] Image _backImage;   // 카드 뒷면
            [SerializeField] Image _Image;   // 카드 이미지
            Button _button;

            private bool _isFlipped = true;    // 카드가 현재 뒤집혀 있는지 여부
            CardInfo _info;
            bool _isOpen = false;

            Sequence _sequence = null;
            public CardType CardType => _info.CardType;
            public bool IsOpen => _isOpen;

            private void Awake()
            {
                _button = GetComponent<Button>();
            }

            public void SetOpen(bool isOpen)
            {
                _isOpen = isOpen;
            }

            public void SetButtonInteractable(bool isInteractable)
            {
                _button.interactable = isInteractable;
            }

            public void Init(CardInfo info)
            {
                OnReset();

                _info = info;
                _Image.sprite = info.Sprite;
            }

            public void OnReset()
            {
                _sequence?.Kill();
                _sequence = null;
                _isFlipped = true;
                _isOpen = false;
                _frontImage.gameObject.SetActive(false);
                _backImage.gameObject.SetActive(true);
                transform.localRotation = Quaternion.identity;
                SetButtonInteractable(false);
            }
            public void OnClickOpen()
            {
                if (_isFlipped)
                {
                    FlipToFront();
                }
            }

            public void Flip()
            {
                if (_isFlipped)
                {
                    FlipToFront();
                }
                else
                {
                    FlipToBack();
                }
            }

            void FlipToFront()
            {
                _sequence?.Kill();
                _sequence = DOTween.Sequence()
                .Append(FrontSequence())
                .OnComplete(() =>
                {
                    GameMemory.Instance.CardChoice(this);
                    _isFlipped = false;
                });

            }

            public void FlipToBack()
            {
                _sequence?.Kill();
                _sequence = DOTween.Sequence()
                .Append(BackSequence())
                .OnComplete(() =>
                {
                    _isFlipped = true;
                });
            }

            Tween FrontSequence()
            {
                return DOTween.Sequence()
                    .Append(transform.DORotate(new Vector3(0, 90, 0), 0.3f))
                    .AppendCallback(() =>
                    {
                        _frontImage.gameObject.SetActive(true);
                        _backImage.gameObject.SetActive(false);
                    })
                    .Append(transform.DORotate(new Vector3(0, 0, 0), 0.3f));
            }

            Tween BackSequence()
            {
                return DOTween.Sequence()
                    .Append(transform.DORotate(new Vector3(0, 90, 0), 0.3f))
                    .AppendCallback(() =>
                    {
                        _frontImage.gameObject.SetActive(false);
                        _backImage.gameObject.SetActive(true);
                    })
                    .Append(transform.DORotate(new Vector3(0, 0, 0), 0.3f));
            }


            public Tween OnStart()
            {
                _isFlipped = true;
                _isOpen = false;
                _frontImage.gameObject.SetActive(false);
                _backImage.gameObject.SetActive(true);
                transform.localRotation = Quaternion.identity;

                _sequence?.Kill();
                _sequence = DOTween.Sequence()
                .Append(FrontSequence())
                .AppendInterval(1f)
                .Append(BackSequence());

                return _sequence;

            }
        }

    }
}