using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetRube;

namespace NetRubeTest
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			var tn = Utils.GetPropertyInfo<B>(b => b.ID).ReflectedType;
			Assert.AreEqual(typeof(B), tn);
		}
	}

	internal class A
	{
		public int ID { get; set; }
	}

	internal class B : A { }
}