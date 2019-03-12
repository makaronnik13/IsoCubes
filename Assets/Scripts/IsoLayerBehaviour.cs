using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class IsoLayerBehaviour : MonoBehaviour
{
    private SpriteRenderer _rend;
    private SpriteRenderer rend
    {
        get
        {
            if (!_rend)
            {
                _rend = GetComponent<SpriteRenderer>();
            }
            return _rend;
        }
    }

    void LateUpdate()
    {
        rend.sortingOrder = (int)Camera.main.WorldToScreenPoint(rend.bounds.min).y * -1;
    }
}
