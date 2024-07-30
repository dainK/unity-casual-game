using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameHeaven
{
    namespace Game3Match
    {
        public class EffectManager : MonoBehaviour
        {
            [SerializeField] UISpriteAnimation _orign;
            [SerializeField] UISpriteAnimation _hint;
            [SerializeField] UISpriteAnimation _change;
            List<List<UISpriteAnimation>> _images = new List<List<UISpriteAnimation>>();


            void Start()
            {
                for (int row = 0; row < Global.row; row++)
                {
                    List<UISpriteAnimation> colArr = new List<UISpriteAnimation>();
                    for (int col = 0; col < Global.col; col++)
                    {
                        UISpriteAnimation image = Instantiate(_orign, GetBlockPos(row, col), Quaternion.identity, transform);
                        image.transform.localPosition = GetBlockPos(row, col);
                        colArr.Add(image);
                    }
                    _images.Add(colArr);
                }
            }

            private Vector2 GetBlockPos(int row, int col)
            {
                return new Vector2((col - (Global.col / 2)) * (Global.size + Global.offset), (row - (Global.row / 2)) * (Global.size + Global.offset));
            }

            public void Play(int row, int col)
            {
                _images[row][col].ChangeSprite(_orign.Sprites);
                _images[row][col].PlayAnimationOnce();
            }
            public void PlayHint(int row, int col)
            {
                _images[row][col].ChangeSprite(_hint.Sprites);
                _images[row][col].PlayAnimationLoop(1f);
            }
            public void PlayChange(int row, int col)
            {
                _images[row][col].ChangeSprite(_change.Sprites);
                _images[row][col].PlayAnimationOnce();
            }
        }
    }
}
