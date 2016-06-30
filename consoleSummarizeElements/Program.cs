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
			List<IfcBuildingElement> elements = project.Extract<IfcBuildingElement>();
			foreach(IfcBuildingElement element in elements)
			{
				double volume = 0.0;
				IfcMaterial ifcmaterial = extractMaterial(element.MaterialSelect);
				string material = ifcmaterial == null ? "" : ifcmaterial.Name;
				
//                myPart.GetAttribute(“MATERIAL”, ref material);
	//			myPart.GetAttribute(“VOLUME”, ref volume);
			}
			
		}
		
		static IfcMaterial extractMaterial(IfcMaterialSelect materialSelect) //To be enabled in opensource Library
		{
			IfcMaterial material = materialSelect as IfcMaterial;
			if (material != null)
				return material;
			IfcMaterialProfile profile = materialSelect as IfcMaterialProfile;
			if (profile != null)
				return profile.Material;
			IfcMaterialProfileSet profileSet = materialSelect as IfcMaterialProfileSet;
			if (profileSet == null)
			{
				IfcMaterialProfileSetUsage profileUsage = materialSelect as IfcMaterialProfileSetUsage;
				if (profileUsage != null)
					profileSet = profileUsage.ForProfileSet;
			}
			if (profileSet != null)
				return profileSet.PrimaryMaterial;
			IfcMaterialLayer layer = materialSelect as IfcMaterialLayer;
			if (layer != null)
				return layer.Material;
			IfcMaterialLayerSet layerSet = materialSelect as IfcMaterialLayerSet;
			if (layerSet != null)
				return layerSet.PrimaryMaterial;
			IfcMaterialLayerSetUsage layerSetUsage = materialSelect as IfcMaterialLayerSetUsage;
			if (layerSetUsage != null)
				return layerSetUsage.PrimaryMaterial;
			IfcMaterialList list = materialSelect as IfcMaterialList;
			if (list != null)
				return list.PrimaryMaterial;
			IfcMaterialConstituent constituent = materialSelect as IfcMaterialConstituent;
			if (constituent != null)
				return constituent.PrimaryMaterial;
			IfcMaterialConstituentSet constituentSet = materialSelect as IfcMaterialConstituentSet;
			if (constituentSet != null)
				return constituentSet.PrimaryMaterial;
			return null;
		}

	}
}
