using System.ComponentModel;

namespace NetRube
{
	/// <summary>移动方向</summary>
	public enum MoveDirection
	{
		/// <summary>向上</summary>
		[Description("向上")]
		Up,
		/// <summary>向下</summary>
		[Description("向下")]
		Down,
		/// <summary>向左</summary>
		[Description("向左")]
		Left,
		/// <summary>向右</summary>
		[Description("向右")]
		Right
	}
}