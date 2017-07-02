using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkBehaviour : MonoBehaviour {

    public Texture2D NoiseTex;

    void Awake()
    {
        
    }


    // Use this for initialization
    void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void CreateTexture(Vec2i ChunkID, int TextureResolution)
    {
        NoiseTex = new Texture2D(TextureResolution, TextureResolution);
        Color tempColor;
        for (int x = 0; x < NoiseTex.width; x++)
        {
            for (int y = 0; y < NoiseTex.height; y++)
            {
                float val = Mathf.PerlinNoise((float)ChunkID.X + (x / (float)(TextureResolution-1)), (float)ChunkID.Y + (y / (float)(TextureResolution-1)));
                //float u = (float)x / (TextureResolution - 1);
                //float v = (float)y / (TextureResolution - 1);
                //float w = 1;
                tempColor = new Color(val, val, 1);
                NoiseTex.SetPixel(NoiseTex.width - x - 1, NoiseTex.width - y - 1, tempColor);
            }
        }
        NoiseTex.Apply();
        gameObject.GetComponent<MeshRenderer>().material.mainTexture = NoiseTex;
    }

    public void CreateTexture(Texture2D _tex)
    {
        gameObject.GetComponent<MeshRenderer>().material.mainTexture = _tex;
    }
}
