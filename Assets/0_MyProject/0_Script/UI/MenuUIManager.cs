using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtility.Base;

public class MenuUIManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

		if (GameManager.Instance.EnumGameState == eGameState.None)
			EventManager.Instance.TriggerEvent<EventShowMenuUI>(new EventShowMenuUI(true, eGameState.Menu));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void InputButton(string a_strButton)
	{
		switch (a_strButton)
		{
			case "Play":
				for (int i = 0; i < 2; i++)
				{
					PlayerData playerData = new PlayerData();
					playerData.m_enumPlayerTurn = (ePlayerTurn)i;
					playerData.m_enumPlayerToken = (ePlayerToken)(i+1);

					GameManager.Instance.SetPlayerData(playerData);
					playerData = null;
				}

				GameManager.Instance.LoadToGame(1);
				break;

			case "Exit":
				Application.Quit();
				break;
		}

	}
}
