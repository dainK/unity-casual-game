using GameHeaven.GameMemory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameHeaven
{
    namespace GameFreeCell
    {
        public class Home : MonoBehaviour
        {
            public Suit Suit;

            List<Card> _cards = new List<Card>();
            public int Count => _cards.Count;
            public int NextNumber => _cards.Count > 0 ? _cards[0].Number : (int)Value.Ace;
            public bool IsEnd => _cards.Count == Global.SuitCount;


            public void Clear()
            {
                _cards.Clear();
            }

            public void AddCard(Card card)
            {
                card.SetInteractable(false);
                card.transform.SetParent(transform);
                card.transform.localPosition = Vector3.zero;
                _cards.Add(card);
            }


            public bool IsMoveEnable(Card card)
            {
                if (_cards.Count == 0)
                {
                    if (card.Number == (int)Value.Ace)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }    

                if (_cards[_cards.Count-1].Number +1 == card.Number)
                    return true;

                return false;
            }
        }

    }
}