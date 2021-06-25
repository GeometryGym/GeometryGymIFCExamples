using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GeometryGym.Ifc;

namespace ConsoleClassificationDemo
{
	class Program
	{
		static void Main(string[] args)
		{
			DatabaseIfc db = new DatabaseIfc(ModelView.Ifc4X3NotAssigned);
			db.Factory.Options.GenerateOwnerHistory = false;

			IfcProject project = new IfcProject(db, "DemoProject");
			IfcProjectLibrary projectLibrary = new IfcProjectLibrary(db, "ClassificationLibrary");
			project.AddDeclared(projectLibrary);

			IfcClassification classification = new IfcClassification(db, "MyClassification");
			new IfcRelAssociatesClassification(classification, projectLibrary);

			IfcClassificationReference buildingElements = new IfcClassificationReference(classification) { Identification = "100", Name = "BuildingElements" };
			IfcClassificationReference walls = new IfcClassificationReference(buildingElements) { Identification = "100.100", Name = "Walls" };
			IfcClassificationReference partionWalls = new IfcClassificationReference(walls) { Identification = "100.100.002", Name = "PartiionWalls" };

			IfcSimplePropertyTemplate simplePropertyTemplate = new IfcSimplePropertyTemplate(db, "IsExternal") { GlobalId = "3Yss80qXKHuO00025QrE$V", PrimaryMeasureType = "IfcBoolean" };
			IfcPropertySetTemplate psetTemplate = new IfcPropertySetTemplate("Pset_WallCommon", simplePropertyTemplate) { GlobalId = "2VWFE0qXKHuO00025QrE$V" };

			IfcPropertySingleValue psv = new IfcPropertySingleValue(db, "IsExternal", new IfcBoolean(false));
			IfcPropertySet pset = new IfcPropertySet("Pset_WallCommon", psv);
			new IfcRelDefinesByTemplate(pset, psetTemplate);

			new IfcRelAssociatesClassification(partionWalls, pset);

			DirectoryInfo di = Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
			di = Directory.GetParent(di.FullName);
			db.WriteFile(Path.Combine(di.FullName, "TestClassification.ifc"));
		}
	}
}
