using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace GameHeaven
{
    namespace Game3Match
    {
        public class Game3Match : TSingleton<Game3Match>
        {
            [SerializeField]  GameObject _board;
            [SerializeField] List<Block> _blockPrefabs;
            [SerializeField] TextMeshProUGUI _bestScoreText;
            [SerializeField]  TextMeshProUGUI _scoreText;
            [SerializeField]  EffectManager _effectManager;
            State _state;
            int _score;
            int _bestScore;
            [SerializeField] GameObject _bestPopup;
            [SerializeField] TextMeshProUGUI _bestPopupText;
            [SerializeField] int _addScore = 10;
            [SerializeField]  Image _timerImage; // Reference to the UI Image
            [SerializeField]  float _totalTime = 100f; // Total time in seconds
            [SerializeField]  float _addTime = 0.5f; // Total time in seconds
             float _remainingTime = 0f;

             List<List<Block>> _blockArr = new List<List<Block>>(); // row, col

            Vector2Int _hint = new Vector2Int();
            [SerializeField] Button _hintButton;
            [SerializeField] GameObject _startButton;
            [SerializeField] PauseButton _pauseButton;
            [SerializeField] GameObject _pauseImage;
            [SerializeField] GameObject _timeOverImage;
            bool _isPaused = false;
            bool _isPlaying = false;

            Sequence _changeBlockSequence = null;
            Sequence _revertBlockSequence = null;
            Sequence _moveDownSequence = null;
            private Tween _delayedCall = null;

            struct ChangeInfo
            {
                public int row1, row2;
                public int col1, col2;

                public void Set(int r1, int r2, int c1, int c2)
                {
                    row1 = r1;
                    row2 = r2;
                    col1 = c1;
                    col2 = c2;
                }
            }
            ChangeInfo _changeInfo;
            private HashSet<Vector2Int> _matches = new HashSet<Vector2Int>();

            private void Start()
            {
                _bestPopup.SetActive(false);
                _timeOverImage.SetActive(false);
                _startButton.SetActive(true);

                _remainingTime = _totalTime;
                _timerImage.fillAmount = _remainingTime / _totalTime;

                _score = 0;
                _scoreText.text = _score.ToString();

                SetPause(false);
                CheckBestScore();

                _isPlaying = false;
                ChangeState(State.None);

            }
            void CheckBestScore()
            {

                if (PlayerPrefs.HasKey("BestGame3Match"))
                {
                    _bestScore = PlayerPrefs.GetInt("BestGame3Match", 1);
                }
                else
                {
                    _bestScore = 0;
                }
                _bestScoreText.text = _bestScore.ToString();
            }

            public void OnPause()
            {
                if (_isPlaying)
                {
                    if (_pauseButton.IsPaused)
                    {
                        SetPause(false);
                    }
                    else
                    {
                        SetPause(true);
                    }
                }
            }
            public void OnResume()
            {
                if (_isPlaying)
                {
                    SetPause(false);
                }
            }

            void SetPause(bool pause)
            {
                _isPaused = pause;
                _pauseButton.SetPause(pause);
                _pauseImage.SetActive(pause);
            }

            public void OnHint()
            {
                if(_isPlaying && !_isPaused && _state == State.Idle)
                {
                    _effectManager.PlayHint(_hint.y, _hint.x);
                    _hintButton.interactable = false;
                }
            }
            public void OnStart()
            {
                OnReplay();
                _startButton.SetActive(false);
                _isPlaying = true;
                CreateBlockGrid();
                ChangeState(State.Start);
            }

            public void OnReplay()
            {
                _startButton.SetActive(true);
                _isPlaying = false;
                _timeOverImage.SetActive(false);
                _bestPopup.SetActive(false);
                SetPause(false);
                _hintButton.interactable = true;
                _bestScoreText.text = _bestScore.ToString();

                _remainingTime = _totalTime;
                _timerImage.fillAmount = _remainingTime / _totalTime;
                _score = 0;
                _scoreText.text = _score.ToString();

                Transform boardTransform = _board.transform;
                for (int i = boardTransform.childCount - 1; i >= 0; i--)
                {
                    Transform child = boardTransform.GetChild(i);
                    Destroy(child.gameObject);
                }

                ChangeState(State.None);

            }

            void Update()
            {
                if(_isPlaying)
                {
                    if(_isPaused) { return; }

                    if (_remainingTime > 0)
                    {
                        _remainingTime -= Time.deltaTime;
                        _timerImage.fillAmount = _remainingTime / _totalTime;
                    }
                    else
                    {
                        _isPlaying = false;
                        ChangeState(State.TimeOver);
                    }
                }
            }

            void AddTime()
            {
                _remainingTime += _addTime;
                if (_remainingTime > _totalTime)
                {
                    _remainingTime = _totalTime;
                }
            }

            void TimeOver()
            {
                _changeBlockSequence?.Kill();
                _changeBlockSequence = null;
                _revertBlockSequence?.Kill();
                _revertBlockSequence = null;
                _moveDownSequence?.Kill();
                _moveDownSequence = null;
                _delayedCall?.Kill();
                _delayedCall = null;

                _timeOverImage.SetActive(true);

                if (_score > _bestScore)
                {
                    _bestScore = _score;
                    _bestPopup.SetActive(true);
                    PlayerPrefs.SetInt("BestGame3Match", _bestScore);
                    _bestPopupText.text = _bestScore.ToString();
                }
            }



            private void ChangeState(State newState)
            {
                _state = newState;
                Debug.Log("State: " + _state);
                switch (_state)
                {
                    case State.None:
                        break;
                    case State.Start:
                        ChangeState(State.CheckMatch);
                        break;
                    case State.Idle:
                        _hintButton.interactable = true;
                        break;
                    case State.CheckMatch:
                        _matches = FindAllMatches();
                        if (_matches.Count > 0)
                        {
                            ChangeState(State.RemoveBlock);
                        }
                        else
                        {
                            ChangeState(State.CheckEnable);
                        }
                        break;
                    case State.CheckEnable:
                        if (IsEnableMatch())
                        {
                            ChangeState(State.Idle);
                        }
                        else
                        {
                            ChangeState(State.Reset);
                        }
                        break;
                    case State.CurrentCheckMatch:
                        _matches = FindAllMatches();

                        Block block1 = _blockArr[_changeInfo.row1][_changeInfo.col1];
                        Block block2 = _blockArr[_changeInfo.row2][_changeInfo.col2];
                        if(block1.IsSpecial() &&  block2.IsSpecial())
                        {
                            _matches.Add(new Vector2Int(_changeInfo.col1, _changeInfo.row1));
                            _matches.Add(new Vector2Int(_changeInfo.col2, _changeInfo.row2));
                        }

                        if (_matches.Count > 0)
                        {
                            ChangeState(State.RemoveBlock);
                        }
                        else
                        {
                            ChangeState(State.Revert);
                        }
                        break;
                    case State.MoveDown:
                        MoveDownBlocks();
                        break;
                    case State.Revert:
                        RevertBlock();
                        break;
                    case State.RemoveBlock:
                        RemoveMatchBlocks();
                        break;
                    case State.Reset:
                        RemoveAllBlocks();
                        //ChangeState(State.MoveDown);
                        break;
                    case State.TimeOver:
                        //Debug.Log("Game Over");
                        TimeOver();
                        break;
                }
            }

            private void CreateBlockGrid()
            {
                _blockArr.Clear();
                for (int row = 0; row < Global.row; row++)
                {
                    List<Block> blockColArr = new List<Block>();
                    for (int col = 0; col < Global.col; col++)
                    {
                        blockColArr.Add(CreateRandomBlock(row, col));
                    }
                    _blockArr.Add(blockColArr);
                }
            }

            private Vector2 GetBlockPos(int row, int col)
            {
                return new Vector2((col - (Global.col / 2)) * (Global.size + Global.offset), (row - (Global.row / 2)) * (Global.size + Global.offset));
            }


            private Block CreateRandomBlock(int row, int col)
            {
                Debug.Log(row.ToString() + "_" + col.ToString());
                //Debug.Log(GetBlockPos(row, col));
                int randomIndex = Random.Range(0, _blockPrefabs.Count);
                Block block = Instantiate(_blockPrefabs[randomIndex], _board.transform);
                block.transform.localPosition = GetBlockPos(row, col);
                block.SetPosInfo(row, col);
                return block;
            }


            public void ChangeBlock(int row1, int col1, int row2, int col2)
            {
                if (_state != State.Idle)
                    return;

                _changeInfo.Set(row1, row2, col1, col2);
                Block block1 = _blockArr[row1][col1];
                Block block2 = _blockArr[row2][col2];
                Vector3 pos1 = GetBlockPos(row1, col1);
                Vector3 pos2 = GetBlockPos(row2, col2);

                _changeBlockSequence = DOTween.Sequence();
                _changeBlockSequence.Append(block1.MoveTo(pos2));
                _changeBlockSequence.Join(block2.MoveTo(pos1));
                _changeBlockSequence.OnComplete(() =>
                {
                    Debug.Log("두 블록의 이동이 모두 완료되었습니다.");
                    block2.SetPosInfo(row1, col1);
                    block1.SetPosInfo(row2, col2);
                    _blockArr[row1][col1] = block2;
                    _blockArr[row2][col2] = block1;
                    ChangeState(State.CurrentCheckMatch);
                });
                _changeBlockSequence.Play();
            }
            private void RevertBlock()
            {
                Block block1 = _blockArr[_changeInfo.row1][_changeInfo.col1];
                Block block2 = _blockArr[_changeInfo.row2][_changeInfo.col2];
                Vector3 pos1 = GetBlockPos(_changeInfo.row1, _changeInfo.col1);
                Vector3 pos2 = GetBlockPos(_changeInfo.row2, _changeInfo.col2);

                _revertBlockSequence = DOTween.Sequence();
                _revertBlockSequence.Append(block1.MoveTo(pos2));
                _revertBlockSequence.Join(block2.MoveTo(pos1));
                _revertBlockSequence.OnComplete(() =>
                {
                    Debug.Log("되돌리기 완료되었습니다.");
                    block2.SetPosInfo(_changeInfo.row1, _changeInfo.col1);
                    block1.SetPosInfo(_changeInfo.row2, _changeInfo.col2);
                    _blockArr[_changeInfo.row1][_changeInfo.col1] = block2;
                    _blockArr[_changeInfo.row2][_changeInfo.col2] = block1;
                    ChangeState(State.Idle);
                });
                _revertBlockSequence.Play();
            }

            private void MoveDownBlocks()
            {
                List<(Block block, int newRow)> downResults = new List<(Block block, int newRow)>();

                for (int row = Global.row - 1; row >= 0; row--)
                {
                    for (int col = 0; col < Global.col; col++)
                    {
                        if (_blockArr[row][col] != null)
                        {
                            int downCount = GetDownCount(row, col);
                            if (downCount > 0)
                            {
                                int newRow = row - downCount;
                                downResults.Add((_blockArr[row][col], newRow));
                            }
                        }
                    }
                }

                for (int col = 0; col < Global.col; col++)
                {
                    int downCount = GetDownCount(Global.row, col);
                    for (int r = 0; r < downCount; r++)
                    {
                        Block block = CreateRandomBlock(Global.row + r, col);
                        int newRow = Global.row + r - downCount;
                        downResults.Add((block, newRow));
                    }
                }

                _moveDownSequence = DOTween.Sequence();

                foreach (var result in downResults)
                {
                    int newRow = result.newRow;
                    int col = result.block.Col;
                    int row = result.block.Row;
                    Block block = result.block;
                    Vector3 newPosition = GetBlockPos(newRow, col);
                    float moveDuration = Global.moveDuration * Mathf.Abs(result.newRow - row);
                    _moveDownSequence.Join(block.transform.DOLocalMove(newPosition, Global.moveDuration));
                }

                _moveDownSequence.OnComplete(() =>
                {
                    foreach (var result in downResults)
                    {
                        int newRow = result.newRow;
                        Block block = result.block;
                        int col = result.block.Col;
                        block.SetPosInfo(newRow, col);
                        _blockArr[newRow][col] = block;
                    }
                    ChangeState(State.CheckMatch);
                });

                _moveDownSequence.Play();
            }

            private int GetDownCount(int row, int col)
            {
                if (row <= 0)
                    return 0;

                int nullCount = 0;
                for (int r = row - 1; r >= 0; r--)
                {
                    if (_blockArr[r][col] == null)
                    {
                        nullCount++;
                    }
                }
                return nullCount;
            }

            private HashSet<Vector2Int> FindAllMatches()
            {
                HashSet<Vector2Int> matchResults = new HashSet<Vector2Int>();
                int[][] idGrid = GetBlockIDGrid();
                for (int row = 0; row < Global.row; row++)
                {
                    for (int col = 0; col < Global.col; col++)
                    {
                        MatchType matchType = CheckMatchType(row, col, idGrid);
                        //_blockArr[row][col].SetMatchType(matchType);
                        if (matchType != MatchType.None)
                        {
                            matchResults.Add(new Vector2Int(col, row));
                        }
                    }
                }
                return matchResults;
            }

            private int[][] GetBlockIDGrid()
            {
                int[][] idGrid = new int[_blockArr.Count][];
                for (int row = 0; row < _blockArr.Count; row++)
                {
                    idGrid[row] = new int[_blockArr[row].Count];
                    for (int col = 0; col < _blockArr[row].Count; col++)
                    {
                        idGrid[row][col] = _blockArr[row][col].GetID();
                    }
                }
                return idGrid;
            }

            private bool IsEnableMatch()
            {
                int[][] idGrid = GetBlockIDGrid();
                for (int row = 0; row < Global.row; row++)
                {
                    for (int col = 0; col < Global.col; col++)
                    {
                        if (CheckAdjacentMatch(row, col, idGrid))
                        {
                            //_hint = new Vector2Int(col, row);
                            return true;
                        }
                    }
                }
                return false;
            }

            private bool CheckAdjacentMatch(int row, int col, int[][] idGrid)
            {
                Block originBlock = _blockArr[row][col];
                int originId = originBlock.GetID();
                Vector2Int[] directions = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };

                foreach (var dir in directions)
                {
                    int newRow = row + dir.x;
                    int newCol = col + dir.y;
                    if (newRow < 0 || newRow >= Global.row || newCol < 0 || newCol >= Global.col)
                    {
                        continue;
                    }

                    Block newBlock = _blockArr[newRow][newCol];

                    if (originBlock.IsSpecial() && newBlock.IsSpecial())
                    {
                        _hint = new Vector2Int(newCol, newRow);
                        return true;
                    }

                    int newId = newBlock.GetID();
                    int[][] newGrid = GetBlockIDGrid();
                    newGrid[row][col] = newId;
                    newGrid[newRow][newCol] = originId;

                    if (CheckMatchType(row, col, newGrid) != MatchType.None)
                    {
                        _hint = new Vector2Int(newCol, newRow);
                        return true;
                    }
                }

                return false;
            }

            private MatchType CheckMatchType(int row, int col, int[][] idGrid)
            {
                int horizontalCount = 1;
                int verticalCount = 1;
                int blockId = idGrid[row][col];
                List<Vector2Int> posList = new List<Vector2Int>();

                // 가로 방향 확인 (오른쪽)
                for (int c = col + 1; c < Global.col; c++)
                {
                    if (idGrid[row][c] == blockId)
                    {
                        horizontalCount++;
                        //rightCount++;
                        posList.Add(new Vector2Int(c, row));
                    }
                    else
                    {
                        break;
                    }
                }

                // 가로 방향 확인 (왼쪽)
                for (int c = col - 1; c >= 0; c--)
                {
                    if (idGrid[row][c] == blockId)
                    {
                        horizontalCount++;
                        posList.Add(new Vector2Int(c, row));
                    }
                    else
                    {
                        break;
                    }
                }

                // 세로 방향 확인 (위)
                for (int r = row + 1; r < Global.row; r++)
                {
                    if (idGrid[r][col] == blockId)
                    {
                        verticalCount++;
                        posList.Add(new Vector2Int(col, r));
                    }
                    else
                    {
                        break;
                    }
                }

                // 세로 방향 확인 (아래)
                for (int r = row - 1; r >= 0; r--)
                {
                    if (idGrid[r][col] == blockId)
                    {
                        verticalCount++;
                        posList.Add(new Vector2Int(col, r));
                    }
                    else
                    {
                        break;
                    }
                }

                if (horizontalCount >= 3 && verticalCount >= 3)
                {
                    foreach (var pos in posList)
                    {
                        Block block = _blockArr[pos.y][pos.x];
                        if (block.MatchType != MatchType.Both)
                        {
                            block.SetMatchType(MatchType.Match3);
                        }
                    }
                    _blockArr[row][col].SetMatchType(MatchType.Both);
                    return MatchType.Both;
                }
                else if (horizontalCount >= 4)
                {
                    bool isBoth = false;
                    foreach(var pos  in posList)
                    {
                        Block block = _blockArr[pos.y][pos.x];
                        if (block.MatchType != MatchType.Both)
                        {
                            block.SetMatchType(MatchType.Match3);
                        }
                        else { 
                            isBoth = true;
                        }
                    }

                    if(isBoth)
                    {
                        _blockArr[row][col].SetMatchType(MatchType.Match3);
                    }
                    else
                    {
                        _blockArr[row][col].SetMatchType(MatchType.Horizontal);
                    }
                    return MatchType.Horizontal;
                }
                else if (verticalCount >= 4)
                {
                    bool isBoth = false;
                    foreach (var pos in posList)
                    {
                        Block block = _blockArr[pos.y][pos.x];
                        if (block.MatchType != MatchType.Both)
                        {
                            block.SetMatchType(MatchType.Match3);
                        }
                        else {
                            isBoth = true;
                        }
                    }

                    if (isBoth)
                    {
                        _blockArr[row][col].SetMatchType(MatchType.Match3);
                    }
                    else
                    {
                        _blockArr[row][col].SetMatchType(MatchType.Vertical);
                    }
                    return MatchType.Vertical;
                }
                else if (horizontalCount >= 3)
                {
                    _blockArr[row][col].SetMatchType(MatchType.Match3);
                    return MatchType.Match3;
                }
                else if (verticalCount >= 3)
                {
                    _blockArr[row][col].SetMatchType(MatchType.Match3);
                    return MatchType.Match3;
                }
                else
                {
                    _blockArr[row][col].SetMatchType(MatchType.None);
                    return MatchType.None;
                }
            }

            private void AddEffectBlocks(HashSet<Vector2Int> effectBlocks, Vector2Int blockpos)
            {

                Block block = _blockArr[blockpos.y][blockpos.x];
                if (block.BlockType == MatchType.None || block.BlockType == MatchType.Match3)
                {
                    return;
                }
                else if (block.BlockType == MatchType.Both)
                {
                    for (int r = 0; r < Global.row; r++)
                    {
                        Vector2Int posInt = new Vector2Int(blockpos.x, r);
                        if (!effectBlocks.Contains(posInt))
                        {
                            effectBlocks.Add(posInt);
                            AddEffectBlocks(effectBlocks, posInt);
                        }
                    }
                    for (int c = 0; c < Global.col; c++)
                    {
                        Vector2Int posInt = new Vector2Int(c, blockpos.y);
                        if (!effectBlocks.Contains(posInt))
                        {
                            effectBlocks.Add(posInt);
                            AddEffectBlocks(effectBlocks, posInt);
                        }
                    }
                }
                else if (block.BlockType == MatchType.Vertical)
                {
                    for (int r = 0; r < Global.row; r++)
                    {
                        Vector2Int posInt = new Vector2Int(blockpos.x, r);
                        if (!effectBlocks.Contains(posInt))
                        {
                            effectBlocks.Add(posInt);
                            AddEffectBlocks(effectBlocks, posInt);
                        }
                    }
                }
                else if (block.BlockType == MatchType.Horizontal)
                {
                    for (int c = 0; c < Global.col; c++)
                    {
                        Vector2Int posInt = new Vector2Int(c, blockpos.y);
                        if (!effectBlocks.Contains(posInt))
                        {
                            effectBlocks.Add(posInt);
                            AddEffectBlocks(effectBlocks, posInt);
                        }
                    }
                }


            }
            private void RemoveMatchBlocks()
            {
                HashSet<Vector2Int> effectBlocks = new HashSet<Vector2Int>();
                foreach (var match in _matches)
                { 
                    AddEffectBlocks(effectBlocks, match);
                }

                    foreach (var effectBlock in effectBlocks)
                {
                    _effectManager.Play(effectBlock.y, effectBlock.x);
                    _matches.Add(effectBlock);
                }

                //_matches.AddRange(effectBlocks);

                if (effectBlocks.Count>0)
                {
                    _delayedCall = DOVirtual.DelayedCall(Global.moveDuration, RemoveMatchBlock);
                    //Invoke("RemoveMatchBlock", Global.moveDuration);

                }
                else
                {
                    RemoveMatchBlock();
                }
            }

            private void RemoveMatchBlock()
            {
                foreach (var match in _matches)
                {
                    if (_blockArr[match.y][match.x] != null)
                    {
                        Block block = _blockArr[match.y][match.x];
                        if (
                           block.MatchType == MatchType.Both ||
                            block.MatchType == MatchType.Vertical ||
                            block.MatchType == MatchType.Horizontal)
                        {
                            block.ChangeBlock();
                        }
                        else
                        {
                            RemoveBlock(match.y, match.x);
                        }
                        _score += _addScore;
                        //_remainingTime += 0.5f;
                        AddTime();
                    }
                }
                _scoreText.text = _score.ToString();
                ChangeState(State.MoveDown);
            }

            private void RemoveBlock(int row, int col)
            {
                Block block = _blockArr[row][col];
                Destroy(block.gameObject);
                _blockArr[row][col] = null;

            }

            private void RemoveAllBlocks()
            {

                for (int row = 0; row < _blockArr.Count; row++)
                {
                    for (int col = 0; col < _blockArr[row].Count; col++)
                    {
                        Block block = _blockArr[row][col];
                        if(block.IsSpecial())
                        {

                        }
                        else
                        {
                            _effectManager.PlayChange(row, col);
                            RemoveBlock(row, col);
                            _blockArr[row][col] = CreateRandomBlock(row, col);
                        }
                    }
                }
                _delayedCall = DOVirtual.DelayedCall(0.5f, () => { ChangeState(State.MoveDown); });
                
            }


        }

    }
}