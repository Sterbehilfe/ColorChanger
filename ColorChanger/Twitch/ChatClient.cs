﻿using ColorChanger.JsonData;
using System;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Enums;
using TwitchLib.Communication.Models;

namespace ColorChanger.Twitch
{
    public class ChatClient
    {
        public TwitchClient TwitchClient { get; }

        public ConnectionCredentials ConnectionCredentials { get; }

        public ClientOptions ClientOptions { get; }

        public TcpClient TcpClient { get; }

        public JsonController JsonControler { get; } = new();

        public int ColorIndex
        {
            get => _index;
            private set
            {
                _index = value;
                if (_index == JsonControler.AppSettings.AccountSettings.Channels.Count)
                {
                    _index = 0;
                }
            }
        }

        private int _index = 0;

        public ChatClient()
        {
            Console.Title = $"Color Changer by Strbhlfe - Connected as {JsonControler.AppSettings.AccountSettings.Username}";
            ConnectionCredentials = new(JsonControler.AppSettings.AccountSettings.Username, JsonControler.AppSettings.AccountSettings.OAuthToken);
            ClientOptions = new()
            {
                ClientType = ClientType.Chat,
                ReconnectionPolicy = new(10000, 30000, 1000),
                UseSsl = true
            };
            TcpClient = new(ClientOptions);
            TwitchClient = new(TcpClient, ClientProtocol.TCP)
            {
                AutoReListenOnException = true
            };
            TwitchClient.Initialize(ConnectionCredentials, JsonControler.AppSettings.AccountSettings.Channels);
            TwitchClient.OnMessageReceived += OnMessageReceived;
            TwitchClient.OnConnected += (sender, e) => Console.WriteLine("Client connected!");
            TwitchClient.OnJoinedChannel += (sender, e) => Console.WriteLine($"Joined channel {e.Channel}");
            TwitchClient.Connect();
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Username == JsonControler.AppSettings.AccountSettings.Username.ToLower() && !Regex.IsMatch(e.ChatMessage.Message, @"color\s#[0-9A-Fa-f]{6}"))
            {
                try
                {
                    string color = JsonControler.AppSettings.Colors[ColorIndex];
                    TwitchClient.SendMessage(JsonControler.AppSettings.AccountSettings.Username, $".color {color}");
                    ColorIndex++;
                    Console.WriteLine($"Changed color to {color}");
                }
                catch (Exception)
                {
                    ColorIndex = 0;
                }
            }
        }
    }
}
