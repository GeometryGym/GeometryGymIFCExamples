using GeometryGym.Ifc;

namespace IFC4X4_Tunnel_Deployment;
class Program
{
	static void Main(string[] args)
	{
		GenerateFile(GeoreferencingOption.RigidOperation_Geographic);
		GenerateFile(GeoreferencingOption.RigidOperation_Projected);
		GenerateFile(GeoreferencingOption.MapConversion);
	}

	public enum GeoreferencingOption { MapConversion, RigidOperation_Geographic, RigidOperation_Projected }
	internal static void GenerateFile(GeoreferencingOption georeferencing)
	{ 
		DatabaseIfc db = new DatabaseIfc(ModelView.Ifc4X4NotAssigned);

		string name = "IFC4X4 Tunnel Deployment Geometry Gym";
		db.Factory.Options.AngleUnitsInRadians = true;
		IfcTunnel tunnel = new IfcTunnel(db, name);
		IfcProject project = new IfcProject(tunnel, name, IfcUnitAssignment.Length.Metre);
		var modelContext = db.Factory.GeometricRepresentationContext(IfcGeometricRepresentationContext.GeometricContextIdentifier.Model);

		IfcClassification uniclass = new IfcClassification(db, "Uniclass");
		uniclass.Source = "NBS";
		uniclass.Specification = "https://uniclass.thenbs.com/";
		new IfcRelAssociatesClassification(uniclass, project);

		IfcClassificationReference entitiesClassification = new IfcClassificationReference(uniclass) { Identification = "En", Name = "Entities", Location = "https://uniclass.thenbs.com/taxon/en" };
		IfcClassificationReference transportEntitiesClassification = new IfcClassificationReference(entitiesClassification) { Identification = "En_80", Name = "Transport Entities", Location = "https://uniclass.thenbs.com/taxon/en_80" };
		IfcClassificationReference tunnelsAndShaftsClassification = new IfcClassificationReference(transportEntitiesClassification) { Identification = "En_80_96", Name = "Tunnels and Shafts", Location = "https://uniclass.thenbs.com/taxon/en_80_96" };
		IfcClassificationReference tunnelsClassification = new IfcClassificationReference(tunnelsAndShaftsClassification) { Identification = "En_80_96_90", Name = "Tunnels", Location = "https://uniclass.thenbs.com/taxon/en_80_96_90" };

		new IfcRelAssociatesClassification(tunnelsClassification, tunnel);

		string filenameSuffix = "";
		if (georeferencing == GeoreferencingOption.RigidOperation_Geographic)
		{
			filenameSuffix = "_Georef_B";
			IfcGeographicCRS geographicCRS = new IfcGeographicCRS(db, "EPSG:7844");
			geographicCRS.AngleUnit = db.Factory.ConversionUnit(IfcConversionBasedUnit.CommonUnitName.degree);
			new IfcRigidOperation(modelContext, geographicCRS, new IfcPlaneAngleMeasure(142.237002802634), new IfcPlaneAngleMeasure(-38.3834420266629), 0);
		}
		else
		{
			IfcProjectedCRS projectedCRS = new IfcProjectedCRS(db, "EPSG:7854");
			string wellKnownText = "PROJCS[\"MGA/20-54\",GEOGCS[\"GDA2020.LL\",DATUM[\"GDA2020-7P\",SPHEROID[\"GRS1980\",6378137.000,298.25722210]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"false_easting\",500000.000],PARAMETER[\"false_northing\",10000000.000],PARAMETER[\"scale_factor\",0.999600000000],PARAMETER[\"central_meridian\",141.00000000000000],PARAMETER[\"latitude_of_origin\",0.00000000000000],UNIT[\"Meter\",1.00000000000000]]";
			new IfcWellKnownText(wellKnownText, projectedCRS);
			double eastings = 608040.1319, northings = 5750917.1421;
			if (georeferencing == GeoreferencingOption.MapConversion)
			{
				filenameSuffix = "_Georef_A";
				new IfcMapConversion(modelContext, projectedCRS, eastings, northings, 0);
			}
			else if (georeferencing == GeoreferencingOption.RigidOperation_Projected)
			{
				filenameSuffix = "_Georef_C";
				new IfcRigidOperation(modelContext, projectedCRS, new IfcLengthMeasure(607814.0), new IfcLengthMeasure(5750920.0), 0);
			}
		}
	

		IfcPropertyTemplate propertyTemplate = new IfcSimplePropertyTemplate(db, "Example Simple Property Template");
		IfcPropertySetTemplate propertySetTemplate = new IfcPropertySetTemplate("Example Property Set Template", propertyTemplate);
		project.AddDeclared(propertySetTemplate);

		string filename = "GeometryGym_Tunnel" + filenameSuffix + ".ifc";
		db.WriteFile(filename);
	}
}

