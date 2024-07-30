using DG.Tweening;
using GameHeaven.Game10x10;
using GameHeaven.GameMemory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace GameHeaven
{
    namespace GameFreeCell
    {
        public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
        {
            [SerializeField] Image _frontImage;
            [SerializeField] Image _backImage;
            [SerializeField] GameObject _button;
            CardInfo _info;
            Sequence _sequence = null;
            GraphicRaycaster _graphicRaycaster;
            Transform _prevParent;
            Card _nextCard = null;

            bool _isClickable;
            bool _isDrag;

            Vector2 _prevPosition;

            public Action<Card,Column, GameObject> OnMoveColumn;
            public Action<Card, Keep, GameObject> OnMoveKeep;
            public Action<Card> OnMoveHome;
            public bool IsColor => _info.Suit == Suit.Hearts || _info.Suit == Suit.Diamonds;
            public int Number => (int)(_info.Value);
            public Suit Suit => _info.Suit;
            public Card NextCard => _nextCard;

            public bool IsPoten;

            public void Init(CardInfo info, GraphicRaycaster graphicRaycaster)
            {
                gameObject.name = info.Suit.ToString() + "_" +info.Value.ToString();
                _info = info;
                _frontImage.sprite = info.Sprite;
                _graphicRaycaster = graphicRaycaster;
                //_parent = column;
                _isClickable = false;
                _isDrag = false;
                _button.SetActive(false);
                _frontImage.gameObject.SetActive(true);
                _backImage.gameObject.SetActive(false);
            }

            public void EnableHome()
            {
                _button.SetActive(true);
            }
            
            public void SetInteractable(bool enable)
            {
                _isClickable = enable;
            }

            public void SetNextCart(Card card)
            {
                _nextCard = card;
            }


            void FlipToFront()
            {
                _sequence?.Kill();
                _sequence = DOTween.Sequence()
                    .Append(transform.DORotate(new Vector3(0, 90, 0), 0.3f))
                    .AppendCallback(() =>
                    {
                        _frontImage.gameObject.SetActive(true);
                        _backImage.gameObject.SetActive(false);
                    })
                    .Append(transform.DORotate(new Vector3(0, 0, 0), 0.3f));
            }
            void FlipToBack()
            {
                _sequence?.Kill();
                _sequence = DOTween.Sequence()
                    .Append(transform.DORotate(new Vector3(0, 90, 0), 0.3f))
                    .AppendCallback(() =>
                    {
                        _frontImage.gameObject.SetActive(false);
                        _backImage.gameObject.SetActive(true);
                    })
                    .Append(transform.DORotate(new Vector3(0, 0, 0), 0.3f));
            }


            public void Return()
            {
                Column col = _prevParent.gameObject.GetComponent<Column>();
                if(col != null)
                {
                    Card card = this;
                    while (card != null)
                    {
                        _prevParent.gameObject.GetComponent<Column>().AddCard(card);
                        card = card.NextCard;
                    }
                    return;
                }
                Keep keep = _prevParent.gameObject.GetComponent<Keep>();
                if(keep  != null)
                {
                    transform.SetParent(_prevParent);
                    transform.localPosition = Vector2.zero;
                }
            }
            public void MoveColumn(Column column)
            {
                Card card = this;
                while (card != null)
                {
                    column.AddCard(card);
                    card = card.NextCard;
                }
            }

            public void OnBeginDrag(PointerEventData eventData)
            {
                if (!_isClickable)
                    return;

                _isDrag = true;

                _prevPosition = eventData.position;
                _prevParent = transform.parent;

                Card card = this;
                while (card != null)
                {
                    card.transform.SetParent(_graphicRaycaster.transform);
                    card = card.NextCard;
                }
            }

            public void OnDrag(PointerEventData eventData)
            {
                if (!_isDrag)
                    return;

                transform.position = eventData.position;
                //transform.localScale = Vector3.one;
                int count = 0;
                Card card = this;
                while (card != null)
                {
                    card.transform.position = eventData.position + Vector2.down * 70f * count;
                    count++;
                    card = card.NextCard;
                }
            }

            public void OnEndDrag(PointerEventData eventData)
            {
                if (!_isDrag)
                    return;

                _isDrag = false;

                if (IsDropEnable(eventData))
                {
                   
                }
                else
                {
                    Return();
                }
            }

            //public void OnPointerClick(PointerEventData eventData)
            //{
            //   if(_isClickable)
            //    {

            //    }
            //}

            bool IsDropEnable(PointerEventData eventData)
            {
                List<RaycastResult> results = new List<RaycastResult>();
                _graphicRaycaster.Raycast(eventData, results);
                foreach (RaycastResult result in results)
                {
                    var tag = result.gameObject.tag;
                    if (tag == "FreeCellBase" || tag == "FreeCellKeep" || tag == "FreeCellHome")
                    {
                        MoveCard(result.gameObject);
                        return true;
                    }
                }
                return false;
            }

            void MoveCard(GameObject o)
            {
                //Column prevCol = _prevParent.gameObject.GetComponent<Column>();
                //Debug.Log("MoveCard");
                string tag = o.tag;
                switch (tag)
                {
                    case "FreeCellBase":
                        Column column = o.GetComponent<Column>();
                        OnMoveColumn?.Invoke(this,column, _prevParent.gameObject);
                        break;
                    case "FreeCellKeep":
                        Keep keep = o.GetComponent<Keep>();
                        OnMoveKeep?.Invoke(this, keep, _prevParent.gameObject);
                        break;
                    case "FreeCellHome":
                        OnMoveHome?.Invoke(this);
                        break;
                }
            }


            

        }


    }
}