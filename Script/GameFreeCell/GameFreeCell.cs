using DG.Tweening;
using GameHeaven.GameMemory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace GameHeaven
{
    namespace GameFreeCell
    {
        [Serializable]
        public class CardInfo
        {
            public Suit Suit;
            public Value Value;
            public Sprite Sprite;
        }
        public enum Suit { None, Hearts, Diamonds, Clubs, Spades }
        public enum Value { None, Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King }

        public class Global
        {
            static public int moveEnableCount = 5;
            static public int SuitCount = 13;
            static public int DeckCount = 52;
        }

        public enum Undo
        {
            ColToCol,
            ColToKeep,
            KeepToCol,
            KeepToKeep,
        }

        public class UndoInfo {
            public Undo UndoType;
            public List<Card> Cards;// = new List<Card>();
            public Column PrevCol;
            public Column NextCol;
            public Keep PrevKeep;
            public Keep NextKeep;
        }

        public class GameFreeCell : MonoBehaviour
        {
            [SerializeField] List<CardInfo> _infos;

            [SerializeField] List<Home> _homes;
            [SerializeField] List<Keep> _keeps;
            [SerializeField] List<Column> _columns;
            [SerializeField] Card _cardPrefab;
            List<Card> _cards = new List<Card>();
            Dictionary<Suit, List<Card>> _dic;
            [SerializeField] GraphicRaycaster _graphicRaycaster;
            Sequence _sequence = null;
            List<Card> _moveSequence = null;//CanComplete();
            List<UndoInfo> _undoInfos = new List<UndoInfo>();
            [SerializeField] Button _undoButton;
            [SerializeField] Button _autoButton;
            [SerializeField] GameObject _endText;
            void Start()
            {
                _undoInfos.Clear();
                _dic = new Dictionary<Suit, List<Card>>()
                {
                    { Suit.Hearts, new List<Card>() },
                    { Suit.Diamonds, new List<Card>() },
                    { Suit.Clubs, new List<Card>() },
                    { Suit.Spades, new List<Card>() }
                };
                for (int i = 0; i < Global.DeckCount; i++)
                {
                    int columnIndex = i % _columns.Count;
                    Column column = _columns[columnIndex];
                    Card card = Instantiate(_cardPrefab, column.transform);
                    card.Init(_infos[i], _graphicRaycaster);
                    _cards.Add(card);
                    card.OnMoveColumn += OnMoveColumn;
                    card.OnMoveKeep += OnMoveKeep;
                    card.OnMoveHome += OnMoveHome;
                    _dic[card.Suit].Add(card);
                }

                Suffle();

                _undoButton.interactable = false;
                _autoButton.interactable = false;
                _endText.SetActive(false);
            }

            public void OnReplay()
            {
                _sequence?.Kill();
                _sequence = null;
                _undoInfos.Clear();

                Global.moveEnableCount = 5;

                foreach (Home home in _homes)
                {
                    home.Clear();
                }

                Suffle();

                _undoButton.interactable = false;
                _autoButton.interactable = false;
                _endText.SetActive(false);
            }

            void UpdateState()
            {
                UpdateKeepState();
                UpdateColumnState();
                UpdateGameState();
            }


            void Suffle()
            {
                Utility.Shuffle(_cards);

                for (int i = 0; i < _cards.Count; i++)
                {
                    int columnIndex = i % _columns.Count;
                    Column column = _columns[columnIndex];
                    Card card = _cards[i];
                    card.transform.SetParent(column.transform, false);
                }

                UpdateState();

            }

            void OnMoveColumn(Card card, Column column, GameObject prev)
            {
                if (column.GetLastCard() != null)
                {
                    if (column.IsMoveEnable(card))
                    {
                        MoveColumn(card, column, prev);
                    }
                    else
                    {
                        card.Return();
                    }
                }
                else
                {
                    MoveColumn(card, column, prev);
                }

                UpdateState();
            }

            void MoveColumn(Card card, Column column, GameObject prev)
            {

                UndoInfo undoInfo = new UndoInfo();
                undoInfo.NextCol = column;
                Column prevCol = prev.GetComponent<Column>();
                if (prevCol != null)
                {
                    undoInfo.UndoType = Undo.ColToCol;
                    undoInfo.PrevCol = prevCol;
                }
                Keep prevKeep = prev.GetComponent<Keep>();
                if (prevKeep)
                {
                    undoInfo.UndoType = Undo.KeepToCol;
                    undoInfo.PrevKeep = prevKeep;
                }

                undoInfo.Cards = new List<Card>();
                Card undoCard = card;
                while (undoCard != null)
                {
                    undoInfo.Cards.Add(undoCard);
                    undoCard = undoCard.NextCard;
                }

                _undoInfos.Add(undoInfo);

                card.MoveColumn(column);
            }


            void OnMoveKeep(Card card, Keep keep, GameObject prev)
            {
                if (keep.IsCard())
                {
                    card.Return();
                }
                else
                {
                    if (card.NextCard == null)
                    {
                        keep.SetCard(card);
                        
                        Column col = prev.GetComponent<Column>();
                        UndoInfo undoInfo = new UndoInfo();
                        if (col != null)
                        {
                            undoInfo.UndoType = Undo.ColToKeep;
                            undoInfo.PrevCol = col;
                            undoInfo.NextKeep = keep;
                        }
                        Keep prevKeep = prev.GetComponent<Keep>();
                        if (prevKeep)
                        {
                            undoInfo.UndoType = Undo.KeepToKeep;
                            undoInfo.PrevKeep = prevKeep;
                            undoInfo.NextKeep = keep;
                        }
                        undoInfo.Cards = new List<Card> { card };
                        _undoInfos.Add(undoInfo);
                    }
                    else
                    {
                        card.Return();
                    }
                }
                UpdateState();
            }

            void OnMoveHome(Card card)
            {
                if (card.NextCard != null)
                {
                    card.Return();
                    return;
                }

                Home home = _homes.FirstOrDefault(h => h.Suit == card.Suit);
                if (home.IsMoveEnable(card))
                {
                    home.AddCard(card);
                    _undoInfos.Clear();
                }
                else
                {
                    card.Return();
                }
                UpdateState();
            }

            void UpdateColumnState()
            {
                foreach (Column column in _columns)
                {
                    column.UpdateState();
                }

            }
            void UpdateKeepState()
            {
                Global.moveEnableCount = 1;
                foreach (Keep keep in _keeps)
                {
                    if (!keep.IsCard())
                    {
                        Global.moveEnableCount++;
                    }
                }
            }
            void UpdateGameState()
            {

                _moveSequence = CanCompleteGame();
                
                _autoButton.interactable = _moveSequence != null && _moveSequence.Count > 0;

                _undoButton.interactable = _undoInfos.Count > 0;


                foreach (Home home in _homes)
                {
                    if (!home.IsEnd)
                        return;
                }
                End();
            }

            void End()
            {
                _undoButton.interactable = false;
                _autoButton.interactable = false;
                _endText.SetActive(true);
            }

            public void OnAutoMove()
            {
                _sequence?.Kill();
                _sequence = DOTween.Sequence();
                int z = 0;
                foreach (Card card in _moveSequence)
                {
                    Home matchingHome = _homes.FirstOrDefault(home => home.Suit == card.Suit);
                    _sequence.Append(card.transform.DOMove(matchingHome.transform.position, 0.1f).OnComplete(() => { matchingHome.AddCard(card); }));
                    
                }

                _moveSequence.Reverse();
                foreach (Card card in _moveSequence)
                {
                    card.SetInteractable(false);
                    card.transform.SetParent(_graphicRaycaster.transform, true);
                }

                _sequence.OnComplete(()=>{
                    _moveSequence?.Clear();
                    End();
                });
            }

            public void OnUndo()
            {
                UndoInfo undo = _undoInfos.Last();
                switch(undo.UndoType)
                {
                    case Undo.ColToCol:
                        foreach (Card card in undo.Cards)
                        {
                            undo.PrevCol.AddCard(card);
                        }
                        break;

                    case Undo.ColToKeep:
                        undo.PrevCol.AddCard(undo.Cards[0]);
                        //undo.PrevKeep.SetCard(undo.Cards[0]);
                        break;

                    case Undo.KeepToCol:
                        undo.PrevKeep.SetCard(undo.Cards[0]);
                        //undo.PrevCol.AddCard(undo.Cards[0]);
                        break;

                    case Undo.KeepToKeep:
                        undo.PrevKeep.SetCard(undo.Cards[0]);
                        break;
                }
                _undoInfos.Remove(undo);

                UpdateState();

            }


            public List<Card> CanCompleteGame()
            {
                List<Card> sequence = new List<Card>();

                var columns = new List<List<Card>>();
                foreach(Column column in _columns)
                {
                    columns.Add(column.GetCards().Reverse<Card>().ToList());
                }
                var keeps = _keeps.Select(keep => keep.GetCard()).Where(card => card != null).ToList();

                List<Card> nextCard = new List<Card>();
                foreach (Home home in _homes)
                {
                    if(home.Count < Global.SuitCount)
                        nextCard.Add(_dic[home.Suit][home.Count]);
                }

                while (nextCard.Count > 0)
                {
                    bool iscommon = false;
                    if (keeps.Count > 0)
                    {
                        List<Card> commonKeepCards = nextCard.Intersect(keeps).ToList();
                        if (commonKeepCards.Count > 0)
                        {
                            iscommon = true;
                            foreach (Card card in commonKeepCards)
                            {
                                nextCard.Remove(card);
                                if (card.Number < Global.SuitCount )
                                {
                                    //int nextNum = card.Number + 1;
                                    nextCard.Add(_dic[card.Suit][card.Number]);
                                }
                                keeps.Remove(card);
                                sequence.Add(card);
                                //break;
                            }
                        }
                    }

                    foreach (List<Card> colCards in columns)
                    {
                        if (colCards.Any())
                        {
                            Card card = colCards.Last();
                            if (nextCard.Contains(card))
                            {
                                nextCard.Remove(card);
                                if (card.Number < Global.SuitCount)
                                {
                                    //int nextNum = card.Number + 1;
                                    nextCard.Add(_dic[card.Suit][card.Number]);
                                }
                                colCards.Remove(card);
                                sequence.Add(card);
                                iscommon = true;
                                break;
                            }
                        }
                    }

                    if(!iscommon)
                    {
                        return null;
                    }
                }
                return sequence;
            }

        }


    }
}