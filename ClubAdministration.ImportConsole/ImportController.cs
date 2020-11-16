using ClubAdministration.Core.Entities;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Utils;

namespace ClubAdministration.ImportConsole
{
    public class ImportController
    {
        const string FileName = "members.csv";
        
        const int Idx_FirstName = 1;
        const int Idx_LastName = 0;
        const int Idx_Section = 2;

        public async static Task<MemberSection[]> ReadFromCsvAsync()
        {
            string[][] matrix = await MyFile.ReadStringMatrixFromCsvAsync(FileName, false);

            //  Ernst; Florian; Tennis
            //  Ferrari; Rene; Tennis
            //  Leimgruber; Florian; Tennis

            var members = matrix.GroupBy(line => $"{line[Idx_FirstName]} {line[Idx_LastName]}")
                .Select(grp => new Member
                {
                    FirstName = grp.First()[Idx_FirstName],
                    LastName = grp.First()[Idx_LastName]
                })
                .ToDictionary(_ => $"{_.FirstName} {_.LastName}");



            var sections = matrix.GroupBy(line => line[Idx_Section])
                .Select(grp => new Section
                {
                    Name = grp.First()[Idx_Section]
                })
                .ToDictionary(_ => _.Name);


            return matrix
                .Select(line => new MemberSection
                {
                    Member = members[$"{line[Idx_FirstName]} {line[Idx_LastName]}"],
                    Section = sections[line[Idx_Section]]
                })
                .ToArray();
        }
    }
}
