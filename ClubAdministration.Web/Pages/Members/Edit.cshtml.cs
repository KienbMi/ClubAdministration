using ClubAdministration.Core.Contracts;
using ClubAdministration.Core.DataTransferObjects;
using ClubAdministration.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ClubAdministration.Web.Pages.Members
{
    public class EditModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public MemberDto Member { get; set; }


        public async Task<IActionResult> OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _unitOfWork
                .MemberRepository
                .GetMemberByIdAsync(id.Value);

            if (member == null)
            {
                return NotFound();
            }

            Member = new MemberDto
            {
                FirstName = member.FirstName,
                LastName = member.LastName,
                Id = member.Id
            };

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Member memberInDb = await _unitOfWork
                .MemberRepository
                .GetMemberByIdAsync(Member.Id);

            memberInDb.FirstName = Member.FirstName;
            memberInDb.LastName = Member.LastName;

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }

            return RedirectToPage("../Index");
        }
    }
}
