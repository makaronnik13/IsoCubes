using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Figure", menuName = "Figure")]
public class Figure : ScriptableObject
{
    public List<Vector2Int> Positions;
}
