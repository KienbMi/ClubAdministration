using ClubAdministration.Core.DataTransferObjects;
using ClubAdministration.Core.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ClubAdministration.Core.Contracts
{
    public interface IMemberRepository
    {
        bool IsMemberDuplicate(Member member);
        Task<IEnumerable<MemberDto>> GetMemberDtoBySectionIdAsync(int sectionId);

        Task<Member> GetMemberByIdAsync(int memberId);
    }
}
