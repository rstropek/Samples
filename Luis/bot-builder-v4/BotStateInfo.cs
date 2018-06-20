using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConferenceConciergeBot
{
    public class BotStateInfo
    {
        public bool FeedbackTopicStarted { get; set; }
        public bool FoodAndDrinksTopicStarted { get; set; }
        public bool CancelTopicStarted { get; set; }
    }
}
