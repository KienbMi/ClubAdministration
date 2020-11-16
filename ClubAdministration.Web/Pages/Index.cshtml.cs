using ClubAdministration.Core.Contracts;
using ClubAdministration.Core.DataTransferObjects;
using ClubAdministration.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;

namespace ClubAdministration.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public MemberDto[] Members { get; set; }
        [BindProperty]
        public Section[] Sections { get; set; }
        [BindProperty]
        public int SelectedSectionId { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Sections = (await _unitOfWork
                .SectionRepository
                .GetAllSectionsAsync())
                .ToArray();                            

            Members = new MemberDto[] { };
            if (Members == null)
            {
                return NotFound();
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            Sections = (await _unitOfWork
                .SectionRepository
                .GetAllSectionsAsync())
                .ToArray();

            Members = (await _unitOfWork
                .MemberRepository
                .GetMemberDtoBySectionIdAsync(SelectedSectionId))
                .ToArray();

            if (Members == null)
            {
                Members = new MemberDto[] { };
                return NotFound();
            }
            
            return Page();
        }
    }
}
