﻿using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using TenmoClient.Data;

namespace TenmoClient.APIClients
{
    public class UserRestClient
    {
        private readonly RestClient client;
        public UserRestClient()
        {
            this.client = new RestClient("https://localhost:44315/");
        }

        private string token;

        public void GetToken(string jwt)
        {
            token = jwt;

            // Any request with this client in the future will AUTOMATICALLY
            // contain the Authorization / Bearer token header
            if (jwt == null)
            {
                client.Authenticator = null;
            }
            else
            {
                client.Authenticator = new JwtAuthenticator(jwt);
            }
        }


        public List<UserModel> GetAllUsers()
        {
            RestRequest request = new RestRequest("api/users/all");
            request.AddHeader("Authorization", "Bearer " + token);
            IRestResponse<List<UserModel>> response = client.Get<List<UserModel>>(request);
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Could not connect to Tenmo Servers; Try again later!");

                return null;
            }

            if (!response.IsSuccessful)
            {
                Console.WriteLine("Problem getting Account: " + response.StatusDescription);
                Console.WriteLine(response.Content);

                return null;
            }

            return response.Data;
        }
    }
}
