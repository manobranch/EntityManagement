using NUnit.Framework;
using OrganizationMapper.Logic;
using OrganizationMapper.Objects;
using System.Collections.Generic;
using System.Linq;

namespace OrganizationMapperTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ParentSubsidiaryCountValidation()
        {
            // Arrange
            List<BusinessEntity> entitiesList;
            var storageConn = "DefaultEndpointsProtocol=https;AccountName=entitymanagement;AccountKey=XWXrZvjbYItcbQVBhxeT1oHiLn37C3Dx8x983MBljRJ3iUL2yCeL1jXgZn+ps166O5e7KwSGE6jvkHUoeJz4Rw==;EndpointSuffix=core.windows.net";

            var parentCount = 0;
            var subsidiariesCount = 0;

            // Act
            entitiesList = new OrganizationLogic().GetOrganizationData(storageConn);

            // Assert
            foreach (var entity in entitiesList)
            {
                parentCount += entity.Parents.Count;
                subsidiariesCount += entity.Subsidiaries.Count;
            }

            Assert.AreEqual(parentCount, subsidiariesCount);
        }

        [Test]
        public void SubsidiariesValidation()
        {
            // Arrange
            var companyB = new BusinessEntity()
            {
                CompanyName = "Company B",
                OrganizationalNumber = "555555-0002",
                InternalCompanyName = "102",
                CompanyType = "AB",
                Acquisition = "NO",
                AcquisitionRegistrationDate = "2010-03-04",
                BoardSeat = "Stockholm",
                BoardMembers = new List<string>() { "John Doe", "Jane Doe" },
                Auditor = "PwC",
                Parents = new List<BusinessRelation>() { new BusinessRelation() { OrganizationalNumber = "555555-0001", Share = "100" } },
                Subsidiaries = new List<BusinessRelation>()
                {
                    new BusinessRelation()
                    {
                        OrganizationalNumber = "555555-0003",
                        Share = "50"
                    },
                    new BusinessRelation()
                    {
                        OrganizationalNumber = "555555-0004",
                        Share = "100"
                    },
                    new BusinessRelation()
                    {
                        OrganizationalNumber =  "555555-0005",
                        Share = "100"
                    }
                },
                AdministeredBy = "John Smith"
            };

            var companyF = new BusinessEntity()
            {
                CompanyName = "Company F",
                OrganizationalNumber = "555555-0006",
                InternalCompanyName = "106",
                CompanyType = "AB",
                Acquisition = "NO",
                AcquisitionRegistrationDate = "2000-01-04",
                BoardSeat = "Stockholm",
                BoardMembers = new List<string>() { "James Doe", "Judith Doe" },
                Auditor = "PwC",
                Parents = new List<BusinessRelation>() { new BusinessRelation() { OrganizationalNumber = "555555-0002", Share = "100" } },
                Subsidiaries = new List<BusinessRelation>()
                {
                    new BusinessRelation()
                    {
                        OrganizationalNumber = "555555-0007",
                        Share = "100"
                    },
                    new BusinessRelation()
                    {
                        OrganizationalNumber = "555555-0004",
                        Share = "100"
                    },
                    new BusinessRelation()
                    {
                        OrganizationalNumber =  "555555-0005",
                        Share = "100"
                    }
                },
                AdministeredBy = "John Smith"
            };

            var entitiesList = new List<BusinessEntity>
            {
                companyB,
                companyF
            };

            BusinessRelation subsidiary = new BusinessRelation();

            // Act
            var result = new OrganizationLogic().AdjustForMissingSubsidiary(entitiesList);

            //Assert
            foreach (var item in result)
            {
                if (item.CompanyName == "Company B")
                {
                    subsidiary = (from hits in item.Subsidiaries
                                  where hits.OrganizationalNumber == "555555-0006"
                                  select hits).FirstOrDefault();
                }

            };

            Assert.IsTrue(subsidiary.OrganizationalNumber == "555555-0006" && subsidiary.Share == "100");
        }
    }
}