using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometryGym.Ifc;

namespace ConsoleCreateSpanAnnotation
{
    class Program
    {
        static void Main(string[] args)
        {
            var database = new DatabaseIfc(ModelView.Ifc4X3NotAssigned);
            database.Factory.ApplicationDeveloper = "Sebastian Esser";
            // turn owner history off
            database.Factory.Options.GenerateOwnerHistory = false;

            // basic setup
            var site = new IfcSite(database, "SiteA");
            var project = new IfcProject(
                site,
                "SampleProject with a span annotation",
                IfcUnitAssignment.Length.Metre
                );

            // create an annotation
            var annotation = new IfcAnnotation(database)
            {
                Name = "DesignSpeed",
            };
            annotation.AddComment("Annotation item span-placed along the alignment");
            
            // link annotation with site
            var contained = new IfcRelContainedInSpatialStructure(site);
            contained.RelatedElements.Add(annotation);

            #region Alignment

            var alignment = new IfcAlignment(site)
            {
                Name = "Basic alignment with a single horizontal segment"
            };
            alignment.AddComment("Generate alignment representation");
            
            // semantic
            var horizSegment = new IfcAlignmentHorizontalSegment(
                new IfcCartesianPoint(database, 5, 10),
                0, 
                0,
                0, 
                200, 
                IfcAlignmentHorizontalSegmentTypeEnum.LINE );

            // geometric
            var curveSegment = new IfcCurveSegment(
                IfcTransitionCode.CONTSAMEGRADIENTSAMECURVATURE, 
                new IfcAxis2Placement2D(new IfcCartesianPoint(database, 0, 0)), 
                200, 
                null);

            var segments = new List<IfcSegment> {curveSegment};
            var compositeCurve = new IfcCompositeCurve(segments);

            var rep = new IfcShapeRepresentation(compositeCurve)
            {
                RepresentationIdentifier = "Axis2D"
            };

            alignment.Representation = new IfcProductDefinitionShape(rep);
            alignment.Axis = compositeCurve;

            var horizSegments = new List<IfcAlignmentHorizontalSegment>() {horizSegment};   // semantic
            
            var alignmentHorizontal = new IfcAlignmentHorizontal(alignment, horizSegments);     // semantic
            new IfcRelAggregates(alignment, alignmentHorizontal);   // semantic

            // var alignmentSegment = new IfcAlignmentSegment(alignmentHorizontal, horizSegment);
            var alignmentSegment = new IfcAlignmentSegment(database);
            new IfcRelNests(alignmentHorizontal, alignmentSegment); // sorted list -> IfcRelNests
                                                                    //alignmentSegment.Representation = new IfcProductDefinitionShape(new IfcShapeRepresentation(curveSegment));

            #endregion

            #region Annotation placement

            alignmentSegment.AddComment("Create placement for annotation");

            var axis2place = new IfcAxis2PlacementLinear(
                new IfcPointByDistanceExpression(25, compositeCurve), 
                null, 
                null);


            var linPlacement = new IfcLinearPlacement(axis2place);
            linPlacement.Distance = new IfcPointByDistanceExpression(128, compositeCurve);
            annotation.ObjectPlacement = linPlacement;

            #endregion

            #region PSet
            //var lengthUnit = new IfcSIUnit(database, IfcUnitEnum.LENGTHUNIT, IfcSIPrefix.NONE, IfcSIUnitName.METRE);

            var lengthDerivedUnit = new IfcSIUnit(database, IfcUnitEnum.LENGTHUNIT, IfcSIPrefix.KILO, IfcSIUnitName.METRE);
            lengthDerivedUnit.AddComment("PSet setup");
            var timeBaseUnit = new IfcSIUnit(database, IfcUnitEnum.TIMEUNIT, IfcSIPrefix.NONE, IfcSIUnitName.SECOND);
            var timeDerivedUnit = new IfcConversionBasedUnit(IfcUnitEnum.TIMEUNIT, "hour", new IfcMeasureWithUnit(new IfcPositiveInteger(3600), timeBaseUnit));

            var ifcderivedunitelem1 = new IfcDerivedUnitElement(lengthDerivedUnit, 1);
            var ifcderivedunitelem2 = new IfcDerivedUnitElement(timeDerivedUnit, -1);
            var speedUnit = new IfcDerivedUnit(
                new List<IfcDerivedUnitElement>{ifcderivedunitelem1, ifcderivedunitelem2},
                IfcDerivedUnitEnum.LINEARVELOCITYUNIT );

            var pSet = new IfcPropertySet(annotation, "PSET_SpeedData", new List<IfcProperty>
            {
                new IfcPropertySingleValue(database, "CargoSpeed", new IfcLinearVelocityMeasure(60), speedUnit),
                new IfcPropertySingleValue(database, "DesignSpeed", new IfcLinearVelocityMeasure(110), speedUnit)
            });
            #endregion
            
            database.WriteFile("AlignmentWithSpanAnnotation.ifc");
        }
    }
}
