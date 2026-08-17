using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScipt : MonoBehaviour
{


    private void OnMouseDown()
    {
        print("TileMapTest");
        SceneManager.LoadScene("TileMapTest");
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
