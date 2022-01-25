﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometryGym.Ifc;

namespace testConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			DatabaseIfc db = new DatabaseIfc(Console.In);
			IfcProject project = db.Project;
			int storeyCount = project.Extract<IfcBuildingStorey>().Count;
			Console.Out.Write("Number of Stories in file :" + storeyCount);
		}
	}
}
