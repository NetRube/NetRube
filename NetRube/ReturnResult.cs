
namespace NetRube
{
	/// <summary>返回操作结果</summary>
	public class ReturnResult
	{
		/// <summary>获取或设置操作是否成功</summary>
		/// <value>如果操作成功，则返回 <c>true</c>，否则返回 <c>false</c>。</value>
		public bool IsSucceed { get; set; }

		/// <summary>获取或设置返回信息</summary>
		/// <value>返回信息</value>
		public string Message { get; set; }

		private string logMessage;
		/// <summary>获取或设置返回的日志消息，用于记录日志</summary>
		/// <value>返回的日志消息</value>
		public string LogMessage
		{
			get { return this.logMessage ?? this.Message; }
			set { this.logMessage = value; }
		}

		/// <summary>获取或设置返回数据</summary>
		/// <value>返回数据</value>
		public object ReturnData { get; set; }
	}

	/// <summary>返回操作结果</summary>
	/// <typeparam name="T"></typeparam>
	public class ReturnResult<T> : ReturnResult
	{
		/// <summary>获取或设置返回数据</summary>
		/// <value>返回数据</value>
		public T ReturnData { get; set; }
	}
}