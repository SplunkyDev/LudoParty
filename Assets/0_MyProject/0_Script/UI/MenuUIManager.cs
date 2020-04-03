using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtility.Base;

public class MenuUIManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {	
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void InputButton(string a_strButton)
	{
		switch (a_strButton)
		{
			case "PlayLocal":
				GameManager.Instance.BOnlineMultiplayer = false;
				GameManager.Instance.LoadToGame(1);
				break;
			case "PlayOnline":
				GameManager.Instance.BOnlineMultiplayer = true;
				GameManager.Instance.LoadToGame(1);
				break;
			case "Exit":
				Application.Quit();
				break;
		}

	}
}
