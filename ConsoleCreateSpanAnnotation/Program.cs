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
            database.Factory.ApplicationDeveloper = "Sebastian Esser - TUM";

            // turn owner history off
            database.Factory.Options.GenerateOwnerHistory = false;
            database.Factory.Options.AngleUnitsInRadians = false;

            // basic setup
            var site = new IfcSite(database, "SiteA");

            var lengthUnit = new IfcSIUnit(database, IfcUnitEnum.LENGTHUNIT, IfcSIPrefix.NONE, IfcSIUnitName.METRE);
            var kilometerUnit = new IfcSIUnit(database, IfcUnitEnum.LENGTHUNIT, IfcSIPrefix.KILO, IfcSIUnitName.METRE);
            var timeUnit = new IfcSIUnit(database, IfcUnitEnum.TIMEUNIT, IfcSIPrefix.NONE, IfcSIUnitName.SECOND);
            var hourUnit = new IfcConversionBasedUnit(IfcUnitEnum.TIMEUNIT, "hours",
                new IfcMeasureWithUnit(new IfcTimeMeasure(60 * 60), timeUnit));
            //database.Factory.ConversionUnit(IfcConversionBasedUnit.Common.hour);
            var angleUnits = database.Factory.ConversionUnit(IfcConversionBasedUnit.Common.degree);

            var linearVelocityUnit = new IfcDerivedUnit(new IfcDerivedUnitElement(kilometerUnit, 1),
                new IfcDerivedUnitElement(hourUnit, -1), IfcDerivedUnitEnum.LINEARVELOCITYUNIT);


            var unitAssignment = new IfcUnitAssignment(lengthUnit, timeUnit, angleUnits, linearVelocityUnit);
            var project = new IfcProject(
                site,
                "SampleProject with span annotation",
                unitAssignment
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
            alignment.ObjectPlacement = database.Factory.RootPlacement;

            // semantic
            var horizSegment = new IfcAlignmentHorizontalSegment(
                new IfcCartesianPoint(database, 5, 10),
                0,
                0,
                0,
                400,
                IfcAlignmentHorizontalSegmentTypeEnum.LINE);
           
             IfcCompositeCurve compositeCurve1 = null;

            // IfcAlignmentHorizontal alignmentHorizontal = new IfcAlignmentHorizontal(alignment.ObjectPlacement, 0, out compositeCurve1, horizSegment);
            IfcAlignmentHorizontal alignmentHorizontal = new IfcAlignmentHorizontal(
                alignment.ObjectPlacement,
                0,
                new List<IfcAlignmentHorizontalSegment>() { horizSegment },
                out compositeCurve1);


            var curveSegment = new IfcCurveSegment(
                             IfcTransitionCode.CONTSAMEGRADIENTSAMECURVATURE,
                             new IfcAxis2PlacementLinear(new IfcPointByDistanceExpression(200, compositeCurve1)),
                             new IfcNonNegativeLengthMeasure(25),
                             new IfcNonNegativeLengthMeasure(110),
                             compositeCurve1);


            // link alignment and and its horizontal part semantically
            new IfcRelAggregates(alignment, alignmentHorizontal);

            // create a new alignmentSegment with then gets one curve segment as its geometric representation
            var alignmentSegment = new IfcAlignmentSegment(database);

            // link horizontal alignment with the recently created alignment segment
            new IfcRelNests(alignmentHorizontal, alignmentSegment); // sorted list -> IfcRelNests

            // connect geom representation to this segment
            IfcGeometricRepresentationSubContext axisContext = database.Factory.SubContext(IfcGeometricRepresentationSubContext.SubContextIdentifier.Axis);
            alignmentSegment.Representation = new IfcProductDefinitionShape(new IfcShapeRepresentation(axisContext, curveSegment, ShapeRepresentationType.Curve2D));

            #endregion Alignment

            #region Annotation placement

            alignmentSegment.AddComment("Create placement for annotation");

            var axis2place = new IfcAxis2PlacementLinear(
                new IfcPointByDistanceExpression(25, compositeCurve1));

            var linPlacement = new IfcLinearPlacement(axis2place)
            {
                Distance = new IfcPointByDistanceExpression(110, compositeCurve1)
            };
            annotation.ObjectPlacement = linPlacement;

            #endregion Annotation placement

            #region PSet

            var pSet = new IfcPropertySet(annotation, "PSET_SpeedData", new List<IfcProperty>
            {
                new IfcPropertySingleValue(database, "CargoSpeed", new IfcLinearVelocityMeasure(60), linearVelocityUnit),
                new IfcPropertySingleValue(database, "DesignSpeed", new IfcLinearVelocityMeasure(110), linearVelocityUnit)
            });

            #endregion PSet

            database.WriteFile("UT_SpanAnnotation1.ifc");
        }
    }
}