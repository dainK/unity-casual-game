using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace GameHeaven
{
    namespace GameMemory
    {
      
        public class GameMemory : TSingleton<GameMemory> {
        
            [SerializeField] List<CardInfo> _infos;
            List<CardInfo> _cardInfos = new List<CardInfo>();
            [SerializeField] GameObject _baord;
            [SerializeField] GameObject _nodePrefab;
             List<GameObject> _nodes = new List<GameObject>();
            [SerializeField] Card _cardPrefab;
             List<Card> _cards = new List<Card> ();
            int _maxCount = 16;
            [SerializeField] GameObject _startButton;
            [SerializeField] TextMeshProUGUI _startButtonText;

            Sequence _sequence = null;
            State  _state = State.None;

            Card _compareCard1 = null;
            Card _compareCard2 = null;

            enum State
            {
                None,
                Start,
                Idle,
                Compare,
                Compare_fail,
                Compare_success,
                End
            }
           
            void Start()
            {
                _cardInfos.AddRange(_infos);
                _cardInfos.AddRange(_infos);

                for (int i = 0; i < _maxCount; i++)
                {
                    GameObject node = Instantiate(_nodePrefab, _baord.transform);
                    _nodes.Add(node);
                    Card card = Instantiate(_cardPrefab, node.transform);
                    card.Init(_cardInfos[i]);
                    _cards.Add(card);
                }
                OnReset();
            }
            void ChangeState( State state )
            {
                _state = state;
                Debug.Log(state);
                switch (state)
                {
                    case State.None:
                        break;
                    case State.Start:
                        StartState();
                        break;
                    case State.Idle:
                        IdleState();
                        break;
                    case State.Compare:
                        CompareState();
                        break;
                    case State.Compare_fail:
                        CompareFailState();
                        break;
                    case State.Compare_success:
                        CompareSuccessState();
                        break;
                    case State.End:
                        _startButton.SetActive(true);
                        _startButtonText.text = "Replay";
                        break;
                }
            }
            public void OnReset()
            {
                _sequence?.Kill();
                _sequence = null;

                _startButton.SetActive(true);
                _startButtonText.text = "Game Start";

                foreach (Card card in _cards)
                {
                    card.OnReset();
                }

                ChangeState(State.None);
            }

            public void OnStart()
            {
                ChangeState( State.Start );
            }

             void StartState()
            {
                _startButton.SetActive(false);
                ShuffleCards();
                _sequence?.Kill();
                _sequence = DOTween.Sequence();
                float delay = 0f;
                foreach (Card card in _cards)
                {
                    card.SetButtonInteractable(false);
                    _sequence.Insert(delay, card.OnStart());
                    delay += 0.1f;
                }

                _sequence.OnComplete(() => {
                    ChangeState(State.Idle);
                    });
            }

            void IdleState()
            {
                int openCount = 0;
                foreach (Card card in _cards)
                {
                    if(!card.IsOpen)
                    {
                        card.SetButtonInteractable(true);
                    }
                    else
                    {
                        openCount++;
                    }
                }

                if(openCount == _maxCount)
                {
                    ChangeState(State.End);
                }
            }
            void CompareState()
            {
                foreach (Card card in _cards)
                {
                    card.SetButtonInteractable(false);
                }

                if(_compareCard1.CardType == _compareCard2.CardType)
                {
                    ChangeState(State.Compare_success);
                }
                else
                {
                    ChangeState(State.Compare_fail);
                }
            }

            void CompareSuccessState()
            {
                _compareCard1.SetOpen(true);
                _compareCard2.SetOpen(true);

                _compareCard1 = null;
                _compareCard2 = null;
                ChangeState(State.Idle);
            }

            void CompareFailState()
            {
                _compareCard1.SetOpen(false);
                _compareCard2.SetOpen(false);

                _compareCard1.FlipToBack();
                _compareCard2.FlipToBack();

                _compareCard1 = null;
                _compareCard2 = null;

                ChangeState(State.Idle);
            }

            public void CardChoice(Card card)
            {
                if (_state != State.Idle)
                    return;

                Debug.Log("choice : " + card.CardType);
                if (_compareCard1 == null)
                {
                    _compareCard1 = card;
                }
                else if (_compareCard2 == null)
                {
                    _compareCard2 = card;
                }

                if(_compareCard1 != null && _compareCard2 != null)
                {
                    ChangeState(State.Compare);
                }
            }

            void ShuffleCards()
            {
                Utility.Shuffle(_cards);
                for (int i = 0; i < _maxCount; i++)
                {
                    _cards[i].transform.SetParent(_nodes[i].transform);
                    _cards[i].transform.localPosition = Vector2.zero;
                }

            }

            //public void Shuffle<T>(List<T> list)
            //{
            //    System.Random rng = new System.Random();
            //    int n = list.Count;
            //    while (n > 1)
            //    {
            //        n--;
            //        int k = rng.Next(n + 1);
            //        T value = list[k];
            //        list[k] = list[n];
            //        list[n] = value;
            //    }
            //}
        }
    }
}