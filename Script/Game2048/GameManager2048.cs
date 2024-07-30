using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

namespace GameHeaven
{
    namespace Game2048 {

        public class GameManager2048 : MonoBehaviour
        {
            [SerializeField] private int _width = 4;
            [SerializeField] private int _height = 4;
            [SerializeField] private GameObject _board;
            [SerializeField] private GameObject _blockboard;
            [SerializeField] private Node _nodePrefab;
            [SerializeField] private Block _blockPrefab;
            [SerializeField] private List<BlockType> _types;
            [SerializeField] private float _travelTime = 0.2f;
            [SerializeField] private int _winCondition = 2048;
            [SerializeField] private GameObject _winText;
            [SerializeField] private GameObject _loseText;
            [SerializeField] private Button _resetButton;

            private List<Node> _nodes;
            private List<Block> _blocks;
            private GameState _state;
            private int _round;
            private Sequence _sequence;

            private BlockType GetBlockTypeByValue(int value) => _types.First(t => t.Value == value);

            [SerializeField] private VariableJoystick _joystick;

            public void FixedUpdate()
            {
                if (_state != GameState.WaitingInput) return;
                if (Math.Abs(_joystick.Horizontal) > 0.5 || Math.Abs(_joystick.Vertical) > 0.5)
                {
                    if (Math.Abs(_joystick.Horizontal) > Math.Abs(_joystick.Vertical))
                    {
                        if (_joystick.Horizontal > 0)
                        {
                            Shift(Vector2.right);
                        }
                        else
                        {
                            Shift(Vector2.left);
                        }
                    }
                    else
                    {
                        if (_joystick.Vertical > 0)
                        {
                            Shift(Vector2.up);
                        }
                        else
                        {
                            Shift(Vector2.down);
                        }
                    }
                    _joystick.OnPointerUp(null);
                }
            }

            // Start is called before the first frame update
            void Start()
            {
                _resetButton.onClick.AddListener(() =>
                {
                    ResetGame();
                });

                _winText.SetActive(false);
                _loseText.SetActive(false);
                ChangeState(GameState.GenerateLevel);
            }

            private void ChangeState(GameState newState)
            {
                _state = newState;
                Debug.Log(_state);

                switch (newState)
                {
                    case GameState.GenerateLevel:
                        GenerateGrid();
                        break;
                    case GameState.SpawningBlocks:
                        SpawnBlocks(_round++ == 0 ? 2 : 1);
                        break;
                    case GameState.WaitingInput:
                        break;
                    case GameState.Moving:
                        break;
                    case GameState.Win:
                        _winText.SetActive(true);
                        break;
                    case GameState.Lose:
                        _loseText.SetActive(true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
                }
            }

            private Vector2 GetNodePos(int row, int col)
            {
                return new Vector2((col - (Global.col / 2)) * (Global.size + Global.offset) + Global.size / 2, (row - (Global.row / 2)) * (Global.size + Global.offset) + Global.size / 2);
            }

            void GenerateGrid()
            {
                _round = 0;
                _nodes = new List<Node>();
                _blocks = new List<Block>();
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        var node = Instantiate(_nodePrefab,  _board.transform);
                        node.Init(x, y);
                        node.transform.localPosition = GetNodePos(y, x);
                        _nodes.Add(node);
                    }
                }

                ChangeState(GameState.SpawningBlocks);
            }

            void SpawnBlocks(int amount)
            {
                var freeNodes = _nodes.Where(n => n.OccupiedBlock == null).OrderBy(b => Random.value).ToList();

                foreach (var node in freeNodes.Take(amount))
                {
                    Spawnblock(node, Random.value > 0.8f ? 4 : 2);
                }

                if (freeNodes.Count() == 1 && !(CanMove(Vector2.left) || CanMove(Vector2.right) || CanMove(Vector2.up) || CanMove(Vector2.down)))
                {
                    ChangeState(GameState.Lose);
                }
                else
                {
                    ChangeState(_blocks.Any(b => b.Value == _winCondition) ? GameState.Win : GameState.WaitingInput);
                }
            }

            void Spawnblock(Node node, int value)
            {
                var block = Instantiate(_blockPrefab, _blockboard.transform);
                block.Init(GetBlockTypeByValue(value));
                block.SetBlock(node);
                block.transform.localPosition = GetNodePos(node.PosInt.y, node.PosInt.x);
                _blocks.Add(block);

            }

            bool CanMove(Vector2 dir)
            {
                foreach (var block in _blocks)
                {
                    var next = block.Node;
                    Vector2Int posint =  next.PosInt;
                    posint.x += (int)dir.x;
                    posint.y += (int)dir.y;

                    var possibleNode = _nodes.FirstOrDefault(n => n.PosInt == posint);
                    if (possibleNode != null)
                    {
                        if (possibleNode.OccupiedBlock == null || possibleNode.OccupiedBlock.CanMarge(block.Value))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            void Shift(Vector2 dir)
            {
                ChangeState(GameState.Moving);
                var orderedBlocks = _blocks.OrderBy(b => b.PosInt.x).ThenBy(b => b.PosInt.y).ToList();
                if (dir == Vector2.right || dir == Vector2.up) orderedBlocks.Reverse();

                foreach (var block in orderedBlocks)
                {
                    var next = block.Node;
                    do
                    {
                        block.SetBlock(next);
                        Vector2Int posint = next.PosInt;
                        posint.x += (int)dir.x;
                        posint.y += (int)dir.y;
                        var possibleNode = _nodes.FirstOrDefault(n => n.PosInt == posint);
                        if (possibleNode != null)
                        {
                            if (possibleNode.OccupiedBlock != null && possibleNode.OccupiedBlock.CanMarge(block.Value))
                            {
                                block.MargeBlock(possibleNode.OccupiedBlock);
                            }

                            else if (possibleNode.OccupiedBlock == null) next = possibleNode;
                        }
                    } while (next != block.Node);

                }

                if (_sequence != null)
                {
                    _sequence.Kill();
                    _sequence = null;
                }

                _sequence = DOTween.Sequence();

                foreach (var block in orderedBlocks)
                {
                    var posInt = block.MargingBlock != null ? block.MargingBlock.Node.PosInt : block.Node.PosInt;
                    // var movePoint = _nodes[posInt.x * 4 + posInt.y].gameObject.transform.position;
                    var movePoint = GetNodePos(posInt.y, posInt.x);
                    _sequence.Insert(0, block.transform.DOLocalMove(movePoint, _travelTime));
                }

                _sequence.OnComplete(() =>
                {
                    foreach (var block in orderedBlocks.Where(b => b.MargingBlock != null))
                    {
                        MargeBlocks(block.MargingBlock, block);
                    }

                     ChangeState(GameState.SpawningBlocks);
                });
            }

            void MargeBlocks(Block baseBlock, Block margingBlock)
            {
                Spawnblock(baseBlock.Node, baseBlock.Value * 2);
                RemoveBlock(baseBlock);
                RemoveBlock(margingBlock);
            }

            void RemoveBlock(Block block)
            {
                _blocks.Remove(block);
                Destroy(block.gameObject);
            }

            void ResetGame()
            {
                if (_sequence != null)
                {
                    _sequence.Kill();
                    _sequence = null;
                }
                _winText.SetActive(false);
                _loseText.SetActive(false);

                _round = 0;

                int count = _blocks.Count();
                for (int i = 0; i < count; i++)
                {
                    RemoveBlock(_blocks[0]);
                }

                foreach (var node in _nodes)
                {
                    node.OccupiedBlock = null;
                }

                ChangeState(GameState.SpawningBlocks);
            }


        }

    }
}
