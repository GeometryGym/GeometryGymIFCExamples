using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using GeometryGym.Ifc;

namespace testConsole
{
	class Program
	{
		static void Main(string[] args) //Example as requested at http://forums.autodesk.com/t5/revit-api/opensource-api-for-reading-ifc-files/m-p/6435644#M17340
		{
			DirectoryInfo di = Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
			di = Directory.GetParent(di.FullName);

			string filename = Path.Combine(di.FullName, "IFC Model.ifc");
			DatabaseIfc db = new DatabaseIfc(filename);
			IfcProject project = db.Project;
			List<IfcBuildingElement> elements = project.Extract<IfcBuildingElement>();
			Console.WriteLine("Mark\tDescription\tSection\tGrade\tLength");//\tQty);
			foreach(IfcBuildingElement element in elements)
			{
				string desc = (element as IfcColumn != null ? "COL" : (element as IfcBeam != null ? "BEAM" : ""));

				//IfcMaterial material = element.MaterialSelect as IfcMaterial;
				//string grade = (material == null ? "" : material.Name);
				string grade = "";
				double length = 0;
				foreach(IfcRelDefinesByProperties rdp in element.IsDefinedBy)
				{
					IfcPropertySet pset = rdp.RelatingPropertyDefinition as IfcPropertySet;
					if (pset == null)
						continue;
					foreach(IfcProperty property in pset.HasProperties)
					{
						IfcPropertySingleValue psv = property as IfcPropertySingleValue;
						if (psv == null)
							continue;
						if(string.Compare("Grade",psv.Name) == 0)
						{
							grade = psv.NominalValue.Value.ToString();
						}
						else if (string.Compare("Length",psv.Name) == 0)
						{
							IfcLengthMeasure lengthmeasure = psv.NominalValue as IfcLengthMeasure;
							if (lengthmeasure != null)
								length = lengthmeasure.Measure;
						}
					}
				}
				if(!string.IsNullOrEmpty(desc))
				{
					Console.WriteLine(element.Tag + "\t" + desc + "\t" + element.ObjectType + "\t" + grade + "\t" + length);
				}
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
