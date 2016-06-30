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
			List<IfcBuildingElement> elements = project.Extract<IfcBuildingElement>();
			List<IfcFacetedBrep> breps = new List<IfcFacetedBrep>();
			foreach(IfcBuildingElement element in elements)
			{
				List<IfcRepresentation> reps = element.Representation.Representations;
				foreach (IfcRepresentation rep in reps)
				{
					IfcShapeRepresentation sr = rep as IfcShapeRepresentation;
					if (sr != null)
					{
						List<IfcRepresentationItem> items = sr.Items;
						foreach (IfcRepresentationItem item in items)
						{
							IfcFacetedBrep fb = item as IfcFacetedBrep;
							if (fb != null)
								breps.Add(fb);
						}

					}
				}
			}
		}
	}
}
