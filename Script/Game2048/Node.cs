using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameHeaven
{
    namespace Game2048
    {

        public class Node : MonoBehaviour
        {
            public Block OccupiedBlock;
            public Vector2Int PosInt = new Vector2Int();
            public void Init(int x, int y)
            {
                PosInt.x = x;
                PosInt.y = y;
            }
        }

    }
}