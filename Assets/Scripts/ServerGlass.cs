using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;  
using System.Net.Sockets;  
using System.Text;  
using System.Threading;

public class ServerGlass: MonoBehaviour {

	//負責發送Touch座標給眼鏡
	//new:負責發送位置座標給眼鏡
	//V21 , new:負責接收撞擊地面時的位置

	public delegate void RecieveGlassEvent(ArgsPosition[] args);
	public static event RecieveGlassEvent recieveGlassEvent;

	public static ServerGlass current;

	int glassServerPort = 25568;

	Socket serverSocket; //服務器端socket  
	Socket clientSocket; //客戶端socket  
	IPEndPoint ipEnd; //偵聽端口  
	string recvStr; //接收的字符串
	string sendStr; //發送的字符串
	byte[] recvData=new byte[1024]; //接收的數據，必須為字節  
	byte[] sendData=new byte[1024]; //發送的數據，必須為字節  
	int recvLen; //接收的數據長度  
	Thread connectThread; //連接線程  

	void Awake(){
		current = this;
	}

	// Use this for initialization
	void Start () {
		Debug.Log ("Start Glass Server at :" + glassServerPort);
		InitSocket ();
	}

	// Update is called once per frame
	void Update () {

	}

	void OnApplicationQuit()
	{  
		SocketQuit();
	}

	void InitSocket()  
	{  
		//定義偵聽端口,偵聽任何IP  
		ipEnd=new IPEndPoint(IPAddress.Any,glassServerPort);
		//定義套接字類型,在主線程中定義
		serverSocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);  

		serverSocket.Bind(ipEnd);  
		//開始偵聽,最大10個連接  
		serverSocket.Listen(1);  


		//開啟一個線程連接，必須的，否則主線程卡死  
		connectThread=new Thread(ServerWork);
		connectThread.Start();
	}

	void ServerWork()  
	{  
		//連接
		SocketConnet();        
		//進入接收循環  
		while(true)  
		{  
			//對data清零  
			recvData=new byte[1024];  
			try
			{
				//獲取收到的數據的長度  
				recvLen = clientSocket.Receive(recvData);
			}
			catch (System.Net.Sockets.SocketException)
			{
				SocketConnet();
				continue;
			}
			//如果收到的數據長度為0，則重連並進入下一個循環  
			if(recvLen==0)  
			{  
				SocketConnet();  
				continue;  
			}  
			//輸出接收到的數據  
			recvStr=Encoding.UTF8.GetString(recvData,0,recvLen);  

			//N,n,n,n,n[/TCP]
			Debug.Log("Server Glass Rcv : " + recvStr);

			//Recieve Data Will Be   245,135,90[/TCP]   , str 不會包含[/TCP]
			char delimiter = ',';
			string delimiterEnd = "[/TCP]";

			//when data send quickly , server will recieve at least 2 [/TCP] , so message will be => 245,135,90[/TCP]125,15,190[/TCP]245,135,90[/TCP]
			//Split should remember that end is a part of substring , for above example , split number is 4  (3 + 1 end )
			string[] fragmentString = recvStr.Split(new[] {delimiterEnd}, System.StringSplitOptions.None);

			int fragmentNum = fragmentString.Length - 1;

			for(int fn = 0; fn < fragmentNum ; fn++)
			{
				string[] substrings = fragmentString [fn].Split (delimiter); // => 245  135  90

				if (substrings.Length > 2) {

					int DataNum = 0;
					int.TryParse (substrings [0], out DataNum);

					if (DataNum == 0 || substrings.Length < DataNum*2 + 1)
						continue;

					ArgsPosition[] myArgs = new ArgsPosition[DataNum];
					for (int i = 0; i < DataNum; i++) {

						myArgs [i] = new ArgsPosition ();
						myArgs [i].x = System.Convert.ToInt32 (substrings [1 + i*2]);
						myArgs [i].y = System.Convert.ToInt32 (substrings [2 + i*2]);
					}

					if (recieveGlassEvent != null) {
						recieveGlassEvent.Invoke (myArgs);
					}

				} // end Length

			} // end fragment

		}  // end While
	}

	void SocketConnet()  
	{  
		if(clientSocket!=null)  
			clientSocket.Close();  
		//控制台輸出偵聽狀態
		print("Waiting for a client");  
		//一旦接受連接，創建一個客戶端  
		clientSocket=serverSocket.Accept();  
		//獲取客戶端的IP和端口  
		IPEndPoint ipEndClient=(IPEndPoint)clientSocket.RemoteEndPoint;  
		//輸出客戶端的IP和端口  
		Debug.Log("Connect with "+ipEndClient.Address.ToString()+":"+ipEndClient.Port.ToString());  

		//連接成功則發送數據  
		//string sendStr="Welcome to my server";
		//SocketSend(sendStr);  

	}  

	//Data to Glass can use UTF8
	public void SocketSend(string sendStr)  
	{
		if (clientSocket == null)
			return;
		if (clientSocket.Connected == false)
			return;

		sendStr = sendStr + "[/TCP]";
		//清空發送緩存  
		sendData = new byte[1024];
		//數據類型轉換  
		sendData = Encoding.UTF8.GetBytes(sendStr);
		//發送  
		clientSocket.Send(sendData,sendData.Length,SocketFlags.None);

		Debug.Log ("Send:" + sendStr);
	}

	void SocketQuit()  
	{  
		//先關閉客戶端  
		if(clientSocket!=null)  
			clientSocket.Close();  
		//再關閉線程  
		if(connectThread!=null)  
		{  
			connectThread.Interrupt();  
			connectThread.Abort();  
		}  
		//最後關閉服務器
		if (serverSocket != null) {
			serverSocket.Close ();  
			print ("diconnect");  
		}
	}  
}
