﻿namespace Exercise8.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Exercise8.Model;
    using Exercise8.Services;
    using Microsoft.Bot.Builder.Dialogs;
    using Util;

    [Serializable]
    public class UserFeedbackRequestDialog : IDialog<object>
    {
        private readonly TextAnalyticsService searchService = new TextAnalyticsService();
        
        public Task StartAsync(IDialogContext context)
        {
            PromptDialog.Text(context, this.MessageReciveAsync, "How would you rate my help?");

            return Task.CompletedTask;
        }

        public async Task MessageReciveAsync(IDialogContext context, IAwaitable<string> result)
        {
            var response = await result;

            double score = await this.searchService.Sentiment(response);

            if (score == double.NaN)
            {
                await context.PostAsync("Ooops! Something went wrong while analying your answer. An IT representative agent will get in touch with you to follow up soon.");
            }
            else
            {
                if (score < 0.5)
                {
                    await context.PostAsync("I understand that you might be dissatisfied with my assistance. An IT representative agent will get in touch with you soon to help you.");
                }
                else
                {
                    await context.PostAsync("Thanks for sharing your experience.");
                }
            }
            
            context.Done<object>(null);
        }
    }
}