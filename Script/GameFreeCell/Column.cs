using GameHeaven.GameMemory;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using static UnityEditor.Progress;
namespace GameHeaven
{
    namespace GameFreeCell
    {
        public class Column : MonoBehaviour
        {
            List<Card> _cards = new List<Card>();

            public List<Card> GetCards() { return _cards; }

            public void AddCard(Card  card)
            {
                card.transform.SetParent(transform);
                _cards.Add(card);
            }
            public void UpdateState()
            { 
                // 맨 아래가 0
                _cards.Clear();
                Card nextCard = null;
                for (int i = transform.childCount-1; i > -1; i--)
                {
                    Transform child = transform.GetChild(i);
                    Card card = child.gameObject.GetComponent<Card>();
                    _cards.Add(card );
                    card.SetInteractable(false);
                    card.SetNextCart(nextCard);
                    nextCard = card;
                }

                if (_cards.Count == 0)
                    return;
                int count = 0;
                Card last = _cards[count];
                last.SetInteractable(true);
                while (count < _cards.Count && count < Global.moveEnableCount -1)
                {
                    if(count + 1 > _cards.Count-1 )
                    {
                        return;
                    }
                    Card top = _cards[count + 1];
                    Card bottom = _cards[count];

                    if (CompareCard(top, bottom))
                    {
                        top.SetInteractable(true);
                        count++;
                    }
                    else
                    {
                        return;
                    }
                }

                
            }

            public Card GetLastCard()
            {
                if(_cards.Count == 0)
                    return null;
                return _cards[_cards.Count-1];
            }

            public bool IsMoveEnable(Card card)
            {
                Card last = _cards[0];
                return CompareCard(last, card);
            }


            bool CompareCard(Card card, Card next)
            {
                if (next.IsColor != card.IsColor)
                {
                    if (next.Number == card.Number - 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

        }

    }
}