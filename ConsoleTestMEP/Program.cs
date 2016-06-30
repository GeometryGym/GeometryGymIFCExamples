using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using GeometryGym.Ifc;

namespace ConsoleTestMEP
{
	class Program
	{
		//Example files for https://sourceforge.net/p/ifcexporter/discussion/general/thread/9e08650b/

		static void Main(string[] args)
		{
			DirectoryInfo di = Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
			di = Directory.GetParent(di.FullName);

			string path = Path.Combine(di.FullName, "examples");
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			Generate(TypeIFC.AirTerminal, ReleaseVersion.IFC2x3, path);
			Generate(TypeIFC.AirTerminal, ReleaseVersion.IFC4A1, path);
			Generate(TypeIFC.Chiller, ReleaseVersion.IFC2x3, path);
			Generate(TypeIFC.Chiller, ReleaseVersion.IFC4A1, path);
			Generate(TypeIFC.ElectricPoint, ReleaseVersion.IFC2x3, path);

		}
		internal enum TypeIFC { Chiller, AirTerminal, ElectricPoint };
		static void Generate(TypeIFC type, ReleaseVersion release, string path)
		{
			DatabaseIfc db = new DatabaseIfc(true,release);
			IfcBuilding building = new IfcBuilding(db, "IfcBuilding") { };
			IfcProject project = new IfcProject(building, "IfcProject", IfcUnitAssignment.Length.Millimetre) { };

			IfcRectangleProfileDef rect = new IfcRectangleProfileDef(db, "Rect", 1000, 500);
			IfcExtrudedAreaSolid extrusion = new IfcExtrudedAreaSolid(rect, new IfcAxis2Placement3D(new IfcCartesianPoint(db, 0, 0, 0)), new IfcDirection(db, 0, 0, 1), 2000);
			IfcProductDefinitionShape rep = new IfcProductDefinitionShape(new IfcShapeRepresentation(extrusion));

			if (type == TypeIFC.Chiller)
			{
				IfcChiller chiller = new IfcChiller(building, null, rep, null) { PredefinedType = IfcChillerTypeEnum.AIRCOOLED };
				if(release == ReleaseVersion.IFC2x3)
					chiller.RelatingType = new IfcChillerType(db, "MyChillerType", IfcChillerTypeEnum.AIRCOOLED);
			}
			else if (type == TypeIFC.AirTerminal)
			{
				IfcAirTerminal terminal = new IfcAirTerminal(building, null, rep, null) { PredefinedType = IfcAirTerminalTypeEnum.DIFFUSER };
				if (release == ReleaseVersion.IFC2x3)
					terminal.RelatingType = new IfcAirTerminalType(db, "MyAirTerminalType", IfcAirTerminalTypeEnum.DIFFUSER);
			}
			else
			{
				IfcElectricDistributionPoint point = new IfcElectricDistributionPoint(building, null, rep, null) { DistributionPointFunction = IfcElectricDistributionPointFunctionEnum.DISTRIBUTIONBOARD };
			}
			db.WriteFile(Path.Combine(path, type.ToString() + " " + release.ToString() + ".ifc"));
		}
	}
}
