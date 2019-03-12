using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Isomap : MonoBehaviour
{
    public GameObject CubePrefab;
    public Transform Field;
    public List<Figure> Figures;

    public Gradient Colors;

    private List<IsoCube> _cubes = new List<IsoCube>();

    public Tilemap Grid;
   

    private void Start()
    {
        for (int i = 0; i < 15; i++)
        {
            Spawn(UnityEngine.Random.Range(0, Figures.Count));
        }
    }

    public void Spawn(int i)
    {
        Vector2Int offset = new Vector2Int(UnityEngine.Random.Range(-9, 5), UnityEngine.Random.Range(-9, 5));
        if (CanSpawn(Figures[i].Positions, offset))
        {
            Spawn(Figures[i].Positions, offset);
        }        
    }

    private bool CanSpawn(List<Vector2Int> positions, Vector2Int offset)
    {
        foreach (Vector2Int v in positions)
        {
            if (GetCube(new Vector3Int(v.x+offset.x, v.y+offset.y, 0)))
            {
                return false;
            }
        }

        return true;
    }

    private void Spawn(List<Vector2Int> positions, Vector2Int offset)
    {
        List<IsoCube> newCubes = new List<IsoCube>();
    
        foreach (Vector2Int v in positions)
        {
            GameObject newCube = Instantiate(CubePrefab);
            newCube.transform.position = Grid.CellToWorld(new Vector3Int(v.x+offset.x, v.y+offset.y, 0));
            newCube.transform.SetParent(Field);
            IsoCube controller = newCube.GetComponentInChildren<IsoCube>();
            controller.Init(this);
            newCubes.Add(controller);
        }

        Color color = Colors.Evaluate(UnityEngine.Random.value);

        foreach (IsoCube cube1 in newCubes)
        {
            cube1.SetColor(color);
            foreach (IsoCube cube2 in newCubes)
            {
                cube1.Connect(cube2);
            }
            _cubes.Add(cube1);
        }
    }

    public IsoCube GetCube(Vector3Int aimCell)
    {
        return _cubes.FirstOrDefault(c => c.Position == aimCell);
    }

    public void DropCube(IsoCube isoCube, int delay)
    {
        isoCube.GetComponent<Collider2D>().enabled = false;
        isoCube.GetComponent<IsoLayerBehaviour>().enabled = false;
        isoCube.transform.parent.gameObject.AddComponent<Rigidbody>();
        _cubes.Remove(isoCube);
        Destroy(isoCube.transform.parent.gameObject, delay);
    }

    public void SpawnFigure()
    {

    }
}
