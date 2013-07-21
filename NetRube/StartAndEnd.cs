
namespace NetRube
{
	/// <summary>开始和结束</summary>
	/// <typeparam name="T">数据类型</typeparam>
	public class StartAndEnd<T>
	{
		/// <summary>获取或设置开始值</summary>
		/// <value>开始值</value>
		public T Start { get; set; }
		/// <summary>获取或设置结束值</summary>
		/// <value>结束值</value>
		public T End { get; set; }

		/// <summary>获取一个值，该值指示此实例的开始值与结束值是否相等</summary>
		/// <value>如果开始值与结束值是否相等，则该值为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool IsEqual
		{
			get { return Start.Equals(End); }
		}
	}
}