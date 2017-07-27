using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometryGym.Ifc;

namespace ConsoleConvertToIFCJson
{
	class Program
	{
		static void Main(string[] args)
		{
			DatabaseIfc db = new DatabaseIfc( Console.In);
			Console.Out.Write(db.JSON);
		}
	}
}
