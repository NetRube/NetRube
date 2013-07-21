using System;

namespace NetRube
{
	/// <summary>名称和版本</summary>
	public class NameAndVersion
	{
		/// <summary>获取或设置名称</summary>
		/// <value>名称</value>
		public string Name { get; set; }

		/// <summary>获取或设置版本</summary>
		/// <value>版本</value>
		public string Version { get; set; }

		/// <summary>获取数字化版本</summary>
		/// <returns>数字化的版本</returns>
		public float GetVersion()
		{
			if(this.Version.IsNullOrEmpty_()) return 0f;
			if(this.Version.IsNumber_()) return this.Version.ToFloat_();
			var a = this.Version.Replace('_', '.').Replace_(@"[^\d\.]", string.Empty);
			if(a.IsNullOrEmpty_()) return 0f;
			var v = a.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
			var l = v.Length;
			if(l > 1)
				return (v[0] + "." + v[1]).ToFloat_();
			else if(l == 1)
				return v[0].ToInt_();
			else
				return 0f;
		}

		/// <summary>返回名称和版本字符串</summary>
		/// <returns>名称和版本字符串</returns>
		public override string ToString()
		{
			return this.Name + " " + this.Version;
		}
	}
}