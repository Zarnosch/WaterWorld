using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshCollider), typeof(MeshFilter), typeof(MeshRenderer))]
public class RaftBehaviour : MonoBehaviour {

    public Vec2i RaftMaxGridSize;


    private List<Vector3> _vertices;
    private List<Vector2> _uv;
    private List<int> _triangles;

    private GameManager _gm;

    private int[,] _raftGrid;
    private int[,] _occupied; // -1 not there, 0 = free, [1-11] building
    public Mesh RaftMesh;

    //public Material RaftMaterial;

    void Awake ()
    {
        _gm = GetComponent<GameManager>();
        _raftGrid = new int[RaftMaxGridSize.X, RaftMaxGridSize.Y];
        _occupied = new int[RaftMaxGridSize.X, RaftMaxGridSize.Y];
    }

	// Use this for initialization
	void Start ()
    {
        createRaftMesh();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    //void OnRenderObject()
    //{
    //    //Graphics.DrawMesh(RaftMesh, transform.position, transform.rotation, RaftMaterial, 0, Camera.current, 0, new MaterialPropertyBlock(), true, true);
    //}

    void OnDrawGizmos()
    {
        if(_vertices != null)
        {
            //for (int x = 0; x < RaftMaxGridSize.X; x++)
            //{
            //    for (int y = 0; y < RaftMaxGridSize.Y; y++)
            //    {
            //        Gizmos.DrawSphere(_vertices[x + y * RaftMaxGridSize.Y], 0.3f);
            //    }
            //}
        }
    }

    private void createRaftMesh()
    {
        RaftMesh = new Mesh();

        int size = RaftMaxGridSize.X * RaftMaxGridSize.Y;
        _vertices = new List<Vector3>(size);
        _uv = new List<Vector2>(size);
        _triangles = new List<int>((RaftMaxGridSize.X - 1) * (RaftMaxGridSize.Y - 1) * 6);

        for (int x = 0; x < RaftMaxGridSize.X; x++)
        {
            for (int y = 0; y < RaftMaxGridSize.Y; y++)
            {
                _vertices.Add(new Vector3(x, 0, y));
                _uv.Add(new Vector2(x, y));

                if(y < RaftMaxGridSize.Y - 1 && x < RaftMaxGridSize.X - 1)
                {
                    int vertID = x + y * RaftMaxGridSize.Y;
                    _triangles.Add(vertID);
                    _triangles.Add(vertID + 1);
                    _triangles.Add(vertID + RaftMaxGridSize.Y);

                    _triangles.Add(vertID + 1);
                    _triangles.Add(vertID + 1 + RaftMaxGridSize.Y);
                    _triangles.Add(vertID + RaftMaxGridSize.Y);
                }
            }
        }

        RaftMesh.SetVertices(_vertices);
        RaftMesh.SetUVs(0, _uv);
        RaftMesh.SetTriangles(_triangles, 0);
        RaftMesh.RecalculateNormals();

        GetComponent<MeshCollider>().sharedMesh = RaftMesh;
        GetComponent<MeshFilter>().mesh = RaftMesh;
    }
}






[System.Serializable]
public class Vec2i
{
    public int X;
    public int Y;

    public Vec2i()
    {
     X = 0;
     Y = 0;
    }

    public Vec2i(int _x, int _y)
    {
        X = _x;
        Y = _y;
    }
}
