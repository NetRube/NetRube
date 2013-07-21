using System;
using System.Collections.Concurrent;
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
		private static ConcurrentDictionary<ConstructorInfo, Delegate> OC;

		/// <summary>通过类的构造器信息创建对象实例</summary>
		/// <param name="constructorInfo">构造器信息</param>
		/// <param name="parameters">参数</param>
		/// <returns>新对象实例</returns>
		public static object FastInvoke(this ConstructorInfo constructorInfo, params object[] parameters)
		{
			if(OC == null) OC = new ConcurrentDictionary<ConstructorInfo, Delegate>();
			Delegate exec;
			if(!OC.TryGetValue(constructorInfo, out exec))
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

				exec = lambda.Compile();
				OC.TryAdd(constructorInfo, exec);
			}
			return ((Func<object[], object>)exec)(parameters);
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
		private static ConcurrentDictionary<MethodInfo, Delegate> MC;

		/// <summary>快速调用对象实例的方法</summary>
		/// <param name="methodInfo">要调用的方法</param>
		/// <param name="instance">对象实例</param>
		/// <param name="parameters">参数</param>
		/// <returns>方法执行的结果</returns>
		public static object FastInvoke(this MethodInfo methodInfo, object instance, params object[] parameters)
		{
			if(methodInfo == null) return null;

			if(MC == null) MC = new ConcurrentDictionary<MethodInfo, Delegate>();
			Delegate exec;
			if(!MC.TryGetValue(methodInfo, out exec))
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
					Func<object, object[], object> execute = (inst, para) =>
					{
						act(inst, para);
						return null;
					};
					exec = execute;
				}
				else
				{
					var castMethodCall = Expression.Convert(methodCall, objectType);
					var lambda = Expression.Lambda<Func<object, object[], object>>(
						castMethodCall, instanceParameter, parametersParameter);

					exec = lambda.Compile();
				}
				MC.TryAdd(methodInfo, exec);
			}
			return ((Func<object, object[], object>)exec)(instance, parameters);
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
		private static ConcurrentDictionary<PropertyInfo, Delegate> PGC;

		/// <summary>快速获取对象属性值</summary>
		/// <param name="propertyInfo">要获取的对象属性</param>
		/// <param name="instance">要获取的对象实例</param>
		/// <returns>属性值</returns>
		public static object FastGetValue(this PropertyInfo propertyInfo, object instance)
		{
			if(propertyInfo == null) return null;
			if(!propertyInfo.CanRead) return null;

			if(PGC == null) PGC = new ConcurrentDictionary<PropertyInfo, Delegate>();
			Delegate exec;
			if(!PGC.TryGetValue(propertyInfo, out exec))
			{
				if(propertyInfo.GetIndexParameters().Length > 0)
				{
					// 对索引属性直接返回默认值
					Func<object, object> execute = (inst) =>
					{
						return null;
					};
					exec = execute;
				}
				else
				{
					var objType = typeof(object);
					var instanceParameter = Expression.Parameter(objType, "instance");
					var instanceCast = propertyInfo.GetGetMethod(true).IsStatic ? null : Expression.Convert(instanceParameter, propertyInfo.ReflectedType);
					var propertyAccess = Expression.Property(instanceCast, propertyInfo);
					var castPropertyValue = Expression.Convert(propertyAccess, objType);
					var lambda = Expression.Lambda<Func<object, object>>(castPropertyValue, instanceParameter);

					exec = lambda.Compile();
				}
				PGC.TryAdd(propertyInfo, exec);
			}
			return ((Func<object, object>)exec)(instance);
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
		private static ConcurrentDictionary<PropertyInfo, Delegate> PSC;

		/// <summary>快速设置对象属性值</summary>
		/// <param name="propertyInfo">要设置的对象属性</param>
		/// <param name="instance">要设置的对象实例</param>
		/// <param name="value">要设置的值</param>
		public static void FastSetValue(this PropertyInfo propertyInfo, object instance, object value)
		{
			if(propertyInfo == null) return;
			if(!propertyInfo.CanWrite) return;

			if(PSC == null) PSC = new ConcurrentDictionary<PropertyInfo, Delegate>();
			Delegate exec;
			if(!PSC.TryGetValue(propertyInfo, out exec))
			{
				if(propertyInfo.GetIndexParameters().Length > 0)
				{
					// 对索引属性直接无视
					Action<object, object> execute = (inst, para) => { };
					exec = execute;
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

					exec = lambda.Compile();
				}
				PSC.TryAdd(propertyInfo, exec);
			}
			((Action<object, object>)exec)(instance, value);
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
		private static ConcurrentDictionary<FieldInfo, Delegate> FGC;

		/// <summary>快速获取对象字段值</summary>
		/// <param name="fieldInfo">要获取的对象字段</param>
		/// <param name="instance">要获取的对象实例</param>
		/// <returns>属性值</returns>
		public static object FastGetValue(this FieldInfo fieldInfo, object instance)
		{
			if(fieldInfo == null) return null;

			if(FGC == null) FGC = new ConcurrentDictionary<FieldInfo, Delegate>();
			Delegate exec;
			if(!FGC.TryGetValue(fieldInfo, out exec))
			{
				var objType = typeof(object);
				var instanceParameter = Expression.Parameter(objType, "instance");
				var instanceCast = fieldInfo.IsStatic ? null : Expression.Convert(instanceParameter, fieldInfo.ReflectedType);
				var fieldAccess = Expression.Field(instanceCast, fieldInfo);
				var castFieldValue = Expression.Convert(fieldAccess, objType);
				var lambda = Expression.Lambda<Func<object, object>>(castFieldValue, instanceParameter);

				exec = lambda.Compile();
				FGC.TryAdd(fieldInfo, exec);
			}
			return ((Func<object, object>)exec)(instance);
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
		private static ConcurrentDictionary<FieldInfo, Delegate> FSC;

		/// <summary>快速设置对象字段值</summary>
		/// <param name="fieldInfo">要设置的对象字段</param>
		/// <param name="instance">要设置的对象实例</param>
		/// <param name="value">要设置的值</param>
		public static void FastSetValue(this FieldInfo fieldInfo, object instance, object value)
		{
			if(fieldInfo == null) return;

			if(FSC == null) FSC = new ConcurrentDictionary<FieldInfo, Delegate>();
			Delegate exec;
			if(!FSC.TryGetValue(fieldInfo, out exec))
			{
				var objType = typeof(object);
				var instanceParameter = Expression.Parameter(objType, "instance");
				var valueParameter = Expression.Parameter(objType, "value");
				var instanceCast = fieldInfo.IsStatic ? null : Expression.Convert(instanceParameter, fieldInfo.ReflectedType);
				var valueCast = Expression.Convert(valueParameter, fieldInfo.FieldType);
				var fieldAccess = Expression.Field(instanceCast, fieldInfo);
				var fieldAssign = Expression.Assign(fieldAccess, valueCast);
				var lambda = Expression.Lambda<Action<object, object>>(fieldAssign, instanceParameter, valueParameter);

				exec = lambda.Compile();
				FSC.TryAdd(fieldInfo, exec);
			}
			((Action<object, object>)exec)(instance, value);
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
		private static ConcurrentDictionary<Type, TypeNames> TNC;
		private static TypeNames GetTypeNames(Type type)
		{
			if(TNC == null) TNC = new ConcurrentDictionary<Type, TypeNames>();

			TypeNames tns;
			if(!TNC.TryGetValue(type, out tns))
			{
				tns = new TypeNames
				{
					TypeName = type.Name,
					FullName = type.FullName,
					AssemblyName = type.AssemblyQualifiedName
				};
				TNC.TryAdd(type, tns);
			}
			return tns;
		}

		/// <summary>快速获取类型的程序集限定名称</summary>
		/// <param name="type">要获取的类型</param>
		/// <returns>类型的程序集限定名称</returns>
		public static string FastGetAssemblyName(this Type type)
		{
			return GetTypeNames(type).AssemblyName;
		}

		/// <summary>快速获取类型的程序集限定名称</summary>
		/// <typeparam name="T">要获取的类型</param>
		/// <returns>类型的程序集限定名称</returns>
		public static string FastGetTypeAssemblyName<T>()
		{
			return GetTypeNames(typeof(T)).AssemblyName;
		}

		/// <summary>快速获取类型名称</summary>
		/// <param name="type">要获取的类型</param>
		/// <returns>类型名称</returns>
		public static string FastGetName(this Type type)
		{
			return GetTypeNames(type).TypeName;
		}

		/// <summary>快速获取类型名称</summary>
		/// <typeparam name="T">要获取的类型</param>
		/// <returns>类型名称</returns>
		public static string FastGetTypeName<T>()
		{
			return GetTypeNames(typeof(T)).TypeName;
		}

		/// <summary>快速获取类型完全限定名称</summary>
		/// <param name="type">要获取的类型</param>
		/// <returns>类型完全限定名称</returns>
		public static string FastGetFullName(this Type type)
		{
			return GetTypeNames(type).FullName;
		}

		/// <summary>快速获取类型完全限定名称</summary>
		/// <typeparam name="T">要获取的类型</param>
		/// <returns>类型完全限定名称</returns>
		public static string FastGetTypeFullName<T>()
		{
			return GetTypeNames(typeof(T)).FullName;
		}

		private static ConcurrentDictionary<string, Type> NTC;

		/// <summary>通过类型的程序集限定名称获取类型</summary>
		/// <param name="typeName">要获取的类型的程序集限定名称</param>
		/// <returns>类型</returns>
		/// <exception cref="ArgumentNullOrEmptyException">类型的程序集限定名称为 null 或为空</exception>
		public static Type FastGetType(string typeName)
		{
			if(typeName.IsNullOrEmpty_())
				throw new ArgumentNullOrEmptyException();

			if(NTC == null) NTC = new ConcurrentDictionary<string, Type>();
			typeName = typeName.Trim();
			Type val = null;
			if(!NTC.TryGetValue(typeName, out val))
			{
				val = Type.GetType(typeName, true, true);
				NTC.TryAdd(typeName, val);
			}
			return val;
		}

		private static ConcurrentDictionary<string, Dictionary<string, Accessor>> AC;

		/// <summary>获取类型的访问器集合</summary>
		/// <param name="type">要获取的类型</param>
		/// <returns>类型的访问器集合</returns>
		public static Dictionary<string, Accessor> FastGetAccessors(this Type type)
		{
			if(AC == null) AC = new ConcurrentDictionary<string, Dictionary<string, Accessor>>();

			Dictionary<string, Accessor> ls;
			var typeName = type.FastGetFullName();
			if(!AC.TryGetValue(typeName, out ls))
			{
				ls = new Dictionary<string, Accessor>();
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

				AC.TryAdd(typeName, ls);
			}

			return ls;
		}

		/// <summary>获取类型的访问器集合</summary>
		/// <typeparam name="T">要获取的类型</param>
		/// <returns>类型的访问器集合</returns>
		public static Dictionary<string, Accessor> FastGetAccessors<T>()
		{
			return typeof(T).FastGetAccessors();
		}
		#endregion
	}
}