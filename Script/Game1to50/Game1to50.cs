using GameHeaven.Game1to50;
using GameHeaven.Game2048;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameHeaven
{
    namespace Game1to50
    {
        public class Game1to50 : TSingleton<Game1to50>
        {
            int _maxIndex = 50;
            [SerializeField] GameObject _startButton;
            [SerializeField] GameObject _board;
            [SerializeField] GameObject _nodePrefab;
            List<GameObject> _nodes = new List<GameObject>();

            [SerializeField] GameObject _blockPrefab;
            List<GameObject> _blocks = new List<GameObject>();

            List<int> _suffleNums = new List<int>();
            int _nextNum = 1;

            [SerializeField] TextMeshProUGUI _bestTimeText;
            float _bestTime;
            [SerializeField] TextMeshProUGUI _timerText;
            [SerializeField] GameObject _bestPopup;
            [SerializeField] TextMeshProUGUI _bestPopupText;
            float _elapsedTime;
            Coroutine _timerCoroutine;
            [SerializeField] GameObject _timeOver;

            public void Start()
            {
                CheckBestTime();
                _startButton.SetActive(true);
                _timeOver.SetActive(false);
                _bestPopup.SetActive(false);

                for (int i = 0; i < _maxIndex / 2; i++)
                {
                    var node = Instantiate(_nodePrefab, _board.transform);
                    _nodes.Add(node);
                }

                for (int i = 0; i < 50; i++)
                {
                    var block = Instantiate(_blockPrefab, transform);
                    _blocks.Add(block);
                }
            }

            void CheckBestTime()
            {

                if (PlayerPrefs.HasKey("BestGame1to50"))
                {
                    _bestTime = PlayerPrefs.GetFloat("BestGame1to50", 1.0f);
                    TimeSpan timeSpan = TimeSpan.FromSeconds(_bestTime);
                    _bestTimeText.text = timeSpan.ToString(@"mm\:ss\:ff");
                }
                else
                {
                    _bestTime = 3600f;
                    _bestTimeText.text = "00:00:00";
                }
            }


            public void Restart()
            {
                CheckBestTime();
                if (_timerCoroutine != null)
                    StopCoroutine(_timerCoroutine);
                _elapsedTime = 0f;
                _timerText.text = "00:00:00";
                _startButton.SetActive(true);
                _timeOver.SetActive(false);
                _bestPopup.SetActive(false);
            }


            public void GameStart()
            {
                _elapsedTime = 0f;
                _timerCoroutine = StartCoroutine(UpdateTimer());

                _startButton.SetActive(false);
                _timeOver.SetActive(false);
                _suffleNums.Clear();

                List<int> intGroup1 = new List<int>();
                List<int> intGroup2 = new List<int>();

                // 블록을 생성하고 그룹에 추가
                for (int i = 0; i < _maxIndex / 2; i++)
                {
                    intGroup1.Add(i + 1);
                    intGroup2.Add(i + 1 + _maxIndex / 2);
                }

                // 그룹을 랜덤하게 섞기

                Utility.Shuffle(intGroup1);
                Utility.Shuffle(intGroup2);

                _suffleNums.AddRange(intGroup1);
                _suffleNums.AddRange(intGroup2);

                _nextNum = 1;


                for (int i = 0; i < _maxIndex / 2; i++)
                {
                    var block = _blocks[i];
                    block.GetComponent<BlockNumber>().Init(i, _suffleNums[i], _nodes[i], false);
                }

            }

            public void OnClickBlock(int num)
            {
                if (_nextNum == num)
                {
                    BlockNumber block = FindBlockByNumber(num).GetComponent<BlockNumber>();
                    block.Hide(ShowNextBlock);
                    if (num == _maxIndex)
                    {
                        StopCoroutine(_timerCoroutine);

                        if( _elapsedTime< _bestTime)
                        {
                            BestTime();
                        } 

                    }
                    else
                    {
                        _nextNum++;
                    }
                }
                else
                {
                }
            }

            void BestTime()
            {
                _bestTime = _elapsedTime;
                _bestPopup.SetActive(true);
                PlayerPrefs.SetFloat("BestGame1to50", _bestTime);
                TimeSpan timeSpan = TimeSpan.FromSeconds(_bestTime);
                _bestPopupText.text = timeSpan.ToString(@"mm\:ss\:ff");
            }

            //public void OnClosePoup()
            //{

            //}

            void ShowNextBlock(int index, GameObject node)
            {
                int nextIndex = index + _maxIndex / 2;
                if (nextIndex < _maxIndex)
                {
                    var block = _blocks[index];
                    block.GetComponent<BlockNumber>().Init(nextIndex, _suffleNums[nextIndex], node, false);
                }

            }

            // 특정 번호의 블록 찾기
            public GameObject FindBlockByNumber(int number)
            {
                return _blocks.Find(block => block.GetComponent<BlockNumber>().Num == number);
            }

            //// Shuffle 메서드
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

            // 타이머 업데이트 코루틴
            private IEnumerator UpdateTimer()
            {
                while (true)
                {
                    _elapsedTime += Time.deltaTime;
                    TimeSpan timeSpan = TimeSpan.FromSeconds(_elapsedTime);
                    _timerText.text = timeSpan.ToString(@"mm\:ss\:ff");

                    // Check if one hour has passed
                    if (_elapsedTime >= 3600f ) // 3600 seconds = 1 hour
                    {;
                        TimeOver();
                        break; // Exit the coroutine if the time is reached
                    }

                    yield return null;
                }
            }

            void TimeOver()
            {

                if (_timerCoroutine != null)
                    StopCoroutine(_timerCoroutine);
                _elapsedTime = 0f;
                _timerText.text = "00:00:00";
                //_startButton.SetActive(true);
                _timeOver.SetActive(true);
            }
        }
    }
}