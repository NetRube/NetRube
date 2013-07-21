using System.Collections.Generic;

namespace NetRube
{
	/// <summary>分页列表</summary>
	/// <typeparam name="T">数据类型</typeparam>
	public class PagedList<T> : List<T>
	{
		/// <summary>初始化一个新 <see cref="PagedList&lt;T&gt;" /> 实例。</summary>
		/// <param name="list">列表源</param>
		/// <param name="pageIndex">当前页索引</param>
		/// <param name="pageSize">每页记录数</param>
		/// <param name="recordCount">总记录数</param>
		/// <param name="context">其它相关内容</param>
		public PagedList(IList<T> list, int pageIndex, int pageSize, int recordCount, object context = null)
		{
			if(!list.IsNullOrEmpty_())
				this.AddRange(list);
			CurrentPageIndex = pageIndex;
			PageSize = pageSize;
			TotalRecordCount = recordCount;
			Context = context;
		}

		/// <summary>获取或设置当前页索引</summary>
		/// <value>当前页索引</value>
		public int CurrentPageIndex { get; set; }

		/// <summary>获取或设置每页记录数</summary>
		/// <value>每页记录数</value>
		public int PageSize { get; set; }

		/// <summary>获取或设置总记录数</summary>
		/// <value>总记录数</value>
		public int TotalRecordCount { get; set; }

		/// <summary>获取总页数</summary>
		/// <value>总页数</value>
		public int TotalPageCount { get { return Utils.GetPages_(TotalRecordCount, PageSize); } }

		/// <summary>获取开始记录索引</summary>
		/// <value>开始记录索引</value>
		public int StartRecordIndex { get { return (CurrentPageIndex - 1) * PageSize + 1; } }

		/// <summary>获取结束记录索引</summary>
		/// <value>结束记录索引</value>
		public int EndRecordIndex { get { return TotalRecordCount > CurrentPageIndex * PageSize ? CurrentPageIndex * PageSize : TotalRecordCount; } }

		/// <summary>获取或设置其它相关内容</summary>
		/// <value>其它相关内容</value>
		public object Context { get; set; }
	}
}