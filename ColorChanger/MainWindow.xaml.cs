﻿using ColorChanger.JsonData;
using ColorChanger.Twitch;
using System.Windows;

namespace ColorChanger
{
    public partial class MainWindow : Window
    {
        private ChatClient _chatClient;

        public MainWindow()
        {
            JsonController.LoadSettings();
            InitializeComponent();
            SetUpChatClient();
        }

        private void SetUpChatClient()
        {
            if (string.IsNullOrEmpty(JsonController.AppSettings.AccountSettings.Username) || string.IsNullOrEmpty(JsonController.AppSettings.AccountSettings.OAuthToken))
            {
                //open log in window
            }
        }
    }
}
