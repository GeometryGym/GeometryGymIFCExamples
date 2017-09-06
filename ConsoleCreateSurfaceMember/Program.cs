using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using GeometryGym.Ifc;

namespace ConsoleCreateSurfaceMember
{
	class Program
	{
		static void Main(string[] args)
		{
			DatabaseIfc db = new DatabaseIfc(ModelView.Ifc4DesignTransfer);
			IfcBuilding building = new IfcBuilding(db, "IfcBuilding") { };
			IfcProject project = new IfcProject(building, "IfcProject", IfcUnitAssignment.Length.Millimetre) { };

			IfcMaterial material = new IfcMaterial(db, "Concrete");

			IfcStructuralAnalysisModel analysisModel = new IfcStructuralAnalysisModel(building, "Analysis Model", IfcAnalysisModelTypeEnum.LOADING_3D);
			double y = -1309.6875;
			IfcCartesianPoint point = new IfcCartesianPoint(db, -3968.75, y, 3000.0);
			IfcPlane plane = new IfcPlane(new IfcAxis2Placement3D(point, db.Factory.YAxisNegative,db.Factory.XAxis));
			List<IfcFaceBound> bounds = new List<IfcFaceBound>();
			List<IfcCartesianPoint> points = new List<IfcCartesianPoint>();
			points.Add(point);
			points.Add(new IfcCartesianPoint(db, 3071.25, y, 3000.0));
			points.Add(new IfcCartesianPoint(db, 3071.25, y, 0.0));
			points.Add(new IfcCartesianPoint(db, -3968.75, y, 0.0));
			bounds.Add(new IfcFaceOuterBound(new IfcPolyloop(points),true));
			points.Clear();
			points.Add(new IfcCartesianPoint(db, 551.25, y, 1000.0));
			points.Add(new IfcCartesianPoint(db, -448.75, y, 1000.0));
			points.Add(new IfcCartesianPoint(db, -448.75, y, 2000.0));
			points.Add(new IfcCartesianPoint(db, 551.25, y, 2000.0));
			bounds.Add(new IfcFaceBound(new IfcPolyloop(points),true));
			IfcFaceSurface face = new IfcFaceSurface(bounds, plane, true);
			IfcStructuralSurfaceMember surfaceMember = new IfcStructuralSurfaceMember(analysisModel, face, material, 1, 200);
			DirectoryInfo di = Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
			di = Directory.GetParent(di.FullName);
			db.WriteFile(Path.Combine(di.FullName, "surfaceMember.ifc"));
			db.WriteFile(Path.Combine(di.FullName, "surfaceMember.ifcxml"));
			db.WriteFile(Path.Combine(di.FullName, "surfaceMember.ifcjson"));
		}
	}
}
