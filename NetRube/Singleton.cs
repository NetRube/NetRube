
namespace NetRube
{
	/// <summary>以单例模式调用 <typeparamref name="T"/></summary>
	/// <typeparam name="T">要使用单例的类型</typeparam>
	public static class Singleton<T> where T : new()
	{
		/// <summary><typeparamref name="T"/> 的实例</summary>
		public static T Instance = new T();
	}
}