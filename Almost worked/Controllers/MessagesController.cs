using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.Collections.Generic;

namespace Almost_worked
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                // atm don't know where to instantiate the accounts :/
                List<Account> accounts = new List<Account>();
                accounts.Add(new Account("0", "Transaction", 1.0, 0.0));
                accounts.Add(new Account("1", "Savings", 3.0, 1.0));

                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                StateClient stateClient = activity.GetStateClient(); //for state services

                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                String userText = activity.Text;

                /* testing cards - this is separate return from the rest of the messages */
                if (userText.ToLower().Contains("info"))
                {
                    Activity myReply = activity.CreateReply();
                    myReply.Recipient = activity.From;
                    myReply.Type = "message";
                    myReply.Attachments = new List<Attachment>();

                    /*  we are just doing a series of messages on the card, no images or buttons required
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "URLHERE.png"));

                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction plButton = new CardAction()
                    {
                        Value = "http://msa.ms",
                        Type = "openUrl",
                        Title = "MSA Website"
                    };
                    cardButtons.Add(plButton);
                    */

                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "http://fixer.io/img/money.png"));

                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction openBankBut = new CardAction()
                    {
                        Value = "http://msa.ms/ContosoBank/",
                        Type = "openUrl",
                        Title = "Go to our website!"
                    };
                    cardButtons.Add(openBankBut);

                    ThumbnailCard infoCard = new ThumbnailCard()
                    {
                        Title = "GET YOU SOME INFO",
                        Text = "Click the button below to go to our website to get said info.",
                        Images = cardImages,
                        Buttons = cardButtons
                    };

                    Attachment helpAttachment = infoCard.ToAttachment();
                    myReply.Attachments.Add(helpAttachment);
                    await connector.Conversations.SendToConversationAsync(myReply);

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                //the above if returns

                string replyStr = "";
                /* the follow is a large if ... elseif ... block, where each segment changes replyStr, which is used 
                 * at the end to actually create the reply.
                 */
                if (userText.ToLower().Contains("help"))
                {
                    replyStr += "HAVE HELPS\n\n";
                    replyStr += "Use \"my name is YOUR NAME\" to set your name (does not support spaces).\n\n" +
                        "Use \"actually it's YOUR NAME\" to update your name.\n\n\n" +
                        "Use \"whoami\" to check what your name is.\n\n" +
                        "Use \"list accounts\" to see all our available accounts.\n\n\n" +
                        "Use \"set currency ###\" where ### is the 3 letter currency code (eg. NZD) to set your currency (for use with exchange rates).\n\n" +
                        "Use \"exchange rates\" to get the conversion between your currency and other common currencies.\n\n" +
                        "Use \"full exchange rates\" to get the conversion between your currency and ALL other currencies.\n\n" +
                        "Use \"clear all\" to clear all data associated to you.\n\n" +
                        "Use \"get timelines\" to see the past exchange rates requests.\n\n" +
                        "Use \"delete timeline UID\" to delete the exchange rate request with unique id UID (eg 5cd5b064-0b0f-432d-9c23-0aff93c7cfe0).\n\n" +
                        "Finally, use \"help\" to view this help.";
                }
                else if (userText.ToLower().Contains("my name is"))
                {
                    int index = userText.IndexOf("my name is");
                    //first remove everyhing before "my name is", then remove "my name is", the remove spaces.
                    String nameStr = userText.Substring(index).Replace("my name is", "").Replace(" ", "");
                    if (!String.IsNullOrEmpty(userData.GetProperty<String>("Name")))
                    {
                        // the string exists
                        if (userData.GetProperty<String>("Name").Equals(nameStr))
                        {
                            replyStr += "Yes, ";
                        }
                        else
                        {
                            replyStr += "No, ";
                        }
                        replyStr += $"your name is {userData.GetProperty<String>("Name")}.\n\n";
                    }
                    else //the string doesn't exist
                    {
                        if (String.IsNullOrEmpty(nameStr))
                        {
                            replyStr += $"You can't have an empty name silly. Try again. \n\n";
                        }
                        else
                        {
                            userData.SetProperty<String>("Name", nameStr);
                            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                            replyStr += $"Congratulations! Your name is now {nameStr}.\n\n";
                        }
                    }
                }
                else if (userText.ToLower().Contains("actually it's") || userText.ToLower().Contains("actually it is"))
                {
                    String nameStr = userText.ToLower().Replace("actually it's", "").Replace("actually it is", "").Replace(" ", "");
                    String oldName = userData.GetProperty<String>("Name");
                    if (!String.IsNullOrEmpty(oldName))
                    {
                        // the string exists for updating name
                        if (oldName.Equals(nameStr))
                        {
                            replyStr += $"Silly {nameStr}, that's already your name!\n\n";
                        }
                        else if (String.IsNullOrEmpty(nameStr))
                        {
                            replyStr += $"{oldName}, you can't have an empty name. Try think of a better one. \n\n";
                        }
                        else
                        {
                            userData.SetProperty<String>("Name", nameStr);
                            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                            replyStr += $"Success! Your name has been changed from {oldName} to {nameStr}.\n\n";
                        }
                    }
                    else //the string doesn't exist for updating name
                    {
                        replyStr += $"You haven't set a name yet silly, anyhow, now it's {nameStr}.\n\n";
                        userData.SetProperty<String>("Name", nameStr);
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                    }
                }

                else if (userText.ToLower().Replace(" ", "").Contains("whoami"))
                {
                    if (!String.IsNullOrEmpty(userData.GetProperty<String>("Name")))
                    {
                        replyStr += $"You are {userData.GetProperty<String>("Name")}!";
                    }
                    else
                    {
                        replyStr += "I don't know you yet, try typing \"my name is ___\".";
                    }
                }

                else if (userText.ToLower().Contains("list account"))
                {
                    replyStr += "The accounts we offer are:\n\n";
                    int i = 1;
                    foreach (Account a in accounts)
                    {
                        replyStr += $"{i}. {a}\n\n";
                        i++;
                    }
                }

                else if (userText.ToLower().Contains("set currency"))
                {
                    int index = userText.IndexOf("set currency");
                    String prefCurrency = userText.Substring(index).Replace("set currency", "").Replace(" ", "").ToUpper(); //eg NZD
                    if (ExchangeObject.CURRENCIES.Contains(prefCurrency))
                    {
                        userData.SetProperty<String>("Currency", prefCurrency);
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                        if (!String.IsNullOrEmpty(userData.GetProperty<String>("Name")))
                        {
                            replyStr += $"{userData.GetProperty<String>("Name")}, y";
                        }
                        else
                        {
                            replyStr += "Y";
                        }
                        replyStr += $"our preffered currency has been set to {prefCurrency}.\n\n";
                    }
                    else
                    {
                        replyStr += $"Sorry, the currency code {prefCurrency} wasn't recognised. Try again with a different currency?\n\n";
                    }
                }
                else if (userText.ToLower().Contains("exchange rate"))
                {
                    String prefCurrency = userData.GetProperty<String>("Currency");
                    if (String.IsNullOrEmpty(prefCurrency))
                    {
                        //ask user to set their currency
                        replyStr += $"To use exchange rates you must first set your exchange rate with \"set currency ###\", once you've done that, try again :)\n\n";
                    }
                    else
                    {
                        //make the api call
                        HttpClient client = new HttpClient();
                        String apiResult = await client.GetStringAsync(new Uri("http://api.fixer.io/latest?base=" + prefCurrency));

                        ExchangeObject.RootObject rootObject;
                        rootObject = JsonConvert.DeserializeObject<ExchangeObject.RootObject>(apiResult);
                        bool full = false;
                        if (userText.ToLower().Contains("full"))
                        {
                            full = true;
                        }
                        replyStr += $"Your current currency is {rootObject.@base}, to update it ask to \"set currency ###\".\n\n";
                        if (full) { replyStr += $"The full "; }
                        else { replyStr += $"The most common "; }
                        replyStr += $"exchange rates for 1 {rootObject.@base} are \n\n{rootObject.rates.getRates(full)}\n\n";

                        Timeline newTimeline = new Timeline()
                        {
                            Date = DateTime.Now,
                            BaseCurrency = rootObject.@base,
                            Full = full,
                        };
                        await AzureManager.AzureManagerInstance.AddTimeline(newTimeline);

                        replyStr += $"Also, the settings for your request were saved to the database: \n\n" +
                            $"[{newTimeline.Date}] Base Currency is {newTimeline.BaseCurrency}.  Want all currencies? {newTimeline.Full}.\n\n" +
                            $"The unique id for this request is {newTimeline.ID}";
                    }
                }

                else if (activity.Text.ToLower().Contains("get timeline"))
                {
                    replyStr += "";
                    List<Timeline> timelines = await AzureManager.AzureManagerInstance.GetTimelines();
                    foreach (Timeline t in timelines)
                    {
                        replyStr += "[" + t.Date + "] Base Currency was " + t.BaseCurrency + ".  Wanted all currencies? " + t.Full + "\n\n";
                    }
                }

                else if (activity.Text.ToLower().Contains("del timeline"))
                {
                    int index = userText.IndexOf("del timeline");
                    String uid = userText.Substring(index).Replace("del timelines", "").Replace("del timeline", "").Replace(" ", "");

                    Timeline deleted = await AzureManager.AzureManagerInstance.DelTimeline(uid);
                    if (deleted != null)
                    {
                        replyStr += $"The following timeline was succesfully deleted:\n\n" +
                        $"[{deleted.Date}] Base Currency was {deleted.BaseCurrency}.  Wanted all currencies? {deleted.Full}.";
                    }
                    else
                    {
                        replyStr += $"The unique id {uid} could not be matched, sorry.";
                    }
                    
                }

                else if (activity.Text.ToLower().Contains("clear all"))
                {
                    replyStr += $"User data for {userData.GetProperty<String>("Name")} has been cleared\n";
                    await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                }

                else
                {
                    replyStr += "Sorry I'm not sure what you meant, try asking for \"help\" to see what I understand.";
                }
                /*if (!userData.GetProperty<bool>("HasSentMessage"))
                {
                    userData.SetProperty<bool>("HasSentMessage", true);
                    userData.SetProperty<String>("Name", activity.Text);
                    await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                    replyStr += $"your name is now \"{activity.Text}\"\n";
                } else
                {
                    replyStr += $"Hi there {userData.GetProperty<String>("Name")}\n";
                }*/

                /*
                        if (activity.Text.ToLower().Contains("new timeline"))
                        {
                            Timeline newTimeline = new Timeline()
                            {
                                Anger = 0.1,
                                Contempt = 0.2,
                                Disgust = 0.3,
                                Fear = 0.3,
                                Happiness = 0.3,
                                Neutral = 0.2,
                                Sadness = 0.4,
                                Surprise = 0.4,
                                Date = DateTime.Now
                            };

                            await AzureManager.AzureManagerInstance.AddTimeline(newTimeline);

                            replyStr += "New timeline added [" + newTimeline.Date + "]\n";
                        }
                */
                /* if (activity.Text.ToLower().Contains("get timeline"))
                   {
                       List<Timeline> timelines = await AzureManager.AzureManagerInstance.GetTimelines();
                       foreach (Timeline t in timelines)
                       {
                           replyStr += "[" + t.Date + "] Happiness " + t.Happiness + ", Sadness " + t.Sadness + "\n\n";
                       }
                   }
                */
                /*
                   if (activity.Attachments.Count != 0) // doesn't work in webclient or skype
                   {
                       VisionServiceClient VSC = new VisionServiceClient("ec825dd397f74a49abda168908724415");
                       AnalysisResult res = await VSC.DescribeAsync(activity.Attachments[0].ContentUrl, 3);
                       replyStr += $"{res.Description.Captions[0].Text}\n";
                   }
                   else
                   {
                       replyStr += $"Try sendinging an image!\n";
                   }
                   */

                //return our reply to the user
                Activity reply = activity.CreateReply(replyStr);

                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}