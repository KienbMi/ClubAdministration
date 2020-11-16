using ClubAdministration.Core.DataTransferObjects;
using ClubAdministration.Core.Entities;
using ClubAdministration.Persistence;
using ClubAdministration.Wpf.Common;
using ClubAdministration.Wpf.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClubAdministration.Wpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private Section _selectedSection;
        private ObservableCollection<Section> _sections;
        private ObservableCollection<MemberDto> _members;
        private MemberDto _selectedMember;

        public ObservableCollection<Section> Sections 
        {
            get => _sections; 
            set
            {
                _sections = value;
                OnPropertyChanged();
            }
        }

        public Section SelectedSection
        {
            get => _selectedSection; 
            set 
            { 
                _selectedSection = value;
                OnPropertyChanged();
                SelectedMember = null;
                _ = LoadMembersAsync();
            }
        }

        public MemberDto SelectedMember
        {
            get => _selectedMember;
            set 
            { 
                _selectedMember = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MemberDto> Members
        {
            get => _members;
            set 
            { 
                _members = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(IWindowController windowController) : base(windowController)
        {
            LoadCommands();
        }

        private void LoadCommands()
        {
        }

        private async Task LoadDataAsync()
        {
            using UnitOfWork uow = new UnitOfWork();
            var sections = await uow.SectionRepository.GetAllSectionsAsync();
            Sections = new ObservableCollection<Section>(sections);
            SelectedSection = Sections.FirstOrDefault();
        }

        private async Task LoadMembersAsync()
        {           
            if (SelectedSection == null)
            {
                return;
            }

            var selcetedMember = SelectedMember;

            using UnitOfWork uow = new UnitOfWork();
            var members = await uow.MemberRepository.GetMemberDtoBySectionIdAsync(SelectedSection.Id);
            Members = new ObservableCollection<MemberDto>(members);
            if (selcetedMember == null)
                SelectedMember = Members.FirstOrDefault();
            else
                SelectedMember = Members.FirstOrDefault(m => m.Id == selcetedMember.Id);
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }

        public static async Task<MainViewModel> CreateAsync(IWindowController windowController)
        {
            var viewModel = new MainViewModel(windowController);
            await viewModel.LoadDataAsync();
            return viewModel;
        }

        // Commands
        private ICommand _cmdEditMember;

        public ICommand CmdEditMember 
        { 
            get
            {
                if (_cmdEditMember == null)
                {
                    _cmdEditMember = new RelayCommand(
                        execute: async _ =>
                        {
                            var window = new EditMemberViewModel(Controller, SelectedMember);
                            Controller.ShowWindow(window, true);
                            await LoadMembersAsync();
                        },
                        canExecute: _ => SelectedMember != null
                        );
                }
                return _cmdEditMember;
            }
        }
    }
}
