using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace NetRube
{
	/// <summary>NetRube 实用操作类库</summary>
	public partial class Utils
	{
		// 磁盘操作

		#region 创建目录
		/// <summary>创建目录</summary>
		/// <param name="path">要创建的目录及路径</param>
		/// <returns>创建的目录路径</returns>
		public static string AddDirectory(string path)
		{
			if(path.IsNullOrEmpty_())
				return string.Empty;

			if(!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}
		#endregion

		#region 重命名
		/// <summary>重命名</summary>
		/// <param name="fileInfo">要重命名的文件实例</param>
		/// <param name="newName">新文件名</param>
		/// <returns>返回重命名后的完整文件路径</returns>
		public static string Rename_(this FileInfo fileInfo, string newName)
		{
			if(fileInfo == null || newName.IsNullOrEmpty_())
				return string.Empty;

			newName = Path.Combine(fileInfo.DirectoryName, newName);
			fileInfo.MoveTo(newName);
			return newName;
		}

		/// <summary>重命名文件</summary>
		/// <param name="path">要重命名的</param>
		/// <param name="newName">新文件名</param>
		/// <returns>如果重命名成功，则返回重命名后的完整文件路径，否则返回空字符串</returns>
		public static string RenameFile(string path, string newName)
		{
			if(File.Exists(path))
				return Rename_(new FileInfo(path), newName);
			return string.Empty;
		}

		/// <summary>重命名</summary>
		/// <param name="dirInfo">要重命名的目录实例</param>
		/// <param name="newName">新目录名称</param>
		/// <returns>如果重命名成功，则返回重命名后的完整目录路径，否则返回空字符串</returns>
		public static string Rename_(this DirectoryInfo dirInfo, string newName)
		{
			if(dirInfo == null || newName.IsNullOrEmpty_())
				return string.Empty;

			newName = Path.Combine(dirInfo.Parent.FullName, newName);
			dirInfo.MoveTo(newName);
			return newName;
		}

		/// <summary>重命名目录</summary>
		/// <param name="path">要重命名的目录路径</param>
		/// <param name="newName">新目录名称</param>
		/// <returns>如果重命名成功，则返回重命名后的完整目录路径，否则返回空字符串</returns>
		public static string RenameDirectory(string path, string newName)
		{
			if(Directory.Exists(path))
				return Rename_(new DirectoryInfo(path), newName);
			return string.Empty;
		}
		#endregion

		#region 获取目录大小
		/// <summary>获取目录的大小（字节）</summary>
		/// <param name="dirInfo">要获取的目录实例</param>
		/// <returns>返回一个表示目录大小（字节）的长整型数字</returns>
		public static long GetSize_(this DirectoryInfo dirInfo)
		{
			long _dirSize = 0L;
			if(dirInfo == null) return _dirSize;
			Queue<DirectoryInfo> _queue = new Queue<DirectoryInfo>();
			while(true)
			{
				FileInfo[] _files = dirInfo.GetFiles();
				foreach(FileInfo _file in _files)
					_dirSize += _file.Length;
				DirectoryInfo[] _dirs = dirInfo.GetDirectories();
				foreach(DirectoryInfo _dir in _dirs)
					_queue.Enqueue(_dir);
				if(_queue.Count > 0)
					dirInfo = _queue.Dequeue();
				else
					break;
			}
			return _dirSize;
		}

		/// <summary>获取指定目录的大小（字节）</summary>
		/// <param name="path">要获取的目录</param>
		/// <returns>一个表示目录大小（字节）的长整型数字</returns>
		public static long GetDirectorySize(string path)
		{
			if(string.IsNullOrEmpty(path)) return 0L;
			if(!Directory.Exists(path)) return 0L;
			DirectoryInfo _dirInfo = new DirectoryInfo(path);
			return GetSize_(_dirInfo);
		}

		/// <summary>获取指定目录里不包含子目录的文件大小（字节）</summary>
		/// <param name="dirInfo">要获取的目录实例</param>
		/// <returns>一个表示目录大小（字节）的长整型数字</returns>
		public static long GetFilesSize_(this DirectoryInfo dirInfo)
		{
			long _filesSize = 0L;
			if(dirInfo == null) return _filesSize;
			foreach(FileInfo _file in dirInfo.GetFiles())
				_filesSize += _file.Length;
			return _filesSize;
		}

		/// <summary>获取指定目录里不包含子目录的文件大小（字节）</summary>
		/// <param name="path">要获取的目录</param>
		/// <returns>一个表示目录下文件大小（字节）的长整型数字</returns>
		public static long GetDirectoryFilesSize(string path)
		{
			if(string.IsNullOrEmpty(path)) return 0L;
			if(!Directory.Exists(path)) return 0L;
			DirectoryInfo _dirInfo = new DirectoryInfo(path);
			return GetFilesSize_(_dirInfo);
		}
		#endregion

		#region 无效字符操作
		/// <summary>是否为不允许用于路径的字符</summary>
		/// <param name="c">要检测的字符</param>
		/// <returns>指示是否为不允许用于路径的字符</returns>
		public static bool IsInvalidPathChar_(this char c)
		{
			return Path.GetInvalidPathChars().Contains_(c);
		}

		/// <summary>移除路径里的不允许使用的字符</summary>
		/// <param name="path">要处理的路径</param>
		/// <returns>处理后的路径</returns>
		public static string RemoveInvalidPathChars(string path)
		{
			if(string.IsNullOrEmpty(path)) return string.Empty;

			return RemoveInvalidChars(path, Path.GetInvalidPathChars());
		}

		/// <summary>是否为不允许用于文件名的字符</summary>
		/// <param name="c">要检测的字符</param>
		/// <returns>指示是否为不允许用于文件名的字符</returns>
		public static bool IsInvalidFileNameChar_(this char c)
		{
			return Path.GetInvalidFileNameChars().Contains_(c);
		}

		/// <summary>移除文件名里的不允许使用的字符</summary>
		/// <param name="path">要处理的文件名</param>
		/// <returns>处理后的文件名</returns>
		public static string RemoveInvalidFileNameChars(string path)
		{
			if(string.IsNullOrEmpty(path)) return string.Empty;

			return RemoveInvalidChars(path, Path.GetInvalidFileNameChars());
		}

		private static string RemoveInvalidChars(string path, char[] invalidChars)
		{
			if(string.IsNullOrEmpty(path)) return string.Empty;

			char[] newPath = new char[path.Length];
			int index = 0;
			foreach(char c in path)
			{
				if(c.In_(invalidChars)) continue;
				newPath[index] = c;
				index++;
			}
			return new string(newPath, 0, index);
		}
		#endregion

		#region 检测目录文件是否存在
		/// <summary>确定指定文件是否存在</summary>
		/// <param name="path">要检查的文件</param>
		/// <returns>指示指定文件是否存在</returns>
		public static bool FileExists(string path)
		{
			return File.Exists(path);
		}

		/// <summary>确定指定目录是否存在</summary>
		/// <param name="path">要检查的目录</param>
		/// <returns>指示指定目录是否存在</returns>
		public static bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}
		#endregion

		#region 获取文件名
		/// <summary>获取绝对路径</summary>
		/// <param name="path">以应用程序域（Asp.Net 中为站点根目录）开始的路径</param>
		/// <returns>绝对路径</returns>
		public static string GetMapPath(string path)
		{
			string _path = path.Replace('/', '\\');
			_path = _path.TrimStart('~', '\\');
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _path);
		}

		/// <summary>获取包含“.”的文件扩展名</summary>
		/// <param name="path">要获取的文件</param>
		/// <returns>包含“.”的文件扩展名</returns>
		public static string GetFileExtension(string path)
		{
			return Path.GetExtension(path);
		}

		/// <summary>获取包含扩展名的文件名</summary>
		/// <param name="path">要获取的路径</param>
		/// <returns>包含扩展名的文件名</returns>
		public static string GetFileName(string path)
		{
			return Path.GetFileName(path);
		}

		/// <summary>获取不包含扩展名的文件名</summary>
		/// <param name="path">要获取的路径</param>
		/// <param name="withPath">是否包含路径</param>
		/// <returns>不包含扩展名的文件名</returns>
		public static string GetFileNameWithoutExtension(string path, bool withPath = false)
		{
			if(withPath)
			{
				string _fileExt = GetFileExtension(path);
				return RemoveRight_(path, _fileExt);
			}
			return Path.GetFileNameWithoutExtension(path);
		}
		#endregion

		#region 格式化字节格式
		/// <summary>格式化字节格式</summary>
		/// <param name="size">要格式化的字节数</param>
		/// <returns>返回以Y、Z、E、T、G、M、K 和 B 表示的大小格式</returns>
		public static string FormatSize(long size)
		{
			const string ft = "{0}{1:N2} {2}";

			if(size == 0)
				return string.Format(ft, null, 0, SizeUnits[0]);

			var abs = Math.Abs((double)size);
			var place = Convert.ToInt32(Math.Floor(Math.Log(abs, 1024)));
			var num = Math.Round(abs / Math.Pow(1024, place), 2);
			return string.Format(ft, size < 0 ? "-" : null, num, SizeUnits[place]);
		}
		private static char[] SizeUnits = { 'B', 'K', 'M', 'G', 'T', 'E', 'Z', 'Y' };
		#endregion

		#region 备份恢复
		/// <summary>备份文件</summary>
		/// <param name="sourceFileName">要备份的文件名</param>
		/// <param name="destFileName">备份后的文件名</param>
		/// <param name="overwrite">是否覆盖存在的文件</param>
		/// <returns></returns>
		public static bool BackupFile(string sourceFileName, string destFileName, bool overwrite = true)
		{
			if(!File.Exists(sourceFileName))
				return false;
			if(!overwrite && File.Exists(destFileName)) return false;

			try
			{
				File.Copy(sourceFileName, destFileName, true);
				return true;
			}
			catch { return false; }
		}

		/// <summary>恢复文件</summary>
		/// <param name="backupFileName">备份文件名</param>
		/// <param name="targetFileName">要恢复的文件名</param>
		/// <param name="backupTargetFileName">恢复时先备份源文件的文件名，如果为空则不先备份</param>
		/// <returns></returns>
		public static bool RestoreFile(string backupFileName, string targetFileName, string backupTargetFileName = null)
		{
			if(!File.Exists(backupFileName))
				return false;

			if(!string.IsNullOrEmpty(backupTargetFileName))
			{
				if(File.Exists(targetFileName))
				{
					try { File.Copy(targetFileName, backupTargetFileName, true); }
					catch { return false; }
				}
			}
			try { File.Copy(backupFileName, targetFileName, true); }
			catch { return false; }

			return true;
		}
		#endregion

		#region 读取写入
		/// <summary>读取 UTF-8 编码的文本文件</summary>
		/// <param name="fileName">文件名</param>
		/// <returns></returns>
		public static string ReadTextFile(string fileName)
		{
			if(!Utils.FileExists(fileName)) return string.Empty;
			using(StreamReader sr = new StreamReader(fileName, Encoding.UTF8, true))
				return sr.ReadToEnd();
		}

		/// <summary>写入 UTF-8 编码的文本文件，不存在时将自动创建该文件</summary>
		/// <param name="text">要写入的文本内容</param>
		/// <param name="fileName">文件名</param>
		public static void WriteTextFile(string text, string fileName)
		{
			using(StreamWriter sw = new StreamWriter(fileName, true, Encoding.UTF8))
				sw.Write(text);
		}

		/// <summary>获取 XML 文件内容，可以是远程的</summary>
		/// <param name="path">XML 文件路径或 URL</param>
		/// <returns></returns>
		public static XmlDocument GetXml(string path)
		{
			XmlDocument _xml = new XmlDocument();
			try { _xml.Load(path); }
			catch { return null; }
			return _xml;
		}
		#endregion

		/// <summary>获取驱动器中文名称</summary>
		/// <param name="type">驱动器类型</param>
		/// <returns>驱动器的中文名称</returns>
		public static string GetDriveTypeName(DriveType type)
		{
			return type.GetLocalization_(Localization.Resources.ResourceManager);
		}
	}
}