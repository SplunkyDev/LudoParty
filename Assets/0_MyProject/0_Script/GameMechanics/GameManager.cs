using System.Collections;
using System.Collections.Generic;
using GameUtility.Base;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private static GameManager m_instance;
	public static GameManager Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = FindObjectOfType<GameManager>();
			}
			return m_instance;
		}
	}

	private GameUtility.Base.ePlayerTurn m_enumPlayerTurn = GameUtility.Base.ePlayerTurn.None;
	public ePlayerTurn EnumPlayerTurn { get => m_enumPlayerTurn;}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
