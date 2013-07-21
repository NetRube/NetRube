using System.ComponentModel;

namespace NetRube
{
	/// <summary>数据库查询排序方向</summary>
	public enum OrderByType
	{
		/// <summary>降序</summary>
		[Description("降序")]
		DESC,
		/// <summary>升序</summary>
		[Description("升序")]
		ASC
	}
}