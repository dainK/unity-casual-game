using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameHeaven
{
    namespace Game10x10
    {
        public class Cell : MonoBehaviour
        {
            BlockCell _occupiedBlock = null;

            int _col;
            int _row;

            public int Col => _col;
            public int Row => _row;


            public void Init(int row, int col)
            {
                _col = col;
                _row = row;
                gameObject.name = "Cell" + "_" + row + "_" + col;
            }

            public void SetOccupied(BlockCell block)
            {
                block.transform.SetParent(transform);
                block.transform.localPosition = Vector3.zero;
                _occupiedBlock = block;
            }

            public bool IsOccupied()
            {
                return !!_occupiedBlock;
            }

            public void SetUnoccupied()
            {
                if (_occupiedBlock != null)
                {
                    _occupiedBlock.transform.SetParent(null);
                    Destroy(_occupiedBlock.gameObject);
                    _occupiedBlock = null;
                }
            }


        }
    }
}