using System.Collections.Generic;
using System.Linq;

namespace NetRube
{
	/// <summary>跟踪实体对象</summary>
	/// <typeparam name="T">实体类型</typeparam>
	public class TrackingEntity<T> where T : new()
	{
		private Parameters PARAMS;

		/// <summary>获取原实体</summary>
		/// <value>原实体</value>
		public T OriginalEntity { get; private set; }

		/// <summary>获取跟踪后的实体</summary>
		/// <value>跟踪后的实体</value>
		public T TrackedEntity { get; private set; }

		/// <summary>初始化一个新 <see cref="TrackingEntity&lt;T&gt;" /> 实例。</summary>
		/// <param name="original">要被跟踪的实体</param>
		/// <param name="param">参数</param>
		public TrackingEntity(T original, Parameters param = null)
		{
			TrackedEntity = original;
			OriginalEntity = original.Clone_();
			this.PARAMS = param ?? new Parameters();
		}

		/// <summary>开始跟踪</summary>
		/// <param name="original">要跟踪的实体</param>
		/// <param name="param">参数</param>
		/// <returns>快照</returns>
		public static TrackingEntity<T> Start(T original, Parameters param = null)
		{
			return new TrackingEntity<T>(original, param);
		}

		/// <summary>获取被更改属性列表</summary>
		/// <param name="entity">要跟踪的实体</param>
		/// <param name="refer">用于参照的实体</param>
		/// <param name="param">参数</param>
		/// <returns>被更改的集合</returns>
		public static List<Change> GetChanges(T entity, T refer, Parameters param = null)
		{
			var als = FastReflection.FastGetAccessors<T>();
			var ls = new List<Change>(als.Count);

			foreach(var a in als.Values)
			{
				if(param.IgnoreVirtual && a.IsVirtual) continue;
				if(!a.CanRade) continue;

				var v1 = a.GetValue(entity);
				var v2 = a.GetValue(refer);
				if(!AreEqual(v1, v2))
					ls.Add(new Change { Name = a.Name, OldValue = v1, NewValue = v2 });
			}

			return ls;
		}

		private static bool AreEqual(object first, object second)
		{
			if(first == null && second == null) return true;
			if(first == null && second != null) return false;
			return first.Equals(second);
		}

		/// <summary>获取被更改属性列表</summary>
		/// <returns>被更改的集合</returns>
		public List<Change> GetChanges()
		{
			return GetChanges(this.OriginalEntity, this.TrackedEntity, this.PARAMS);
		}

		/// <summary>获取被更改属性名称列表</summary>
		/// <returns>被更改的属性名称集合</returns>
		public List<string> GetChangeNames()
		{
			return this.GetChanges().Select(c => c.Name).ToList();
		}

		/// <summary>更改内容</summary>
		public class Change
		{
			/// <summary>获取或设置属性名</summary>
			/// <value>属性名</value>
			public string Name { get; set; }
			/// <summary>获取或设置原始值</summary>
			/// <value>原始值</value>
			public object OldValue { get; set; }
			/// <summary>获取或设置新值</summary>
			/// <value>新值</value>
			public object NewValue { get; set; }
		}

		/// <summary>参数</summary>
		public class Parameters
		{
			/// <summary>
			/// 是否忽略虚属性，默认为是。
			/// </summary>
			public bool IgnoreVirtual = true;
		}
	}
}