using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameHeaven
{
    namespace Game10x10
    {
        public class BlockCell : MonoBehaviour

        {
            GraphicRaycaster _raycaster;

            Image _image;
            public GameObject _edge;

            Cell _triggerCell;

            private void Awake()
            {
                _image = GetComponent<Image>();
                //_raycaster = FindObjectOfType<GraphicRaycaster>();
            }

            public void Init(Color color, GraphicRaycaster graphicRaycaster)
            {
                _image.color = color;
                _raycaster = graphicRaycaster;
                //_sr.color = color;
            }

            public void SetEnable(bool enable)
            {
                _edge.SetActive(enable);
            }

            public void Raycasting(PointerEventData pointerEventData)
            {
                _triggerCell = null;
                pointerEventData.position = transform.position; 

                // ����ĳ��Ʈ ����� ������ ����Ʈ ����
                List<RaycastResult> results = new List<RaycastResult>();

                // GraphicRaycaster�� ����Ͽ� ����ĳ��Ʈ ����
                _raycaster.Raycast(pointerEventData, results);



                // ����ĳ��Ʈ�� �浹�� UI ��� ó��
                foreach (RaycastResult result in results)
                {
                    // �浹�� UI ��Ұ� Cell Ŭ������ ������ �ִ��� Ȯ��
                    if(result.gameObject.CompareTag("1010BaseCell"))
                        _triggerCell = result.gameObject.GetComponent<Cell>();
                }
            }

            public Cell GetCell()
            {
                return _triggerCell;
            }

            public void Drop()
            {
                _triggerCell.SetOccupied(this);
            }


        }
    }
}