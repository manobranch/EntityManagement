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

        public string GetOrganizationDataAsJson(string storageConn)
        {
            var organizationRawData = blobStorageLogic.GetOrganizationData(storageConn);

            var entitiesList = MapOrganizationRawData(organizationRawData);

            return JsonSerializer.Serialize(entitiesList);
        }

        private object MapOrganizationRawData(List<string> organizationRawData)
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
