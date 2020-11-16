using ClubAdministration.Core.DataTransferObjects;
using ClubAdministration.Core.Entities;
using ClubAdministration.Persistence;
using ClubAdministration.Wpf.Common;
using ClubAdministration.Wpf.Common.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClubAdministration.Wpf.ViewModels
{
    public class EditMemberViewModel : BaseViewModel
    {
        private string _lastName;
        private string _firstName;
        private MemberDto _member;

        public string LastName
        {
            get => _lastName;
            set 
            { 
                _lastName = value;
                OnPropertyChanged();
                Validate();
            }
        }

        public string FirstName
        {
            get => _firstName;
            set 
            { 
                _firstName = value;
                OnPropertyChanged();
                Validate();
            }
        }

        public EditMemberViewModel(IWindowController controller, MemberDto member) : base(controller)
        {
            LastName = member.LastName;
            FirstName = member.FirstName;
            _member = member;
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(LastName))
            {
                yield return new ValidationResult("Lastname is required", new string[] { nameof(LastName) });
            }
            else if (LastName.Length < 2)
            {
                yield return new ValidationResult("Lastname must be at least two characters long", new string[] { nameof(LastName) });
            }

            if (_member != null)
            {
                using UnitOfWork uow = new UnitOfWork();
                var editedMember = new Member { FirstName = FirstName, LastName = LastName, Id = _member.Id };
                if (uow.MemberRepository.IsMemberDuplicate(editedMember))
                {
                    yield return new ValidationResult($"Member {LastName} {FirstName} already exists", new string[] { nameof(LastName), nameof(FirstName) });
                }
            }
        }

        // Commands
        private ICommand _cmdSaveMember;

        public ICommand CmdSaveMember 
        { 
            get
            {
                if (_cmdSaveMember == null)
                {
                    _cmdSaveMember = new RelayCommand(
                        execute: async _ => 
                        {
                            await SaveChangesAsync();
                        },

                        canExecute: _ => IsValid
                        );
                }
                return _cmdSaveMember;
            }
        }

        private async Task SaveChangesAsync()
        {
            using UnitOfWork uow = new UnitOfWork();
            var memberInDb = await uow.MemberRepository.GetMemberByIdAsync(_member.Id);

            memberInDb.FirstName = FirstName;
            memberInDb.LastName = LastName;

            try
            {
                await uow.SaveChangesAsync();
                Controller.CloseWindow(this);
            }
            catch (ValidationException validationException)
            {
                if (validationException.Value is IEnumerable<string> properties)
                {
                    foreach (var property in properties)
                    {
                        AddErrorsToProperty(property, new List<string> { validationException.ValidationResult.ErrorMessage });
                    }
                }
                else
                {
                    DbError = validationException.ValidationResult.ToString();
                }
            }
        }
    }
}
