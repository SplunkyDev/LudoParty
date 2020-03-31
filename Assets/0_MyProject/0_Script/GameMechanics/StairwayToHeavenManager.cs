using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairwayToHeavenManager : MonoBehaviour
{
	//This is to set which token this path belongs to, RED, BLUE etc
	[SerializeField] private GameUtility.Base.eSafePathType m_enumSafePathType = GameUtility.Base.eSafePathType.None;
	private Dictionary<int, PathTileData> m_dicStairwayToHeaven = new Dictionary<int, PathTileData>();

    // Start is called before the first frame update
    void Start()
    {
		int index = 1;
		foreach (Transform child in transform)
		{

			PathTileData refPathTileData = child.gameObject.GetComponent<PathTileData>();
			if (refPathTileData == null)
			{
				Debug.LogError("[StairwayToHeavenManager] PathTileData not FOUND");
				return;
			}
			refPathTileData.ITileIndex = index;
			m_dicStairwayToHeaven.Add(index, refPathTileData);
			index++;
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
