using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtility.Base;

#region WARP
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using com.shephertz.app42.gaming.multiplayer.client.command;
using com.shephertz.app42.gaming.multiplayer.client.listener;
#endregion



public class WarpNetworkManager : MBSingleton<WarpNetworkManager>
{
	private WarpClient m_warpClient;
	private WarpListerner m_warpListener = new WarpListerner();
	private bool m_bIsConnected = false;
	public bool ConnectionEstablished
	{
		get
		{
			return m_bIsConnected;
		}

		set
		{
			m_bIsConnected = value;
		}
	}

	private string m_strPlayerName;
	public string PlayerName
	{
		set
		{
			m_strPlayerName = value;
		}
		get
		{
			return m_strPlayerName;
		}
	}

	public float m_fInterval = 0.1f;
	private float m_fTimer = 0;

	private int m_iSessionID = 0;
	private bool m_bApplictaionQuit = false;
	public bool ApplicationQuiting
	{
		get
		{
			return m_bApplictaionQuit;
		}
	}

	private Dictionary<int, string> m_dicPlayerInRoom = new Dictionary<int, string>();
	private int m_iNumberOfPlayersInRoom = 0;

	//Api refernce: http://appwarp.shephertz.com/game-development-center/csharp-api-reference/
	//Callbacks: http://appwarp.shephertz.com/game-development-center/windows-game-developers-home/windows-client-listener/#connectionrequestlistener
	// Start is called before the first frame update

	private void RegisterEvents()
	{
		EventManager.Instance.RegisterEvent<EventInitializeNetworkApi>(Init);
		EventManager.Instance.RegisterEvent<EventConnectToServer>(ConnectedToServer);
		EventManager.Instance.RegisterEvent<EventJoinRoom>(JoinRoom);
		EventManager.Instance.RegisterEvent<EventReconnectServer>(ReconnectToServer);
		EventManager.Instance.RegisterEvent<EventSubscribeRoom>(SubscribeRoom);
	}

	private void DeregisterEvents()
	{
		if (EventManager.Instance == null)
			return;

		EventManager.Instance.DeRegisterEvent<EventInitializeNetworkApi>(Init);
		EventManager.Instance.DeRegisterEvent<EventConnectToServer>(ConnectedToServer);
		EventManager.Instance.DeRegisterEvent<EventSubscribeRoom>(SubscribeRoom);
		EventManager.Instance.DeRegisterEvent<EventJoinRoom>(JoinRoom);
		EventManager.Instance.DeRegisterEvent<EventReconnectServer>(ReconnectToServer);
		EventManager.Instance.DeRegisterEvent<EventSubscribeRoom>(SubscribeRoom);
	}

	private void Awake()
    {
	
	}

	private void OnEnable()
	{
		RegisterEvents();
	}

	private void OnDisable()
	{
		DeregisterEvents();
	}

	private void Init(IEventBase a_Event)
	{

		EventInitializeNetworkApi data = a_Event as EventInitializeNetworkApi;
		if(data == null)
		{
			Debug.LogError("[WarpNetworkManager] EventInitializeNetworkApi Error");
			return;
		}

		WarpClient.initialize(EssentialDataManager.Instance.AppKey, EssentialDataManager.Instance.SecretKey);
		WarpClient.setRecoveryAllowance(5);

		m_warpClient = WarpClient.GetInstance();

		//Registering to connection callbacks
		m_warpClient.AddConnectionRequestListener(m_warpListener);
		//Registering to Chat callbacks
		m_warpClient.AddChatRequestListener(m_warpListener);
		//Registering to Update Request callbacks
		m_warpClient.AddUpdateRequestListener(m_warpListener);
		//Registering to Lobby callbacks
		m_warpClient.AddLobbyRequestListener(m_warpListener);
		//Registering to Notifier callbacks
		m_warpClient.AddNotificationListener(m_warpListener);
		//Registering to Room/Subscribe callbacks
		m_warpClient.AddRoomRequestListener(m_warpListener);
		//Registering to Zone callbacks
		m_warpClient.AddZoneRequestListener(m_warpListener);
		//Registering to Turn Based callbacks
		m_warpClient.AddTurnBasedRoomRequestListener(m_warpListener);
	}

	public void BroadCastMessageInRoom(string a_strMessage)
	{
		m_warpClient.SendChat(a_strMessage);
	}

	private void ConnectedToServer(IEventBase a_Event)
	{

		EventConnectToServer data = a_Event as EventConnectToServer;
		if(data == null)
		{
			Debug.Log("[WarpNetworkManager] EventConnectToServer  Error");
			return;
		}

		m_strPlayerName = string.Empty;

		if (ConnectionEstablished)
		{
			Debug.LogWarning("[WarpNetworkManager] Connection already established");
			return;
		}

		m_strPlayerName = System.DateTime.UtcNow.Ticks.ToString();
		Debug.Log("[WarpNetworkManager] Connecting to server: PLAYERNAME: " + m_strPlayerName);

		
		m_warpClient.Connect(m_strPlayerName);
	}

	private void DisconnectedFromServer()
	{
		Debug.Log("[WarpNetworkManager] Disconnection from server: PLAYERNAME: " + m_strPlayerName);
		m_warpClient.Disconnect();
	}

	private void SubscribeRoom(IEventBase a_Event)
	{
		EventSubscribeRoom data = a_Event as EventSubscribeRoom;
		if (data == null)
		{
			Debug.Log("[WarpNetworkManager] EventSubscribeRoom  Error");
			return;
		}
		Debug.Log("[WarpNetworkManager]Subscribe to room [SubscribeRoom]");
		//Subscribing to the room
		m_warpClient.SubscribeRoom(EssentialDataManager.Instance.RoomID);
	}

	private void JoinRoom(IEventBase a_Event)
	{
		EventJoinRoom data = a_Event as EventJoinRoom;
		if (data == null)
		{
			Debug.Log("[WarpNetworkManager] EventJoinRoom  Error");
			return;
		}
		Debug.Log("[WarpNetworkManager] Join room [JoinRoom]");
		//Joining the room
		m_warpClient.JoinRoom(EssentialDataManager.Instance.RoomID);
	}

	private void ReconnectToServer( IEventBase a_Event)
	{
		EventReconnectServer data = a_Event as EventReconnectServer;
		if (data == null)
		{
			Debug.Log("[WarpNetworkManager] EventReconnectServer  Error");
			return;
		}

		if (m_warpClient == null)
		{
			Debug.LogError("[WarpNetworkManager] not initialized, its NULL [ReconnectToServer]");
			return;
		}
		if (m_iSessionID == 0)
		{
			Debug.Log("[WarpNetworkManager] no session ID [ReconnectToServer]");
			m_warpClient.RecoverConnection();
		}
		else
		{
			m_warpClient.RecoverConnectionWithSessioId(m_iSessionID,PlayerName);
		}
	}

	private void PlayerLeftRoom(string a_strUsername)
	{
		if (m_dicPlayerInRoom.ContainsValue(a_strUsername))
		{
			
			foreach(var item in m_dicPlayerInRoom)
			{
				if(string.Compare(item.Value,a_strUsername) == 0)
				{
					m_dicPlayerInRoom.Remove(item.Key);
					m_iNumberOfPlayersInRoom--;
					Debug.Log("[WarpNetworkManager] Player Removed form Dictionary: " + a_strUsername);
				}
			}
			
		}
	}

	private void PlayerJoinedRoom(string a_strUsername)
	{
		if (!m_dicPlayerInRoom.ContainsValue(a_strUsername))
		{			
			m_iNumberOfPlayersInRoom++;
			m_dicPlayerInRoom.Add(m_iNumberOfPlayersInRoom, a_strUsername);
			Debug.Log("[WarpNetworkManager] Player Added to Dictionary: " + a_strUsername);
		}
	}

	private bool IsPlayerAllInRoom(string a_strUsername)
	{
		if (!m_dicPlayerInRoom.ContainsValue(a_strUsername))
		{
			return false;
		}
		return true;
	}

//Connection States
//byte CONNECTED = 0;
//byte CONNECTING = 1;
//byte DISCONNECTED = 2;
//byte DISCONNECTING = 3;
//byte RECOVERING = 4;

	private void GetConnectionState()
	{
		m_warpClient.GetConnectionState();
	}

	private void Update()
    {
		if(m_warpClient != null)
			m_warpClient.Update();

		if (Input.GetKeyUp(KeyCode.Escape))
		{
			m_bApplictaionQuit = true;
			DisconnectedFromServer();			
		}
	}

	private void OnApplicationQuit()
	{
		if (ConnectionEstablished)
		{
			DisconnectedFromServer();
		}
	}

}

//WarpResponseResultCode States
 //SUCCESS = 0;
 //AUTH_ERROR = 1;
 //RESOURCE_NOT_FOUND = 2;
 //RESOURCE_MOVED = 3;
 //BAD_REQUEST = 4;
 //CONNECTION_ERR = 5;
 //UNKNOWN_ERROR = 6;
 //RESULT_SIZE_ERROR = 7;
 //SUCCESS_RECOVERED = 8;
 //CONNECTION_ERROR_RECOVERABLE = 9;
 //USER_PAUSED_ERROR = 10;


/// <summary>
/// This class is used to register istelf to all the listeners provided by AppWarp sdk
/// </summary>
public class WarpListerner : ConnectionRequestListener, LobbyRequestListener, ZoneRequestListener, RoomRequestListener, ChatRequestListener, UpdateRequestListener, NotifyListener, TurnBasedRoomListener
{

	//Reference: http://appwarp.shephertz.com/game-development-center/windows-game-developers-home/windows-client-listener
	#region ConnectionListener

	//Callback on Connection 
	public void onConnectDone(ConnectEvent eventObj)
	{

		switch (eventObj.getResult())
		{
			case WarpResponseResultCode.SUCCESS:
				Debug.Log("[WarpNetworkManager] connection success: " + eventObj.getResult().ToString()+" Subscribe to Room");
				WarpNetworkManager.Instance.ConnectionEstablished = true;

				//Room will be present as the challenger creates the room
				EventManager.Instance.TriggerEvent<EventSubscribeRoom>(new EventSubscribeRoom());		

				break;
			case WarpResponseResultCode.CONNECTION_ERR:
				Debug.Log("[WarpNetworkManager] connection error [Non-Recoverable]: " + eventObj.getResult().ToString());
				break;
			case WarpResponseResultCode.CONNECTION_ERROR_RECOVERABLE:
				Debug.Log("[WarpNetworkManager] connection error [Recoverable]: " + eventObj.getResult().ToString());
				EventManager.Instance.TriggerEvent<EventReconnectServer>(new EventReconnectServer());
				break;
			case WarpResponseResultCode.AUTH_ERROR:
				Debug.Log("[WarpNetworkManager] connection error: " + eventObj.getResult().ToString());
				break;
			case WarpResponseResultCode.BAD_REQUEST:
				Debug.Log("[WarpNetworkManager] connection error: " + eventObj.getResult().ToString());
				break;	
			case WarpResponseResultCode.RESOURCE_MOVED:
				Debug.Log("[WarpNetworkManager] connection error: " + eventObj.getResult().ToString());
				break;
			case WarpResponseResultCode.RESOURCE_NOT_FOUND:
				Debug.Log("[WarpNetworkManager] connection error: " + eventObj.getResult().ToString());
				break;
			case WarpResponseResultCode.RESULT_SIZE_ERROR:
				Debug.Log("[WarpNetworkManager] connection error: " + eventObj.getResult().ToString());
				break;
			case WarpResponseResultCode.UNKNOWN_ERROR:
				Debug.Log("[WarpNetworkManager] connection error: " + eventObj.getResult().ToString());
				break;
			case WarpResponseResultCode.USER_PAUSED_ERROR:
				Debug.Log("[WarpNetworkManager] connection error: " + eventObj.getResult().ToString());
				break;
			default:
				break;
		}
	}

	public void onDisconnectDone(ConnectEvent eventObj)
	{
		WarpNetworkManager.Instance.ConnectionEstablished = false;
		Debug.Log("[WarpListerner] Disconnected from server");
		if(WarpNetworkManager.Instance.ApplicationQuiting)
		{
			Debug.Log("[WarpListerner Quiting application after disconncetion");
			Application.Quit();
		}
	}
	public void onInitUDPDone(byte resultCode)
	{
		// handle onInitUDPDone here      
	}
	#endregion

	//LobbyRequestListener
	#region LobbyRequestListener
	public void onJoinLobbyDone (LobbyEvent eventObj)
	{
		Debug.Log("onJoinLobbyDone : " + eventObj.getResult());
		if(eventObj.getResult() == 0)
		{

		}
	}
		
	public void onLeaveLobbyDone (LobbyEvent eventObj)
	{
		Debug.Log("onLeaveLobbyDone : " + eventObj.getResult());
	}
		
	public void onSubscribeLobbyDone (LobbyEvent eventObj)
	{
		Debug.Log("onSubscribeLobbyDone : " + eventObj.getResult());
		if(eventObj.getResult() == 0)
		{
			WarpClient.GetInstance().JoinLobby();
		}
	}
		
	public void onUnSubscribeLobbyDone (LobbyEvent eventObj)
	{
		Debug.Log("onUnSubscribeLobbyDone : " + eventObj.getResult());
	}
		
	public void onGetLiveLobbyInfoDone (LiveRoomInfoEvent eventObj)
	{
		Debug.Log("onGetLiveLobbyInfoDone : " + eventObj.getResult());
	}
	#endregion
		
	//ZoneRequestListener
	#region ZoneRequestListener
	public void onDeleteRoomDone (RoomEvent eventObj)
	{
		Debug.Log("onDeleteRoomDone : " + eventObj.getResult());
	}
		
	public void onGetAllRoomsDone (AllRoomsEvent eventObj)
	{
		Debug.Log("onGetAllRoomsDone : " + eventObj.getResult());
		for(int i=0; i< eventObj.getRoomIds().Length; ++i)
		{
			Debug.Log("Room ID : " + eventObj.getRoomIds()[i]);
		}
	}
	

	//Callback on RoomCreated
	public void onCreateRoomDone (RoomEvent eventObj)
	{
		Debug.Log("onCreateRoomDone : " + eventObj.getResult());


		if(eventObj.getData() == null)
		{
			Debug.LogError("[WarpNetworkManager] Room data null error");
			return;
		}


		Debug.Log("[WarpNetworkManager] Room Name: "+ eventObj.getData().getName());
		Debug.Log("[WarpNetworkManager] Room ID: "+ eventObj.getData().getId());

		//Setting the RoomName and RoomID
		EssentialDataManager.Instance.RoomName = eventObj.getData().getName();
		EssentialDataManager.Instance.RoomID = eventObj.getData().getId();

		//Subscribing to the Room that was created
		EventManager.Instance.TriggerEvent<EventSubscribeRoom>(new EventSubscribeRoom());
	}
		
	public void onGetOnlineUsersDone (AllUsersEvent eventObj)
	{
		Debug.Log("onGetOnlineUsersDone : " + eventObj.getResult());
	}
		
	public void onGetLiveUserInfoDone (LiveUserInfoEvent eventObj)
	{
		Debug.Log("onGetLiveUserInfoDone : " + eventObj.getResult());
	}
		
	public void onSetCustomUserDataDone (LiveUserInfoEvent eventObj)
	{
		Debug.Log("onSetCustomUserDataDone : " + eventObj.getResult());
	}
		
    public void onGetMatchedRoomsDone(MatchedRoomsEvent eventObj)
	{
		if (eventObj.getResult() == WarpResponseResultCode.SUCCESS)
        {
            Debug.Log("GetMatchedRooms event received with success status");
            foreach (var roomData in eventObj.getRoomsData())
            {
                Debug.Log("Room ID:" + roomData.getId());
            }
        }
	}		
	#endregion

	//RoomRequestListener
	#region RoomRequestListener

	//Callback on subscribe
	public void onSubscribeRoomDone (RoomEvent eventObj)
	{
		if(eventObj.getResult() == 0)
		{
			Debug.Log("[WarpListener] Subscribe successful");
			EventManager.Instance.TriggerEvent<EventJoinRoom>(new EventJoinRoom());
		}
			
		Debug.Log("onSubscribeRoomDone : " + eventObj.getResult());
	}
		
	public void onUnSubscribeRoomDone (RoomEvent eventObj)
	{
		Debug.Log("onUnSubscribeRoomDone : " + eventObj.getResult());
	}
		
	public void onJoinRoomDone (RoomEvent eventObj)
	{
		if (eventObj.getResult() == 0)
		{
			Debug.Log("[WarpListener] Joining Room successful");
		}
		Debug.Log("onJoinRoomDone : " + eventObj.getResult());	
	}
		
	public void onLockPropertiesDone(byte result)
	{
		Debug.Log("onLockPropertiesDone : " + result);
	}
		
	public void onUnlockPropertiesDone(byte result)
	{
		Debug.Log("onUnlockPropertiesDone : " + result);
	}
		
	public void onLeaveRoomDone (RoomEvent eventObj)
	{
		Debug.Log("onLeaveRoomDone : " + eventObj.getResult());
	}
		
	public void onGetLiveRoomInfoDone (LiveRoomInfoEvent eventObj)
	{
		Debug.Log("onGetLiveRoomInfoDone : " + eventObj.getResult());
	}
		
	public void onSetCustomRoomDataDone (LiveRoomInfoEvent eventObj)
	{
		Debug.Log("onSetCustomRoomDataDone : " + eventObj.getResult());
	}
		
	public void onUpdatePropertyDone(LiveRoomInfoEvent eventObj)
    {
        if (WarpResponseResultCode.SUCCESS == eventObj.getResult())
        {
            Debug.Log("UpdateProperty event received with success status");
        }
        else
        {
            Debug.Log("Update Propert event received with fail status. Status is :" + eventObj.getResult().ToString());
        }
    }
	#endregion
		
	//ChatRequestListener
	#region ChatRequestListener
	public void onSendChatDone (byte result)
	{
		//Debug.Log("onSendChatDone result : " + result);		
	}
		
	public void onSendPrivateChatDone(byte result)
	{
		Debug.Log("onSendPrivateChatDone : " + result);
	}
	#endregion
		
	//UpdateRequestListener
	#region UpdateRequestListener
	public void onSendUpdateDone (byte result)
	{
	}
	public void onSendPrivateUpdateDone (byte result)
	{
		Debug.Log("onSendPrivateUpdateDone : " + result);
	}
	#endregion

	//NotifyListener
	#region NotifyListener
	public void onRoomCreated (RoomData eventObj)
	{
		Debug.Log("onRoomCreated: ROOMNAME: "+eventObj.getName()+" ROOM ID: "+eventObj.getId());
	
	}
	public void onPrivateUpdateReceived (string sender, byte[] update, bool fromUdp)
	{
		Debug.Log("onPrivateUpdate");
	}
	public void onRoomDestroyed (RoomData eventObj)
	{
		Debug.Log("onRoomDestroyed");
	}


	//Callback user left 
	public void onUserLeftRoom(RoomData eventObj, string username)
	{
		Debug.Log("onUserLeftRoom : " + username);
		//TODO: Check user and remove the user from game
		if (string.Compare(username,WarpNetworkManager.Instance.PlayerName) != 0)
		{
			EventManager.Instance.TriggerEvent<EventOpponentLeftRoom>(new EventOpponentLeftRoom());
		}
	}


	//Callback on Join
	public void onUserJoinedRoom (RoomData eventObj, string username)
	{
		Debug.Log("onUserJoinedRoom : " + username);
		{
			Debug.Log("[WarpNetworkManager] user joined Room successfully: " + username);
			if (string.Compare(username, WarpNetworkManager.Instance.PlayerName) != 0)
			{
				Debug.Log("[WarpNetworkManager] Opponent has joined");				
			}
			else 
			{			
				Debug.Log("[WarpNetworkManager] self has joined");			
			}
		}
	}
		
	public void onUserLeftLobby (LobbyData eventObj, string username)
	{
		Debug.Log("onUserLeftLobby : " + username);
	}
		
	public void onUserJoinedLobby (LobbyData eventObj, string username)
	{
		Debug.Log("onUserJoinedLobby : " + username);
	}
		
	public void onUserChangeRoomProperty(RoomData roomData, string sender, Dictionary<string, object> properties, Dictionary<string, string> lockedPropertiesTable)
	{
		Debug.Log("onUserChangeRoomProperty : " + sender);
	}
			
	public void onPrivateChatReceived(string sender, string message)
	{
		Debug.Log("onPrivateChatReceived : " + sender + "Message: "+message);
	}
		
	public void onMoveCompleted(MoveEvent move)
	{
		Debug.Log("onMoveCompleted by : " + move.getSender());
	}
	
	//Callback on data sent over AppWarp
	public void onChatReceived (ChatEvent eventObj)
	{
		//com.shephertz.app42.gaming.multiplayer.client.SimpleJSON.JSONNode NetworkData =  com.shephertz.app42.gaming.multiplayer.client.SimpleJSON.JSON.Parse(eventObj.getMessage());
		//UnityEngine.Vector3 vec3Pos = new UnityEngine.Vector3(NetworkData["x"].AsFloat, NetworkData["y"].AsFloat, NetworkData["z"].AsFloat);
		//UnityEngine.Vector3 vec3Euler = new UnityEngine.Vector3(NetworkData["xEuler"].AsFloat, NetworkData["yEuler"].AsFloat, NetworkData["zEuler"].AsFloat);

		string strNetworkMessage = eventObj.getMessage();
		//Sending the data over to MessageManager where it will be serialized and these data will be managed
		if (eventObj.getSender() != WarpNetworkManager.Instance.PlayerName)
		{
			EventManager.Instance.TriggerEvent<EventReadInGameMessage>(new EventReadInGameMessage(strNetworkMessage));
		}
	
	}
		
	public void onUpdatePeersReceived (UpdateEvent eventObj)
	{
		Debug.Log("onUpdatePeersReceived");
	}
		
	public void onUserChangeRoomProperty(RoomData roomData, string sender, Dictionary<string, System.Object> properties)
    {
        Debug.Log("Notification for User Changed Room Propert received");
        Debug.Log(roomData.getId());
        Debug.Log(sender);
        foreach (KeyValuePair<string, System.Object> entry in properties)
        {
            Debug.Log("KEY:" + entry.Key);
            Debug.Log("VALUE:" + entry.Value.ToString());
        }
    }

		
	public void onUserPaused(string locid, bool isLobby, string username)
	{
		Debug.Log("onUserPaused");
	}
		
	public void onUserResumed(string locid, bool isLobby, string username)
	{
		Debug.Log("onUserResumed");
	}
		
	public void onGameStarted(string sender, string roomId, string nextTurn)
	{
		Debug.Log("onGameStarted");
	}
		
	public void onGameStopped(string sender, string roomId)
	{
		Debug.Log("onGameStopped");
	}

	public void onNextTurnRequest (string lastTurn)
	{
		Debug.Log("onNextTurnRequest");
	}
	#endregion

	//TurnBasedRoomListener
	#region TurnBasedRoomListener
	public void onSendMoveDone(byte result)
	{

	}
		
	public void onStartGameDone(byte result)
	{
	}
		
	public void onStopGameDone(byte result)
	{
	}
		
	public void onSetNextTurnDone(byte result)
	{
	}
		
	public void onGetMoveHistoryDone(byte result, MoveEvent[] moves)
	{
	}
	#endregion

}