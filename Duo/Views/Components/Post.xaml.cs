using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.UI.Controls;

// is this the right way to access userService and its methods?
using static Duo.App;
using Duo.Views.Pages;
using Duo.ViewModels;
namespace Duo.Views.Components
{
    public sealed partial class Post : UserControl
    {
        // Constants
        private const string UNKNOWN_USER_TEXT = "Unknown User";
        
        public static readonly DependencyProperty UsernameProperty = 
            DependencyProperty.Register(nameof(Username), typeof(string), typeof(Post), new PropertyMetadata(""));
        
        public static readonly DependencyProperty DateProperty = 
            DependencyProperty.Register(nameof(Date), typeof(string), typeof(Post), new PropertyMetadata(""));
        
        public static readonly DependencyProperty TitleProperty = 
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(Post), new PropertyMetadata(""));
        
        public static new readonly DependencyProperty ContentProperty = 
            DependencyProperty.Register(nameof(Content), typeof(string), typeof(Post), new PropertyMetadata(""));
        
        public static readonly DependencyProperty LikeCountProperty = 
            DependencyProperty.Register(nameof(LikeCount), typeof(int), typeof(Post), new PropertyMetadata(0));
        
        public static readonly DependencyProperty HashtagsProperty = 
            DependencyProperty.Register(nameof(Hashtags), typeof(IEnumerable<string>), typeof(Post), new PropertyMetadata(null));

        public static readonly DependencyProperty PostIdProperty = 
            DependencyProperty.Register(nameof(PostId), typeof(int), typeof(Post), new PropertyMetadata(0));

        public static readonly DependencyProperty IsAlwaysHighlightedProperty = 
            DependencyProperty.Register(nameof(IsAlwaysHighlighted), typeof(bool), typeof(Post), new PropertyMetadata(false, OnIsAlwaysHighlightedChanged));
            
        private static void OnIsAlwaysHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Post post)
            {
                post.UpdateHighlightState();
            }
        }

        private bool _isPointerOver;
        private LikeButton? _likeButton;

        public Post()
        {
            InitializeComponent();
            
            _ = UpdateMoreOptionsVisibilityAsync();
            UpdateHighlightState();
            
            // Subscribe to the Loaded event
            Loaded += Post_Loaded;
        }
        
        private void Post_Loaded(object sender, RoutedEventArgs e)
        {
            // Find and connect to the LikeButton
            _likeButton = FindDescendant<LikeButton>(this);
            if (_likeButton != null)
            {
                _likeButton.LikeClicked += LikeButton_LikeClicked;
            }
        }
        
        // Find a descendant control of a specific type
        private T FindDescendant<T>(DependencyObject parent) where T : DependencyObject
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                
                if (child is T result)
                {
                    return result;
                }
                
                T descendant = FindDescendant<T>(child);
                if (descendant != null)
                {
                    return descendant;
                }
            }
            
            return null;
        }
        
        private void LikeButton_LikeClicked(object sender, LikeButtonClickedEventArgs e)
        {
            if (e.TargetType == LikeTargetType.Post && e.TargetId == PostId)
            {
                try
                {
                    _ = LikePostAsync(e.TargetId);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error liking post: {ex.Message}");
                }
            }
        }

        private async Task LikePostAsync(int postId)
        {
            bool success = await App._postService.LikePost(postId);
            if (success)
            {
                if (_likeButton != null)
                {
                    _likeButton.IncrementLikeCount();
                }
                
                System.Diagnostics.Debug.WriteLine($"Post liked: ID {PostId}, new count: {LikeCount}");
            }
        }

        private async Task UpdateMoreOptionsVisibilityAsync()
        {
            try
            {
                var currentUser = await userService.GetCurrentUserAsync();
                if (currentUser != null)
                {
                    MoreOptions.Visibility = (this.Username == currentUser.UserName) 
                        ? Visibility.Visible 
                        : Visibility.Collapsed;
                }
                else
                {
                    MoreOptions.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating options visibility: {ex.Message}");
                MoreOptions.Visibility = Visibility.Collapsed;
            }
        }

        // Handle pointer entered event for hover effects
        private void PostBorder_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _isPointerOver = true;
            
            if (!IsAlwaysHighlighted) 
            {
                if (sender is Border border)
                {
                    border.Background = Application.Current.Resources["SystemControlBackgroundAltHighBrush"] as Microsoft.UI.Xaml.Media.Brush;
                    border.BorderBrush = Application.Current.Resources["SystemControlBackgroundListLowBrush"] as Microsoft.UI.Xaml.Media.Brush;
                }
            }
        }

        // Handle pointer exited event for hover effects
        private void PostBorder_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            _isPointerOver = false;
            
            if (!IsAlwaysHighlighted) 
            {
                if (sender is Border border)
                {
                    border.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                        Microsoft.UI.Colors.Transparent);
                    border.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                        Microsoft.UI.Colors.Transparent);
                }
            }
        }

        // Handle tapped event for navigation
        private void PostBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (IsAlwaysHighlighted)
            {
                return;
            }
            
            // Check if the tap originated from a LikeButton or its children
            if (IsLikeButtonTap(e.OriginalSource as DependencyObject))
            {
                // Skip navigation if the tap was on the like button
                return;
            }
            
            // Get the parent frame for navigation
            var frame = FindParentFrame();
            if (frame != null)
            {
                // Create a Post with the current post's data
                var post = new DuolingoClassLibrary.Entities.Post
                {
                    Title = this.Title ?? string.Empty,
                    Description = this.Content ?? string.Empty,
                    Id = this.PostId,
                    Username = this.Username ?? UNKNOWN_USER_TEXT,
                    Date = this.Date ?? string.Empty,
                    LikeCount = this.LikeCount,
                    Hashtags = this.Hashtags?.ToList() ?? new List<string>()
                };
                
                // Navigate to the DetailPage
                frame.Navigate(typeof(PostDetailPage), post);
            }
        }
        
        // Helper method to determine if a tap originated from the LikeButton
        private bool IsLikeButtonTap(DependencyObject element)
        {
            if (element == null)
            {
                return false;
            }
            
            // Check if the element is a LikeButton
            if (element is LikeButton)
            {
                return true;
            }
            
            // Recursively check parent elements
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            if (parent != null)
            {
                return IsLikeButtonTap(parent);
            }
            
            return false;
        }

        // Helper method to find the parent Frame
        private Frame FindParentFrame()
        {
            DependencyObject element = this;
            while (element != null && !(element is Frame))
            {
                element = VisualTreeHelper.GetParent(element);
            }
            
            return element as Frame;
        }

        // Event handlers for the MoreDropdown component
        private async void MoreOptions_EditClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if the current user is the author of the post
                var currentUser = await userService.GetCurrentUserAsync();
                if (currentUser == null || currentUser.UserName != this.Username)
                {
                    return;
                }
                
                // Create a content dialog for editing the post
                ContentDialog editDialog = new ContentDialog
                {
                    Title = "Edit Post",
                    PrimaryButtonText = "Save",
                    CloseButtonText = "Cancel",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.XamlRoot
                };
                
                // Create layout for edit fields
                StackPanel editPanel = new StackPanel
                {
                    Spacing = 15
                };
                
                // Title field
                TextBox titleBox = new TextBox
                {
                    Header = "Title",
                    Text = this.Title,
                    PlaceholderText = "Enter post title",
                    IsSpellCheckEnabled = true
                };
                
                // Content field
                TextBox contentBox = new TextBox
                {
                    Header = "Content",
                    Text = this.Content,
                    PlaceholderText = "Enter post content",
                    AcceptsReturn = true,
                    TextWrapping = TextWrapping.Wrap,
                    Height = 150,
                    IsSpellCheckEnabled = true
                };
                
                // Add fields to panel
                editPanel.Children.Add(titleBox);
                editPanel.Children.Add(contentBox);
                
                // Set content of dialog
                editDialog.Content = editPanel;
                
                // Show the dialog and handle result
                var result = await editDialog.ShowAsync();
                
                if (result == ContentDialogResult.Primary)
                {
                    // Get updated values from dialog
                    string updatedTitle = titleBox.Text?.Trim() ?? string.Empty;
                    string updatedContent = contentBox.Text?.Trim() ?? string.Empty;
                    
                    // Validate input
                    if (string.IsNullOrEmpty(updatedTitle) || string.IsNullOrEmpty(updatedContent))
                    {
                        // Show validation error dialog
                        ContentDialog errorDialog = new ContentDialog
                        {
                            Title = "Validation Error",
                            Content = "Title and content cannot be empty.",
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot
                        };
                        
                        await errorDialog.ShowAsync();
                        return;
                    }
                    
                    // Create updated post
                    var updatedPost = new DuolingoClassLibrary.Entities.Post
                    {
                        Id = this.PostId,
                        Title = updatedTitle,
                        Description = updatedContent,
                        UpdatedAt = DateTime.UtcNow
                    };
                    
                    try
                    {
                        // Call service to update post
                        await App._postService.UpdatePost(updatedPost);
                        
                        // Update UI
                        this.Title = updatedTitle;
                        this.Content = updatedContent;
                    }
                    catch (Exception ex)
                    {
                        // Show error dialog
                        ContentDialog errorDialog = new ContentDialog
                        {
                            Title = "Error",
                            Content = $"Failed to update post: {ex.Message}",
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot
                        };
                        
                        await errorDialog.ShowAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error editing post: {ex.Message}");
            }
        }

        private async void MoreOptions_DeleteClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if the current user is the author of the post
                var currentUser = await userService.GetCurrentUserAsync();
                if (currentUser == null || currentUser.UserName != this.Username)
                {
                    return;
                }
                
                // Show confirmation dialog
                ContentDialog deleteDialog = new ContentDialog
                {
                    Title = "Delete Post",
                    Content = "Are you sure you want to delete this post? This action cannot be undone.",
                    PrimaryButtonText = "Delete",
                    CloseButtonText = "Cancel",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.XamlRoot
                };
                
                // Show dialog and handle result
                var result = await deleteDialog.ShowAsync();
                
                if (result == ContentDialogResult.Primary)
                {
                    try
                    {
                        // Call service to delete post
                        await App._postService.DeletePost(this.PostId);
                        
                        // Navigate back if in detail view
                        var frame = FindParentFrame();
                        if (frame != null && frame.CanGoBack)
                        {
                            frame.GoBack();
                        }
                        else
                        {
                            // If in list view, remove this post
                            var parent = VisualTreeHelper.GetParent(this) as Panel;
                            if (parent != null)
                            {
                                parent.Children.Remove(this);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Show error dialog
                        ContentDialog errorDialog = new ContentDialog
                        {
                            Title = "Error",
                            Content = $"Failed to delete post: {ex.Message}",
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot
                        };
                        
                        await errorDialog.ShowAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting post: {ex.Message}");
            }
        }

        // Event handlers for MarkdownTextBlock
        private void MarkdownText_MarkdownRendered(object sender, CommunityToolkit.WinUI.UI.Controls.MarkdownRenderedEventArgs e)
        {
            // This method can be left empty or used for additional handling after markdown renders
        }

        private async void MarkdownText_LinkClicked(object sender, CommunityToolkit.WinUI.UI.Controls.LinkClickedEventArgs e)
        {
            // Handle link clicks in the markdown text
            if (!string.IsNullOrEmpty(e.Link))
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(e.Link));
            }
        }

        private void UpdateHighlightState()
        {
            if (IsAlwaysHighlighted)
            {
                PostBorder.Background = Application.Current.Resources["SystemControlBackgroundAltHighBrush"] as Microsoft.UI.Xaml.Media.Brush;
                PostBorder.BorderBrush = Application.Current.Resources["SystemControlBackgroundListLowBrush"] as Microsoft.UI.Xaml.Media.Brush;
            }
            else if (!_isPointerOver)
            {
                PostBorder.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.Colors.Transparent);
                PostBorder.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.Colors.Transparent);
            }
        }

        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }
        
        public string Date
        {
            get { return (string)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }
        
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        
        public new string Content
        {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        
        public int LikeCount
        {
            get { return (int)GetValue(LikeCountProperty); }
            set { SetValue(LikeCountProperty, value); }
        }
        
        public IEnumerable<string> Hashtags
        {
            get { return (IEnumerable<string>)GetValue(HashtagsProperty); }
            set { SetValue(HashtagsProperty, value); }
        }
        
        public int PostId
        {
            get { return (int)GetValue(PostIdProperty); }
            set { SetValue(PostIdProperty, value); }
        }
        
        public bool IsAlwaysHighlighted
        {
            get { return (bool)GetValue(IsAlwaysHighlightedProperty); }
            set { SetValue(IsAlwaysHighlightedProperty, value); }
        }
    }
} 