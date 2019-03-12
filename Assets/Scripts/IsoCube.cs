using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class IsoCube : MonoBehaviour, IPointerEnterHandler, IDragHandler, IPointerExitHandler, IEndDragHandler
{
    private bool _moving = false;
    private Vector2Int _aimVector = Vector2Int.zero;
    private Isomap _map;
    private bool _draggable = true;
    private Dictionary<IsoCube, Vector3Int> _connectedCubes = new Dictionary<IsoCube, Vector3Int>();

    public Vector3Int Position
    {
        get
        {
            return _map.Grid.WorldToCell(transform.parent.position);
        }
    }

    public void Init(Isomap isomap)
    {
        _map = isomap;
        Connect(this);
    }

    private void Update()
    {
        if (_aimVector != Vector2Int.zero && !_moving)
        {
            //Vector3Int aimCell = Position + new Vector3Int(_aimVector.x, _aimVector.y, 0);

            if (CanMoveFigure(_aimVector))
            {
                foreach (KeyValuePair<IsoCube, Vector3Int> cube in _connectedCubes)
                {
                    cube.Key.Move(_aimVector);
                }
            }
            
        }
    }

    public void Move(Vector2Int aimVector)
    {
        Vector3Int aimCell = Position + new Vector3Int(aimVector.x, aimVector.y, 0);
        Vector3 aimPosition = _map.Grid.CellToWorld(aimCell);
        StartCoroutine(MoveTo(aimPosition));
    }

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    private bool CanMoveFigure(Vector2Int aimVector)
    {
        foreach (KeyValuePair<IsoCube, Vector3Int> cube in _connectedCubes)
        {
            Debug.Log(cube.Key.CanMove(aimVector));

            if (!cube.Key.CanMove(aimVector))
            {
                return false;
            }
        }

        return true;
    }

    private bool CanMove(Vector2Int aimVector)
    {
        Vector3Int aimCell = Position + new Vector3Int(aimVector.x, aimVector.y, 0);

        IsoCube aimCellCube = _map.GetCube(aimCell);

        if (aimCellCube == null || _connectedCubes.ContainsKey(aimCellCube))
        {
            return true;
        }

        return false;
    }

    private IEnumerator MoveTo(Vector3 aimPosition)
    {
        _moving = true;
        float t = 0.5f;
        Vector3 startPosition = transform.parent.position;
        while (t>0)
        {
            t -= Time.deltaTime;
            transform.parent.position = Vector3.Lerp(aimPosition, startPosition, t*2);
            yield return new WaitForEndOfFrame();
        }

        Vector3Int pos = _map.Grid.WorldToCell(transform.parent.position);
        if (pos.x>6 || pos.y>6)
        {
            GetComponent<SpriteRenderer>().sortingOrder = -1001;

            _map.DropCube(this, 3);
            _draggable = false;
            _aimVector = Vector2Int.zero;
            for (int i = _connectedCubes.Count-1; i >=0; i--)
            {
                _connectedCubes.ElementAt(i).Key.Disconnect(this);
            }
        }
        if (pos.x <-9 || pos.y < -9)
        {
            _map.DropCube(this, 3);
            _draggable = false;
            _aimVector = Vector2Int.zero;
            for (int i = _connectedCubes.Count - 1; i >= 0; i--)
            {
                _connectedCubes.ElementAt(i).Key.Disconnect(this);
            }
        }

        _moving = false;
    }

    private Vector2Int GetAimVector(Vector3 v)
    {
        int alpha = Mathf.RoundToInt(Vector2.Angle(Vector2.up, new Vector2(v.x, v.y)));
        if (v.x < 0)
        {
            alpha = 360 - alpha;
        }

        switch (alpha / 90)
        {
            case 0:
                return Vector2Int.right;
            case 1:
                return Vector2Int.down;
            case 2:
                return Vector2Int.left;
            case 3:
                return Vector2Int.up;
        }
        return Vector2Int.zero;
    }

    public void Disconnect(IsoCube cube)
    {
        if (!_connectedCubes.ContainsKey(cube))
        {
            return;
        }

        _connectedCubes.Remove(cube);
    }

    public void Connect(IsoCube cube)
    {
        if (_connectedCubes.ContainsKey(cube))
        {
            return;
        }

        _connectedCubes.Add(cube, Vector3Int.zero);
        if (cube!=this)
        {
            cube.Connect(this);
        }
    }

    public void Select()
    {
        transform.parent.localScale = Vector3.one * 1.1f * 0.3f;
    }

    public void Deselect()
    {
        transform.parent.localScale = Vector3.one * 0.3f;
    }

    #region Handlers

    public void OnDrag(PointerEventData eventData)
    {
        if (_draggable)
        {
            _aimVector = GetAimVector(Camera.main.ScreenToWorldPoint(eventData.position) - transform.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _aimVector = Vector2Int.zero;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_draggable)
        {
            foreach (KeyValuePair<IsoCube, Vector3Int> cube in _connectedCubes)
            {
                cube.Key.Deselect();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_draggable)
        {
            foreach (KeyValuePair<IsoCube, Vector3Int> cube in _connectedCubes)
            {
                cube.Key.Select();
            }
        }
    }

    #endregion
}
