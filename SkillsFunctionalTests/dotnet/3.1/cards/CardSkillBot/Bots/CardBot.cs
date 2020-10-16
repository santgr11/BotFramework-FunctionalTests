// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.EchoSkillBot.Bots
{
    
    public class CardBot : ActivityHandler
    {
        public string WelcomeMessage = "Send me one of these messages for a card: botActions, taskModule, submitAction, hero.";

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Text != null)
            {
                if (turnContext.Activity.Text.Contains("end") || turnContext.Activity.Text.Contains("stop"))
                {
                    // Send End of conversation at the end.
                    var messageText = $"ending conversation from the skill...";
                    await turnContext.SendActivityAsync(MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput), cancellationToken);
                    var endOfConversation = Activity.CreateEndOfConversationActivity();
                    endOfConversation.Code = EndOfConversationCodes.CompletedSuccessfully;
                    await turnContext.SendActivityAsync(endOfConversation, cancellationToken);
                }
                else
                {
                    switch (turnContext.Activity.Text.ToLowerInvariant())
                    {
                        case "botactions":
                            await SendAdaptiveCardAsync(turnContext, 1, cancellationToken).ConfigureAwait(false);
                            break;
                        case "taskmodule":
                            await SendAdaptiveCardAsync(turnContext, 2, cancellationToken).ConfigureAwait(false);
                            break;
                        case "submitaction":
                            await SendAdaptiveCardAsync(turnContext, 3, cancellationToken).ConfigureAwait(false);
                            break;
                        case "hero":
                            await turnContext.SendActivityAsync(MessageFactory.Attachment(CardSampleHelper.CreateHeroCard().ToAttachment()), cancellationToken).ConfigureAwait(false);
                            break;
                        default:
                            await turnContext.SendActivityAsync(MessageFactory.Text(WelcomeMessage), cancellationToken).ConfigureAwait(false);
                            break;
                    }
                }
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("App sent a message with empty text"), cancellationToken).ConfigureAwait(false);
                if (turnContext.Activity.Value != null)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"but with value {JsonConvert.SerializeObject(turnContext.Activity.Value)}"), cancellationToken).ConfigureAwait(false);
                }
            }
        }

        protected override Task OnEndOfConversationActivityAsync(ITurnContext<IEndOfConversationActivity> turnContext, CancellationToken cancellationToken)
        {
            // This will be called if the root bot is ending the conversation.  Sending additional messages should be
            // avoided as the conversation may have been deleted.
            // Perform cleanup of resources if needed.
            return Task.CompletedTask;
        }

        private static async Task SendAdaptiveCardAsync(ITurnContext<IMessageActivity> turnContext, int cardNumber, CancellationToken cancellationToken)
        {
            AdaptiveCard adaptiveCard = cardNumber switch
            {
                1 => CardSampleHelper.CreateAdaptiveCard1(),
                2 => CardSampleHelper.CreateAdaptiveCard2(),
                3 => CardSampleHelper.CreateAdaptiveCard3(),
                _ => throw new ArgumentOutOfRangeException(nameof(cardNumber)),
            };
            var replyActivity = MessageFactory.Attachment(adaptiveCard.ToAttachment());
            await turnContext.SendActivityAsync(replyActivity, cancellationToken).ConfigureAwait(false);
        }

       
    }
}
