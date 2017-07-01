using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshCollider), typeof(MeshFilter), typeof(MeshRenderer))]
public class RaftBehaviour : MonoBehaviour {

    public Vec2i RaftMaxGridSize;
    public Vec2i RaftStartSize;


    private List<Vector3> _vertices;
    private List<Vector2> _uv;
    private List<int> _triangles;

    private GameManager _gm;

    private int[,] _raftGrid;
    public int[,] RaftGridInfo; // -1 not there, 0 = free, [1-11] building
    public Mesh RaftMesh;

    private Shader _raftSurfaceShader;
    private Material _raftSurfaceMaterial;

    public Texture2D _buildTexture;

    void Awake ()
    {
        _gm = GetComponent<GameManager>();
        _raftGrid = new int[RaftMaxGridSize.X, RaftMaxGridSize.Y];
        RaftGridInfo = new int[RaftMaxGridSize.X, RaftMaxGridSize.Y];

        _raftSurfaceShader = Shader.Find("Unlit/RaftUnlitShader");
        if (_raftSurfaceShader == null)
        {
            UnityEngine.Debug.LogError("RaftUnlitShader could not be loaded!");
        }
        else
        {
            _raftSurfaceMaterial = new Material(_raftSurfaceShader);
            GetComponent<MeshRenderer>().material = _raftSurfaceMaterial;
        }
        _buildTexture = new Texture2D(RaftMaxGridSize.X, RaftMaxGridSize.Y);
        _raftSurfaceMaterial.SetInt("_ResX", RaftMaxGridSize.X);
        _raftSurfaceMaterial.SetInt("_ResY", RaftMaxGridSize.Y);
    }

	// Use this for initialization
	void Start ()
    {
        createRaftMesh();
        initTiles(RaftStartSize.X, RaftStartSize.Y);
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

    public bool Build(Vec2i _buildPosition, Building _building)
    {
        if(_buildPosition.X >= 0 && _buildPosition.X < RaftMaxGridSize.X && _buildPosition.Y >= 0 && _buildPosition.Y < RaftMaxGridSize.Y)
        {
            if(_building == Building.Raft && RaftGridInfo[_buildPosition.X, _buildPosition.Y] == -1)
            {
                RaftGridInfo[_buildPosition.X, _buildPosition.Y] = (int)_building;
                _buildTexture.SetPixel(_buildPosition.X, _buildPosition.Y, new Color(1, 0, 0, 0));
                _raftSurfaceMaterial.SetTexture("_Building", _buildTexture);
                // TODO: Instantiate Raft tile
                return true;
            }
            if(RaftGridInfo[_buildPosition.X, _buildPosition.Y] == 0)
            {
                RaftGridInfo[_buildPosition.X, _buildPosition.Y] = (int)_building;
                // TODO: Instantiate Building here
                return true;
            }
            return false;
        }
        return false;
    }



    public Vec2i LightMapPosToHeightmap(Vector2 lightMapPos)
    {
        int x = (int)Mathf.Round(lightMapPos.x * RaftMaxGridSize.X);
        int y = (int)Mathf.Round(lightMapPos.y * RaftMaxGridSize.Y);
        return new Vec2i(x, y);
    }

    private void initTiles(int _width, int _height)
    {
        Vec2i center = new Vec2i(RaftMaxGridSize.X / 2, RaftMaxGridSize.Y / 2);
        for (int x = 0; x < RaftMaxGridSize.X; x++)
        {
            for (int y = 0; y < RaftMaxGridSize.Y; y++)
            {
                if (x == center.X && y == center.Y)
                {
                    RaftGridInfo[x, y] = 0;
                    _buildTexture.SetPixel(x, y, new Color(1, 0, 0, 0));
                }
                else
                {
                    RaftGridInfo[x, y] = -1;
                    _buildTexture.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }
        }
        _buildTexture.Apply();
        _raftSurfaceMaterial.SetTexture("_Building", _buildTexture);
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
