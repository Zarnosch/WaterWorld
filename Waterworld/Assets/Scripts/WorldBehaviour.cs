using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WorldBehaviour : MonoBehaviour {

    public int ChunkResolution;
    public float NoiseScale;
    public Vector2 WorldStartPos;
    public Vec2i LastChunkPos;
    public Vec2i MidNoise;

    public GameObject ChunkTile;

    private GameObject[,] _activeChunks;

    private int Left = 0;
    private int Right = 2;
    private int Top = 2;
    private int Bot = 0;

    void Awake()
    {
        _activeChunks = new GameObject[3, 3];
    }

    // Use this for initialization
    void Start ()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                _activeChunks[x, y] = Instantiate(ChunkTile) as GameObject;
                _activeChunks[x, y].transform.SetParent(gameObject.transform);
                _activeChunks[x, y].transform.position = new Vector3(x * transform.lossyScale.x * 10, -10, y * transform.lossyScale.z * 10);
                _activeChunks[x, y].GetComponent<ChunkBehaviour>().CreateTexture(new Vec2i(x, y), ChunkResolution);
                _activeChunks[x, y].SetActive(true);
                _activeChunks[x, y].name = "(" + x + ", " + y + ")";
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        float defaultScale = 3 * 10; // 3 for a grid of 3x3 and 10 for the default plane size
        float parentScale = transform.lossyScale.x;
        float scaleFactor = defaultScale * parentScale;
        Vec2i newPos = new Vec2i((int) transform.position.x / 10,(int) transform.position.z / 10);
        if(LastChunkPos.X > newPos.X) // went to the left
        {
            //Debug.Log("moved Left from " + LastChunkPos.X + " to: " + newPos.X);
            MidNoise = new Vec2i(MidNoise.X + 1, MidNoise.Y);
            //Debug.Log("Active NoiseMiddlePoint: " + MidNoise.X + " " + MidNoise.Y);
            _activeChunks[Left, 0].transform.position = new Vector3(_activeChunks[Left, 0].transform.position.x + scaleFactor, _activeChunks[Left, 0].transform.position.y, _activeChunks[Left, 0].transform.position.z);
            _activeChunks[Left, 0].GetComponent<ChunkBehaviour>().CreateTexture(new Vec2i(MidNoise.X + 1, MidNoise.Y -1), ChunkResolution);
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
        else if(LastChunkPos.X < newPos.X) // went to the right
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
