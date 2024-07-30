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
                        // ���� �Ʒ��� �浹�� ��� ������ ����
                        _remainTime = _timer;
                    }
                    else
                    {
                        // ���� ������ �浹�� ��� ������ ����
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
                        _remainTime = _timer; // Ÿ�̸� �ʱ�ȭ
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
                        // �浹 �ð��� 2.5�ʸ� ������ ���� ���� ȣ��
                        _isFinished = MergeGame.Instance.CheckGameOver();

                        // ���� ���� ��, �ʿ��� ��� �ʱ�ȭs
                        // remainTime = timer; // Ÿ�̸� �ʱ�ȭ
                    }
                }
            }



        }

    }
}
