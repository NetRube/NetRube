using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetRube;
using NetRube.FastJson;

namespace NetRubeTest
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			var em = new BB { ID = 2, Date = DateTime.Now };

			Console.WriteLine(em.ToJson_().ToObject_<BB>().ToJson_());

			//FastReflection.FastSetPropertyValue("NN", em, new int[] { 1, 2, 3 });
			//var nn = FastReflection.FastGetPropertyValue<int[]>("NN", em);
			//Assert.AreEqual(em.NN, nn);

			//var obj = em.Clone_();
			//Assert.AreEqual(em, obj);
			//var p = em.GetType().GetProperty("Item");
			//var m = p.GetSetMethod();
			//m.FastInvoke(em, 4);
			//p.FastSetValue(em, 2);
			//Assert.AreEqual(1, p.FastGetValue(em));

			//Assert.AreEqual(typeof(BB), em.GetType());
			
			//em.ID = 2;
			//Assert.AreEqual(2, em.ID);
			//object n = 1;
			//FastReflection.FastInvoke("SetID", em, n);
			//Assert.AreEqual(1, FastReflection.FastGetPropertyValue<int>("ID", em));
			//Assert.AreEqual("", ((object)9).GetType().Name);

			//var val = FastReflection.FastGetPropertyValue<BB, int>("ID", em);
			//Assert.AreEqual(1, val);

			//var id = FastReflection.FastInvoke<BB, int>("SetID", em, 1);
			//Assert.AreEqual(1, em.ID);

			//FastReflection.FastInvoke<BB, string>("SetName", em, "aa");
			//FastReflection.FastSetPropertyValue("Name", em, "aa");
			//Assert.AreEqual("aa", em.Name);

			//FastReflection.FastSetFieldValue("DD", em, 3);
			//var dd = FastReflection.FastGetFieldValue<BB, int>("DD", em);
			//Assert.AreEqual(2, dd);


			//object bb = new BB { ID = 1, Name = "aa" };
			//var dt = new System.Data.DataTable("TTT");
			//dt.Columns.Add("C1", typeof(int));
			//dt.Columns.Add("C2", typeof(string));
			//dt.Rows.Add(1, "aa");
			//dt.Rows.Add(2, "bb");
			////Assert.AreEqual("", Json.Instance.DeepCopy(dt).ToJson_());

			//var dt2 = dt.ToJson_().ToObject_<System.Data.DataTable>();
			//var ls = new List<object>(3);
			//ls.Add(bb);
			//ls.Add(bb);
			//ls.Add(bb);
			//Assert.AreEqual("", ls.ToJson_().ToObject_<List<BB>>()[0].Name);

			//var dt = new DateTime(2000, 1, 20);
			//var du = new DateTime(1970, 1, 1);
			//Console.WriteLine(du.Ticks);
			//Console.WriteLine((dt.Ticks - du.Ticks) / 10000);
			//Console.WriteLine((dt.ToUniversalTime().Ticks - du.ToUniversalTime().Ticks) / 10000);
			//var dt = new DateTime(946684800000 * 10000 + 621355680000000000, DateTimeKind.Local);
			//Console.WriteLine(dt);
			//Console.WriteLine(dt.ToLocalTime());
			//Console.WriteLine(dt.ToUniversalTime());
		}
	}

	internal class BB
	{
		//public int DD = 2;
		public int ID { get; set; }
		public int[] NN { get; set; }
		//public string Name { get; set; }
		//public int SetID(int id)
		//{
		//	ID = id;
		//	return ID;
		//}
		//public void SetName(string name)
		//{
		//	Name = name;
		//}
		private Dictionary<int,string> ls=new Dictionary<int,string>();
		public string this[int n]
		{
			get { return ls[n]; }
			set { ls[n] = value; }
		}
		public DateTime Date { get; set; }
	}

	internal class CC : BB
	{
		public int AA { get; set; }
	}
}