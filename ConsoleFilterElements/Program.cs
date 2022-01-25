using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GeometryGym.Ifc;

namespace ConsoleFilterElements
{
	class Program
	{
		//Demonstration of approach to filter out fire rated walls into seperate IFC file
		static void Main(string[] args)
		{
			DatabaseIfc db = new DatabaseIfc(args[0]);

			DuplicateOptions duplicateOptions = new DuplicateOptions(db.Tolerance);
			duplicateOptions.DuplicateDownstream = false;
			DatabaseIfc filteredDb = new DatabaseIfc(db);
			IfcProject project = filteredDb.Factory.Duplicate(db.Project, duplicateOptions) as IfcProject;

			duplicateOptions.DuplicateDownstream = true;

			List<IfcWall> walls = db.Project.Extract<IfcWall>();
			foreach(IfcWall wall in walls)
			{
				IfcPropertySingleValue property = wall.FindProperty("FireRating", true) as IfcPropertySingleValue;
				if(property != null && property.NominalValue != null)
				{
					string value = property.NominalValue.ValueString.Trim();
					if (value != "0")
						filteredDb.Factory.Duplicate(wall, duplicateOptions);
				}
			}

			filteredDb.WriteFile(args[1]);
		}
	}
}
