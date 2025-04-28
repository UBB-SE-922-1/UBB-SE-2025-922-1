using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System.Diagnostics;
using Duo.Views;
using Microsoft.Extensions.Configuration;
using Duo.ViewModels;
using Duo.Services;
using Duo.Data;
using Duo.Repositories;
using Duo.Repositories.Interfaces;
using DuolingoClassLibrary.Entities;
using Duo.Interfaces;
using Duo.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Duo.Services.Interfaces;
using DuolingoClassLibrary.Repositories;
using DuolingoClassLibrary.Repositories.Interfaces;

namespace Duo
{

    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider;
        public static User? CurrentUser { get; set; }
        public static Window? MainAppWindow { get; private set; }
        public static UserService userService;
        private static IConfiguration _configuration;
        public static DataLink _dataLink;
        public static UserRepository userRepository;
        public static IPostRepository _postRepository;
        public static IHashtagRepository _hashtagRepository;
        public static IPostService _postService;
        public static ICategoryService _categoryService;
        public static SearchService _searchService;

        public App()
        {
            this.InitializeComponent();

            _configuration = InitializeConfiguration();

            _dataLink = new DataLink(_configuration);

            userRepository = new UserRepository(_dataLink);
            _hashtagRepository = new HashtagRepository(_dataLink);
            ICategoryRepository categoryRepository = new CategoryRepositoryProxi();
            _postRepository = new PostRepositoryProxi();

            userService = new UserService(userRepository);
            _searchService = new SearchService();
            _postService = new PostService(_postRepository, _hashtagRepository, userService, _searchService);
            _categoryService = new CategoryService(categoryRepository);

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Register configuration
            services.AddSingleton(_configuration!);

            // Register data access
            services.AddSingleton<IDataLink, DataLink>();
            services.AddSingleton<DataLink>();

            // Register repositories
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<UserRepository>();
            services.AddSingleton<IFriendsRepository, FriendsRepository>();
            services.AddSingleton<ListFriendsRepository>();
            services.AddSingleton<IPostRepository, PostRepositoryProxi>();
            services.AddSingleton<ICommentRepository, CommentRepositoryProxi>();

            // Register services
            services.AddSingleton<ICommentService, CommentService>();
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<FriendsService>();
            services.AddTransient<SignUpService>();
            services.AddTransient<ProfileService>();
            services.AddTransient<LeaderboardService>();

            // Register view models
            services.AddTransient<LoginViewModel>();
            services.AddTransient<SignUpViewModel>();
            services.AddTransient<ResetPassViewModel>();
            services.AddTransient<ListFriendsViewModel>();
            services.AddTransient<ProfileViewModel>();
            services.AddTransient<LeaderboardViewModel>();
        }

        private IConfiguration InitializeConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("duoappsettings.json", optional: false, reloadOnChange: true);

            return builder.Build();
        }

        /// <summary>
        /// Handles the application launch.
        /// </summary>
        /// <param name="args">Launch arguments.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            MainAppWindow = new MainWindow();
            MainAppWindow.Activate();
        }

        private Window? window;
    }
}