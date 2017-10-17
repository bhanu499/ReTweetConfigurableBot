using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterAPITest
{
    interface ITweet
    {
        void GetTweetsAsync(string searchText);
    }
}
