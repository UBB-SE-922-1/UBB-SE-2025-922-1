using System;
using System.Windows.Input;
using DuolingoClassLibrary.Entities;
using Duo.Services;
using Duo.Commands;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duo.Services.Interfaces;

namespace Duo.ViewModels
{
    public class PostListViewModel : INotifyPropertyChanged
    {
        private readonly IPostService _postService;
        
        // Constants for validation and defaults
        private const int INVALID_ID = 0;
        private const int DEFAULT_ITEMS_PER_PAGE = 5;
        private const int DEFAULT_PAGE_NUMBER = 1;
        private const int DEFAULT_TOTAL_PAGES = 1;
        private const int DEFAULT_COUNT = 0;
        private const string ALL_HASHTAGS_FILTER = "All";

        
        private int? _categoryID;
        private string _categoryName;
        private string _filterText;
        private ObservableCollection<Post> _posts;
        private int _currentPage;
        private HashSet<string> _selectedHashtags = new HashSet<string>();
        private const int ItemsPerPage = 5;
        private int _totalPages = 1;
        private List<string> _allHashtags = new List<string>();
        private int _totalPostCount = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public PostListViewModel(IPostService postService, ICategoryService? categoryService = null)
        {
            _postService = postService ?? App._postService;
            _posts = new ObservableCollection<Post>();
            _currentPage = DEFAULT_PAGE_NUMBER;
            _selectedHashtags.Add(ALL_HASHTAGS_FILTER);

            LoadPostsCommand = new RelayCommand(async () => await LoadPostsAsync());
            NextPageCommand = new RelayCommand(async () => await NextPageAsync());
            PreviousPageCommand = new RelayCommand(async () => await PreviousPageAsync());
            FilterPostsCommand = new RelayCommand(async () => await FilterPostsAsync());
            ClearFiltersCommand = new RelayCommand(async () => await ClearFiltersAsync());

            LoadAllHashtagsAsync();
        }

        public ObservableCollection<Post> Posts
        {
            get => _posts;
            set
            {
                _posts = value;
                OnPropertyChanged();
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                OnPropertyChanged();
                FilterPostsAsync();
            }
        }

        public int CategoryID
        {
            get => _categoryID ?? INVALID_ID;
            set
            {
                _categoryID = value;
                OnPropertyChanged();

                LoadAllHashtagsAsync();
            }
        }

        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                OnPropertyChanged();
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged();
            }
        }

        public HashSet<string> SelectedHashtags => _selectedHashtags;

        public List<string> AllHashtags 
        {
            get => _allHashtags;
            set
            {
                _allHashtags = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadPostsCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand FilterPostsCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        private async void LoadAllHashtagsAsync()
        {
            try
            {
                _allHashtags.Clear();
                _allHashtags.Add(ALL_HASHTAGS_FILTER);

                var hashtags = await _postService.GetHashtags(_categoryID);
                foreach (var hashtag in hashtags)
                {
                    if (!_allHashtags.Contains(hashtag.Name))
                    {
                        _allHashtags.Add(hashtag.Name);
                    }
                }

                OnPropertyChanged(nameof(AllHashtags));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading hashtags: {ex.Message}");
            }
        }

        public async Task LoadPostsAsync()
        {
            try
            {
                var (posts, totalCount) = await _postService.GetFilteredAndFormattedPosts(
                    _categoryID,
                    _selectedHashtags.ToList(),
                    _filterText,
                    _currentPage,
                    ItemsPerPage);

                Posts.Clear();
                foreach (var post in posts)
                {
                    Posts.Add(post);
                }

                _totalPostCount = totalCount;
                TotalPages = Math.Max(DEFAULT_TOTAL_PAGES, (int)Math.Ceiling(_totalPostCount / (double)ItemsPerPage));
                OnPropertyChanged(nameof(TotalPages));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading posts: {ex.Message}");
            }
        }

        private async Task NextPageAsync()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await LoadPostsAsync();
            }
        }

        private async Task PreviousPageAsync()
        {
            if (CurrentPage > DEFAULT_PAGE_NUMBER)
            {
                CurrentPage--;
                await LoadPostsAsync();
            }
        }

        public async Task ToggleHashtagAsync(string hashtag)
        {
            _selectedHashtags = _postService.ToggleHashtagSelection(_selectedHashtags, hashtag, ALL_HASHTAGS_FILTER);
            CurrentPage = DEFAULT_PAGE_NUMBER;
            OnPropertyChanged(nameof(SelectedHashtags));
            await LoadPostsAsync();
        }

        public async Task FilterPostsAsync()
        {
            CurrentPage = DEFAULT_PAGE_NUMBER;
            await LoadPostsAsync();
        }

        public async Task ClearFiltersAsync()
        {
            FilterText = string.Empty;
            _selectedHashtags.Clear();
            _selectedHashtags.Add(ALL_HASHTAGS_FILTER);
            CurrentPage = DEFAULT_PAGE_NUMBER;
            await LoadPostsAsync();
            OnPropertyChanged(nameof(SelectedHashtags));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Legacy methods for backward compatibility
        public void LoadPosts()
        {
            _ = LoadPostsAsync();
        }

        public void ToggleHashtag(string hashtag)
        {
            _ = ToggleHashtagAsync(hashtag);
        }

        public void FilterPosts()
        {
            _ = FilterPostsAsync();
        }

        public void ClearFilters()
        {
            _ = ClearFiltersAsync();
        }

        private void NextPage()
        {
            _ = NextPageAsync();
        }

        private void PreviousPage()
        {
            _ = PreviousPageAsync();
        }
    }
}
