create table Users (			--- Users table not User -->
	userID int primary key identity(1, 1),
	username varchar(30) unique
)

CREATE TABLE Hashtags (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Tag NVARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE Categories (
    Id INT PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

INSERT INTO Categories (Id, Name) VALUES
(1, 'General-Discussion'),
(2, 'Lesson-Help'),
(3, 'Off-topic'),
(4, 'Discover'),
(5, 'Announcements');

CREATE TABLE Posts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(50) NOT NULL,
    Description NVARCHAR(4000) NOT NULL,		--- We need to modify in the diagram(can t put 5000) -->
    UserID INT NOT NULL,
    CategoryID INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    LikeCount INT DEFAULT 0,

    CONSTRAINT fk_user FOREIGN KEY (userID) REFERENCES Users(userID) ON DELETE CASCADE,
    CONSTRAINT fk_category FOREIGN KEY (CategoryID) REFERENCES Categories(Id) ON DELETE CASCADE
);

--- Need to add a many to many table -->
CREATE TABLE PostHashtags (
    PostID INT NOT NULL,
    HashtagID INT NOT NULL,

    PRIMARY KEY (PostID, HashtagID),

    CONSTRAINT fk_post FOREIGN KEY (PostID) REFERENCES Posts(Id) ON DELETE CASCADE,
    CONSTRAINT fk_hashtag FOREIGN KEY (HashtagID) REFERENCES Hashtags(Id) ON DELETE CASCADE
);

--- Look for delete and update -->
CREATE TABLE Comments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Content NVARCHAR(1000) NOT NULL,
    UserID INT NOT NULL,
    PostID INT NOT NULL,
    ParentCommentID INT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    LikeCount INT DEFAULT 0,
    Level INT CHECK (Level BETWEEN 1 AND 3),

    CONSTRAINT fk_userid FOREIGN KEY (UserID) REFERENCES Users(userID) on delete no action,
    CONSTRAINT fk_posts FOREIGN KEY (PostID) REFERENCES Posts(Id) on delete no action,
    CONSTRAINT fk_parent_comment FOREIGN KEY (ParentCommentID) REFERENCES Comments(Id) on delete no action
);

--- Insert section -->

insert into Users values ('Andrei') , ('Ion'), ('Maria'), ('Alex'), ('Elena'), ('Gabriel'), ('Sofia'), ('Matei'), ('Ana'), ('David'), ('Ioana'), ('Luca'), ('Bianca');

insert into Hashtags values ('a'), ('Mac'), ('Tech'), ('Innovation'), ('AI'), ('MachineLearning'), ('Coding'), ('Cybersecurity'), ('CloudComputing'), ('WebDevelopment'), ('MobileTech'), ('FutureOfTech');

INSERT INTO Posts (Title, Description, UserID, CategoryID) VALUES
('New Gadget', 'A review of the latest tech gadget.', 1, 1),
('Paris Trip', 'My amazing trip to Paris and the Eiffel Tower.', 2, 2),
('Delicious Pizza', 'Tried a new pizza place, it was fantastic!', 3, 3),
('Coding Tips', 'Some useful tips for beginner programmers.', 1, 1),
('Mountain Hike', 'Hiking in the beautiful mountains.', 2, 2),
('Homemade Pasta', 'Making fresh pasta at home.', 3, 3),
('AI Advancements', 'The latest developments in artificial intelligence.', 1, 1),
('Tokyo Adventures', 'Exploring the streets of Tokyo.', 2, 2),
('Chocolate Cake','A delicious chocolate cake recipe.', 3, 3),
('Cloud Security', 'Understanding the importance of cloud security.', 4, 1),
('London Calling', 'Exploring the iconic landmarks of London.', 5, 2),
('Burger Mania', 'Trying out the best burger joint in town.', 6, 3),
('Python Tricks', 'Advanced techniques in Python programming.', 7, 1),
('Italian Dolomites', 'A breathtaking hike in the Italian Dolomites.', 8, 2),
('Vegan Feast', 'Preparing a delicious and healthy vegan meal.', 9, 3),
('Quantum Computing', 'An overview of the basics of quantum computing.', 10, 1),
('New York City Guide', 'My recommendations for visiting New York City.', 3, 2),
('Sushi Night', 'Enjoying a delightful sushi dinner.', 2, 3),
('VR Development', 'Getting started with Virtual Reality development.', 4, 1);

INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Great post!', 1, 1, NULL, 1);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('I agree!', 2, 1, 1, 2);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Totally!', 3, 1, 2, 3);

INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Thanks for sharing!', 3, 2, NULL, 1);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('My pleasure!', 4, 2, 4, 2);

INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('This is helpful.', 1, 3, NULL, 1);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Glad you found it so!', 5, 3, 7, 2);

INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Interesting perspective.', 4, 4, NULL, 1);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Care to elaborate?', 6, 4, 9, 2);

INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Where did you take this hike?', 5, 5, NULL, 1);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('In the mountains nearby!', 7, 5, 11, 2);

INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Looks delicious!', 6, 6, NULL, 1);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('It was indeed!', 8, 6, 13, 2);

INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Can you elaborate on that?', 7, 7, NULL, 1);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Sure thing...', 9, 7, 15, 2);

INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Beautiful scenery!', 8, 8, NULL, 1);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Absolutely stunning!', 10, 8, 17, 2);

INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('I should try this recipe.', 9, 9, NULL, 1);

INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('What are your thoughts on this?', 10, 10, NULL, 1);

-- Additional 10 comments for posts 1 to 10 --
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Love this content!', 3, 1, NULL, 1);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Very insightful.', 4, 2, NULL, 1);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Well explained.', 5, 3, NULL, 1);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Could you clarify?', 6, 4, NULL, 1);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Great hike!', 7, 5, NULL, 1);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('I have the same recipe!', 8, 6, NULL, 1);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Very thought-provoking.', 9, 7, NULL, 1);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Planning to visit soon!', 10, 8, NULL, 1);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Great discussion.', 1, 9, NULL, 1);
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Really useful information!', 2, 10, NULL, 1);

INSERT INTO PostHashtags (PostID, HashtagID) VALUES
(1, 3),  -- New Gadget -> Tech
(1, 4),  -- New Gadget -> Innovation
(1, 11), -- New Gadget -> MobileTech

(2, 1),  -- Paris Trip -> a
(2, 12), -- Paris Trip -> FutureOfTech

(3, 1),  -- Delicious Pizza -> a

(4, 7),  -- Coding Tips -> Coding
(4, 3),  -- Coding Tips -> Tech

(5, 1),  -- Mountain Hike -> a

(6, 1),  -- Homemade Pasta -> a

(7, 5),  -- AI Advancements -> AI
(7, 6),  -- AI Advancements -> MachineLearning
(7, 4),  -- AI Advancements -> Innovation

(8, 1),  -- Tokyo Adventures -> a

(9, 1),  -- Chocolate Cake -> a

(10, 8), -- Cloud Security -> Cybersecurity
(10, 9), -- Cloud Security -> CloudComputing

(11, 1), -- London Calling

(12, 1), -- Burger Mania

(13, 7), -- Python Tricks -> Coding
(13, 3), -- Python Tricks -> Tech

(14, 1), -- Italian Dolomites -> a

(15, 1), -- Vegan Feast -> a

(16, 4), -- Quantum Computing -> Innovation
(16, 6), -- Quantum Computing -> MachineLearning
(16, 10), -- Quantum Computing -> WebDevelopment

(17, 1), -- New York City Guide

(18, 1), -- Sushi Night 

(19, 4), -- VR Development -> Innovation
(19, 11); -- VR Development -> MobileTech