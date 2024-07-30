using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace GameHeaven
{
    namespace Game10x10
    {
        public class Block : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
        {
            float _cellSize = 100f;
            int _col;
            int _row;
            List<BlockPos> _blockPos;
            GameObject _blockCell;
            Color _color;
            int[,] _pattern;
            List<GameObject> _blockCells = new List<GameObject>();
            bool _isClickable = false;
            bool _isDrag = false;
            Tween _delayedCall = null;

            public int BlockCount => _blockCells.Count;
            public List<BlockPos> BlockPos => _blockPos;
            GraphicRaycaster _graphicRaycaster;

            private void Awake()
            {
                _isClickable = false;
                transform.localScale = Vector3.one * 0.5f;
            }


            public void Init(BlockPattern blockPattern, GameObject blockCell, GraphicRaycaster graphicRaycaster)
            {
                _col = blockPattern.Col;
                _row = blockPattern.Row;
                _blockPos = blockPattern.BlockPos;
                _color = blockPattern.Color;
                _blockCell = blockCell;
                _graphicRaycaster = graphicRaycaster;


                _pattern = new int[_row, _col];
                foreach (var p in _blockPos)
                {

                    _pattern[p.y, p.x] = 1;

                }


                CreateBlock();
            }


            void CreateBlock()
            {
                int rows = _pattern.GetLength(0);
                int cols = _pattern.GetLength(1);

                // 셀들의 중앙을 계산
                float xOffset = (cols - 1) * _cellSize / 2.0f;
                float yOffset = (rows - 1) * _cellSize / 2.0f;

                // 블록 생성
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        if (_pattern[row, col] == 1)
                        {
                            Vector3 position = new Vector3(col * _cellSize - xOffset, row * _cellSize - yOffset, 0);
                            CreateCell(position);
                        }
                    }
                }
            }

            void CreateCell(Vector3 position)
            {
                GameObject cell = Instantiate(_blockCell, transform);
                cell.transform.localPosition = position;
                cell.GetComponent<BlockCell>().Init(_color, _graphicRaycaster);
                _blockCells.Add(cell);
            }

            void Start()
            {
                _delayedCall?.Kill();
                _delayedCall = null;
                transform.localPosition = Vector2.right * 1000;
                _delayedCall = transform.DOLocalMoveX(0, 1f).OnComplete(() => { _isClickable = true; });
            }

            private void OnDestroy()
            {
                _delayedCall?.Kill();
                _delayedCall = null;
            }


            public void OnBeginDrag(PointerEventData eventData)
            {
                if (_isClickable)
                    _isDrag = true;
            }

            public void OnDrag(PointerEventData eventData)
            {
                if (!_isDrag)
                    return;

                transform.position = eventData.position;
                transform.localScale = Vector3.one;

                foreach (GameObject b in _blockCells)
                {
                    b.GetComponent<BlockCell>().Raycasting(eventData);
                }

                foreach (GameObject b in _blockCells)
                {
                    b.GetComponent<BlockCell>().SetEnable(IsDropable());
                }
            }

            public void OnEndDrag(PointerEventData eventData)
            {
                _isDrag = false;

                if (IsDropable())
                {
                    foreach (GameObject b in _blockCells)
                    {
                        b.GetComponent<BlockCell>().SetEnable(false);
                    }
                    DropBlock();
                    Game1010.Instance.DropBlock(this);
                }
                else
                {
                    Return();
                }
            }

            bool IsDropable()
            {
                foreach (GameObject b in _blockCells)
                {
                    Cell c = b.GetComponent<BlockCell>().GetCell();
                    if (!!c)
                    {
                        if (c.IsOccupied())
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }

            void DropBlock()
            {
                foreach (GameObject b in _blockCells)
                {
                    b.GetComponent<BlockCell>().Drop();
                }
            }

            public void Return()
            {
                // 원상태
                transform.localScale = Vector3.one * 0.5f;
                transform.localPosition = Vector2.zero;
            }


        }
    }
}