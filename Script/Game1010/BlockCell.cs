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

                // 레이캐스트 결과를 저장할 리스트 생성
                List<RaycastResult> results = new List<RaycastResult>();

                // GraphicRaycaster를 사용하여 레이캐스트 수행
                _raycaster.Raycast(pointerEventData, results);



                // 레이캐스트가 충돌한 UI 요소 처리
                foreach (RaycastResult result in results)
                {
                    // 충돌한 UI 요소가 Cell 클래스를 가지고 있는지 확인
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