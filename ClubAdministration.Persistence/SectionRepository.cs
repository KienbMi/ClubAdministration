using ClubAdministration.Core.Contracts;
using ClubAdministration.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubAdministration.Persistence
{
    public class SectionRepository : ISectionRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SectionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Section>> GetAllSectionsAsync()
        {
            return await _dbContext.Sections
                .OrderBy(_ => _.Name)
                .ToArrayAsync();
        }

        public async Task<string[]> GetSectionNamesForMemberAsync(int memberId)
        {
            return await _dbContext.MemberSections.Where(_ => _.MemberId == memberId)
                .OrderBy(_ => _.Section.Name)
                .Select(_ => _.Section.Name)
                .ToArrayAsync();
        }
    }
}