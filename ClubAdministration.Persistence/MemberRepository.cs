using ClubAdministration.Core.Contracts;
using ClubAdministration.Core.DataTransferObjects;
using ClubAdministration.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ClubAdministration.Persistence
{
    public class MemberRepository : IMemberRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public MemberRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Member> GetMemberByIdAsync(int memberId)
            => await _dbContext.Members.SingleOrDefaultAsync(_ => _.Id == memberId);

        public async Task<Member> GetMemberByNameAsync(string lastName, string firstName)
        {
            return await _dbContext
                .Members.FirstOrDefaultAsync(m => m.FirstName == firstName && m.LastName == lastName);
        }

        public async Task<IEnumerable<MemberDto>> GetMemberDtoBySectionIdAsync(int sectionId)
        {
            return (await _dbContext.MemberSections
                .Where(_ => _.SectionId == sectionId)
                .Include(_ => _.Member)
                .ToArrayAsync())
                .GroupBy(_ => _.Member)
                .Select(grp => new MemberDto
                {
                    Id = grp.Key.Id,
                    FirstName = grp.Key.FirstName,
                    LastName = grp.Key.LastName,
                    CountSections = _dbContext.MemberSections.Where(ms => ms.MemberId == grp.Key.Id).Count()
                })
                .OrderBy(_ => _.LastName)
                .ThenBy(_ => _.LastName)
                .ToArray();
        }

        public async Task<string[]> GetMemberNamesAsync()
        {
            return await _dbContext.Members
                .OrderBy(_ => _.LastName)
                    .ThenBy(_ => _.FirstName)
                .Select(_ => $"{_.LastName} {_.FirstName}")
                .ToArrayAsync();
        }

        public bool IsMemberDuplicate(Member member)
        {
            Member memberInDb = _dbContext
                .Members
                .FirstOrDefault(_ => member.Id != _.Id &&
                                    member.FirstName == _.FirstName &&
                                    member.LastName == _.LastName);

            return memberInDb != null;
        }
    }
}