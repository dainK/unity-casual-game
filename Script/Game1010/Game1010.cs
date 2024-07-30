using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameHeaven
{
    namespace Game10x10
    {
        [System.Serializable]
        public class BlockPos
        {
            public int x;
            public int y;
        }

        [System.Serializable]
        public class BlockPattern
        {
            [SerializeField] public int Col;
            [SerializeField] public int Row;
            [SerializeField] public List<BlockPos> BlockPos;
            [SerializeField] public Color Color;
        }

        public class Game1010 : TSingleton<Game1010>
        {
             const int _girdSize = 10;
             const float _weight = 10.0f;

            [SerializeField] Transform _gridParent;
            [SerializeField] GameObject _baseCellPrebab;
            [SerializeField] List<Transform> _blockSpawnTransforms;

             List<GameObject> _cells = new List<GameObject>();
             List<GameObject> _blocks = new List<GameObject>();

            [SerializeField] GameObject _blockPrefab;
            [SerializeField] GameObject _blockCellPrefab;
            [SerializeField]  List<BlockPattern> _patterns;
             List<float> _weights = new List<float>();
             float _totalWeight = 0;

            [SerializeField] TextMeshProUGUI _scoreText;
            [SerializeField] TextMeshProUGUI _bestScoreText;
             int _score = 0;
            int _bestScore;
            [SerializeField] GameObject _bestPopup;
            [SerializeField] TextMeshProUGUI _bestPopupText;

            [SerializeField] GameObject _startButton;
            [SerializeField] GameObject _gameOverButton;

            [SerializeField] GraphicRaycaster _graphicRaycaster;

            void Start()
            {
                InitializeWeights();
                CheckBestScore();
                _score = 0;
                _scoreText.text = _score.ToString();
                _startButton.SetActive(true);
                _gameOverButton.SetActive(false);
                _bestPopup.SetActive(false);
            }

            public void OnRestart()
            {
                _score = 0;
                _scoreText.text = _score.ToString();
                CheckBestScore();

                for (int i = 0; i < _cells.Count; i++)
                {
                    Destroy(_cells[i]);
                    _cells[i] = null;
                }
                _cells.Clear();

                Transform boardTransform = _gridParent.transform;
                for (int i = boardTransform.childCount - 1; i >= 0; i--)
                {
                    Transform child = boardTransform.GetChild(i);
                    Destroy(child.gameObject);
                }

                for (int i = 0; i < _blocks.Count; i++)
                {
                    Destroy(_blocks[i]);
                    _blocks[i] = null;
                }
                _blocks.Clear();

                _gameOverButton.SetActive(false);
                _startButton.SetActive(true);
                _bestPopup.SetActive(false);
            }

            public void OnStart()
            {
                OnRestart();
                _startButton.SetActive(false);

                GenerateGrid();
                SpawnBlocks();
            }
            void CheckBestScore()
            {
                if (PlayerPrefs.HasKey("BestGame10x10"))
                {
                    _bestScore = PlayerPrefs.GetInt("BestGame10x10", 1);
                }
                else
                {
                    _bestScore = 0;
                }
                _bestScoreText.text = _bestScore.ToString();
            }

            private void InitializeWeights()
            {
                foreach (BlockPattern pattern in _patterns)
                {
                    float weight = _weight / pattern.BlockPos.Count;
                    _weights.Add(weight);
                    _totalWeight += weight;
                }
            }

            private void GenerateGrid()
            {
                for (int i = 0; i < _girdSize; i++)
                {
                    for (int j = 0; j < _girdSize; j++)
                    {
                        GameObject cell = Instantiate(_baseCellPrebab, _gridParent);
                        _cells.Add(cell);
                        cell.GetComponent<Cell>().Init(i, j);
                    }
                }
            }

            private void SpawnBlocks()
            {
                for (int i = 0; i < _blockSpawnTransforms.Count; i++)
                {
                    SpawnBlock(i);
                }
            }

            private int RandomPatternIndex()
            {
                float randomValue = Random.Range(0, _totalWeight);
                float cumulativeWeight = 0;

                for (int i = 0; i < _patterns.Count; i++)
                {
                    cumulativeWeight += _weights[i];
                    if (randomValue < cumulativeWeight)
                    {
                        return i;
                    }
                }

                return _patterns.Count - 1;
            }

            private void SpawnBlock(int i)
            {
                int randomIndex = RandomPatternIndex();
                try
                {
                    GameObject block = Instantiate(_blockPrefab, _blockSpawnTransforms[i]);
                    block.GetComponent<Block>().Init(_patterns[randomIndex], _blockCellPrefab, _graphicRaycaster);
                    block.name = "Block_" + randomIndex;
                    _blocks.Add(block);
                }
                catch (System.IndexOutOfRangeException ex)
                {
                    Debug.LogError("An error occurred while spawning block: " + ex.Message);
                    Debug.Log("SpawnBlock Index: " + randomIndex);
                }
            }

            public void DropBlock(Block block)
            {
                _blocks.Remove(block.gameObject);
                Destroy(block.gameObject);

                CheckCompleteLines();

                if (_blocks.Count == 0)
                {
                    SpawnBlocks();
                }

                if(!CanPlaceBlock())
                {
                    GameOver();
                }
            }

            public void GameOver()
            {

                _gameOverButton.SetActive(true);
                if (_score > _bestScore)
                {
                    _bestScore = _score;
                    _bestPopup.SetActive(true);
                    PlayerPrefs.SetInt("BestGame10x10", _bestScore);
                    _bestPopupText.text = _bestScore.ToString();
                }
            }

            private void CheckCompleteLines()
            {
                int completeLineCount = 0;
                List<int> rowsToClear = new List<int>();
                List<int> colsToClear = new List<int>();

                for (int i = 0; i < _girdSize; i++)
                {
                    if (IsRowComplete(i)) rowsToClear.Add(i);
                    if (IsColComplete(i)) colsToClear.Add(i);
                }

                foreach (int row in rowsToClear) ClearRow(row);
                foreach (int col in colsToClear) ClearCol(col);

                completeLineCount = rowsToClear.Count + colsToClear.Count;
                _score += SumOfNaturalNumbers(completeLineCount) * 10;
                _scoreText.text = _score.ToString();
            }

            private bool IsRowComplete(int row)
            {
                for (int j = 0; j < _girdSize; j++)
                {
                    if (!_cells[row * _girdSize + j].GetComponent<Cell>().IsOccupied())
                    {
                        return false;
                    }
                }
                return true;
            }

            private bool IsColComplete(int col)
            {
                for (int i = 0; i < _girdSize; i++)
                {
                    if (!_cells[i * _girdSize + col].GetComponent<Cell>().IsOccupied())
                    {
                        return false;
                    }
                }
                return true;
            }

            private void ClearRow(int row)
            {
                for (int j = 0; j < _girdSize; j++)
                {
                    _cells[row * _girdSize + j].GetComponent<Cell>().SetUnoccupied();
                }
            }

            private void ClearCol(int col)
            {
                for (int i = 0; i < _girdSize; i++)
                {
                    _cells[i * _girdSize + col].GetComponent<Cell>().SetUnoccupied();
                }
            }

            public bool CanPlaceBlock()
            {
                foreach (var block in _blocks)
                {
                    Block blockComponent = block.GetComponent<Block>();

                    foreach (var cell in _cells)
                    {
                        Cell cellComponent = cell.GetComponent<Cell>();

                        if (CanPlaceBlockAtCell(blockComponent, cellComponent))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            private bool CanPlaceBlockAtCell(Block block, Cell startCell)
            {
                foreach (var blockPos in block.BlockPos)
                {
                    int targetRow = startCell.Row + blockPos.y;
                    int targetCol = startCell.Col + blockPos.x;

                    if (IsOutOfBounds(targetRow, targetCol) || _cells[targetRow * 10 + targetCol].GetComponent<Cell>().IsOccupied())
                    {
                        return false;
                    }
                }
                return true;
            }

            private bool IsOutOfBounds(int row, int col)
            {
                return row < 0 || row >= 10 || col < 0 || col >= 10;
            }

            private int SumOfNaturalNumbers(int n)
            {
                return n * (n + 1) / 2;
            }
        }
    }
}
