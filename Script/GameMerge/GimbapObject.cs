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
            public float fallSpeedMultiplier = 2.0f; // �ӵ� ���� ���

            private Rigidbody2D rb;
            private bool isFirst = true;
            //public UnityAction<Vector2,int> OnMerge;
            private bool isCheckEnable = false;
            // ������Ƽ ����
            public bool IsCheckEnable
            {
                get { return isCheckEnable; }
                set { isCheckEnable = value; }
            }

            void Start()
            {
                // Rigidbody2D ������Ʈ�� �����ɴϴ�.
                rb = GetComponent<Rigidbody2D>();
                //Invoke("CheckCollision", delay);
                //col.enabled = false;
            }

            void OnCollisionEnter2D(Collision2D collision)
            {
                // �浹�� ������Ʈ�� �±װ� ������ Ȯ��
                if (tag == collision.gameObject.tag)
                {
                    isFirst = false;
                    // �浹 ���� ���
                    Vector2 collisionPoint = collision.contacts[0].point;

                    // �±װ� "2"�� �� ����
                    //Instantiate(tag2Prefab, collisionPoint, Quaternion.identity);
                    Merge(collisionPoint);
                }

                if (isFirst && collision.gameObject.tag == "Bottom")
                {
                    isFirst = false;
                    // �浹 �� ���� �������� ���� ���մϴ�.
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
                // Rigidbody2D�� �ӵ��� ������ŵ�ϴ�.
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

