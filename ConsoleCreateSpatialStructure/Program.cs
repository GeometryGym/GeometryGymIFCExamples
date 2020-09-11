using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometryGym.Ifc;

namespace ConsoleCreateSpatialStructure
{
    class Program
    {
        static void Main(string[] args)
        {
            // create database 
            var database = new DatabaseIfc(ModelView.Ifc4X3NotAssigned);

            // create IfcSite instance
            var site = new IfcSite(database, "sampleSite");

            // create top-most spatial structure element IfcProject, set units and assign facility to project
            var project = new IfcProject(site, "myProject", IfcUnitAssignment.Length.Metre);


            // -- create facility representing the logical unit of road-bridge-road -- 
            var trafficFacility = new IfcFacility(site, "TrafficWayA")
            {
                CompositionType = IfcElementCompositionEnum.COMPLEX
            };
            // create parts/child-facilities and assign them to the traffic way
            var facilityPart1 = new IfcFacilityPart(
                trafficFacility,
                "myRoadPart01",
                new IfcFacilityPartTypeSelect(
                    IfcRoadPartTypeEnum.ROADSEGMENT),
                IfcFacilityUsageEnum.LONGITUDINAL);

            facilityPart1.Description = "TrafficWayA -> Segment 1";

            var facilityPart2 = new IfcFacility(trafficFacility, "myBridge")
            {
                Description = "TrafficWayA -> Segment 2"
            };

            var facilityPart3 = new IfcFacilityPart(
                trafficFacility,
                "myRoadPart02",
                new IfcFacilityPartTypeSelect(
                    IfcRoadPartTypeEnum.ROADSEGMENT),
                IfcFacilityUsageEnum.LONGITUDINAL)
            {
                Description = "TrafficWayA -> Segment 3"
            };

            // -- river facility --
            var river = new IfcFacility(site, "River")
            {
                CompositionType = IfcElementCompositionEnum.NOTDEFINED
            };
            // add a part to the river
            var riverPart = new IfcFacilityPart(
                river,
                "myRiver",
                new IfcFacilityPartTypeSelect(
                    IfcMarinePartTypeEnum.WATERFIELD),
                IfcFacilityUsageEnum.LONGITUDINAL) {Description = "River that passes under the bridge"};
            
            // store the IFC model
            database.WriteFile("IFC4X3RC1_spatialDecomposition.ifc");

        }
    }
}
