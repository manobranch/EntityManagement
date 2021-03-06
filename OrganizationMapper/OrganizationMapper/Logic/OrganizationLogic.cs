using OrganizationMapper.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrganizationMapper.Logic
{
    public class OrganizationLogic
    {
        readonly Helper helper = new Helper();
        readonly BlobStorageLogic blobStorageLogic = new BlobStorageLogic();

        public OrganizationLogic()
        {

        }

        public void UploadFileToBlobStorage(string storageConn, Microsoft.AspNetCore.Http.IFormFile file)
        {
            blobStorageLogic.UploadBlob(storageConn, file);
        }
        ///
        public List<BusinessEntity> GetOrganizationData(string storageConn)
        {
            var entitiesList = FetchEntitiesList(storageConn);

            // Sometimes, the shares are empty for a company. 
            // Sometimes, Company X does not have a subsidiary but Company Y has Company X as parent. 
            // Lets adjust these relations below, if needed for the case task.
            bool adjustFormissingData = true;
            if (adjustFormissingData)
            {
                // Adjust for missing subsidiaries
                entitiesList = AdjustForMissingSubsidiary(entitiesList);
                entitiesList = AdjustForMissingShares(entitiesList);

                // TODO For another time
                // Adjust for missing parents
            }

            return entitiesList;
        }

        public string GetOrganizationDataAsJson(List<BusinessEntity> entitiesList)
        {
            return JsonSerializer.Serialize(entitiesList);
        }

        public List<BusinessEntity> FetchEntitiesList(string storageConn)
        {
            var organizationRawData = blobStorageLogic.GetOrganizationData(storageConn);

            return MapOrganizationRawData(organizationRawData);
        }

        public List<BusinessEntity> AdjustForMissingSubsidiary(List<BusinessEntity> entitiesList)
        {
            foreach (var entity in entitiesList)
            {
                // Find any company for which the current entity is parent.
                var havingEntityAsParent = (from hits in entitiesList
                                            where hits.Parents.Any(a => a.OrganizationalNumber == entity.OrganizationalNumber)
                                            select hits).ToList();

                // Get the subsidiaries that is not in entities list
                var missingSubsidiaries = havingEntityAsParent.Where(x => !entity.Subsidiaries.Any(z => z.OrganizationalNumber == x.OrganizationalNumber)).ToList();

                foreach (var subSidiary in missingSubsidiaries)
                {
                    var share = (from hits in subSidiary.Parents
                                 where hits.OrganizationalNumber == entity.OrganizationalNumber
                                 select hits.Share).FirstOrDefault()?.ToString();

                    entity.Subsidiaries.Add(new BusinessRelation()
                    {
                        OrganizationalNumber = subSidiary.OrganizationalNumber,
                        Share = share
                    });
                }
            }

            return entitiesList;
        }

        private List<BusinessEntity> AdjustForMissingShares(List<BusinessEntity> entitiesList)
        {
            foreach (var entity in entitiesList)
            {
                foreach (var subsidiary in entity.Subsidiaries)
                {
                    if (subsidiary.Share == string.Empty)
                    {
                        subsidiary.Share = FindShareFromSubsidiary(entity.OrganizationalNumber, subsidiary.OrganizationalNumber, entitiesList);
                    }
                }
            }

            return entitiesList;
        }

        private string FindShareFromSubsidiary(string entityOrgNr, string subOrgNr, List<BusinessEntity> entitiesList)
        {
            // Get the subsidiary for the entity.
            var subEntity = entitiesList.Where(a => a.OrganizationalNumber == subOrgNr).FirstOrDefault();

            // Find the share from the parent information
            foreach (var parent in subEntity?.Parents)
            {
                if (parent.OrganizationalNumber == entityOrgNr)
                {
                    return parent.Share;
                }
            }

            return string.Empty;
        }

        private List<BusinessEntity> MapOrganizationRawData(List<string> organizationRawData)
        {
            var entitiesList = new List<BusinessEntity>();
            bool headerLine = true;

            foreach (var input in organizationRawData)
            {
                // Ignore mapping the header
                if (headerLine)
                {
                    headerLine = false;
                    continue;
                }

                var splittedLine = helper.SplitCsvString(input);

                entitiesList.Add(new BusinessEntity(splittedLine));
            }

            return entitiesList;
        }
    }
}
