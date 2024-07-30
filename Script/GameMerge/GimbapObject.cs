using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;
using Vector2 = UnityEngine.Vector2;
using Random = UnityEngine.Random;
using System.ComponentModel;


namespace GameHeaven
{
    namespace MergeGame
    {

        public class GimbapObject : MonoBehaviour
        {
            public int index;
            public float fallSpeedMultiplier = 2.0f; // 속도 증가 배수

            private Rigidbody2D rb;
            private bool isFirst = true;
            //public UnityAction<Vector2,int> OnMerge;
            private bool isCheckEnable = false;
            // 프로퍼티 선언
            public bool IsCheckEnable
            {
                get { return isCheckEnable; }
                set { isCheckEnable = value; }
            }

            void Start()
            {
                // Rigidbody2D 컴포넌트를 가져옵니다.
                rb = GetComponent<Rigidbody2D>();
                //Invoke("CheckCollision", delay);
                //col.enabled = false;
            }

            void OnCollisionEnter2D(Collision2D collision)
            {
                // 충돌한 오브젝트의 태그가 같은지 확인
                if (tag == collision.gameObject.tag)
                {
                    isFirst = false;
                    // 충돌 지점 계산
                    Vector2 collisionPoint = collision.contacts[0].point;

                    // 태그가 "2"인 원 생성
                    //Instantiate(tag2Prefab, collisionPoint, Quaternion.identity);
                    Merge(collisionPoint);
                }

                if (isFirst && collision.gameObject.tag == "Bottom")
                {
                    isFirst = false;
                    // 충돌 시 랜덤 방향으로 힘을 가합니다.
                    Vector2 forceDirection = new Vector2(Random.Range(-1f, 1f), 1).normalized;
                    rb.AddForce(forceDirection * 0.1f, ForceMode2D.Impulse);

                }

            }


            public void Merge(Vector2 pos)
            {
                MergeGame.Instance.Merge(pos, index);
                Destroy(gameObject);
            }

            void Update()
            {
                // Rigidbody2D의 속도를 증가시킵니다.
                //rb.velocity += Vector2.down * fallSpeedMultiplier * Time.deltaTime;
                if (rb.gravityScale > 0f)
                {
                    //rb.velocity = rb.velocity * fallSpeedMultiplier;
                    rb.velocity += Vector2.down * fallSpeedMultiplier * Time.deltaTime;
                }
            }

        }

    }
}

