using System.Collections;
using UnityEngine;


namespace GameHeaven
{
    namespace MergeGame
    {

        public class Line : MonoBehaviour
        {
            private int _triggerCount = 0;
            private float _timer = 3.0f;
            private float _remainTime = 1.0f;
            private bool _isFinished = false;


            public void OnReset()
            {
                _isFinished = false;
            }

            void OnTriggerEnter2D(Collider2D other)
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("MergeGimbap"))
                {
                    Vector2 gimbapPosition = other.transform.position;
                    Vector2 linePosition = transform.position;

                    if (gimbapPosition.y < linePosition.y)
                    {
                        // 원이 아래서 충돌한 경우 실행할 로직
                        _remainTime = _timer;
                    }
                    else
                    {
                        // 원이 위에서 충돌한 경우 실행할 로직
                    }

                    _triggerCount++;
                }
            }

            void OnTriggerExit2D(Collider2D other)
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("MergeGimbap"))
                {
                    _triggerCount--;
                    if (_triggerCount == 0)
                    {
                        _remainTime = _timer; // 타이머 초기화
                    }
                }
            }

            private void Update()
            {
                if (_isFinished)
                    return;

                if (_triggerCount > 0)
                {
                    _remainTime -= Time.deltaTime;

                    if (_remainTime <= 0f)
                    {
                        // 충돌 시간이 2.5초를 넘으면 게임 오버 호출
                        _isFinished = MergeGame.Instance.CheckGameOver();

                        // 게임 오버 후, 필요한 경우 초기화s
                        // remainTime = timer; // 타이머 초기화
                    }
                }
            }



        }

    }
}
