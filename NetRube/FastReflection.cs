using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NetRube
{
	/// <summary>
	/// 快速反射类
	/// </summary>
	public static class FastReflection
	{
		#region 创建实例
		private static Dict<ConstructorInfo, Func<object[], object>> OC;

		/// <summary>通过类的构造器信息创建对象实例</summary>
		/// <param name="constructorInfo">构造器信息</param>
		/// <param name="parameters">参数</param>
		/// <returns>新对象实例</returns>
		public static object FastInvoke(this ConstructorInfo constructorInfo, params object[] parameters)
		{
			if(OC == null) OC = new Dict<ConstructorInfo, Func<object[], object>>();

			var exec = OC.Get(constructorInfo, () =>
			{
				var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");
				var parameterExpressions = new List<Expression>();
				var paramInfos = constructorInfo.GetParameters();

				for(int i = 0, l = paramInfos.Length; i < l; i++)
				{
					var valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
					var valueCast = Expression.Convert(valueObj, paramInfos[i].ParameterType);

					parameterExpressions.Add(valueCast);
				}

				var instanceCreate = Expression.New(constructorInfo, parameterExpressions);
				var instanceCreateCast = Expression.Convert(instanceCreate, typeof(object));
				var lambda = Expression.Lambda<Func<object[], object>>(instanceCreateCast, parametersParameter);

				return lambda.Compile();
			});
			return exec(parameters);
		}

		/// <summary>通过类的构造器信息创建对象实例</summary>
		/// <typeparam name="T">返回对象类型</typeparam>
		/// <param name="constructorInfo">构造器信息</param>
		/// <param name="parameters">参数</param>
		/// <returns>新对象实例</returns>
		public static T FastInvoke<T>(this ConstructorInfo constructorInfo, params object[] parameters)
		{
			return (T)constructorInfo.FastInvoke(parameters);
		}

		/// <summary>通过对象类型创建对象实例</summary>
		/// <param name="type">对象类型</param>
		/// <param name="parameters">参数</param>
		/// <returns>新对象实例</returns>
		public static object FastInvoke(this Type type, params object[] parameters)
		{
			var paramLen = parameters.Length;
			var types = new Type[paramLen];
			for(int i = 0; i < paramLen; i++)
				types[i] = parameters[i].GetType();
			var consInfo = type.GetConstructor(types);
			return consInfo.FastInvoke(parameters);
		}

		/// <summary>通过对象类型创建对象实例</summary>
		/// <typeparam name="T">返回对象类型</typeparam>
		/// <param name="type">对象类型</param>
		/// <param name="parameters">参数</param>
		/// <returns>新对象实例</returns>
		public static T FastInvoke<T>(this Type type, params object[] parameters)
		{
			return (T)type.FastInvoke(parameters);
		}

		/// <summary>创建对象实例</summary>
		/// <typeparam name="T">返回对象类型</typeparam>
		/// <param name="parameters">参数</param>
		/// <returns>新对象实例</returns>
		public static T FastInvoke<T>(params object[] parameters)
		{
			return FastInvoke<T>(typeof(T), parameters);
		}

		/// <summary>通过对象类型名称创建对象实例</summary>
		/// <param name="typeName">对象类型名称，格式为“类型名, 程序集名”，如：NetRube.Plugin.Mail, NetRube.Plugin</param>
		/// <param name="parameters">参数</param>
		/// <returns>新对象实例</returns>
		public static object FastInvoke(string typeName, params object[] parameters)
		{
			return FastInvoke(FastGetType(typeName), parameters);
		}

		/// <summary>通过对象类型名称创建对象实例</summary>
		/// <typeparam name="T">返回对象类型</typeparam>
		/// <param name="typeName">对象类型名称，格式为“类型名, 程序集名”，如：NetRube.Plugin.Mail, NetRube.Plugin</param>
		/// <param name="parameters">参数</param>
		/// <returns>新对象实例</returns>
		public static T FastInvoke<T>(string typeName, params object[] parameters)
		{
			return (T)FastInvoke(typeName, parameters);
		}
		#endregion

		#region 方法
		private static Dict<MethodInfo, Func<object, object[], object>> MC;

		/// <summary>快速调用对象实例的方法</summary>
		/// <param name="methodInfo">要调用的方法</param>
		/// <param name="instance">对象实例</param>
		/// <param name="parameters">参数</param>
		/// <returns>方法执行的结果</returns>
		public static object FastInvoke(this MethodInfo methodInfo, object instance, params object[] parameters)
		{
			if(methodInfo == null) return null;

			if(MC == null) MC = new Dict<MethodInfo, Func<object, object[], object>>();
			var exec = MC.Get(methodInfo, () =>
			{
				var objectType = typeof(object);
				var objectsType = typeof(object[]);

				var instanceParameter = Expression.Parameter(objectType, "instance");
				var parametersParameter = Expression.Parameter(objectsType, "parameters");

				var parameterExpressions = new List<Expression>();
				var paramInfos = methodInfo.GetParameters();
				for(int i = 0, l = paramInfos.Length; i < l; i++)
				{
					var valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
					var valueCast = Expression.Convert(valueObj, paramInfos[i].ParameterType);

					parameterExpressions.Add(valueCast);
				}

				var instanceCast = methodInfo.IsStatic ? null : Expression.Convert(instanceParameter, methodInfo.ReflectedType);
				var methodCall = Expression.Call(instanceCast, methodInfo, parameterExpressions);

				if(methodCall.Type == typeof(void))
				{
					var lambda = Expression.Lambda<Action<object, object[]>>(methodCall, instanceParameter, parametersParameter);

					var act = lambda.Compile();
					return (inst, para) =>
					{
						act(inst, para);
						return null;
					};
				}
				else
				{
					var castMethodCall = Expression.Convert(methodCall, objectType);
					var lambda = Expression.Lambda<Func<object, object[], object>>(
						castMethodCall, instanceParameter, parametersParameter);

					return lambda.Compile();
				}
			});
			return exec(instance, parameters);
		}

		/// <summary>通过方法名称快速调用对象实例的方法</summary>
		/// <param name="methodName">方法名称</param>
		/// <param name="instance">对象实例</param>
		/// <param name="parameters">参数</param>
		/// <returns>方法执行的结果</returns>
		public static object FastInvoke(string methodName, object instance, params object[] parameters)
		{
			if(methodName.IsNullOrEmpty_()) return null;
			int paramsLen = parameters.Length;
			var typeArray = new Type[paramsLen];
			for(int i = 0; i < paramsLen; i++)
				typeArray[i] = parameters[i].GetType();
			var method = instance.GetType().GetMethod(methodName, typeArray);
			return method.FastInvoke(instance, parameters);
		}
		#endregion

		#region 属性
		#region 获取
		private static Dict<PropertyInfo, Func<object, object>> PGC;

		/// <summary>快速获取对象属性值</summary>
		/// <param name="propertyInfo">要获取的对象属性</param>
		/// <param name="instance">要获取的对象实例</param>
		/// <returns>属性值</returns>
		public static object FastGetValue(this PropertyInfo propertyInfo, object instance)
		{
			if(propertyInfo == null) return null;
			if(!propertyInfo.CanRead) return null;

			if(PGC == null) PGC = new Dict<PropertyInfo, Func<object, object>>();
			var exec = PGC.Get(propertyInfo, () =>
			{
				if(propertyInfo.GetIndexParameters().Length > 0)
				{
					// 对索引属性直接返回默认值
					return (inst) =>
					{
						return null;
					};
				}
				else
				{
					var objType = typeof(object);
					var instanceParameter = Expression.Parameter(objType, "instance");
					var instanceCast = propertyInfo.GetGetMethod(true).IsStatic ? null : Expression.Convert(instanceParameter, propertyInfo.ReflectedType);
					var propertyAccess = Expression.Property(instanceCast, propertyInfo);
					var castPropertyValue = Expression.Convert(propertyAccess, objType);
					var lambda = Expression.Lambda<Func<object, object>>(castPropertyValue, instanceParameter);

					return lambda.Compile();
				}
			});
			return exec(instance);
		}

		/// <summary>通过属性名称快速获取对象属性值</summary>
		/// <param name="propertyName">要获取的对象属性名称</param>
		/// <param name="instance">要获取的对象实例</param>
		/// <returns>属性值</returns>
		public static object FastGetPropertyValue(string propertyName, object instance)
		{
			if(propertyName.IsNullOrEmpty_()) return null;
			var propertyInfo = instance.GetType().GetProperty(propertyName);
			return propertyInfo.FastGetValue(instance);
		}

		/// <summary>快速获取对象属性值</summary>
		/// <typeparam name="T">返回的数据类型</typeparam>
		/// <param name="propertyInfo">要获取的对象属性</param>
		/// <param name="instance">要获取的对象实例</param>
		/// <returns>属性值</returns>
		public static T FastGetValue<T>(this PropertyInfo propertyInfo, object instance)
		{
			return (T)propertyInfo.FastGetValue(instance);
		}

		/// <summary>通过属性名称快速获取对象属性值</summary>
		/// <typeparam name="T">返回的数据类型</typeparam>
		/// <param name="propertyName">要获取的对象属性名称</param>
		/// <param name="instance">要获取的对象实例</param>
		/// <returns>属性值</returns>
		public static T FastGetPropertyValue<T>(string propertyName, object instance)
		{
			return (T)FastGetPropertyValue(propertyName, instance);
		}
		#endregion

		#region 设置
		private static Dict<PropertyInfo, Action<object, object>> PSC;

		/// <summary>快速设置对象属性值</summary>
		/// <param name="propertyInfo">要设置的对象属性</param>
		/// <param name="instance">要设置的对象实例</param>
		/// <param name="value">要设置的值</param>
		public static void FastSetValue(this PropertyInfo propertyInfo, object instance, object value)
		{
			if(propertyInfo == null) return;
			if(!propertyInfo.CanWrite) return;

			if(PSC == null) PSC = new Dict<PropertyInfo, Action<object, object>>();
			var exec = PSC.Get(propertyInfo, () =>
			{
				if(propertyInfo.GetIndexParameters().Length > 0)
				{
					// 对索引属性直接无视
					return (inst, para) => { };
				}
				else
				{
					var objType = typeof(object);
					var instanceParameter = Expression.Parameter(objType, "instance");
					var valueParameter = Expression.Parameter(objType, "value");
					var instanceCast = propertyInfo.GetSetMethod(true).IsStatic ? null : Expression.Convert(instanceParameter, propertyInfo.ReflectedType);
					var valueCast = Expression.Convert(valueParameter, propertyInfo.PropertyType);
					var propertyAccess = Expression.Property(instanceCast, propertyInfo);
					var propertyAssign = Expression.Assign(propertyAccess, valueCast);
					var lambda = Expression.Lambda<Action<object, object>>(propertyAssign, instanceParameter, valueParameter);

					return lambda.Compile();
				}
			});
			exec(instance, value);
		}

		/// <summary>通过属性名称快速设置对象属性值</summary>
		/// <param name="propertyName">要设置的对象属性名称</param>
		/// <param name="instance">要设置的对象实例</param>
		/// <param name="value">要设置的值</param>
		public static void FastSetPropertyValue(string propertyName, object instance, object value)
		{
			if(propertyName.IsNullOrEmpty_()) return;
			var propertyInfo = instance.GetType().GetProperty(propertyName);
			propertyInfo.FastSetValue(instance, value);
		}
		#endregion
		#endregion

		#region 字段
		#region 读取
		private static Dict<FieldInfo, Func<object, object>> FGC;

		/// <summary>快速获取对象字段值</summary>
		/// <param name="fieldInfo">要获取的对象字段</param>
		/// <param name="instance">要获取的对象实例</param>
		/// <returns>属性值</returns>
		public static object FastGetValue(this FieldInfo fieldInfo, object instance)
		{
			if(fieldInfo == null) return null;

			if(FGC == null) FGC = new Dict<FieldInfo, Func<object, object>>();
			var exec = FGC.Get(fieldInfo, () =>
			{
				var objType = typeof(object);
				var instanceParameter = Expression.Parameter(objType, "instance");
				var instanceCast = fieldInfo.IsStatic ? null : Expression.Convert(instanceParameter, fieldInfo.ReflectedType);
				var fieldAccess = Expression.Field(instanceCast, fieldInfo);
				var castFieldValue = Expression.Convert(fieldAccess, objType);
				var lambda = Expression.Lambda<Func<object, object>>(castFieldValue, instanceParameter);

				return lambda.Compile();
			});
			return exec(instance);
		}

		/// <summary>通过字段名称快速获取对象字段值</summary>
		/// <param name="fieldName">要获取的对象字段名称</param>
		/// <param name="instance">要获取的对象实例</param>
		/// <returns>属性值</returns>
		public static object FastGetFieldValue(string fieldName, object instance)
		{
			if(fieldName.IsNullOrEmpty_()) return null;
			var fieldInfo = instance.GetType().GetField(fieldName);
			return fieldInfo.FastGetValue(instance);
		}

		/// <summary>快速获取对象字段值</summary>
		/// <typeparam name="T">返回的数据类型</typeparam>
		/// <param name="fieldInfo">要获取的对象字段</param>
		/// <param name="instance">要获取的对象实例</param>
		/// <returns>属性值</returns>
		public static T FastGetValue<T>(this FieldInfo fieldInfo, object instance)
		{
			return (T)fieldInfo.FastGetValue(instance);
		}

		/// <summary>通过字段名称快速获取对象字段值</summary>
		/// <typeparam name="T">返回的数据类型</typeparam>
		/// <param name="fieldName">要获取的对象字段名称</param>
		/// <param name="instance">要获取的对象实例</param>
		/// <returns>属性值</returns>
		public static T FastGetFieldValue<T>(string fieldName, object instance)
		{
			return (T)FastGetFieldValue(fieldName, instance);
		}
		#endregion

		#region 设置
		private static Dict<FieldInfo, Action<object, object>> FSC;

		/// <summary>快速设置对象字段值</summary>
		/// <param name="fieldInfo">要设置的对象字段</param>
		/// <param name="instance">要设置的对象实例</param>
		/// <param name="value">要设置的值</param>
		public static void FastSetValue(this FieldInfo fieldInfo, object instance, object value)
		{
			if(fieldInfo == null) return;

			if(FSC == null) FSC = new Dict<FieldInfo, Action<object, object>>();
			var exec = FSC.Get(fieldInfo, () =>
			{
				var objType = typeof(object);
				var instanceParameter = Expression.Parameter(objType, "instance");
				var valueParameter = Expression.Parameter(objType, "value");
				var instanceCast = fieldInfo.IsStatic ? null : Expression.Convert(instanceParameter, fieldInfo.ReflectedType);
				var valueCast = Expression.Convert(valueParameter, fieldInfo.FieldType);
				var fieldAccess = Expression.Field(instanceCast, fieldInfo);
				var fieldAssign = Expression.Assign(fieldAccess, valueCast);
				var lambda = Expression.Lambda<Action<object, object>>(fieldAssign, instanceParameter, valueParameter);

				return lambda.Compile();
			});
			exec(instance, value);
		}

		/// <summary>通过字段名称快速设置对象字段值</summary>
		/// <param name="fieldName">要设置的对象字段名称</param>
		/// <param name="instance">要设置的对象实例</param>
		/// <param name="value">要设置的值</param>
		public static void FastSetFieldValue(string fieldName, object instance, object value)
		{
			if(fieldName.IsNullOrEmpty_()) return;
			var fieldInfo = instance.GetType().GetField(fieldName);
			fieldInfo.FastSetValue(instance, value);
		}
		#endregion
		#endregion

		#region 类型相关
		#region 获取类型
		private static Dict<string, Type> NTC;

		/// <summary>通过类型的程序集限定名称获取类型</summary>
		/// <param name="typeName">要获取的类型的程序集限定名称</param>
		/// <returns>类型</returns>
		/// <exception cref="ArgumentNullOrEmptyException">类型的程序集限定名称为 null 或为空</exception>
		public static Type FastGetType(string typeName)
		{
			if(typeName.IsNullOrEmpty_())
				throw new ArgumentNullOrEmptyException();

			if(NTC == null) NTC = new Dict<string, Type>(StringComparer.OrdinalIgnoreCase);
			typeName = typeName.Trim();
			return NTC.Get(typeName, () =>
			{
				return Type.GetType(typeName, true, true);
			});
		}
		#endregion

		#region 获取类型访问器
		private static Dict<string, Dictionary<string, Accessor>> AC;

		/// <summary>获取类型的访问器集合</summary>
		/// <param name="type">要获取的类型</param>
		/// <returns>类型的访问器集合</returns>
		public static Dictionary<string, Accessor> FastGetAccessors(this Type type)
		{
			if(AC == null) AC = new Dict<string, Dictionary<string, Accessor>>(StringComparer.OrdinalIgnoreCase);

			var typeName = type.FullName;
			return AC.Get(typeName, () =>
			{
				var ls = new Dictionary<string, Accessor>(StringComparer.OrdinalIgnoreCase);
				var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
				var name = string.Empty;
				foreach(var p in props)
				{
					name = p.Name;
					var a = new Accessor
					{
						Name = name,
						Member = p,
						DataType = p.PropertyType,
						AccessorType = AccessorType.Property,
						CanRade = p.CanRead,
						CanWrite = p.CanWrite,
						IsVirtual = p.IsVirtual_()
					};
					ls.Add(name, a);
				}

				var fis = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach(var f in fis)
				{
					name = f.Name;
					var a = new Accessor
					{
						Name = name,
						Member = f,
						DataType = f.FieldType,
						AccessorType = AccessorType.Field,
						CanRade = true,
						CanWrite = true,
						IsVirtual = false
					};
					ls.Add(name, a);
				}

				return ls;
			});
		}

		/// <summary>获取类型的访问器集合</summary>
		/// <typeparam name="T">要获取的类型</typeparam>
		/// <returns>类型的访问器集合</returns>
		public static Dictionary<string, Accessor> FastGetAccessors<T>()
		{
			return typeof(T).FastGetAccessors();
		}
		#endregion
		#endregion
	}
}