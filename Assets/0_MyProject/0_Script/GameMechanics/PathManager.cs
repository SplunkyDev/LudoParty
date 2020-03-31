using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
	[SerializeField] private List<PathTileData> m_lstPathTileTypes = new List<PathTileData>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in transform)
		{
			m_lstPathTileTypes.Add(child.gameObject.GetComponent<PathTileData>());
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
