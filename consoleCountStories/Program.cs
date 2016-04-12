using System;
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
			IfcSpatialElement rootElement = project.RootElement;
			int buildingStoreyCount = CountStories(rootElement);
			Console.Out.Write("Number of Stories in file :" + buildingStoreyCount);
		}

		static int CountStories(IfcSpatialElement element)
		{
			if (element == null)
				return 0;
			int result = 0;
			List<IfcRelAggregates> aggregates = element.IsDecomposedBy;
			foreach (IfcRelAggregates rag in aggregates)
			{
				List<IfcObjectDefinition> objects = rag.RelatedObjects;
				foreach (IfcObjectDefinition obj in objects)
				{
					IfcBuildingStorey storey = obj as IfcBuildingStorey;
					if (storey != null)
						result++;
					else
					{
						IfcSpatialElement se = obj as IfcSpatialElement;
						if (se != null)
							result += CountStories(se);
					}
				}
			}
			return result;
		}
	}
}
