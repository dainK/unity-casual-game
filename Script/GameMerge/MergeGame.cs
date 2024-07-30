using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameHeaven
{
    namespace MergeGame
    {

        public class MergeGame : TSingleton<MergeGame>
        {
            [SerializeField] private List<GimbapType> _gimbaps; // 원 프리팹
            [SerializeField] private GameObject _parent;
            [SerializeField] private Line _line;

            [SerializeField] private Camera _mainCamera;       // 메인 카메라
            [SerializeField] private Collider2D _clickableArea; // 클릭 가능한 영역

            [SerializeField] private Transform _newPos; // 새로 생기는 위치

            [SerializeField] private TextMeshProUGUI _scoreText;
            [SerializeField] private TextMeshProUGUI _bestScoreText;
            [SerializeField] GameObject _bestPopup;
            [SerializeField] TextMeshProUGUI _bestPopupText;

            private GameObject _newObject = null;
            private int _nextIndex = -1;
            private int _score = 0;
            private int _bestScore = 0;
            private bool _isOver = false;
            private Tween _delayedCall = null;

            [SerializeField] private GameObject _startButton;
            [SerializeField] private GameObject _overButton;

            private void Start()
            {
                //Create();
                OnRestart();
            }

            public void OnRestart()
            {
                _delayedCall?.Kill();
                _delayedCall = null;

                _startButton.SetActive(true);
                _overButton.SetActive(false);
                _bestPopup.SetActive(false);

                _score = 0;
                UpdateScore();
                CheckBestScore();

                _isOver = false;
                _line.OnReset(); 


                Transform boardTransform = _parent.transform;
                for (int i = boardTransform.childCount - 1; i >= 0; i--)
                {
                    Transform child = boardTransform.GetChild(i);
                    Destroy(child.gameObject);
                }

            }

            public void OnStart()
            {
                OnRestart();
                _startButton.SetActive(false);
                Create();

            }
            void CheckBestScore()
            {

                if (PlayerPrefs.HasKey("BestGameMerge"))
                {
                    _bestScore = PlayerPrefs.GetInt("BestGameMerge", 1);
                }
                else
                {
                    _bestScore = 0;
                }
                _bestScoreText.text = _bestScore.ToString();
            }

            void Update()
            {
                if (_isOver)
                    return;

                if (Input.GetMouseButtonDown(0))
                {
                    // 클릭한 위치의 월드 좌표 구하기
                    Vector2 clickPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

                    // 클릭 위치가 콜라이더 내에 있는지 확인
                    if (_clickableArea == null || _clickableArea.OverlapPoint(clickPosition))
                    {
                        // 원 생성
                        // 0번째부터 4번째 프리팹 중 랜덤하게 선택
                        //int randomIndex = Random.Range(0, Mathf.Min(_prefabs.Count, 5));
                        //Create(clickPosition, randomIndex);
                        Drop(clickPosition);


                    }
                }
            }

            void Create()
            {
                if (_isOver)
                    return;
                Vector2 pos = _newPos.position;
                int randomIndex = Random.Range(0, Mathf.Min(_gimbaps.Count, 4));
                GameObject selectedPrefab = _gimbaps[randomIndex].Prefab;
                _newObject = Instantiate(selectedPrefab, pos, Quaternion.identity, _parent.transform);
                _newObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                _newObject.GetComponent<Collider2D>().enabled = false;
                //_newObject.GetComponent<GimbapObject>().IsCheckEnable = false;
            }

            void Drop(Vector2 pos)
            {
                if (_newObject != null)
                {
                    pos.y = _newPos.position.y;
                    _newObject.transform.position = pos;
                    _newObject.GetComponent<Rigidbody2D>().gravityScale = 1;
                    _newObject.GetComponent<Collider2D>().enabled = true;
                    _newObject = null;

                    //Invoke("Create", 0.75f);
                    _delayedCall = DOVirtual.DelayedCall(0.75f, Create);
                }
                //if (_newObject == null)
            }

            //void Create(Vector2 pos,int index)
            //{
            //    if(index < _prefabs.Count)
            //    {
            //        GameObject selectedPrefab = _prefabs[index];
            //        Instantiate(selectedPrefab, pos, Quaternion.identity, _parent.transform);
            //    }

            //}

            public void Merge(Vector2 pos, int index)
            {
                if (_nextIndex == index)
                {
                    _nextIndex = -1;
                    _score += _gimbaps[index].Score;
                    UpdateScore();
                    //Create(_clickableArea.transform.position, index + 1);
                    if (index < _gimbaps.Count)
                    {
                        GameObject selectedPrefab = _gimbaps[index + 1].Prefab;
                        GameObject newGimbap = Instantiate(selectedPrefab, pos, Quaternion.identity, _parent.transform);
                        newGimbap.GetComponent<GimbapObject>().IsCheckEnable = true;
                    }

                    //if(!_newObject)
                    //{
                    //    Invoke("Create", 0.1f);

                    //}
                }
                else
                {
                    _nextIndex = index;
                }
            }

            public void UpdateScore()
            {
                _scoreText.text = _score.ToString();
            }

            public bool CheckGameOver()
            {
                foreach (Transform child in transform)
                {
                    GameObject gimbapObject = child.gameObject;

                    Rigidbody2D rb = gimbapObject.GetComponent<Rigidbody2D>();
                    if (rb != null && rb.velocity.sqrMagnitude > 0.01f)
                    {
                        // 하나라도 움직이고 있으면
                        return false;
                    }

                }

                Debug.Log("Game Over");
                _isOver = true;

                _delayedCall?.Kill();
                _delayedCall = null;

                _overButton.SetActive(true);
                if (_score > _bestScore)
                {
                    _bestScore = _score;
                    _bestPopup.SetActive(true);
                    PlayerPrefs.SetInt("BestGameMerge", _bestScore);
                    _bestPopupText.text = _bestScore.ToString();
                }
                return true;
            }
        }

    }
}
