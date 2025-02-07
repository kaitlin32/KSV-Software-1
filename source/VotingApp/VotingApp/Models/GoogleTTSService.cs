﻿using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;
using VotingApp.Attributes;
using VotingApp.DAL.Abstract;

namespace VotingApp.Models
{
    public class GoogleTtsService
    {
        private readonly ICreatedVoteRepository _createdVoteRepository;

        public GoogleTtsService(ICreatedVoteRepository createdVoteRepository)
        {
            _createdVoteRepository = createdVoteRepository;
        }
        public byte[] CreateVoteAudio(CreatedVote vote)
        {
            
            string voteAsString = "";
            voteAsString += $"Vote Title, {vote.VoteTitle}. Vote Description, {vote.VoteDiscription}. Vote Options, ";
            foreach (var option in vote.VoteOptions)
            {
                voteAsString += $"{option.VoteOptionString},";
            }

            voteAsString += ".";
            string value = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            var clientAsync = TextToSpeechClient.CreateAsync();
            var input = new SynthesisInput
            {
                Text = voteAsString
            };
            var voiceSelection = new VoiceSelectionParams
            {
                LanguageCode = "en-US",
                SsmlGender = SsmlVoiceGender.Male
            };
            var audioConfig = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3
            };
            clientAsync.Wait();
            var client = clientAsync.Result;
            
            var response = client.SynthesizeSpeech(input, voiceSelection, audioConfig);
            var bytes = response.AudioContent.ToByteArray(); // change to byte string
            //SynthesizeSpeechResponse audio = SynthesizeSpeechResponse.Parser.;
            return bytes;
            //_createdVoteRepository.AddOrUpdate(vote);
            //var audio = SynthesizeSpeechResponse.Parser.ParseFrom(bytes); //change the bytes to audio

        }

        public void CreateAudioFiles(CreatedVote vote)
        {
            Directory.SetCurrentDirectory(@"wwwroot/TempAudio");
            if (Directory.Exists($"{vote.Id}"))
            {
                Directory.Delete($"{vote.Id}");
            }

            Directory.CreateDirectory($"{vote.Id}");
            Directory.SetCurrentDirectory(@$"{vote.Id}");
            if (vote.VoteAudioBytes != null)
            {
                File.WriteAllBytes($"{vote.Id}.mp3", vote.VoteAudioBytes);
            }
        }
    }
}
