using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GeometryGym.Ifc;

namespace ConsoleParametricFooting
{
	class Program
	{
		static void Main(string[] args)
		{
			DatabaseIfc db = new DatabaseIfc(ModelView.Ifc4NotAssigned);
			IfcProjectLibrary context = new IfcProjectLibrary(db, "ObjectLibrary", IfcUnitAssignment.Length.Millimetre);

			IfcFootingType parametricType = generate(db, true, 800, 800, 300);

			IfcFootingType footingType1 = generate(db, false, 800, 800, 300);
			IfcFootingType footingType2 = generate(db, false, 600, 600, 250);
			IfcFootingType footingType3 = generate(db, false, 400, 400, 200);

			new IfcRelAssignsToProduct(new List<IfcObjectDefinition>() { footingType1, footingType2, footingType3 }, parametricType);
			new IfcRelDeclares(context, new List<IfcDefinitionSelect>() { footingType1, footingType2, footingType3 });
			DirectoryInfo di = Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
			di = Directory.GetParent(di.FullName);
			db.WriteFile(Path.Combine(di.FullName,"ParametricFooting.ifc"));
		}
		internal static IfcFootingType generate(DatabaseIfc db, bool parametric, double length, double width, double height)
		{
			string name = parametric ? "PadFootingParametric" : "PadFooting" + length + "x" + width + "x" + height;
			IfcFootingType footingType = new IfcFootingType(db, name, IfcFootingTypeEnum.PAD_FOOTING);
			IfcRectangleProfileDef rpd = new IfcRectangleProfileDef(db, name, length,width);
			footingType.RepresentationMaps = new List<IfcRepresentationMap>() { new IfcRepresentationMap(new IfcExtrudedAreaSolid(rpd, new IfcAxis2Placement3D(new IfcCartesianPoint(db, 0, 0, -height)), new IfcDirection(db, 0, 0, 1), height)) };
			Qto_FootingBaseQuantities baseQuantities = new Qto_FootingBaseQuantities(footingType);
			baseQuantities.Length = length;
			baseQuantities.Width = width;
			baseQuantities.Height = height;
			List<IfcPhysicalQuantity> quantities = baseQuantities.Quantities;
			if (parametric)
			{
				string prefix = @"RepresentationMaps[1].MappedRepresentation.Items[1]\IfcExtrudedAreaSolid.";
				CreateConstraint("Length", footingType, quantities[0], prefix + @"SweptArea\IfcRectangleProfileDef.XDim");
				CreateConstraint("Width", footingType, quantities[1], prefix + @"SweptArea\IfcRectangleProfileDef.YDim");
				CreateConstraint("Height", footingType, quantities[2], prefix + @"Depth");
				IfcAppliedValue appv = new IfcAppliedValue(new IfcAppliedValue( IfcReference.ParseDescription(db, "HasPropertySets['" + baseQuantities.Name + "'].HasProperties['" +quantities[2].Name + "']")), IfcArithmeticOperatorEnum.MULTIPLY, new IfcAppliedValue(db, new IfcReal(-1)));
				CreateConstraint("Offset",footingType,appv, @"Position.Location.CoordinateZ");
			}
			return footingType;
		}
		internal static IfcMetric CreateConstraint(string name, IfcElementType elementType, IfcResourceObjectSelect related, string referenceDesc)
		{
			IfcMetric metric = new IfcMetric(elementType.Database, name, IfcConstraintEnum.HARD) { ReferencePath = IfcReference.ParseDescription(elementType.Database, referenceDesc), BenchMark = IfcBenchmarkEnum.EQUALTO };
			IfcResourceConstraintRelationship rcr = new IfcResourceConstraintRelationship(metric, related);
			new IfcRelAssociatesConstraint(elementType, metric);
			return metric;
		}
	}
}
