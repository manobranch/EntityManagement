using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrganizationMapper.Objects
{
    public class BusinessEntity
    {
        public string CompanyName { get; set; }
        public string OrganizationalNumber { get; set; }
        public string InternalCompanyName { get; set; }
        public string CompanyType { get; set; }
        public string Acquisition { get; set; }
        public string AcquisitionRegistrationDate { get; set; }
        public string BoardSeat { get; set; }
        public List<string> BoardMembers { get; set; }
        public string Auditor { get; set; }
        public List<BusinessRelation> Parents { get; set; }
        public List<BusinessRelation> Subsidiaries { get; set; }
        public string AdministeredBy { get; set; }

        public BusinessEntity(string[] splittedLine)
        {
            CompanyName = splittedLine[0];
            OrganizationalNumber = splittedLine[1];
            InternalCompanyName = splittedLine[2];
            CompanyType = splittedLine[3];
            Acquisition = splittedLine[4];
            AcquisitionRegistrationDate = splittedLine[5];
            BoardSeat = splittedLine[6];
            BoardMembers = MapBoardMembers(splittedLine[7]);
            Auditor = splittedLine[8];
            Parents = MapBusinessRelation(splittedLine[9], splittedLine[10]);
            Subsidiaries = MapBusinessRelation(splittedLine[11], splittedLine[12]);
            AdministeredBy = splittedLine[13];
        }

        private List<string> MapBoardMembers(string input)
        {
            var listOfBoardMembers = input.Trim('"').Split(';').ToList();

            return (from hits in listOfBoardMembers
                    select hits.Trim(' ')).ToList();
        }

        private List<BusinessRelation> MapBusinessRelation(string companyNumber, string companyShare)
        {
            var splittedNames = companyNumber.Trim('"').Split(';');
            var splittedShares = companyShare.Trim('"').Split(';');

            var returnObject = new List<BusinessRelation>();

            if (splittedNames.Length == splittedShares.Length)
            {
                for (int i = 0; i < splittedNames.Length; i++)
                {
                    if (splittedNames[i] != string.Empty)
                    {
                        returnObject.Add(new BusinessRelation()
                        {
                            OrganizationalNumber = splittedNames[i],
                            Share = splittedShares[i].Trim('%')
                        });
                    }
                }
            }
            else
            {
                returnObject = splittedNames.Select(a => new BusinessRelation()
                {
                    OrganizationalNumber = a,
                    Share = string.Empty
                }).ToList();
            }

            return returnObject;
        }
    }
}
