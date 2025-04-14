using System;
using System.Collections.Generic;
using Duo.Models;
using Duo.Repositories;
using Duo.Services;
using Moq;
using Xunit;

namespace TestsDuo2.Services
{
    public class FriendsServiceTests
    {
        private readonly Mock<ListFriendsRepository> _mockFriendRepository;
        private readonly FriendsService _friendsService;
        
        public FriendsServiceTests()
        {
            _mockFriendRepository = new Mock<ListFriendsRepository>();
            _friendsService = new FriendsService(_mockFriendRepository.Object);
        }
        
       
        
      
       
       
        
       
    }
} 