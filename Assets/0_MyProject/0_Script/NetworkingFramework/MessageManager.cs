﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtility.Base;

public class MessageManager : MBSingleton<MessageManager>
{
    public MessageManager()
    {

    }


    private float m_fOpponentScoreFromServer = 0;
    public float OpponentScoreInServer
    {
        get => m_fOpponentScoreFromServer;
    }

    private void RegisterEvents()
    {
        EventManager.Instance.RegisterEvent<EventInsertInGameMessage>(InsertInGameMessageEvent);
        EventManager.Instance.RegisterEvent<EventReadInGameMessage>(ReadInGameMessageEvent);
    }

    private void DeregisterEvents()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.DeRegisterEvent<EventInsertInGameMessage>(InsertInGameMessageEvent);
        EventManager.Instance.DeRegisterEvent<EventReadInGameMessage>(ReadInGameMessageEvent);
    }


    private void OnEnable()
    {
        RegisterEvents();
    }

    private void OnDisable()
    {
        DeregisterEvents();

    }


    private void Start()
    {
        m_JsonObjectInGame = new JSONObject(JSONObject.Type.OBJECT);
        JSONObject arr = new JSONObject(JSONObject.Type.ARRAY);
    }

    private void InsertInGameMessageEvent(IEventBase a_Event)
    {
        EventInsertInGameMessage data = a_Event as EventInsertInGameMessage;
        if (data == null)
        {
            Debug.LogError("[MessageManager] EventInsertInGameMessage Error");
        }

        InsertInGameMessages(data.EMessageType);
    }

    private void ReadInGameMessageEvent(IEventBase a_Event)
    {
        EventReadInGameMessage data = a_Event as EventReadInGameMessage;
        if (data == null)
        {
            Debug.LogError("[MessageManager] EventReadInGameMessage Error");
        }

        ReadInGameMessage(data.StrInGameMessage);
    }

    private JSONObject m_JsonObjectInGame;
    private JSONObject arr;
    private void InsertInGameMessages(eMessageType[] a_arrMessageType)
    {
        Debug.Log("[MessageManager] InsertInGameMessages");

        m_JsonObjectInGame = null;
        arr = null;

        m_JsonObjectInGame = new JSONObject(JSONObject.Type.OBJECT);
        arr = new JSONObject(JSONObject.Type.ARRAY); ;

        for (int i = 0; i < a_arrMessageType.Length; i++)
        {
			switch (a_arrMessageType[i])
			{
				case eMessageType.None:
					break;
				case eMessageType.StartAcknowledgement:
					//Sends the player turn to opponent
					if (m_JsonObjectInGame.HasField("StartAcknowledgement"))
					{
						m_JsonObjectInGame.SetField("StartAcknowledgement", 1); //(int)EssentialDataManager.Instance.EnumOppoentPlayerTurn
					}
					else
					{
						m_JsonObjectInGame.AddField("StartAcknowledgement", 1); //(int)EssentialDataManager.Instance.EnumOppoentPlayerTurn
					}

					break;
				case eMessageType.PlayerTurn:
					break;
				case eMessageType.GameEnded:
					break;
				case eMessageType.PlayerDiceRoll:
					//Sends the player turn to opponent
					if (m_JsonObjectInGame.HasField("PlayerDiceRoll"))
					{
						m_JsonObjectInGame.SetField("PlayerDiceRoll", GameManager.Instance.ICurrentDiceValue);
					}
					else
					{
						m_JsonObjectInGame.AddField("PlayerDiceRoll", GameManager.Instance.ICurrentDiceValue);
					}
					break;
				case eMessageType.PlayerTokenSelected:
					//TODO: Manager reading opponents token 
					//if (m_JsonObjectInGame.HasField("PlayerDiceRoll"))
					//{
					//	m_JsonObjectInGame.SetField("PlayerDiceRoll", TokenManager.Instance.Curre);
					//}
					//else
					//{
					//	m_JsonObjectInGame.AddField("PlayerDiceRoll", GameManager.Instance.ICurrentDiceValue);
					//}
					break;
				case eMessageType.GameStart:
					break;
				default:
					break;
			}

		}

		//The State of the message will be checked and that field will be retrieved
		if (m_JsonObjectInGame.HasField("MessageType"))
        {
            m_JsonObjectInGame.SetField("MessageType", arr);
        }
        else
        {
            m_JsonObjectInGame.AddField("MessageType", arr);
        }

        for (int i = 0; i < a_arrMessageType.Length; i++)
        {
            arr.Add((int)a_arrMessageType[i]);
        }


        //Debug.Log("[MessageManager] InsertInGameMessages: Sending: " + m_JsonObjectInGame.Print());
        //Sending the Json to the room sever
        WarpNetworkManager.Instance.BroadCastMessageInRoom(m_JsonObjectInGame.Print());

	}

	private void ReadInGameMessage(string a_strMessage)
    {
        Debug.Log("[MessageManager] ReadInGameMessage: received: " + a_strMessage);
        m_JsonObjectInGame = null;
        arr = null;

        m_JsonObjectInGame = new JSONObject(a_strMessage);
        arr = new JSONObject(JSONObject.Type.ARRAY);

        if (m_JsonObjectInGame.HasField("MessageType"))
        {
            arr = m_JsonObjectInGame["MessageType"];
            for (int i = 0; i < arr.Count; i++)
            {

				switch ((eMessageType)arr[i].n)
				{
					case eMessageType.None:
						break;
					case eMessageType.StartAcknowledgement:
						//set dice roll value got from opponent
						if (m_JsonObjectInGame.HasField("StartAcknowledgement"))
						{
							if((int)m_JsonObjectInGame.GetField("StartAcknowledgement").i ==1)
							{

							}
							else
							{
								Debug.LogError("[MessageManager] StartAcknowledgement FOUND, INVALID DATA");
							}
						}
						else
						{
							Debug.LogError("[MessageManager] StartAcknowledgement NOT FOUND");
						}
						break;
					case eMessageType.GameEnded:
						break;
					case eMessageType.PlayerTurn:
						break;
					case eMessageType.PlayerDiceRoll:
						//set dice roll value got from opponent
						if (m_JsonObjectInGame.HasField("PlayerDiceRoll"))
						{
							EventManager.Instance.TriggerEvent<EventOpponentDiceRoll>(new EventOpponentDiceRoll((int)m_JsonObjectInGame.GetField("PlayerDiceRoll").i));
						}
						else
						{
							Debug.LogError("[MessageManager] PlayerDiceRoll NOT FOUND");
						}

						break;
					case eMessageType.PlayerTokenSelected:
						break;
					default:
						break;
				}
			}
		}
		else
        {
            Debug.LogError("[EssentialDataManager] MessageType NOT FOUND");
        }
	}

}
