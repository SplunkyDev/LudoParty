using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtility.Base;

public class MenuUIManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
		EventManager.Instance.TriggerEvent<EventShowMenuUI>(new EventShowMenuUI(true, eGameState.Menu));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void PlayGame()
	{

	}
}
