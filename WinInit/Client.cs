using System;
using System.IO;
using System.Net;
using System.Net.Sockets;


namespace WinInit
{
	public static class FileClient
	{
		public static int SendVarData(Socket s, byte[] data)
		{
			int total = 0;
			int size = data.Length;
			int dataleft = size;
			int sent;

			try
			{
				while (total < size)
				{
					sent = s.Send(data, total, dataleft, SocketFlags.None);
					total += sent;
					dataleft -= sent;
				}

				return total;
			}
			catch
			{
				return 3;

			}
		}
		public static bool SendFile(string IP, int Port, string fullPath)
		{
			//创建一个文件对象
			FileInfo EzoneFile = new FileInfo(fullPath);
			
			//打开文件流
			FileStream EzoneStream = EzoneFile.OpenRead();

			//包的大小
			int PacketSize = 4096;

			//包的数量
			int PacketCount = (int)(EzoneStream.Length / ((long)PacketSize));

			//最后一个包的大小
			int LastDataPacket = (int)(EzoneStream.Length - ((long)(PacketSize * PacketCount)));

			//指向远程服务端节点
			IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(IP), Port);

			//创建套接字
			Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			//连接到发送端
			client.Connect(ipep);

			//获得客户端节点对象
			IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;


			//发送[文件名]到客户端
			byte[] name = new byte[PacketSize];
			System.Text.Encoding.UTF8.GetBytes(EzoneFile.Name).CopyTo(name, 0);
			SendVarData(client, name);

			byte[] count = new byte[PacketSize];
			System.Text.Encoding.ASCII.GetBytes(PacketCount.ToString()).CopyTo(count, 0);
			SendVarData(client, count);

			byte[] lastlen = new byte[PacketSize];
			System.Text.Encoding.ASCII.GetBytes(LastDataPacket.ToString()).CopyTo(lastlen, 0);
			SendVarData(client, lastlen);

			bool isCut = false;
			//数据包
			byte[] data = new byte[PacketSize];
			//开始循环发送数据包
			for (int i = 0; i < PacketCount; i++)
			{
				//从文件流读取数据并填充数据包
				EzoneStream.Read(data, 0, data.Length);
				//发送数据包
				if (SendVarData(client, data) == 3)
				{
					isCut = true;
					return false;
				}
			}

			//如果还有多余的数据包，则应该发送完毕！
			if (LastDataPacket != 0)
			{
				data = new byte[LastDataPacket];
				EzoneStream.Read(data, 0, data.Length);
				SendVarData(client, data);
			}

			//关闭套接字
			client.Close();
			//关闭文件流
			EzoneStream.Close();
			if (!isCut)
			{
				return true;
			}
			return false;
		}
	}
}