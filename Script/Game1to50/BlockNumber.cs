using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;
using static UnityEngine.RuleTile.TilingRuleOutput;
using System.Reflection;

namespace GameHeaven
{
    namespace Game1to50
    {

        public class BlockNumber : MonoBehaviour
        {
            public TextMeshProUGUI _numberText;
            private int _num;
            private int _index;
            public int Num => _num;
            private GameObject _node;
            public GameObject Node => _node;

            private void Awake()
            {
                gameObject.SetActive(false);
            }
            public void Init(int index, int num, GameObject node, bool isShow)
            {
                gameObject.SetActive(true);
                _node = node;
                transform.SetParent(node.transform);
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;
                _num = num;
                _index = index;
                name = num.ToString();
                _numberText.text = name;
                if (isShow)
                {
                    Show(node);
                }
            }

            public void OnClick()
            {
                //Hide();
                Game1to50.Instance.OnClickBlock(_num);
            }


            void Show(GameObject node)
            {

                // �������� 0���� ����
                transform.localScale = Vector3.zero;

                // �������� 0���� 1�� 0.5�� ���� �ִϸ��̼�
                transform.DOScale(Vector3.one, 0.5f);
            }

            public void Hide(Action<int,GameObject> callback)
            {
                transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    //transform.SetParent(null);
                    //Destroy(gameObject);
                    callback?.Invoke(_index,_node);
                });

            }
        }
    }
}