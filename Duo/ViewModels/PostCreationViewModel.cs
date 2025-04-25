using System;
using System.Windows.Input;
using DuolingoClassLibrary.Entities;
using Duo.Services;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Duo.Commands;
using System.Collections.Generic;
using System.Diagnostics;
using static Duo.App;
using System.Collections.ObjectModel;
using Duo.Views.Components;
using System.Threading.Tasks;
using Duo.Helpers;
using Duo.Services.Interfaces;

namespace Duo.ViewModels
{
    /// <summary>
    /// The PostCreationViewModel is responsible for managing the creation and editing of posts.
    /// It provides properties and methods for interacting with the post creation UI,
    /// and handles the communication with the database through the PostService.
    /// 
    /// Features:
    /// - Managing post title and content
    /// - Handling hashtags (add, remove)
    /// - Community selection
    /// - Post creation with validation
    /// - Error handling
    /// </summary>
    public class PostCreationViewModel : INotifyPropertyChanged
    {
        // Constants for validation and defaults
        private const int INVALID_ID = 0;
        private const int DEFAULT_COUNT = 0;
        private const string EMPTY_STRING = "";
        
        // Services
        private readonly IPostService _postService;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;

        // Properties
        private string _postTitle = string.Empty;
        private string _postContent = string.Empty;
        private int _selectedCategoryId;
        private ObservableCollection<string> _postHashtags = new ObservableCollection<string>();
        private ObservableCollection<CommunityItem> _postCommunities = new ObservableCollection<CommunityItem>();
        private string _lastError = string.Empty;
        private bool _isLoading;
        private bool _isSuccess;

        // Commands
        public ICommand CreatePostCommand { get; private set; }
        public ICommand AddHashtagCommand { get; private set; }
        public ICommand RemoveHashtagCommand { get; private set; }
        public ICommand SelectCommunityCommand { get; private set; }

        // Property changed event
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler PostCreationSuccessful;


        public string Title
        {
            get => _postTitle;
            set
            {
                if (_postTitle != value)
                {
                    _postTitle = value;
                    var (isValid, errorMessage) = ValidationHelper.ValidatePostTitle(value);
                    if (!isValid)
                    {
                        LastError = errorMessage;
                    }
                    else
                    {
                        LastError = EMPTY_STRING;
                    }
                    OnPropertyChanged();
                }
            }
        }

        public string Content
        {
            get => _postContent;
            set
            {
                if (_postContent != value)
                {
                    _postContent = value;
                    var (isValid, errorMessage) = ValidationHelper.ValidatePostContent(value);
                    if (!isValid)
                    {
                        LastError = errorMessage;
                    }
                    else
                    {
                        LastError = EMPTY_STRING;
                    }
                    OnPropertyChanged();
                }
            }
        }

        public int SelectedCategoryId
        {
            get => _selectedCategoryId;
            set
            {
                if (_selectedCategoryId != value)
                {
                    _selectedCategoryId = value;
                    OnPropertyChanged();
                    UpdateSelectedCommunity();
                }
            }
        }

        public ObservableCollection<string> Hashtags => _postHashtags;

        public ObservableCollection<CommunityItem> Communities => _postCommunities;

        public string LastError
        {
            get => _lastError;
            set
            {
                if (_lastError != value)
                {
                    _lastError = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSuccess
        {
            get => _isSuccess;
            set
            {
                if (_isSuccess != value)
                {
                    _isSuccess = value;
                    OnPropertyChanged();
                }
            }
        }


        public PostCreationViewModel()
        {
            // Get services from App
            _postService = _postService ?? App._postService;
            _categoryService = _categoryService ?? App._categoryService;
            _userService = _userService ?? App.userService;

            // Initialize commands
            CreatePostCommand = new RelayCommand(async () => await CreatePostAsync());
            AddHashtagCommand = new RelayCommandWithParameter<string>(AddHashtag);
            RemoveHashtagCommand = new RelayCommandWithParameter<string>(RemoveHashtag);
            SelectCommunityCommand = new RelayCommandWithParameter<int>(SelectCommunity);

            // Load initial data
            LoadCommunities();
        }

        #region Public Methods

        public async Task CreatePostAsync()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Content))
            {
                LastError = "Title and content are required.";
                return;
            }

            var (isTitleValid, titleError) = ValidationHelper.ValidatePostTitle(Title);
            if (!isTitleValid)
            {
                LastError = titleError;
                return;
            }

            if (SelectedCategoryId <= INVALID_ID)
            {
                LastError = "Please select a community for your post.";
                return;
            }

            IsLoading = true;
            LastError = EMPTY_STRING;

            try
            {
                // Get current user ID
                var currentUser = await _userService.GetCurrentUserAsync();
                
                // Create a new Post object
                var newPost = new DuolingoClassLibrary.Entities.Post
                {
                    Title = Title,
                    Description = Content,
                    UserID = currentUser.UserId,
                    CategoryID = SelectedCategoryId,
                    CreatedAt = DateTimeHelper.EnsureUtcKind(DateTime.UtcNow),
                    UpdatedAt = DateTimeHelper.EnsureUtcKind(DateTime.UtcNow)
                };
                
                // Create post in database using the original CreatePost method
                int createdPostId = await _postService.CreatePost(newPost);
                
                // Add hashtags if any
                if (Hashtags.Count > DEFAULT_COUNT)
                {
                    foreach (var hashtagText in Hashtags)
                    {
                        try
                        {
                            await _postService.AddHashtagToPost(createdPostId, hashtagText, currentUser.UserId);
                        }
                        catch (Exception hashtagException)
                        {
                            Debug.WriteLine($"Error adding hashtag '{hashtagText}' to post: {hashtagException.Message}");
                            // Continue with other hashtags even if one fails
                        }
                    }
                }

                // Handle success
                IsSuccess = true;
                PostCreationSuccessful?.Invoke(this, EventArgs.Empty);

                // Clear form
                ClearForm();
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                LastError = $"Error creating post: {ex.Message}";
                Debug.WriteLine($"Error creating post: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<bool> CreatePostAsync(string title, string content, int categoryId, List<string> hashtags = null)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                LastError = "Title and content are required.";
                return false;
            }

            var (isTitleValid, titleError) = ValidationHelper.ValidatePostTitle(title);
            if (!isTitleValid)
            {
                LastError = titleError;
                return false;
            }

            if (categoryId <= INVALID_ID)
            {
                LastError = "Please select a valid community.";
                return false;
            }

            IsLoading = true;
            LastError = EMPTY_STRING;

            try
            {
                // Get current user ID
                var currentUser = await _userService.GetCurrentUserAsync();
                
                // Create a new Post object
                var newPost = new DuolingoClassLibrary.Entities.Post
                {
                    Title = title,
                    Description = content,
                    UserID = currentUser.UserId,
                    CategoryID = categoryId,
                    CreatedAt = DateTimeHelper.EnsureUtcKind(DateTime.UtcNow),
                    UpdatedAt = DateTimeHelper.EnsureUtcKind(DateTime.UtcNow)
                };
                
                // Create post in database using the CreatePostWithHashtags method
                var hashtagsList = hashtags ?? new List<string>();
                int createdPostId = await _postService.CreatePostWithHashtags(newPost, hashtagsList, currentUser.UserId);
                
                if (createdPostId > 0)
                {
                    // Handle success
                    IsSuccess = true;
                    PostCreationSuccessful?.Invoke(this, EventArgs.Empty);
                    return true;
                }
                else
                {
                    LastError = "Error creating post. Please try again.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                LastError = $"Error creating post: {ex.Message}";
                Debug.WriteLine($"Error creating post: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void AddHashtag(string hashtag)
        {
            if (string.IsNullOrWhiteSpace(hashtag))
            {
                return;
            }

            // Normalize hashtag format
            hashtag = hashtag.Trim();
            if (!hashtag.StartsWith("#"))
            {
                hashtag = "#" + hashtag;
            }

            // Check if hashtag already exists
            if (!Hashtags.Contains(hashtag))
            {
                Hashtags.Add(hashtag);
            }
        }

        public void RemoveHashtag(string hashtag)
        {
            if (string.IsNullOrWhiteSpace(hashtag))
            {
                return;
            }

            if (Hashtags.Contains(hashtag))
            {
                Hashtags.Remove(hashtag);
            }
        }

        public void SelectCommunity(int communityId)
        {
            SelectedCategoryId = communityId;
        }

        public void ClearForm()
        {
            Title = EMPTY_STRING;
            Content = EMPTY_STRING;
            SelectedCategoryId = 0;
            Hashtags.Clear();
            LastError = EMPTY_STRING;
            IsSuccess = false;
        }

        #endregion

        #region Private Methods

        private async void LoadCommunities()
        {
            try
            {
                Communities.Clear();
                var categories = await _categoryService.GetAllCategories();
                Communities.Add(new CommunityItem { Id = 0, Name = "Select a community" });
                
                foreach (var category in categories)
                {
                    if (category != null)
                    {
                        Communities.Add(new CommunityItem 
                        { 
                            Id = category.Id, 
                            Name = category.Name 
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LastError = $"Error loading communities: {ex.Message}";
                Debug.WriteLine($"Error loading communities: {ex.Message}");
            }
        }

        private void UpdateSelectedCommunity()
        {
            // Update the selected community in the UI, if needed
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        
        // Legacy method for backward compatibility
        public void CreatePost()
        {
            _ = CreatePostAsync();
        }
    }
}
