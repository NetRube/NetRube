namespace NetRube
{
	/// <summary>转换大小写类型</summary>
	public enum TextCaseType
	{
		/// <summary>不处理</summary>
		None,
		/// <summary>小写</summary>
		LowerCase,
		/// <summary>大写</summary>
		UpperCase,
		/// <summary>首字母大写（每个单词的首字母大写）</summary>
		PascalCase,
		/// <summary>驼峰格式（首个单词的首字母小写，其余单词的首字母大写）</summary>
		CamelCase,
		/// <summary>以连字符分隔</summary>
		Hyphenate
	}
}