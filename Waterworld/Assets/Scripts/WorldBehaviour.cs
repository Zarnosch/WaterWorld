using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WorldBehaviour : MonoBehaviour {

    public int FluidsurfaceMeshSize;
    public int ChunkResolution;
    public Vec2i LastChunkPos;
    public Vec2i MidNoise;

    public GameObject ChunkTile;

    public Material FluidSurfaceMaterial;
    public Material FluidSimMaterial;

    private Shader _fluidSurfaceShader;
    private Shader _fluidSimShader;

    private Mesh _fluidSurfaceMesh;



    #region FluidSurface
    public Texture2D _FluidHeight;
    public Texture2D _Flux;
    public RenderTexture _fluidRTHeight;
    public RenderTexture _fluxRT;
    private RenderTexture _activeRT;
    #endregion

    #region infinity Map
    private GameObject[,] _activeChunks;
    private int Left = 0;
    private int Right = 2;
    private int Top = 2;
    private int Bot = 0;
    #endregion


    void Awake()
    {
        _activeChunks = new GameObject[3, 3];
        loadShader();
        initTextures();
        setTextures();

    }

    // Use this for initialization
    void Start ()
    {
        initInfinityMap(false);
        createFluidsurfaceMesh();
    }
	
	// Update is called once per frame
	void Update ()
    {
        updateInfinityMap();
    }

    void FixedUpdate()
    {
        _activeRT = RenderTexture.active;

        // boundary
        RenderTexture.active = _fluidRTHeight;
        FluidSimMaterial.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Points, FluidsurfaceMeshSize * FluidsurfaceMeshSize);
        Graphics.CopyTexture(_fluidRTHeight, _FluidHeight);
        // flux
        //RenderTexture.active = _fluxRT;
        //FluidSimMaterial.SetPass(1);
        //Graphics.DrawProcedural(MeshTopology.Points, FluidsurfaceMeshSize * FluidsurfaceMeshSize);
        //Graphics.CopyTexture(_fluxRT, _Flux);
        //// volume change
        //RenderTexture.active = _fluidRTHeight;
        //FluidSimMaterial.SetPass(2);
        //Graphics.DrawProcedural(MeshTopology.Points, FluidsurfaceMeshSize * FluidsurfaceMeshSize);
        //Graphics.CopyTexture(_fluidRTHeight, _FluidHeight);




        RenderTexture.active = _activeRT;

    }

    void OnRenderObject()
    {
        
        Graphics.DrawMesh(_fluidSurfaceMesh, Vector2.zero, Quaternion.identity, FluidSurfaceMaterial, 0, Camera.main);
        if(UnityEditor.SceneView.currentDrawingSceneView != null)
        {
            Graphics.DrawMesh(_fluidSurfaceMesh, Vector2.zero, Quaternion.identity, FluidSurfaceMaterial, 0, UnityEditor.SceneView.currentDrawingSceneView.camera);
        }

    }

    private void setTextures()
    {
        FluidSurfaceMaterial.SetTexture("_FluidHeight", _FluidHeight);
        FluidSimMaterial.SetTexture("_FluidHeight", _FluidHeight);
        FluidSimMaterial.SetTexture("_Flux", _Flux);
    }

    private void initTextures()
    {
        _fluidRTHeight = new RenderTexture(FluidsurfaceMeshSize, FluidsurfaceMeshSize, 16, RenderTextureFormat.ARGB32);
        _fluxRT = new RenderTexture(FluidsurfaceMeshSize, FluidsurfaceMeshSize, 16, RenderTextureFormat.ARGB32);

        _FluidHeight = new Texture2D(FluidsurfaceMeshSize, FluidsurfaceMeshSize, TextureFormat.ARGB32, false);
        _Flux = new Texture2D(FluidsurfaceMeshSize, FluidsurfaceMeshSize, TextureFormat.ARGB32, false);
        for (int x = 0; x < FluidsurfaceMeshSize; x++)
        {
            for (int y = 0; y < FluidsurfaceMeshSize; y++)
            {
                _FluidHeight.SetPixel(x, y, new Color(Mathf.PerlinNoise((float)x / FluidsurfaceMeshSize, (float) y / FluidsurfaceMeshSize), 0, 0, 0));
                _Flux.SetPixel(x, y, new Color(0, 0, 0, 0));
            }
        }
        _FluidHeight.Apply();
        _Flux.Apply();
    }

    private void loadShader()
    {
        _fluidSurfaceShader = Shader.Find("Unlit/FluidSurfaceShader");
        if (_fluidSurfaceShader == null)
        {
            UnityEngine.Debug.LogError("Fluid Render Shader could not be loaded!");
        }
        else
        {
            FluidSurfaceMaterial = new Material(_fluidSurfaceShader);
        }

        _fluidSimShader = Shader.Find("Unlit/FluidSimShader");
        if (_fluidSimShader == null)
        {
            UnityEngine.Debug.LogError("Fluid Simulation Shader could not be loaded!");
        }
        else
        {
            FluidSimMaterial = new Material(_fluidSimShader);
        }
    }

    private void createFluidsurfaceMesh()
    {
        List<Vector3> _vertices;
        List<Vector2> _uv;
        List<int> _triangles;

        _fluidSurfaceMesh = new Mesh();

        int size = FluidsurfaceMeshSize * FluidsurfaceMeshSize;
        _vertices = new List<Vector3>(size);
        _uv = new List<Vector2>(size);
        _triangles = new List<int>((FluidsurfaceMeshSize - 1) * (FluidsurfaceMeshSize - 1) * 6);

        for (int x = 0; x < FluidsurfaceMeshSize; x++)
        {
            for (int y = 0; y < FluidsurfaceMeshSize; y++)
            {
                _vertices.Add(new Vector3(x, 0, y));
                _uv.Add(new Vector2(x, y));

                if (y < FluidsurfaceMeshSize - 1 && x < FluidsurfaceMeshSize - 1)
                {
                    int vertID = x + y * FluidsurfaceMeshSize;
                    _triangles.Add(vertID);
                    _triangles.Add(vertID + 1);
                    _triangles.Add(vertID + FluidsurfaceMeshSize);

                    _triangles.Add(vertID + 1);
                    _triangles.Add(vertID + 1 + FluidsurfaceMeshSize);
                    _triangles.Add(vertID + FluidsurfaceMeshSize);
                }
            }
        }

        _fluidSurfaceMesh.SetVertices(_vertices);
        _fluidSurfaceMesh.SetUVs(0, _uv);
        _fluidSurfaceMesh.SetTriangles(_triangles, 0);
        _fluidSurfaceMesh.RecalculateNormals();
        _fluidSurfaceMesh.UploadMeshData(true);
    }

    private void initInfinityMap(bool setActive)
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                _activeChunks[x, y] = Instantiate(ChunkTile) as GameObject;
                _activeChunks[x, y].transform.SetParent(gameObject.transform);
                _activeChunks[x, y].transform.position = new Vector3(x * transform.lossyScale.x * 10, transform.position.y, y * transform.lossyScale.z * 10);
                _activeChunks[x, y].GetComponent<ChunkBehaviour>().CreateTexture(new Vec2i(x, y), ChunkResolution);
                _activeChunks[x, y].SetActive(setActive);
                _activeChunks[x, y].name = "(" + x + ", " + y + ")";
            }
        }
    }

    private void updateInfinityMap()
    {
        float defaultScale = 3 * 10; // 3 for a grid of 3x3 and 10 for the default plane size
        float parentScale = transform.lossyScale.x;
        float scaleFactor = defaultScale * parentScale;
        Vec2i newPos = new Vec2i((int)transform.position.x / 10, (int)transform.position.z / 10);
        if (LastChunkPos.X > newPos.X) // went to the left
        {
            //Debug.Log("moved Left from " + LastChunkPos.X + " to: " + newPos.X);
            MidNoise = new Vec2i(MidNoise.X + 1, MidNoise.Y);
            //Debug.Log("Active NoiseMiddlePoint: " + MidNoise.X + " " + MidNoise.Y);
            _activeChunks[Left, 0].transform.position = new Vector3(_activeChunks[Left, 0].transform.position.x + scaleFactor, _activeChunks[Left, 0].transform.position.y, _activeChunks[Left, 0].transform.position.z);
            _activeChunks[Left, 0].GetComponent<ChunkBehaviour>().CreateTexture(new Vec2i(MidNoise.X + 1, MidNoise.Y - 1), ChunkResolution);
            _activeChunks[Left, 0].name = "(" + (MidNoise.X + 1) + ", " + (MidNoise.Y - 1) + ")";

            _activeChunks[Left, 1].transform.position = new Vector3(_activeChunks[Left, 1].transform.position.x + scaleFactor, _activeChunks[Left, 1].transform.position.y, _activeChunks[Left, 1].transform.position.z);
            _activeChunks[Left, 1].GetComponent<ChunkBehaviour>().CreateTexture(new Vec2i(MidNoise.X + 1, MidNoise.Y), ChunkResolution);
            _activeChunks[Left, 1].name = "(" + (MidNoise.X + 1) + ", " + (MidNoise.Y) + ")";

            _activeChunks[Left, 2].transform.position = new Vector3(_activeChunks[Left, 2].transform.position.x + scaleFactor, _activeChunks[Left, 2].transform.position.y, _activeChunks[Left, 2].transform.position.z);
            _activeChunks[Left, 2].GetComponent<ChunkBehaviour>().CreateTexture(new Vec2i(MidNoise.X + 1, MidNoise.Y + 1), ChunkResolution);
            _activeChunks[Left, 2].name = "(" + (MidNoise.X + 1) + ", " + (MidNoise.Y + 1) + ")";

            Right = Left;
            Left = (Left + 1) % 3;
        }
        else if (LastChunkPos.X < newPos.X) // went to the right
        {
            //Debug.Log("moved Right from " + LastChunkPos.X + " to: " + newPos.X);
            MidNoise = new Vec2i(MidNoise.X - 1, MidNoise.Y);
            //Debug.Log("Active NoiseMiddlePoint: " + MidNoise.X + " " + MidNoise.Y);
            _activeChunks[Right, 0].transform.position = new Vector3(_activeChunks[Right, 0].transform.position.x - scaleFactor, _activeChunks[Right, 0].transform.position.y, _activeChunks[Right, 0].transform.position.z);
            _activeChunks[Right, 0].GetComponent<ChunkBehaviour>().CreateTexture(new Vec2i(MidNoise.X - 1, MidNoise.Y - 1), ChunkResolution);
            _activeChunks[Right, 0].name = "(" + (MidNoise.X - 1) + ", " + (MidNoise.Y - 1) + ")";

            _activeChunks[Right, 1].transform.position = new Vector3(_activeChunks[Right, 1].transform.position.x - scaleFactor, _activeChunks[Right, 1].transform.position.y, _activeChunks[Right, 1].transform.position.z);
            _activeChunks[Right, 1].GetComponent<ChunkBehaviour>().CreateTexture(new Vec2i(MidNoise.X - 1, MidNoise.Y), ChunkResolution);
            _activeChunks[Right, 1].name = "(" + (MidNoise.X - 1) + ", " + (MidNoise.Y) + ")";

            _activeChunks[Right, 2].transform.position = new Vector3(_activeChunks[Right, 2].transform.position.x - scaleFactor, _activeChunks[Right, 2].transform.position.y, _activeChunks[Right, 2].transform.position.z);
            _activeChunks[Right, 2].GetComponent<ChunkBehaviour>().CreateTexture(new Vec2i(MidNoise.X - 1, MidNoise.Y + 1), ChunkResolution);
            _activeChunks[Right, 2].name = "(" + (MidNoise.X - 1) + ", " + (MidNoise.Y + 1) + ")";

            Left = Right;
            Right = (Right + 2) % 3;

        }
        else if (LastChunkPos.Y > newPos.Y) // went down
        {
            //Debug.Log("moved Down from " + LastChunkPos.Y + " to: " + newPos.Y);
            MidNoise = new Vec2i(MidNoise.X, MidNoise.Y + 1);
            //Debug.Log("Active NoiseMiddlePoint: " + MidNoise.X + " " + MidNoise.Y);
            _activeChunks[0, Bot].transform.position = new Vector3(_activeChunks[0, Bot].transform.position.x, _activeChunks[0, Bot].transform.position.y, _activeChunks[0, Bot].transform.position.z + scaleFactor);
            _activeChunks[0, Bot].GetComponent<ChunkBehaviour>().CreateTexture(new Vec2i(MidNoise.X - 1, MidNoise.Y + 1), ChunkResolution);
            _activeChunks[0, Bot].name = "(" + (MidNoise.X - 1) + ", " + (MidNoise.Y + 1) + ")";

            _activeChunks[1, Bot].transform.position = new Vector3(_activeChunks[1, Bot].transform.position.x, _activeChunks[1, Bot].transform.position.y, _activeChunks[1, Bot].transform.position.z + scaleFactor);
            _activeChunks[1, Bot].GetComponent<ChunkBehaviour>().CreateTexture(new Vec2i(MidNoise.X, MidNoise.Y + 1), ChunkResolution);
            _activeChunks[1, Bot].name = "(" + (MidNoise.X) + ", " + (MidNoise.Y + 1) + ")";

            _activeChunks[2, Bot].transform.position = new Vector3(_activeChunks[2, Bot].transform.position.x, _activeChunks[2, Bot].transform.position.y, _activeChunks[2, Bot].transform.position.z + scaleFactor);
            _activeChunks[2, Bot].GetComponent<ChunkBehaviour>().CreateTexture(new Vec2i(MidNoise.X + 1, MidNoise.Y + 1), ChunkResolution);
            _activeChunks[2, Bot].name = "(" + (MidNoise.X + 1) + ", " + (MidNoise.Y + 1) + ")";

            Top = Bot;
            Bot = (Bot + 1) % 3;
        }
        else if (LastChunkPos.Y < newPos.Y) // went top
        {
            //Debug.Log("moved Top from " + LastChunkPos.Y + " to: " + newPos.Y);
            MidNoise = new Vec2i(MidNoise.X, MidNoise.Y - 1);
            //Debug.Log("Active NoiseMiddlePoint: " + MidNoise.X + " " + MidNoise.Y);
            _activeChunks[0, Top].transform.position = new Vector3(_activeChunks[0, Top].transform.position.x, _activeChunks[0, Top].transform.position.y, _activeChunks[0, Top].transform.position.z - scaleFactor);
            _activeChunks[0, Top].GetComponent<ChunkBehaviour>().CreateTexture(new Vec2i(MidNoise.X - 1, MidNoise.Y - 1), ChunkResolution);
            _activeChunks[0, Top].name = "(" + (MidNoise.X - 1) + ", " + (MidNoise.Y - 1) + ")";

            _activeChunks[1, Top].transform.position = new Vector3(_activeChunks[1, Top].transform.position.x, _activeChunks[1, Top].transform.position.y, _activeChunks[1, Top].transform.position.z - scaleFactor);
            _activeChunks[1, Top].GetComponent<ChunkBehaviour>().CreateTexture(new Vec2i(MidNoise.X, MidNoise.Y - 1), ChunkResolution);
            _activeChunks[1, Top].name = "(" + (MidNoise.X) + ", " + (MidNoise.Y - 1) + ")";

            _activeChunks[2, Top].transform.position = new Vector3(_activeChunks[2, Top].transform.position.x, _activeChunks[2, Top].transform.position.y, _activeChunks[2, Top].transform.position.z - scaleFactor);
            _activeChunks[2, Top].GetComponent<ChunkBehaviour>().CreateTexture(new Vec2i(MidNoise.X + 1, MidNoise.Y - 1), ChunkResolution);
            _activeChunks[2, Top].name = "(" + (MidNoise.X + 1) + ", " + (MidNoise.Y - 1) + ")";

            Bot = Top;
            Top = (Bot + 2) % 3;
        }

        LastChunkPos = newPos;
    }
}
