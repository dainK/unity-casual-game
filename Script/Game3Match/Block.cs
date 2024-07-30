using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.IO;
using GameHeaven.Game2048;

namespace GameHeaven
{
    namespace Game3Match
    {

        public class Block : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
        {
            public int blockId = -1;
            private bool isDragging = false;
            private int _row = -1;
            private int _col = -1;
            private MatchType _matchType = MatchType.None;
            private MatchType _blockType = MatchType.None;

            private Vector2 touchStartPos;

            [SerializeField] private GameObject _vertical;
            [SerializeField] private GameObject _horizontal;

            public MatchType MatchType => _matchType;
            public MatchType BlockType => _blockType;

            private void Awake()
            {
                _vertical.SetActive(false);
                _horizontal.SetActive(false);
            }

            public void SetMatchType(MatchType matchType)
            {
                _matchType = matchType;
            }

            public bool IsSpecial()
            {
                return _blockType == MatchType.Both || _blockType == MatchType.Vertical || _blockType == MatchType.Horizontal;
            }

            public void ChangeBlock()
            {
                _vertical.SetActive(false);
                _horizontal.SetActive(false);
                switch (_matchType)
                {
                    case MatchType.Both:
                        _vertical.SetActive(true);
                        _horizontal.SetActive(true);
                        break;
                    case MatchType.Vertical:
                        _vertical.SetActive(true);
                        _horizontal.SetActive(false);
                        break;
                    case MatchType.Horizontal:
                        _vertical.SetActive(false);
                        _horizontal.SetActive(true);
                        break;
                }
                _blockType = _matchType;
                _matchType = MatchType.None;
            }

            public void SetPosInfo(int row, int col)
            {
                this._row = row;
                this._col = col;
                gameObject.name = $"block_{row}_{col}";
            }

            public int Col => this._col;
            public int Row => this._row;

            public int GetID()
            {
                return blockId;
            }

            public Tween MoveTo(Vector3 newPosition)
            {
                return transform.DOLocalMove(newPosition, Global.moveDuration).SetEase(Ease.InOutQuad);
            }

            public void OnPointerDown(PointerEventData eventData)
            {
                isDragging = true;
                touchStartPos = eventData.position;
            }

            public void OnDrag(PointerEventData eventData)
            {
                if (!isDragging) return;

                Vector2 loc = eventData.position;
                float deltaX = loc.x - touchStartPos.x;
                float deltaY = loc.y - touchStartPos.y;

                float absX = Mathf.Abs(deltaX);
                float absY = Mathf.Abs(deltaY);

                if (absX > 20 || absY > 20)
                {
                    Debug.Log($"Drag log: row={_row}, col={_col}, deltaX={deltaX}, deltaY={deltaY}");
                    if (absX > absY)
                    {
                        // Horizontal move
                        if (deltaX > 0 && _col < Global.col - 1)
                        {
                            // Right
                            Game3Match.Instance.ChangeBlock(_row, _col, _row, _col + 1);
                        }
                        else if (deltaX < 0 && _col > 0)
                        {
                            // Left
                            Game3Match.Instance.ChangeBlock(_row, _col, _row, _col - 1);
                        }
                    }
                    else
                    {
                        // Vertical move
                        if (deltaY > 0 && _row < Global.row - 1)
                        {
                            // Up
                            Game3Match.Instance.ChangeBlock(_row, _col, _row + 1, _col);
                        }
                        else if (deltaY < 0 && _row > 0)
                        {
                            // Down
                            Game3Match.Instance.ChangeBlock(_row, _col, _row - 1, _col);
                        }
                    }
                    isDragging = false;
                }
            }

            public void OnPointerUp(PointerEventData eventData)
            {
                isDragging = false;
            }

        }

    }
}