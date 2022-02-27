using System;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
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
        private List<VoteOption> _voteOptions;

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
                new CreatedVote { Id = 1, VoteType = _voteTypes[0], AnonymousVote = false, UserId = null, VoteTitle = "Title", VoteDiscription="This is the description"},
                new CreatedVote { Id = 2, VoteType = _voteTypes[0], AnonymousVote = true, UserId = null, VoteTitle = null, VoteDiscription=null},
                new CreatedVote { Id = 3, VoteType = _voteTypes[2], AnonymousVote = false, UserId = null, VoteTitle = "Mult Choice Vote", VoteDiscription="Mult choice description", VoteOptions = _voteOptions}
            };
            _voteOptions = new List<VoteOption>()
            {
                new VoteOption {CreatedVote = _createdVotes[2], CreatedVoteId = 3, Id = 1, VoteOptionString = "option 1"},
                new VoteOption {CreatedVote = _createdVotes[2], CreatedVoteId = 3, Id = 2, VoteOptionString = "option 2"},
                new VoteOption {CreatedVote = _createdVotes[2], CreatedVoteId = 3, Id = 3, VoteOptionString = "option 3"}
            };
            _voteTypesSet = GetMockDbSet(_voteTypes.AsQueryable());
            _createdVoteSet = GetMockDbSet(_createdVotes.AsQueryable());
            _voteOptionSet = GetMockDbSet(_voteOptions.AsQueryable());
            _mockContext = new Mock<VotingAppDbContext>();
            _mockContext.Setup(ctx => ctx.VoteTypes).Returns(_voteTypesSet.Object);
            _mockContext.Setup(ctx => ctx.Set<VoteType>()).Returns(_voteTypesSet.Object);
            _mockContext.Setup(ctx => ctx.CreatedVotes).Returns(_createdVoteSet.Object);
            _mockContext.Setup(ctx => ctx.Set<CreatedVote>()).Returns(_createdVoteSet.Object);
            _mockContext.Setup(ctx => ctx.VoteOptions).Returns(_voteOptionSet.Object);
            _mockContext.Setup(ctx => ctx.Set<VoteOption>()).Returns(_voteOptionSet.Object);
        }

        [Test]
        public void Test_VoteOptionRepo_RemoveOptionById_ShouldRemoveOption()
        {
            IVoteOptionRepository repo = new VoteOptionRepository(_mockContext.Object);

            var check = repo.RemoveOptionById(1);
            Assert.IsTrue(check);
        }

        //[Test]
        //[ExpectedException(typeof(ArgumentException))]
        //public void Test_VoteOptionRepo_RemoveOptionById_ShouldReturnNulException()
        //{
        //    IVoteOptionRepository repo = new VoteOptionRepository(_mockContext.Object);
        //    repo.RemoveOptionById(0);
        //}

        [Test]
        public void Test_VoteOptionRepo_RemoveAllOptions_ShouldRemoveAllOptions()
        {
            IVoteOptionRepository repo = new VoteOptionRepository(_mockContext.Object);
            repo.RemoveAllOptions(_voteOptions);
            var check = _voteOptions.Count();
            Assert.IsTrue(check == 0);
        }

        [Test]
        public void Test_CreatedVoteRepo_GetVoteTitle_Should_Return_Title()
        {
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object);
            var result = repo.GetVoteTitle(1);
            Assert.AreEqual(result, "Title");
        }

        [Test]
        public void Test_CreatedVoteRepo_GetVoteTitle_Should_Return_Null()
        {
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object);
            var result = repo.GetVoteTitle(2);
            Assert.AreEqual(result, null);
        }
        [Test]
        public void Test_CreatedVoteRepo_GetVoteTitle_Should_Throw_Exception()
        {
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object);
            var result = repo.GetVoteTitle(10);
            Assert.AreEqual(result, null);
        }

        [Test]
        public void Test_CreatedVoteRepo_GetVoteDescription_Should_Return_Description()
        {
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object);
            var result = repo.GetVoteDescription(1);
            Assert.AreEqual(result, "This is the description");
        }

        [Test]
        public void Test_CreatedVoteRepo_GetVote_Should_Return_vote()
        {
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object);
            var result = repo.GetById(1);
            Assert.AreEqual(result, _createdVotes[0]);
        }

        [Test]
        public void Test_CreatedVoteRepo_GetVoteDescription_Should_Return_Null()
        {
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object);
            var result = repo.GetVoteDescription(2);
            Assert.AreEqual(result, null);
        }
        [Test]
        public void Test_CreatedVoteRepo_GetVoteDescription_Should_Throw_Exception()
        {
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object);
            var result = repo.GetVoteDescription(10);
            Assert.AreEqual(result, null);
        }

        [Test]
        public void Test_CreatedVoteRepo_SetAnonymous_Should_Set_To_True()
        {
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object);
            var result = repo.SetAnonymous(1);
            
            Assert.AreEqual(result, true);
        }

        [Test]
        public void Test_CreatedVoteRepo_SetAnonymous_Should_return_false()
        {
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object);
            var result = repo.SetAnonymous(2);

            Assert.AreEqual(result, false);
        }
        [Test]
        public void Test_CreatedVoteRepo_SetAnonymous_for_invalid_id_Should_return_false()
        {
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object);
            var result = repo.SetAnonymous(10);

            Assert.AreEqual(result, false);
        }
        [Test]
        public void Test_CreatedVoteRepo_AddOrUpdate_throws_exception_if_Null()
        {
            ICreatedVoteRepository repo = new CreatedVoteRepository(_mockContext.Object);
            Assert.Throws<ArgumentNullException>(() => repo.AddOrUpdate(null));
            
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
        public void Test_VoteTypeRepo_GetVoteHeader_should_return_header()
        {
            IVoteTypeRepository repo = new VoteTypeRepository(_mockContext.Object);
            var result = repo.GetChosenVoteHeader(_voteTypes[0].VotingType);
            Assert.IsTrue(result == "You have chosen to create a yes/no vote");

        }

        [Test]
        public void Test_VoteTypeRepo_GetVoteHeader_should_return_null()
        {
            IVoteTypeRepository repo = new VoteTypeRepository(_mockContext.Object);
            var result = repo.GetChosenVoteHeader("test");
            Assert.IsTrue(result == null);

        }
    }
}