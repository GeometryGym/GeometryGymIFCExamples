using GeometryGym.Ifc;

namespace IFC4X4_Tunnel_Deployment;
class Program
{
	static void Main(string[] args)
	{
		DatabaseIfc db = new DatabaseIfc(ModelView.Ifc4X4NotAssigned);

		string name = "IFC4X4 Tunnel Deployment Geometry Gym";
		db.Factory.Options.AngleUnitsInRadians = true;
		IfcTunnel tunnel = new IfcTunnel(db, name);
		IfcProject project = new IfcProject(tunnel, name, IfcUnitAssignment.Length.Metre);
		var modelContext = db.Factory.GeometricRepresentationContext(IfcGeometricRepresentationContext.GeometricContextIdentifier.Model);

		IfcPropertyTemplate propertyTemplate = new IfcSimplePropertyTemplate(db, "Example Simple Property Template");
		IfcPropertySetTemplate propertySetTemplate = new IfcPropertySetTemplate("Example Property Set Template", propertyTemplate);
		project.AddDeclared(propertySetTemplate);


		db.WriteFile(args[0]);
	}
}

