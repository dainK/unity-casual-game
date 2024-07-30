using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameHeaven
{
    namespace GameFreeCell
    {
        public class Keep : MonoBehaviour
        {
            public void SetCard(Card card)
            {
                card.transform.SetParent(transform);
                card.transform.localPosition = Vector3.zero;
                card.SetInteractable(true);
            }

            public bool IsCard()
            {
                return transform.childCount > 0;
            }

            public Card GetCard()
            {
                return transform.childCount > 0 ? transform.GetChild(0).GetComponent<Card>() : null;
            }
        }
    }
}
