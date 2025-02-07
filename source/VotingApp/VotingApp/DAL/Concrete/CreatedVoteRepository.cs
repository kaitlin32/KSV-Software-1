﻿using EmailService;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using VotingApp.DAL.Abstract;
using VotingApp.DAL.Concrete;
using VotingApp.Data;
using VotingApp.Models;

namespace VotingApp.DAL.Concrete
{
    public class CreatedVoteRepository : ICreatedVoteRepository
    {
        private VotingAppDbContext _context;
        private IEmailSender _emailSender;

        public CreatedVoteRepository(VotingAppDbContext ctx, IEmailSender emailSender)
        {
            _context = ctx;
            _emailSender = emailSender;
        }
        public CreatedVote AddOrUpdate(CreatedVote createdVote)
        {
            if (createdVote == null)
            {
                throw new ArgumentNullException("Entity must not be null to add or update");
            }
            _context.Update(createdVote);
            _context.SaveChanges();
            return createdVote;
        }

        public Boolean SetAnonymous(int id)
        {
            var createdVote = _context.CreatedVotes.Where(a => a.Id == id).FirstOrDefault();
            
            if (createdVote != null && createdVote.AnonymousVote == false)
            {
                createdVote.AnonymousVote = true;
                AddOrUpdate(createdVote);
                return true;
            }

            return false;
        }

        public CreatedVote GetVoteByAccessCode(string code)
        {
            var vote = _context.CreatedVotes.Where(a => a.VoteAccessCode == code).Include(a => a.VoteType).Include(a => a.VoteOptions).FirstOrDefault();
            return vote;
        }

        public CreatedVote GetById(int id)
        {
            return _context.CreatedVotes.Where(a => a.Id == id).Include(a => a.VoteOptions).Include(a => a.VoteAuthorizedUsers).FirstOrDefault();
        }

        public string GetVoteTitle(int id)
        {
            var voteTitle = _context.CreatedVotes.Where(a => a.Id == id).Select(ab => ab.VoteTitle).FirstOrDefault();
            if (voteTitle == null)
            {
                return null;
            }
            return voteTitle;
        }
        public string GetVoteDescription(int id)
        {
            var voteDescription = _context.CreatedVotes.Where(a => a.Id == id).Select(ab => ab.VoteDiscription).FirstOrDefault();
            if (voteDescription == null)
            {
                return null;
            }
            return voteDescription;
        }

        public IList<CreatedVote> GetAll()
        {
            return _context.CreatedVotes.Include(a => a.VoteType).ToList();
        }

        public IList<CreatedVote> GetAllForUserId(int userId)
        {
            return _context.CreatedVotes.Include(a => a.VoteType).Where(v => v.UserId == userId).ToList();
        }

        public IList<CreatedVote> GetAllVotesWithNoAccessCode()
        {
            return _context.CreatedVotes.Where(v => v.VoteAccessCode == null).ToList();
        }

        public void SendEmails(IList<VoteAuthorizedUser> users, CreatedVote vote, string accessCode)
        {
            var voteTitle = vote.VoteTitle;
            var voteDescription = vote.VoteDiscription;

            foreach (var user in users)
            {
                var email = user.UserName;

                var message = new Message(new string[] { email }, "New Vote Authorization from Opiniony",
                    "You have been authorized to submit your vote for: " + $"<br/><br/>Title: '{voteTitle}'<br/>Description: '{voteDescription}'<br/><br/>Click <a href='{accessCode}'>here</a> to go to submit a vote.");

                _emailSender.SendEmail(message);
            }
        }

        public IList<CreatedVote> GetAllClosedMultiRoundVotes()
        {
            return _context.CreatedVotes.Where(v => v.NextRoundId == 0 && v.VoteTypeId == 3 && v.VoteCloseDateTime != null && DateTime.Compare(DateTime.Now, v.VoteCloseDateTime.Value) > 0).ToList();
        }
    }
}

