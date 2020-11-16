using ClubAdministration.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClubAdministration.Persistence.Validations
{
    public class DuplicateMemberValidation : ValidationAttribute
    {
        private readonly UnitOfWork _unitOfWork;

        public DuplicateMemberValidation(UnitOfWork unitOfWork) : base()
        {
            _unitOfWork = unitOfWork;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is Member member))
            {
                throw new ArgumentException("Value is not a member", nameof(value));
            }

            if (_unitOfWork.MemberRepository.IsMemberDuplicate(member))
                return new ValidationResult("Es existiert bereits ein Mitglied mit dem Namen");
            else
                return ValidationResult.Success;
        }
    }
}
