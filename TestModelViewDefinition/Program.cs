using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using GeometryGym.Ifc;

namespace TestModelViewDefinition
{
	class Program
	{
		static void Main(string[] args)
		{
			DirectoryInfo di = Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
			di = Directory.GetParent(di.FullName);

			string path = Path.Combine(di.FullName, "examples");
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			Generate(ModelView.Ifc4DesignTransfer, path);
			Generate(ModelView.Ifc4Reference, path);
		}

		internal static DatabaseIfc Generate(ModelView mdv, string path)
		{
			DatabaseIfc db = new DatabaseIfc(true, mdv);
			//IfcGeometricRepresentationContext
			//IfcCartesianPoint
			//IfcAxis2Placement3D
			//IfcDirection
			//IfcAxis2Placement2D
			//IfcSIUnit
			//IfcLocalPlacement
			IfcSite site = new IfcSite(db, "TestSite");
			//IfcPerson  NOT RV
			//IfcOrganization  NOT RV
			//IfcPersonAndOrganization  NOT RV
			//IfcOwnerHistory  NOT RV
			//fcApplication NOT RV
			//                     IfcProjectLibrary NOT RV Can't have multiple context
			IfcProject project = new IfcProject(site, "TestProject", IfcUnitAssignment.Length.Millimetre) { };
			//IfcUnitAssignment
			//IfcRelAggregates
			IfcBuilding building = new IfcBuilding(site, "TestBuilding") { };
			IfcBuildingStorey buildingStorey = new IfcBuildingStorey(building, "TestBuildingStorey", 3000);
			IfcSpace space = new IfcSpace(buildingStorey, null, null) { Name = "TestSpace" };
			space.RelatingType = new IfcSpaceType(db, "TestSpaceType", IfcSpaceTypeEnum.INTERNAL);

			IfcZone zone = new IfcZone(buildingStorey, "TestZone", "TestZoneLongName", new List<IfcSpace>() { space });

			IfcLocalPlacement localPlacement = buildingStorey.Placement as IfcLocalPlacement;

			IfcMaterial material = new IfcMaterial(db, "TestMaterial") { Description = "TestDescription", Category = "TestCategory" };
			IfcMaterialProperties materialProperties = new IfcMaterialProperties("TestMaterialProperties", material) { Properties = new List<IfcProperty>() { new IfcPropertySingleValue(db, "MassDensity", new IfcMassDensityMeasure(1)) } };
			IfcSurfaceStyleShading surfaceStyleShading = new IfcSurfaceStyleShading(new IfcColourRgb(db, "Red", 1, 0, 0));
			IfcSurfaceStyle surfaceStyle = new IfcSurfaceStyle(surfaceStyleShading,null,null,null,null);
			IfcMaterialDefinitionRepresentation materialDefinitionRepresentation = new IfcMaterialDefinitionRepresentation(new List<IfcStyledRepresentation>() { new IfcStyledRepresentation(new IfcStyledItem(surfaceStyle, "TestStyledItem")) }, material);

			IfcIndexedPolyCurve indexedPolyCurve = IPE200Curve(db);
			IfcArbitraryClosedProfileDef arbitraryClosedProfileDef = new IfcArbitraryClosedProfileDef("IPE200", indexedPolyCurve);
			IfcMaterialProfile materialProfile = new IfcMaterialProfile("TestMaterialProfile", material, arbitraryClosedProfileDef);
			IfcMaterialProfileSet materialProfileSet = new IfcMaterialProfileSet("TestMaterialProfileSet", materialProfile);

			IfcExtrudedAreaSolid extrudedAreaSolid = new IfcExtrudedAreaSolid(arbitraryClosedProfileDef, new IfcAxis2Placement3D(new IfcCartesianPoint(db,0,0,0),new IfcDirection(db,1,0,0),new IfcDirection(db,0,1,0)), new IfcDirection(db, 0, 0, 1), 5000);
			IfcBeamType beamType = new IfcBeamType(db, "TestBeamType", IfcBeamTypeEnum.BEAM);
			beamType.MaterialSelect = materialProfileSet; 
			IfcBeam beam = new IfcBeam(buildingStorey,new IfcLocalPlacement(localPlacement, new IfcAxis2Placement3D(new IfcCartesianPoint(db, 1000, 2000, 0)) { RefDirection = new IfcDirection(db, 0, 1, 0) }),new IfcProductDefinitionShape(new IfcShapeRepresentation(extrudedAreaSolid)));
			//IfcGeometricRepresentationSubContext
			beam.RelatingType = beamType;
			//IfcRelDefinesByType

			//	if(mdv != ModelView.Ifc4Reference)
			//			IfcActorRole  NOT RV

			//IfcActuator
			//IfcActuatorType
			//IfcAdvancedBrep  NOT RV
			//IfcAdvancedFace   NOT RV
			//IfcAirTerminal
			//IfcAirTerminalBox
			//IfcAirTerminalBoxType
			//IfcAirTerminalType
			//IfcAirToAirHeatRecovery
			//IfcAirToAirHeatRecoveryType
			//IfcAlarm
			//IfcAlarmType
			//IfcArbitraryOpenProfileDef
			//IfcArbitraryProfileDefWithVoids
			//IfcAsymmetricIShapeProfileDef  NOT RV
			//IfcAudioVisualAppliance
			//IfcAudioVisualApplianceType
			//IfcAxis1Placement
			//IfcBeamStandardCase  NOT RV
			//IfcBlock   NOT RV
			//IfcBoiler
			//IfcBoilerType
			//IfcBooleanClippingResult  NOT RV
			//IfcBooleanResult  NOT RV
			//IfcBSplineCurveWithKnots  NOT RV
			//IfcBSplineSurface  NOT RV
			//IfcBSplineSurfaceWithKnots  NOT RV
			//IfcBuildingElementPart
			//IfcBuildingElementPartType
			//IfcBuildingElementProxy
			//IfcBuildingElementProxyType
			//IfcBuildingSystem
			//IfcBurner
			//IfcBurnerType
			//IfcCableCarrierFitting
			//IfcCableCarrierFittingType
			//IfcCableCarrierSegment
			//IfcCableCarrierSegmentType
			//IfcCableFitting
			//IfcCableFittingType
			//IfcCableSegment
			//IfcCableSegmentType
			//IfcCartesianPointList2D
			//IfcCartesianPointList3D
			//IfcCartesianTransformationOperator2D
			//IfcCartesianTransformationOperator2DnonUniform
			//IfcCartesianTransformationOperator3D
			//IfcCartesianTransformationOperator3DnonUniform
			//IfcCenterLineProfileDef
			//IfcChiller
			//IfcChillerType
			//IfcChimney
			//IfcChimneyType
			//IfcCircle
			//IfcCircleHollowProfileDef  NOT RV
			//IfcCircleProfileDef  NOT RV
			//IfcCivilElement
			//IfcCivilElementType
			//IfcClassification
			//IfcClassificationReference
			//IfcClosedShell
			//IfcCoil
			//IfcCoilType
			//IfcColourRgbList
			//IfcColourSpecification
			//IfcColumn
			//IfcColumnStandardCase  NOT RV
			//IfcColumnType
			//IfcCommunicationsAppliance
			//IfcCommunicationsApplianceType
			//IfcComplexProperty
			//IfcCompositeProfileDef  NOT RV
			//IfcCompressor
			//IfcCompressorType
			//IfcCondenser
			//IfcCondenserType
			//IfcConnectedFaceSet  NOT RV
			//IfcConnectionCurveGeometry  NOT RV
			//IfcConnectionVolumeGeometry  NOT RV
			//IfcController
			//IfcControllerType
			//IfcConversionBasedUnit
			//IfcConversionBasedUnitWithOffset
			//IfcCooledBeam
			//IfcCooledBeamType
			//IfcCoolingTower
			//IfcCoolingTowerType
			//IfcCovering
			//IfcCoveringType
			//IfcCsgSolid  NOT RV
			//IfcCShapeProfileDef  NOT RV
			//IfcCurtainWall
			//IfcCurtainWallType
			//IfcCurveStyle
			//IfcCurveStyleFont
			//IfcCurveStyleFontPattern
			//IfcDamper
			//IfcDamperType
			//IfcDerivedProfileDef  NOT RV
			//IfcDerivedUnit
			//IfcDerivedUnitElement
			//IfcDimensionalExponents
			//IfcDiscreteAccessory
			//IfcDiscreteAccessoryType
			//IfcDistributionChamberElement
			//IfcDistributionChamberElementType
			//IfcDistributionCircuit
			//IfcDistributionControlElement
			//IfcDistributionControlElementType
			//IfcDistributionElement
			//IfcDistributionElementType
			//IfcDistributionFlowElement
			//IfcDistributionFlowElementType
			//IfcDistributionPort
			//IfcDistributionSystem
			//IfcDocumentReference
			//IfcDoor
			//IfcDoorLiningProperties
			//IfcDoorPanelProperties
			//IfcDoorStandardCase  NOT RV
			//IfcDoorType
			//IfcDuctFitting
			//IfcDuctFittingType
			//IfcDuctSegment
			//IfcDuctSegmentType
			//IfcDuctSilencer
			//IfcDuctSilencerType
			//IfcEdge	 NOT RV
			//IfcEdgeCurve  NOT RV
			//IfcEdgeLoop  NOT RV
			//IfcElectricAppliance
			//IfcElectricApplianceType
			//IfcElectricDistributionBoard
			//IfcElectricDistributionBoardType
			//IfcElectricFlowStorageDevice
			//IfcElectricFlowStorageDeviceType
			//IfcElectricGenerator
			//IfcElectricGeneratorType
			//IfcElectricMotor
			//IfcElectricMotorType
			//IfcElectricTimeControl
			//IfcElectricTimeControlType
			//IfcElementAssembly
			//IfcElementAssemblyType
			//IfcElementComponent
			//IfcElementComponentType
			//IfcElementQuantity
			//IfcEllipseProfileDef  NOT RV
			//IfcEnergyConversionDevice
			//IfcEnergyConversionDeviceType
			//IfcEngine
			//IfcEngineType
			//IfcEvaporativeCooler
			//IfcEvaporativeCoolerType
			//IfcEvaporator
			//IfcEvaporatorType
			//IfcExtendedProperties
			//IfcExternalInformation
			//IfcExternalReference
			//IfcExtrudedAreaSolidTapered  NOT RV
			//IfcFace  NOT RV
			//IfcFaceBasedSurfaceModel  NOT RV
			//IfcFaceBound  NOT RV
			//IfcFaceOuterBound  NOT RV
			//IfcFaceSurface  NOT RV
			//IfcFacetedBrep  NOT RV
			//IfcFan
			//IfcFanType
			//IfcFastener
			//IfcFastenerType
			//IfcFeatureElement
			//IfcFeatureElementAddition  NOT RV
			//IfcFeatureElementSubtraction
			//IfcFillAreaStyle
			//IfcFillAreaStyleHatching
			//IfcFilter
			//IfcFilterType
			//IfcFireSuppressionTerminal
			//IfcFireSuppressionTerminalType
			//IfcFixedReferenceSweptAreaSolid  NOT RV
			//IfcFlowController
			//IfcFlowControllerType
			//IfcFlowFitting
			//IfcFlowFittingType
			//IfcFlowInstrument
			//IfcFlowInstrumentType
			//IfcFlowMeter
			//IfcFlowMeterType
			//IfcFlowMovingDevice
			//IfcFlowMovingDeviceType
			//IfcFlowSegment
			//IfcFlowSegmentType
			//IfcFlowStorageDevice
			//IfcFlowStorageDeviceType
			//IfcFlowTerminal
			//IfcFlowTerminalType
			//IfcFlowTreatmentDevice
			//IfcFlowTreatmentDeviceType
			//IfcFooting
			//IfcFootingType
			//IfcFurnishingElement
			//IfcFurnishingElementType
			//IfcFurniture
			//IfcFurnitureType
			//IfcGeographicElement
			//IfcGeographicElementType
			//IfcGeometricCurveSet
			//IfcGeometricSet
			//IfcGrid
			//IfcGridAxis
			//IfcGridPlacement  NOT RV
			//IfcGroup
			//IfcHalfSpaceSolid  NOT RV
			//IfcHeatExchanger
			//IfcHeatExchangerType
			//IfcHumidifier
			//IfcHumidifierType
			//IfcIndexedColourMap
			//IfcIndexedTextureMap
			//IfcIndexedTriangleTextureMap
			//IfcInterceptor
			//IfcInterceptorType
			//IfcIShapeProfileDef
			//IfcJunctionBox
			//IfcJunctionBoxType
			//IfcLamp
			//IfcLampType
			//IfcLibraryInformation  NOT RV
			//IfcLibraryReference  NOT RV
			//IfcLightFixture
			//IfcLightFixtureType
			//IfcLine
			//IfcLoop   NOT RV
			//IfcLShapeProfileDef  NOT RV
			//IfcMapConversion
			//IfcMappedItem
			//IfcMaterialConstituent
			//IfcMaterialConstituentSet
			//IfcMaterialLayer
			//IfcMaterialLayerSet
			//IfcMaterialLayerSetUsage  NOT RV
			//IfcMaterialLayerWithOffsets  NOT RV
			//IfcMaterialProfileSetUsage  NOT RV
			//IfcMaterialProfileSetUsageTapering  NOT RV
			//IfcMaterialProfileWithOffsets  NOT RV
			//IfcMaterialUsageDefinition  NOT RV
			//IfcMeasureWithUnit 
			//IfcMechanicalFastener
			//IfcMechanicalFastenerType
			//IfcMedicalDevice
			//IfcMedicalDeviceType
			//IfcMember
			//IfcMemberStandardCase  NOT RV
			//IfcMemberType
			//IfcMonetaryUnit
			//IfcMotorConnection
			//IfcMotorConnectionType
			//IfcNamedUnit
			//IfcOpeningElement
			//IfcOpeningStandardCase  NOT RV
			//IfcOpenShell  NOT RV
			//IfcOrientedEdge  NOT RV
			//IfcOutlet
			//IfcOutletType
			//IfcPcurve  NOT RV
			//IfcPhysicalComplexQuantity
			//IfcPhysicalQuantity
			//IfcPhysicalSimpleQuantity
			//IfcPile
			//IfcPileType
			//IfcPipeFitting
			//IfcPipeFittingType
			//IfcPipeSegment
			//IfcPipeSegmentType
			//IfcPlane  NOT RV
			//IfcPlate 
			//IfcPlateStandardCase  NOT RV
			//IfcPlateType
			//IfcPoint
			//IfcPolygonalBoundedHalfSpace  NOT RV
			//IfcPolyline  NOT RV
			//IfcPolyLoop  NOT RV
			//IfcPort
			//IfcPostalAddress
			//IfcPreDefinedPropertySet
			//IfcPresentationItem
			//IfcPresentationLayerAssignment
			//IfcPresentationStyle
			//IfcPresentationStyleAssignment
			//IfcProductDefinitionShape
			//IfcProductRepresentation
			//IfcProfileDef  
			//IfcProfileProperties
			//IfcProjectedCRS
			//IfcProjectionElement  NOT RV
			//IfcProperty
			//IfcPropertyAbstraction
			//IfcPropertyBoundedValue
			//IfcPropertyDefinition
			//IfcPropertyEnumeratedValue
			//IfcPropertyEnumeration
			//IfcPropertyListValue
			//IfcPropertySet
			//IfcPropertySetTemplate  NOT RV
			//IfcPropertyTableValue
			//IfcPropertyTemplate  NOT RV
			//IfcPropertyTemplateDefinition  NOT RV
			//IfcProtectiveDevice
			//IfcProtectiveDeviceTrippingUnit
			//IfcProtectiveDeviceTrippingUnitType
			//IfcProtectiveDeviceType
			//IfcPump
			//IfcPumpType
			//IfcQuantityArea
			//IfcQuantityCount
			//IfcQuantityLength
			//IfcQuantitySet
			//IfcQuantityTime
			//IfcQuantityVolume
			//IfcQuantityWeight
			//IfcRailing
			//IfcRailingType
			//IfcRamp
			//IfcRampFlight
			//IfcRampFlightType
			//IfcRampType
			//IfcRectangleHollowProfileDef  NOT RV
			//IfcRectangleProfileDef  NOT RV
			//IfcRectangularPyramid  NOT RV
			//IfcReinforcingBar
			//IfcReinforcingBarType
			//IfcReinforcingElement
			//IfcReinforcingElementType
			//IfcReinforcingMesh
			//IfcReinforcingMeshType
			//IfcRelAssignsToGroup
			//IfcRelAssociatesClassification
			//IfcRelAssociatesDocument
			//IfcRelAssociatesLibrary  NOT RV
			//IfcRelAssociatesMaterial
			//IfcRelConnectsElements  NOT RV
			//IfcRelConnectsPathElements  NOT RV
			//IfcRelConnectsPorts
			//IfcRelConnectsWithRealizingElements  NOT RV
			//IfcRelContainedInSpatialStructure
			//IfcRelCoversBldgElements
			//IfcRelDeclares
			//IfcRelDefinesByProperties
			//IfcRelFillsElement
			//IfcRelFlowControlElements  NOT RV
			//IfcRelInterferesElements  NOT RV
			//IfcRelNests
			//IfcRelProjectsElement  NOT RV
			//IfcRelServicesBuildings
			//IfcRelVoidsElement
			//IfcRepresentation
			//IfcRepresentationContext
			//IfcRepresentationItem
			//IfcRepresentationMap
			//IfcRevolvedAreaSolid
			//IfcRevolvedAreaSolidTapered  NOT RV
			//IfcRightCircularCone  NOT RV
			//IfcRightCircularCylinder  NOT RV
			//IfcRoof
			//IfcRoofType
			//IfcRoundedRectangleProfileDef  NOT RV
			//IfcSanitaryTerminal
			//IfcSanitaryTerminalType
			//IfcSensor
			//IfcSensorType
			//IfcShadingDevice
			//IfcShadingDeviceType
			//IfcShapeRepresentation
			//IfcShellBasedSurfaceModel  NOT RV
			//IfcSimplePropertyTemplate  NOT RV
			//IfcSlab
			//IfcSlabElementedCase  NOT RV
			//IfcSlabStandardCase  NOT RV
			//IfcSlabType
			//IfcSolarDevice
			//IfcSolarDeviceType
			//IfcSpaceHeater
			//IfcSpaceHeaterType
			//IfcSpatialZone
			//IfcSpatialZoneType
			//IfcSphere  NOT RV
			//IfcStackTerminal
			//IfcStackTerminalType
			//IfcStair
			//IfcStairFlight
			//IfcStairFlightType
			//IfcStairType
			//IfcStyleModel
			//IfcSurface  NOT RV
			//IfcSurfaceCurveSweptAreaSolid  NOT RV
			//IfcSurfaceOfLinearExtrusion  NOT RV
			//IfcSurfaceOfRevolution  NOT RV
			//IfcSurfaceStyleRendering
			//IfcSurfaceStyleWithTextures surfaceStyleWithTextures = new IfcSurfaceStyleWithTextures(new IfcImageTexture(db,true,true,""));
			//IfcSweptDiskSolid
			//IfcSwitchingDevice
			//IfcSwitchingDeviceType
			//IfcSystemFurnitureElement
			//IfcSystemFurnitureElementType
			//IfcTank
			//IfcTankType
			//IfcTelecomAddress  NOT RV
			//IfcTendon
			//IfcTendonAnchor
			//IfcTendonAnchorType
			//IfcTendonType
			//IfcTessellatedFaceSet
			//IfcTessellatedItem
			//IfcTextureCoordinate
			//IfcTextureVertexList
			//IfcTransformer
			//IfcTransformerType
			//IfcTransportElement
			//IfcTransportElementType
			//IfcTriangulatedFaceSet
			//IfcTrimmedCurve
			//IfcTShapeProfileDef  NOT RV
			//IfcTubeBundle
			//IfcTubeBundleType
			//IfcUnitaryControlElement
			//IfcUnitaryControlElementType
			//IfcUnitaryEquipment
			//IfcUnitaryEquipmentType
			//IfcUShapeProfileDef  NOT RV
			//IfcValve
			//IfcValveType
			//IfcVector
			//IfcVertex  NOT RV
			//IfcVertexPoint  NOT RV
			//IfcVibrationIsolator
			//IfcVibrationIsolatorType
			//IfcVirtualGridIntersection  NOT RV
			//IfcWall
			//IfcWallElementedCase  NOT RV
			//IfcWallStandardCase  NOT RV
			//IfcWallType
			//IfcWasteTerminal
			//IfcWasteTerminalType
			//IfcWindow
			//IfcWindowLiningProperties
			//IfcWindowPanelProperties
			//IfcWindowStandardCase  NOT RV
			//IfcWindowType
			//IfcZShapeProfileDef  NOT RV
			db.WriteFile(Path.Combine(path, mdv.ToString() + ".ifc"));
			return db;
		}

		internal static IfcIndexedPolyCurve IPE200Curve(DatabaseIfc db)
		{
			List<Tuple<double, double>> tuples = new List<Tuple<double, double>>() { new Tuple<double, double>(2.8, -79.5), new Tuple<double, double>(2.8, 79.5), new Tuple<double, double>(6.314719, 87.985281), new Tuple<double, double>(14.8, 91.5), new Tuple<double, double>(50.0, 91.5), new Tuple<double, double>(50.0, 100.0), new Tuple<double, double>(-50.0, 100.0), new Tuple<double, double>(-50.0, 91.5), new Tuple<double, double>(-14.8, 91.5), new Tuple<double, double>(-6.314719, 87.985281), new Tuple<double, double>(-2.8, 79.5), new Tuple<double, double>(-2.8, -79.5), new Tuple<double, double>(-6.314719, -87.985281), new Tuple<double, double>(-14.8, -91.5), new Tuple<double, double>(-50.0, -91.5), new Tuple<double, double>(-50.0, -100.0), new Tuple<double, double>(50.0, -100.0), new Tuple<double, double>(50.0, -91.5), new Tuple<double, double>(14.8, -91.5), new Tuple<double, double>(6.314719, -87.985281) };
			IfcCartesianPointList2D points = new IfcCartesianPointList2D(db, tuples);
			List<IfcSegmentIndexSelect> segments = new List<IfcSegmentIndexSelect>();
			segments.Add(new IfcLineIndex(1, 2));
			segments.Add(new IfcArcIndex(2, 3, 4));
			segments.Add(new IfcLineIndex(4,5));
			segments.Add(new IfcLineIndex(5,6));
			segments.Add(new IfcLineIndex(6,7));
			segments.Add(new IfcLineIndex(7,8));
			segments.Add(new IfcLineIndex(8,9));
			segments.Add(new IfcArcIndex(9,10,11));
			segments.Add(new IfcLineIndex(11,12));
			segments.Add(new IfcArcIndex(12,13,14));
			segments.Add(new IfcLineIndex(14,15));
			segments.Add(new IfcLineIndex(15,16));
			segments.Add(new IfcLineIndex(16,17));
			segments.Add(new IfcLineIndex(17,18));
			segments.Add(new IfcLineIndex(18,19));
			segments.Add(new IfcArcIndex(19,20,1));
			return new IfcIndexedPolyCurve(points, segments);
		}
	}
}
