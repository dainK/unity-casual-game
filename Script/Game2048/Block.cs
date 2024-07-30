using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace GameHeaven
{
    namespace Game2048
    {

        public class Block : MonoBehaviour
        {
            public Node Node;
            public int Value;
            public Block MargingBlock;
            public bool Marging;
            public Vector2 PosInt = new Vector2(0, 0);
            [SerializeField] private Image _image;
            [SerializeField] private TextMeshProUGUI _text;

            public void Init(BlockType type)
            {
                Value = type.Value;
                _image.color = type.Color;
                _text.text = type.Value.ToString();
            }

            public void SetBlock(Node node)
            {
                PosInt = node.PosInt;
                if (Node != null)
                {
                    Node.OccupiedBlock = null;
                }
                Node = node;
                Node.OccupiedBlock = this;
            }

            public void MargeBlock(Block blockToMergeWith)
            {
                MargingBlock = blockToMergeWith;
                Node.OccupiedBlock = null;
                blockToMergeWith.Marging = true;
            }

            public bool CanMarge(int value) => value == Value && !Marging && MargingBlock == null;
        }

    }
}
