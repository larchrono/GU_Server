using UnityEngine;  
using System.Collections;  
//引入庫
using System.Net;  
using System.Net.Sockets;  
using System.Text;  
using System.Threading;  

public class Serversc : MonoBehaviour  
{  
	Socket serverSocket; //服務器端socket  
	Socket clientSocket; //客戶端socket  
	IPEndPoint ipEnd; //偵聽端口  
	string recvStr; //接收的字符串
	string sendStr; //發送的字符串
	byte[] recvData=new byte[1024]; //接收的數據，必須為字節  
	byte[] sendData=new byte[1024]; //發送的數據，必須為字節  
	int recvLen; //接收的數據長度  
	Thread connectThread; //連接線程  

	//初始化  
	void InitSocket()  
	{  
		//定義偵聽端口,偵聽任何IP  
		ipEnd=new IPEndPoint(IPAddress.Any,25566);
		//定義套接字類型,在主線程中定義  
		serverSocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);  
		//連接  
		serverSocket.Bind(ipEnd);  
		//開始偵聽,最大10個連接  
		serverSocket.Listen(10);  


		//開啟一個線程連接，必須的，否則主線程卡死  
		connectThread=new Thread(new ThreadStart(SocketReceive));
		connectThread.Start();
	}  

	//連接  
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
		print("Connect with "+ipEndClient.Address.ToString()+":"+ipEndClient.Port.ToString());  
		//連接成功則發送數據  
		sendStr="Welcome to my server";  
		SocketSend(sendStr);  
	}  

	void SocketSend(string sendStr)  
	{  
		//清空發送緩存  
		sendData=new byte[1024];  
		//數據類型轉換  
		sendData=Encoding.UTF8.GetBytes(sendStr);  
		//發送  
		clientSocket.Send(sendData,sendData.Length,SocketFlags.None);  
	}  

	//服務器接收  
	void SocketReceive()  
	{  
		//連接  
		SocketConnet();        
		//進入接收循環  
		while(true)  
		{  
			//對data清零  
			recvData=new byte[1024];  
			//獲取收到的數據的長度  
			recvLen=clientSocket.Receive(recvData);  
			//如果收到的數據長度為0，則重連並進入下一個循環  
			if(recvLen==0)  
			{  
				SocketConnet();  
				continue;  
			}  
			//輸出接收到的數據  
			recvStr=Encoding.UTF8.GetString(recvData,0,recvLen);  
			print(recvStr);  
			//將接收到的數據經過處理再發送出去  
			//sendStr="From Server: "+recvStr;  
			//SocketSend(sendStr);  
		}  
	}  

	//連接關閉  
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

	// Use this for initialization  
	void Start()  
	{  
		InitSocket(); //在這裏初始化server  
	}


	// Update is called once per frame  
	void Update()  
	{

	}

	void OnApplicationQuit()
	{  
		SocketQuit();
	}
}