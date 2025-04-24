## UBB-SE-2025-Messi (Messi > Ronaldo)

![CleanShot 2025-03-25 at 2  02 49@2x](https://github.com/user-attachments/assets/56bb5065-f478-4481-81dd-3cb8ba18745e)

> Best looking Windows app ^^

## Dev

Create a `appsettings.json` file in the `Duo` directory with the following content:

```json
{
  "LocalDataSource": "np:\\\\.\\pipe\\your_pipe_name\\tsql\\query",
  "InitialCatalog": "your_database_name" // e.g. "Duo"
}
```

You can find the pipe by running:

```cmd
SqlLocalDB.exe start
SqlLocalDB.exe info MSSQLLocalDB
```

Go inside `Duo/Data` and from there run the `run_all_procedures.ps1` script to run all stored procedures. You can get mock data from [here](https://github.com/cristicretu/UBB-SE-2025-Messi/blob/main/Duo/Data/mockData.sql).

## Project Structure

```
UBB-SE-2025-Messi/
├── 📱 Duo/                           # Main application directory
│   ├── 🖼️ Views/                     # UI components (View layer)
│   │   ├── Pages/                    # Main application pages
│   │   └── Components/               # Reusable UI components
│   │
│   ├── 📊 ViewModels/                # View models (ViewModel layer)
│   │   ├── Base/                     # Base view model classes
│   │   ├── MainViewModel.cs          # Main view model
│   │   ├── LoginViewModel.cs         # Login view model
│   │   ├── PostViewModel.cs          # Post view model
│   │   ├── PostListViewModel.cs      # Post list view model
│   │   ├── PostCreationViewModel.cs  # Post creation view model
│   │   ├── CommentViewModel.cs       # Comment view model
│   │   ├── CommentCreationViewModel.cs # Comment creation view model
│   │   ├── CategoryViewModel.cs      # Category view model
│   │   └── SearchViewModel.cs        # Search view model
│   │
│   ├── 📦 Models/                    # Data models (Model layer)
│   │   ├── User.cs                   # User model
│   │   ├── Post.cs                   # Post model
│   │   ├── PostHashtag.cs            # Post hashtag model
│   │   ├── Comment.cs                # Comment model
│   │   ├── Category.cs               # Category model
│   │   └── Hashtag.cs                # Hashtag model
│   │
│   ├── 🔄 Services/                  # Business logic services
│   │   ├── UserService.cs            # User-related services
│   │   ├── PostService.cs            # Post-related services
│   │   ├── CommentService.cs         # Comment-related services
│   │   ├── CategoryService.cs        # Category-related services
│   │   ├── SearchService.cs          # Search functionality
│   │   └── MarkdownService.cs        # Markdown processing
│   │
│   ├── 🗄️ Repositories/              # Data access layer
│   │   ├── UserRepository.cs         # User data access
│   │   ├── PostRepository.cs         # Post data access
│   │   ├── CommentRepository.cs      # Comment data access
│   │   ├── CategoryRepository.cs     # Category data access
│   │   └── HashtagRepository.cs      # Hashtag data access
│   │
│   ├── 🧰 Helpers/                   # Helper utilities
│   │
│   ├── 🗄️ Data/                      # Data layer
│   │   └── 📊 Queries/               # Database queries
│   │
│   ├── 🎨 Resources/                 # Application resources
│   │   ├── 🖼️ Images/                # Image assets
│   │   └── 🎭 Styles/                # Style definitions
│   │
│   ├── 🖼️ Assets/                    # Application assets
│   │
│   ├── ⚙️ Properties/                # Project properties
│   │
│   ├── 📄 App.xaml                   # Application definition
│   ├── 📄 App.xaml.cs                # Application code-behind
│   ├── 📄 MainWindow.xaml            # Main window definition
│   ├── 📄 MainWindow.xaml.cs         # Main window code-behind
│   ├── 📄 Duo.csproj                 # Project file
│   └── 📄 Duo.sln                    # Solution file
│
├── 📝 planning/                      # Project planning documents
│   ├── 📊 usecase diagram.jpeg       # Use case diagram image
│   ├── 📊 uscase diagram.drawio      # Use case diagram source
│   ├── 📄 requirements.pdf           # Project requirements
│   ├── 📊 diagram.png                # Architecture diagram image
│   └── 📊 diagram.mdj                # Architecture diagram source
```
