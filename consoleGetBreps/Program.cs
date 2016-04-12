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
			List<IfcFacetedBrep> breps = getBreps(rootElement);
			//Console.Out.Write("Number of Stories in file :" + buildingStoreyCount);
		}

		static List<IfcFacetedBrep> getBreps(IfcProduct product)
		{
			List<IfcFacetedBrep> result = new List<IfcFacetedBrep>();
			if (product == null)
				return result;
			IfcSpatialElement spatial = product as IfcSpatialElement;
			if(spatial != null)
			{
				List<IfcRelContainedInSpatialStructure> containers = spatial.ContainsElements;
				foreach (IfcRelContainedInSpatialStructure rcc in containers)
				{
					List<IfcProduct> related = rcc.RelatedElements;
					foreach (IfcProduct p in related)
						result.AddRange(getBreps(p));
				}
			}
			List<IfcRelAggregates> aggregates = product.IsDecomposedBy;
			foreach(IfcRelAggregates rag in aggregates)
			{
				List<IfcObjectDefinition> objects = rag.RelatedObjects;
				foreach(IfcObjectDefinition obj in objects)
				{
					IfcProduct p = obj as IfcProduct;
					if (p != null)
						result.AddRange(getBreps(p));	
				}
			}
			IfcBuildingElement element = product as IfcBuildingElement;
			if (product != null)
			{
				List<IfcRepresentation> reps = element.Representation.Representations;
				foreach (IfcRepresentation rep in reps)
				{
					IfcShapeRepresentation sr = rep as IfcShapeRepresentation;
					if (sr != null)
					{
						List<IfcRepresentationItem> items = sr.Items;
						foreach(IfcRepresentationItem item in items)
						{
							IfcFacetedBrep fb = item as IfcFacetedBrep;
							if (fb != null)
								result.Add(fb);
						}
	
					}
				}
			}
			return result;
		}
	}
}
