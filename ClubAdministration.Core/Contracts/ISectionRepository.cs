using ClubAdministration.Core.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClubAdministration.Core.Contracts
{
    public interface ISectionRepository
    {
        Task<IEnumerable<Section>> GetAllSectionsAsync();
        Task<string[]> GetSectionNamesForMemberAsync(int memberId);
    }
}
