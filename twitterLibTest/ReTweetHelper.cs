using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Tweetinvi;
using Tweetinvi.Models;

namespace ReTweetConfigurableBot
{
    class ReTweetHelper
    {
        private bool IsRetweet(JObject twt)
        {
            if (twt["retweeted_status"] != null)
                return true;
            else
            {
                string rtExists = (string)twt["text"];
                if (rtExists.StartsWith("RT"))
                    return true;
                else
                    return false;
            }
        }

        public bool PublishReTweet (JObject twt , int benchmarkFollowersCount, int benchmarkTweetsCount, bool allowReTweetsPublish = false)
        {
            bool publishStatus = false;
            if (allowReTweetsPublish || !IsRetweet(twt))
            {
                if (IsValuableTweet(twt, benchmarkFollowersCount, benchmarkTweetsCount))
                {
                    var rootTweet = Tweet.GetTweet((long)twt["id_str"]);
                    var reTweet = Tweet.PublishRetweet(rootTweet);
                    publishStatus = true;
                }
            }
            return publishStatus;
        }

        public bool IsValuableTweet (JObject twt , int benchmarkFollowersCount , int benchmarkTweetsCount)
        {
           
                var userDetails = twt["user"];
                var followersCount =(long) userDetails["followers_count"];
            var tweetsCount = (long)userDetails["statuses_count"];
               // var userName = userDetails["name"];
                if (followersCount > benchmarkFollowersCount && tweetsCount < benchmarkTweetsCount)
                    return true;
                else
                    return false;
              
        }

    }
}
