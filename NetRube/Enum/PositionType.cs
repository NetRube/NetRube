using System.ComponentModel;

namespace NetRube
{
	/// <summary>表示方位类型</summary>
	public enum PositionType
	{
		/// <summary>左上</summary>
		[Description("左上")]
		TopLeft,
		/// <summary>上</summary>
		[Description("上")]
		Top,
		/// <summary>右上</summary>
		[Description("右上")]
		TopRight,
		/// <summary>左</summary>
		[Description("左")]
		Left,
		/// <summary>中间</summary>
		[Description("中间")]
		Center,
		/// <summary>右</summary>
		[Description("右")]
		Right,
		/// <summary>左下</summary>
		[Description("左下")]
		BottomLeft,
		/// <summary>下</summary>
		[Description("下")]
		Bottom,
		/// <summary>右下</summary>
		[Description("右下")]
		BottomRight
	}
}