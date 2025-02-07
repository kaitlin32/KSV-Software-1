using System;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using EmailService;
using VotingApp.DAL.Abstract;
using VotingApp.DAL.Concrete;
using VotingApp.Models;

namespace Tests_NUnit_Voting_App
{
    public class RepoTests
    {
        private Mock<VotingAppDbContext> _mockContext;
        private Mock<DbSet<CreatedVote>> _createdVoteSet;
        private Mock<DbSet<VoteType>> _voteTypesSet;
        private Mock<DbSet<VoteOption>> _voteOptionSet;
        private List<CreatedVote> _createdVotes;
        private List<VoteType> _voteTypes;
        private List<VoteOption> _voteOption;
        private Mock<DbSet<VotingUser>> _votingUsersSet;
        private List<VotingUser> _votingUsers;
        

        private Mock<DbSet<T>> GetMockDbSet<T>(IQueryable<T> entities) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(entities.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(entities.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(entities.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(entities.GetEnumerator());
            return mockSet;
        }
        [SetUp]
        public void Setup()
        {
            _voteTypes = new List<VoteType>()
            {
                new VoteType { Id = 1,VotingType ="Yes/No Vote" ,VoteTypeDescription = "yes/no discription" },
                new VoteType { Id = 2,VotingType =null ,VoteTypeDescription = "null discription" },
                new VoteType { Id = 3,VotingType ="Multiple Choice Vote" ,VoteTypeDescription = "multiple choice description"}
            };
            _createdVotes = new List<CreatedVote>()
            {
                new CreatedVote { Id = 1, VoteType = _voteTypes[0], AnonymousVote = false, UserId = 1, VoteTitle = "Title", VoteDiscription="This is the description", VoteAccessCode = "abc123"},
                new CreatedVote { Id = 2, VoteType = _voteTypes[0], AnonymousVote = true, UserId = 1, VoteTitle = null, VoteDiscription=null},
                new CreatedVote { Id = 3, VoteType = _voteTypes[2], AnonymousVote = false, UserId = 1, VoteTitle = "Mult Choice Vote", VoteDiscription="Mult choice description", VoteOptions = _voteOption}
            };
            _voteOption = new List<VoteOption>()
            {
                new VoteOption {CreatedVote = _createdVotes[2], CreatedVoteId = 3, Id = 1, VoteOptionString = "option 1"},
                new VoteOption {CreatedVote = _createdVotes[2], CreatedVoteId = 3, Id = 2, VoteOptionString = "option 2"},
                new VoteOption {CreatedVote = _createdVotes[2], CreatedVoteId = 3, Id = 3, VoteOptionString = "option 3"}
            };
            
            _votingUsers = new List<VotingUser>()
            {
                new VotingUser { Id = 1, NetUserId = "ABC123CBA31", UserName = "name@mail.com" },
                new VotingUser { Id = 2, NetUserId = "123456", UserName = "name2@mail.com" }
            };
           
            _voteTypesSet = GetMockDbSet(_voteTypes.AsQueryable());
            _createdVoteSet = GetMockDbSet(_createdVotes.AsQueryable());
            _voteOptionSet = GetMockDbSet(_voteOption.AsQueryable());
            _votingUsersSet = GetMockDbSet(_votingUsers.AsQueryable());

            _mockContext = new Mock<VotingAppDbContext>();
            _mockContext.Setup(ctx => ctx.VoteTypes).Returns(_voteTypesSet.Object);
            _mockContext.Setup(ctx => ctx.Set<VoteType>()).Returns(_voteTypesSet.Object);
            _mockContext.Setup(ctx => ctx.CreatedVotes).Returns(_createdVoteSet.Object);
            _mockContext.Setup(ctx => ctx.Set<CreatedVote>()).Returns(_createdVoteSet.Object);
            _mockContext.Setup(ctx => ctx.VoteOptions).Returns(_voteOptionSet.Object);
            _mockContext.Setup(ctx => ctx.Set<VoteOption>()).Returns(_voteOptionSet.Object);
            _mockContext.Setup(ctx => ctx.VotingUsers).Returns(_votingUsersSet.Object);
            _mockContext.Setup(ctx => ctx.Set<VotingUser>()).Returns(_votingUsersSet.Object);
        }

        [Test]
        public void Test_VoteOptionRepo_RemoveOptionById_ShouldRemoveOption()
        {
            IVoteOptionRepository repo = new VoteOptionRepository(_mockContext.Object);

            var check = repo.RemoveOptionById(1);
            Assert.IsTrue(check);
        }

        [Test]
        public void Test_VoteOptionRepo_RemoveAllOptions_ShouldRemoveAllOptions()
        {
            IVoteOptionRepository repo = new VoteOptionRepository(_mockContext.Object);
            var check = repo.RemoveAllOptions(_voteOption);
            Assert.IsTrue(check);
        }

        [Test]
        public void Test_VoteOptionRepo_RemoveAllOptions_ShouldThrowNullException()
        {
            IVoteOptionRepository repo = new VoteOptionRepository(_mockContext.Object);
            List<VoteOption> voteOptions = null;
            Assert.Throws<ArgumentNullException>(() => repo.RemoveAllOptions(voteOptions)); 

        }

        [Test]
        public void Test_VoteOptionsRepo_GetByID_should_return_option()
        {
            IVoteOptionRepository repo = new VoteOptionRepository(_mockContext.Object);
            var results = repo.GetById(1);
            Assert.AreEqual(results, _voteOption[0]);
        }

        [Test]
        public void Test_VoteOptionsRepo_GetOptionsByVoteID_should_return_all_options()
        {
            IVoteOptionRepository repo = new VoteOptionRepository(_mockContext.Object);
            var results = repo.GetAllByVoteID(_createdVotes[2].Id);
            Assert.AreEqual(results, _voteOption);
        }

        [Test]
        public void Test_VotingUserRepo_removeUserById_should_remove_user()
        {
            IVotingUserRepositiory repo = new VotingUserRepository(_mockContext.Object);
            repo.RemoveUser(_votingUsers[0]);
            
            Assert.True(true);
        }
        [Test]
        public void Test_VotingUserRepo_removeUserById_should_raise_exception()
        {
            IVotingUserRepositiory repo = new VotingUserRepository(_mockContext.Object);
           
            Assert.Throws<ArgumentNullException>(() => repo.RemoveUser(null));
        }

        [Test]
        public void Test_VotingUserRepo_AddOrUpdate_should_return_user()
        {
            IVotingUserRepositiory repo = new VotingUserRepository(_mockContext.Object);
            VotingUser newuser = new VotingUser{ NetUserId="asdf",UserName="newuser"};
            var result = repo.AddOrUpdate(newuser);
            Assert.AreEqual(newuser, result);
        }

        [Test]
        public void Test_VotingUserRepo_AddOrUpdate_should_raise_exception()
        {
            IVotingUserRepositiory repo = new VotingUserRepository(_mockContext.Object);
            Assert.Throws<ArgumentNullException>(() => repo.AddOrUpdate(null));
        }
        [Test]
        public void Test_VotingUserRepo_GetUserByAspid_should_return_user()
        {
            IVotingUserRepositiory repo = new VotingUserRepository(_mockContext.Object);
            var result = repo.GetUserByAspId("ABC123CBA31");
            Assert.AreEqual(_votingUsers[0], result);
        }
        [Test]
        public void Test_CreatedVoteRepo_GetAll_should_return_all_createdVotes()
        {
            EmailConfiguration emailConfig = new EmailConfiguration();
            IEmailSender emailSender = new EmailSender(emailConfig);
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object, emailSender);
            var result = repo.GetAll();
            Assert.AreEqual(_createdVotes, result);
        }

        [Test]
        public void Test_CreatedVoteRepo_GetAllForUserId_should_return_all_createdVotes_for_user()
        {
            EmailConfiguration emailConfig = new EmailConfiguration();
            IEmailSender emailSender = new EmailSender(emailConfig);
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object, emailSender);
            var result = repo.GetAllForUserId(1);
            Assert.AreEqual(_createdVotes, result);
        }

        [Test]
        public void Test_CreatedVoteRepo_GetByAccessCodeShouldReturnCreatedVote()
        {
            EmailConfiguration emailConfig = new EmailConfiguration();
            IEmailSender emailSender = new EmailSender(emailConfig);
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object, emailSender);
            var result = repo.GetVoteByAccessCode("abc123");
            Assert.AreEqual(_createdVotes[0], result);
        }

        [Test]
        public void Test_CreatedVoteRepo_GetVoteTitle_Should_Return_Title()
        {
            EmailConfiguration emailConfig = new EmailConfiguration();
            IEmailSender emailSender = new EmailSender(emailConfig);
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object, emailSender);
            var result = repo.GetVoteTitle(1);
            Assert.AreEqual(result, "Title");
        }

        [Test]
        public void Test_CreatedVoteRepo_GetVoteTitle_Should_Return_Null()
        {
            EmailConfiguration emailConfig = new EmailConfiguration();
            IEmailSender emailSender = new EmailSender(emailConfig);
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object, emailSender);
            var result = repo.GetVoteTitle(2);
            Assert.AreEqual(result, null);
        }
        [Test]
        public void Test_CreatedVoteRepo_GetVoteTitle_Should_Throw_Exception()
        {
            EmailConfiguration emailConfig = new EmailConfiguration();
            IEmailSender emailSender = new EmailSender(emailConfig);
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object, emailSender);
            var result = repo.GetVoteTitle(10);
            Assert.AreEqual(result, null);
        }

        [Test]
        public void Test_CreatedVoteRepo_GetVoteDescription_Should_Return_Description()
        {
            EmailConfiguration emailConfig = new EmailConfiguration();
            IEmailSender emailSender = new EmailSender(emailConfig);
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object, emailSender);
            var result = repo.GetVoteDescription(1);
            Assert.AreEqual(result, "This is the description");
        }

        [Test]
        public void Test_CreatedVoteRepo_GetVote_Should_Return_vote()
        {
            EmailConfiguration emailConfig = new EmailConfiguration();
            IEmailSender emailSender = new EmailSender(emailConfig);
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object, emailSender);
            var result = repo.GetById(1);
            Assert.AreEqual(result, _createdVotes[0]);
        }

        [Test]
        public void Test_CreatedVoteRepo_GetVoteDescription_Should_Return_Null()
        {
            EmailConfiguration emailConfig = new EmailConfiguration();
            IEmailSender emailSender = new EmailSender(emailConfig);
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object, emailSender);
            var result = repo.GetVoteDescription(2);
            Assert.AreEqual(result, null);
        }
        [Test]
        public void Test_CreatedVoteRepo_GetVoteDescription_Should_Throw_Exception()
        {
            EmailConfiguration emailConfig = new EmailConfiguration();
            IEmailSender emailSender = new EmailSender(emailConfig);
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object, emailSender);
            var result = repo.GetVoteDescription(10);
            Assert.AreEqual(result, null);
        }

        [Test]
        public void Test_CreatedVoteRepo_SetAnonymous_Should_Set_To_True()
        {
            EmailConfiguration emailConfig = new EmailConfiguration();
            IEmailSender emailSender = new EmailSender(emailConfig);
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object, emailSender);
            var result = repo.SetAnonymous(1);
            
            Assert.AreEqual(result, true);
        }

        [Test]
        public void Test_CreatedVoteRepo_SetAnonymous_Should_return_false()
        {
            EmailConfiguration emailConfig = new EmailConfiguration();
            IEmailSender emailSender = new EmailSender(emailConfig);
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object, emailSender);
            var result = repo.SetAnonymous(2);

            Assert.AreEqual(result, false);
        }
        [Test]
        public void Test_CreatedVoteRepo_SetAnonymous_for_invalid_id_Should_return_false()
        {
            EmailConfiguration emailConfig = new EmailConfiguration();
            IEmailSender emailSender = new EmailSender(emailConfig);
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object, emailSender);
            var result = repo.SetAnonymous(10);

            Assert.AreEqual(result, false);
        }
        [Test]
        public void Test_CreatedVoteRepo_AddOrUpdate_throws_exception_if_Null()
        {
            EmailConfiguration emailConfig = new EmailConfiguration();
            IEmailSender emailSender = new EmailSender(emailConfig);
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object, emailSender);
            Assert.Throws<ArgumentNullException>(() => repo.AddOrUpdate(null));
            
        }

        [Test]
        public void Test_VoteTypeRepo_CheckForChangeFromYesNoVoteType_ShouldReturnCorrectVoteTypeId()
        {
            IVoteTypeRepository repo = new VoteTypeRepository(_mockContext.Object);
            var result = repo.CheckForChangeFromYesNoVoteType(1);
            Assert.IsTrue(result == 0);
        }

        [Test]
        public void Test_VoteTypeRepo_VoteTypes_Should_return_list_of_types()
        {
            IVoteTypeRepository repo = new VoteTypeRepository(_mockContext.Object);
            var result = repo.VoteTypes();
            Assert.AreEqual(result, _voteTypes.ToList());

        }
        [Test]
        public void Test_VoteTypeRepo_GetVoteType_Should_return_votetype()
        {
            IVoteTypeRepository repo = new VoteTypeRepository(_mockContext.Object);
            var result = repo.GetVoteType(1);
            Assert.AreEqual(result, _voteTypes[0].VotingType);

        }
        [Test]
        public void Test_VoteTypeRepo_GetVoteType_Should_return_null()
        {
            IVoteTypeRepository repo = new VoteTypeRepository(_mockContext.Object);
            var result = repo.GetVoteType(2);
            Assert.AreEqual(result, _voteTypes[1].VotingType);

        }
        [Test]
        public void Test_VoteTypeRepo_GetVoteType_invalid_id_Should_return_null()
        {
            IVoteTypeRepository repo = new VoteTypeRepository(_mockContext.Object);
            var result = repo.GetVoteType(10);
            Assert.AreEqual(result, null);

        }
        [Test]
        public void Test_VoteTypeRepo_GetVoteOptions_should_return_options()
        {
            IVoteTypeRepository repo = new VoteTypeRepository(_mockContext.Object);
            var result = repo.GetVoteOptions(_voteTypes[0].VotingType);
            Assert.IsTrue(result[0] == "Yes" && result[1] == "No");

        }

        [Test]
        public void Test_VoteTypeRepo_GetVoteHeader_should_return_yes_no_header()
        {
            IVoteTypeRepository repo = new VoteTypeRepository(_mockContext.Object);
            var result = repo.GetChosenVoteHeader(_voteTypes[0].VotingType);
            Assert.IsTrue(result == "You have chosen to create a yes/no vote");
        }


        [Test]
        public void Test_VoteTypeRepo_GetVoteHeader_should_return_mult_choice_header()
        {
            IVoteTypeRepository repo = new VoteTypeRepository(_mockContext.Object);
            var result = repo.GetChosenVoteHeader(_voteTypes[2].VotingType);
            Assert.IsTrue(result == "You have chosen to create a multiple choice vote");
        }

        [Test]
        public void Test_VoteTypeRepo_GetVoteHeader_should_return_null()
        {
            IVoteTypeRepository repo = new VoteTypeRepository(_mockContext.Object);
            var result = repo.GetChosenVoteHeader("test");
            Assert.IsTrue(result == null);

        }

        [Test]
        public void Test_VoteTypeRepo_CreateYesNoVoteOptions_should_return_list_of_options()
        {
            IVoteTypeRepository repo = new VoteTypeRepository(_mockContext.Object);
            var result = repo.CreateYesNoVoteOptions();
            List<VoteOption> expected = new List<VoteOption>();
            expected.Add(new VoteOption { VoteOptionString = "Yes" });
            expected.Add(new VoteOption { VoteOptionString = "No" });
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].VoteOptionString, result[i].VoteOptionString);
            }

        }
    }
}