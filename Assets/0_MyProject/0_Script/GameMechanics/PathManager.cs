using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
	private Dictionary<int,PathTileData> m_lstPathTileTypes = new Dictionary<int, PathTileData>();

    // Start is called before the first frame update
    void Start()
    {
		int index = 0;
        foreach(Transform child in transform)
		{
			m_lstPathTileTypes.Add(index,child.gameObject.GetComponent<PathTileData>());
			index++;
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
