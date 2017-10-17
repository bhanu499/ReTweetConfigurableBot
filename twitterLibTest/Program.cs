using System;
using Tweetinvi;
using Tweetinvi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Tweetinvi.Parameters;
using Microsoft.Extensions.Configuration;

namespace ReTweetConfigurableBot
{
    class Program
    {
        static void Main(string[] args)
        {
            //Using Configuration 
            var builder = new ConfigurationBuilder()
                             .SetBasePath(Directory.GetCurrentDirectory())
                             .AddJsonFile("appsettings.json");

            var config = builder.Build();



            // Set up your credentials (https://apps.twitter.com)
            Auth.SetUserCredentials(config["consumerKey"],config["consumerSecret"], config["userAccessToken"], config["userAccessSecret"]);


            // Publish the Tweet "Hello World" on your Timeline
            // Tweet.PublishTweet("Hello World!");
            // var matchingTweets = Search.SearchTweets("#datascience");
            // var temp = Tweet.GetTweet(920144293406261248);//920148632522903553);
            // var rtwt = Tweet.PublishRetweet(temp);
            // var textToPublish = string.Format("@{0} {1}", temp.CreatedBy.ScreenName, "Very Helpful");
            // Tweet.PublishTweet(textToPublish, new PublishTweetOptionalParameters { InReplyToTweet = temp});
            int count = 0;
            var stream = Tweetinvi.Stream.CreateFilteredStream();
            stream.AddTweetLanguageFilter(LanguageFilter.English);

            var tracks = config["tracks"].Split(';');

            foreach (var track in tracks)
            {
                stream.AddTrack(track);
            }
            stream.MatchingTweetReceived += (sender, TweetReceivedEventArgs) =>
            {
                // Do what you want with the Tweet.
                // Console.WriteLine(TweetReceivedEventArgs.Json);               
                //Console.WriteLine((string)jo["text"] + (string)jo["id_str"]);
                //string FinalTweet = (string)jo["id_str"] + "~~" + (string)jo["text"] + Environment.NewLine;
                //File.AppendAllText("DataScienceTweets.csv", FinalTweet);
                JObject jo = JObject.Parse(TweetReceivedEventArgs.Json);
                ReTweetHelper helper = new ReTweetHelper();
                int benchMarkFollowersCount = Convert.ToInt32(config["benchMarkFollowersCount"]);

                if (helper.PublishReTweet(jo, benchMarkFollowersCount))
                {
                    count++;
                    Console.WriteLine("Tweet Republished " + count);
                }


            };
            stream.StartStreamMatchingAllConditions();


            Console.ReadKey();

        }

      
    }
}