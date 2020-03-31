using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairwayToHeavenManager : MonoBehaviour
{
	[SerializeField] private GameUtility.Base.eSafePathType m_enumSafePathType = GameUtility.Base.eSafePathType.None;
	private Dictionary<int, PathTileData> m_dicStairwayToHeaven = new Dictionary<int, PathTileData>();

    // Start is called before the first frame update
    void Start()
    {
		int index = 0;
		foreach (Transform child in transform)
		{
			m_dicStairwayToHeaven.Add(index, child.gameObject.GetComponent<PathTileData>());
			index++;
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
