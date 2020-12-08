using GeometryGym.Ifc;
using GeometryGym.STEP;
using System.Collections.Generic;

namespace ConsoleCreateSpanAnnotation
{
    internal class Program
    {
        private static void Main(string[] args)
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
                IfcAlignmentHorizontalSegmentTypeEnum.LINE);

            // geometric representation of a single segment. It gets referenced by IfcCompositeCurve.segments and IfcAlignmentSegment.Representation
            var curveSegment = new IfcCurveSegment(
                 IfcTransitionCode.CONTSAMEGRADIENTSAMECURVATURE,
                 new IfcAxis2Placement2D(new IfcCartesianPoint(database, 0, 0)),
                 new IfcParameterValue(0),
                 new IfcParameterValue(200),
                 null);


            var segments = new List<IfcSegment> { curveSegment };
            var compositeCurve = new IfcCompositeCurve(segments);

            var rep = new IfcShapeRepresentation(compositeCurve)
            {
                RepresentationIdentifier = "Axis",
                RepresentationType = "Curve2D"
            };

            alignment.Representation = new IfcProductDefinitionShape(rep);
            alignment.Axis = compositeCurve;

            // create an alignment horizontal instance that takes a list of horizontal segments
            var alignmentHorizontal = new IfcAlignmentHorizontal(database)
            {
                Segments = new LIST<IfcAlignmentHorizontalSegment>() { horizSegment }
            };

            // link alignment and and its horizontal part semantically
            new IfcRelAggregates(alignment, alignmentHorizontal);   

            // create a new alignmentSegment with then gets one curve segment as its geometric representation
            var alignmentSegment = new IfcAlignmentSegment(database);

            // link horizontal alignment with the recently created alignment segment
            new IfcRelNests(alignmentHorizontal, alignmentSegment); // sorted list -> IfcRelNests
            
            // connect geom representation to this segment
            alignmentSegment.Representation = new IfcProductDefinitionShape(new IfcShapeRepresentation(curveSegment));

            #endregion Alignment

            #region Annotation placement

            alignmentSegment.AddComment("Create placement for annotation");

            var axis2place = new IfcAxis2PlacementLinear(
                new IfcPointByDistanceExpression(25, compositeCurve),
                null,
                null);

            var linPlacement = new IfcLinearPlacement(axis2place);
            linPlacement.Distance = new IfcPointByDistanceExpression(128, compositeCurve);
            annotation.ObjectPlacement = linPlacement;

            #endregion Annotation placement

            #region PSet

            //var lengthUnit = new IfcSIUnit(database, IfcUnitEnum.LENGTHUNIT, IfcSIPrefix.NONE, IfcSIUnitName.METRE);

            var lengthDerivedUnit = new IfcSIUnit(database, IfcUnitEnum.LENGTHUNIT, IfcSIPrefix.KILO, IfcSIUnitName.METRE);
            lengthDerivedUnit.AddComment("PSet setup");
            var timeBaseUnit = new IfcSIUnit(database, IfcUnitEnum.TIMEUNIT, IfcSIPrefix.NONE, IfcSIUnitName.SECOND);
            var timeDerivedUnit = new IfcConversionBasedUnit(IfcUnitEnum.TIMEUNIT, "hour", new IfcMeasureWithUnit(new IfcPositiveInteger(3600), timeBaseUnit));

            var ifcderivedunitelem1 = new IfcDerivedUnitElement(lengthDerivedUnit, 1);
            var ifcderivedunitelem2 = new IfcDerivedUnitElement(timeDerivedUnit, -1);
            var speedUnit = new IfcDerivedUnit(
                new List<IfcDerivedUnitElement> { ifcderivedunitelem1, ifcderivedunitelem2 },
                IfcDerivedUnitEnum.LINEARVELOCITYUNIT);

            var pSet = new IfcPropertySet(annotation, "PSET_SpeedData", new List<IfcProperty>
            {
                new IfcPropertySingleValue(database, "CargoSpeed", new IfcLinearVelocityMeasure(60), speedUnit),
                new IfcPropertySingleValue(database, "DesignSpeed", new IfcLinearVelocityMeasure(110), speedUnit)
            });

            #endregion PSet

            database.WriteFile("AlignmentWithSpanAnnotation.ifc");
        }
    }
}