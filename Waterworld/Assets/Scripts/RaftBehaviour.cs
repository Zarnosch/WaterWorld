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

    public int[,] RaftGridInfo; // -1 not there, 0 = free, [1-11] building
    public Mesh RaftMesh;

    int raftGridInfo;

    void Awake ()
    {
        RaftGridInfo = new int[RaftMaxGridSize.X, RaftMaxGridSize.Y];
    }

	// Use this for initialization
	void Start ()
    {
        createRaftMesh();
        initTiles(RaftStartSize.X, RaftStartSize.Y);
    }

    public bool Build(Vec2i _buildPosition, Building _building)
    {
        if (_buildPosition.X < 0 || _buildPosition.X > RaftMaxGridSize.X 
            || _buildPosition.Y < 0 || _buildPosition.Y > RaftMaxGridSize.Y) 
        {
            Debug.Log(_buildPosition);
            return false;
        }
        
        raftGridInfo = RaftGridInfo[_buildPosition.X, _buildPosition.Y];

        // if empty space build raft
        if (_building == Building.Raft && raftGridInfo == -1) {
            RaftGridInfo[_buildPosition.X, _buildPosition.Y] = (int)_building;
            return true;
        }

        // if raft built building
        if (raftGridInfo == 0) {
            RaftGridInfo[_buildPosition.X, _buildPosition.Y] = (int)_building;
            return true;
        }
        
        return false;
    }

    public Vec2i LightMapPosToHeightmap(Vector2 lightMapPos)
    {
        int x = (int) lightMapPos.x;
        int y = (int) lightMapPos.y;
        return new Vec2i(x, y);
    }

    private void initTiles(int _width, int _height) {
        for (int x = 0; x < RaftMaxGridSize.X; x++) {
            for (int y = 0; y < RaftMaxGridSize.Y; y++) {
                if (x <= _width && y <= _height) {
                    RaftGridInfo[x, y] = 0;
                }
                else {
                    RaftGridInfo[x, y] = -1;
                }
            }
        }
    }

    void OnDrawGizmos() {
        if (RaftGridInfo == null) { return; }
		for (int x = 0; x < RaftGridInfo.GetLength(0); x++) {
            for (int y = 0; y < RaftGridInfo.GetLength(1); y++) {
                if (RaftGridInfo[x, y] == 0) {
                    Gizmos.color = Color.yellow;
		            Gizmos.DrawSphere(new Vector3(x, 3, y), 0.05f);
                }

                if (RaftGridInfo[x, y] > 0) {
                    Gizmos.color = Color.red;
		            Gizmos.DrawSphere(new Vector3(x, 3, y), 0.05f);
                }
            }
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

    public override string ToString() {
        return "[ " + X + ", " + Y + " ]";
    }
}
