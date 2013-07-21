using System;
using System.Runtime.Serialization;

namespace NetRube
{
	/// <summary>当将空引用（null 或 empyt）传递给不接受它作为有效参数的方法时引发的异常。</summary>
	public class ArgumentNullOrEmptyException : ArgumentException
	{
		/// <summary>初始化 <see cref="ArgumentNullOrEmptyException"/> 类的新实例。</summary>
		public ArgumentNullOrEmptyException() : base(Localization.Resources.ArgumentNullOrEmptyException) { }

		/// <summary>使用导致此异常的参数的名称初始化 <see cref="ArgumentNullOrEmptyException"/> 类的新实例。</summary>
		/// <param name="paramName">导致异常的参数的名称。</param>
		public ArgumentNullOrEmptyException(string paramName) : base(Localization.Resources.ArgumentNullOrEmptyException, paramName) { }

		/// <summary>用序列化数据初始化 <see cref="ArgumentNullOrEmptyException"/> 类的新实例。</summary>
		/// <param name="info">保存序列化对象数据的对象。</param>
		/// <param name="context">有关源或目标的上下文信息。</param>
		protected ArgumentNullOrEmptyException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		/// <summary>使用指定的错误消息和导致此异常的参数的名称来初始化 <see cref="ArgumentNullOrEmptyException"/> 类的实例。</summary>
		/// <param name="paramName">导致异常的参数的名称。</param>
		/// <param name="message">描述错误的消息。</param>
		public ArgumentNullOrEmptyException(string paramName, string message) : base(message, paramName) { }
	}
}