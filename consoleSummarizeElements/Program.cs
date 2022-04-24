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
		internal class MyElement
		{
			internal string mMark = "", mDescription = "", mSection = "", mGrade = "";
			internal double mLength = 0;
			internal int mQuantity = 1;
			internal MyElement(string mark, string description, string section, string grade, double length)
			{
				mMark = mark;
				mDescription = description;
				mSection = section;
				mGrade = grade;
				mLength = length;
			}
		}
		static void Main(string[] args) //Example as requested at http://forums.autodesk.com/t5/revit-api/opensource-api-for-reading-ifc-files/m-p/6435644#M17340
		{
			DirectoryInfo di = Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
			di = Directory.GetParent(di.FullName);

			string filename = Path.Combine(di.FullName, "IFC Model.ifc");
			DatabaseIfc db = new DatabaseIfc(filename);
			IfcProject project = db.Project;
			List<IfcBuiltElement> elements = project.Extract<IfcBuiltElement>(); //IfcBuiltElement renamed from IfcBuildingElement
			Dictionary<string, MyElement> dictionary = new Dictionary<string, MyElement>();
			foreach(IfcBuiltElement element in elements)
			{
				string desc = (element as IfcColumn != null ? "COL" : (element as IfcBeam != null ? "BEAM" : ""));

				string mark = element.Tag;
				if (!string.IsNullOrEmpty(desc))
				{
					if (dictionary.ContainsKey(mark))
						dictionary[mark].mQuantity++;
					else
					{
						string grade = "";
						double length = 0;
						foreach (IfcRelDefinesByProperties rdp in element.IsDefinedBy)
						{
							foreach (IfcPropertySet pset in rdp.RelatingPropertyDefinition.OfType<IfcPropertySet>())
							{
								foreach (System.Collections.Generic.KeyValuePair<string, IfcProperty> pair in pset.HasProperties)
								{
									IfcPropertySingleValue psv = pair.Value as IfcPropertySingleValue;
									if (psv == null)
										continue;
									if (string.Compare("Grade", psv.Name) == 0)
									{
										grade = psv.NominalValue.Value.ToString();
									}
									else if (string.Compare("Length", psv.Name) == 0)
									{
										IfcLengthMeasure lengthmeasure = psv.NominalValue as IfcLengthMeasure;
										if (lengthmeasure != null)
											length = lengthmeasure.Measure;
									}
								}
							}
						}
						dictionary.Add(mark, new MyElement(mark, desc, element.ObjectType, grade, length));
					}
				}
			}
			Console.WriteLine("Mark\tDescription\tSection\tGrade\tLength\tQty");

			foreach(MyElement element in dictionary.ToList().ConvertAll(x=>x.Value).OrderBy(x => x.mMark))
				Console.WriteLine(element.mMark + "\t" + element.mDescription + "\t" + element.mSection + "\t" + element.mGrade + "\t" + element.mLength + "\t" + element.mQuantity);
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
				return profileSet.PrimaryMaterial();
			IfcMaterialLayer layer = materialSelect as IfcMaterialLayer;
			if (layer != null)
				return layer.Material;
			IfcMaterialLayerSet layerSet = materialSelect as IfcMaterialLayerSet;
			if (layerSet != null)
				return layerSet.PrimaryMaterial();
			IfcMaterialLayerSetUsage layerSetUsage = materialSelect as IfcMaterialLayerSetUsage;
			if (layerSetUsage != null)
				return layerSetUsage.PrimaryMaterial();
			IfcMaterialList list = materialSelect as IfcMaterialList;
			if (list != null)
				return list.PrimaryMaterial();
			IfcMaterialConstituent constituent = materialSelect as IfcMaterialConstituent;
			if (constituent != null)
				return constituent.PrimaryMaterial();
			IfcMaterialConstituentSet constituentSet = materialSelect as IfcMaterialConstituentSet;
			if (constituentSet != null)
				return constituentSet.PrimaryMaterial();
			return null;
		}

	}
}
