using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GeometryGym.Ifc;

namespace ConsoleCheckModelDemo
{
	internal class Program
	{
		static void Main(string[] args)
		{
			DatabaseIfc db = new DatabaseIfc(args[0]);
			
		}

	}

	internal class ModelCheckerDemo
	{
		internal void checkModel(DatabaseIfc db)
		{
			List<IfcWall> walls = db.OfType<IfcWall>().ToList();
			checkWallsFireRating(walls);
		}

		internal void checkWallsFireRating(List<IfcWall> walls)
		{
			foreach (IfcWall wall in walls)
			{
				if(isClassified("21.22", "NL-Sfb", wall))
				{
					checkProperty(wall, "AedesUVIP", "Firerating", true, new HashSet<string>() { "30", "60", "90" });	
				}
			}
		}
		internal void checkProperty(IfcObjectDefinition obj, string psetName, string propertyName, bool permitType, HashSet<String> allowableStrings)
		{
			IfcPropertySingleValue propertySingleValue = null;
			if (!string.IsNullOrEmpty(psetName))
			{
				IfcPropertySet propertySet = null;
				if (obj is IfcObject ifcObject)
					propertySet = ifcObject.FindPropertySet(psetName, permitType) as IfcPropertySet;
				else
					propertySet = obj.FindPropertySet(psetName) as IfcPropertySet;
				if (propertySet == null)
				{
					System.Diagnostics.Debug.WriteLine("Test Fail " + obj.GlobalId + " " + obj.Name + " missing pset " + psetName);
					return;
				}
				propertySingleValue = propertySet.FindProperty(propertyName) as IfcPropertySingleValue;
			}
			else
				propertySingleValue = obj.FindProperty(propertyName) as IfcPropertySingleValue;
			if(propertySingleValue == null)
			{
				System.Diagnostics.Debug.WriteLine("Test Fail " + obj.GlobalId + " " + obj.Name + " missing property " + propertyName);
				return;
			}
			if(propertySingleValue.NominalValue == null)
			{
				System.Diagnostics.Debug.WriteLine("Test Fail " + obj.GlobalId + " " + obj.Name + " property " + propertyName + " has no value!");
				return;
			}
			if(allowableStrings.Count > 0)
			{
				string propertyValue = propertySingleValue.NominalValue.ValueString;
				if(!allowableStrings.Contains(propertyValue))
				{
					System.Diagnostics.Debug.WriteLine("Test Fail " + obj.GlobalId + " " + obj.Name + " property " + propertyName + " has unacceptable value :" + propertyValue);
					return;
				}	

			}

		}
		internal bool isClassified(string classificationId, string classificationName, IfcObjectDefinition obj)
		{
			List<IfcRelAssociatesClassification> references = obj.HasAssociations.OfType<IfcRelAssociatesClassification>().ToList();
			if(obj is IfcObject ifcObject)
			{
				IfcTypeObject typeObject = ifcObject.RelatingType();
				if (typeObject != null)
					references.AddRange(typeObject.HasAssociations.OfType<IfcRelAssociatesClassification>());
			}

			foreach(IfcClassificationReference classificationReference in references.Select(x=>x.RelatingClassification).OfType<IfcClassificationReference>())
			{
				if (string.Compare(classificationId, classificationReference.Identification, true) == 0)
				{
					if (string.IsNullOrEmpty(classificationName))
						return true;
					IfcClassification classification = classificationReference.ReferencedClassification();
					if(classification != null && string.Compare(classification.Name, classificationName,true) == 0)
						return true;
				}
			}

			return false;
		}
	}
}
