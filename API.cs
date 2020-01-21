﻿using System;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CrappyPrizm
{
    public static class API
    {
        #region Var
        private const string Url = "https://wallet.prizm.space/";
        private static readonly HttpClient Client = new HttpClient { BaseAddress = new Uri(Url) };
        #endregion

        #region Methods
        public static Task<Account?> GetAccountAsync(string address) => MakeRequestAsync<Account?>("getAccount", ("account", address));

        private static Task<BroadcastedTransaction?> BroadcastTransactionAsync(string attachment, string bytes) => MakeRequestAsync<BroadcastedTransaction?>("broadcastTransaction", ("prunableAttachmentJSON", attachment), ("transactionBytes", bytes));
        #endregion

        #region Helpers
        private static async Task<T> MakeRequestAsync<T>(string type, params (string key, string value)[] parameters)
        {
            HttpResponseMessage response = await MakeRequestAsync(type, parameters);
            string json = await response.Content.ReadAsStringAsync();
            return json.Contains("error") ? default : JsonConvert.DeserializeObject<T>(json);
        }

        private static Task<HttpResponseMessage> MakeRequestAsync(string type, params (string key, string value)[] parameters)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(
                parameters.Select(param => new KeyValuePair<string, string>(param.key, param.value))
                .Append(new KeyValuePair<string, string>("rnd", new Random().Next(int.MinValue, 0).ToString()))
            );
            string requestUri = $"/prizm?requestType={type}";
            return Client.PostAsync(requestUri, content);
        }
        #endregion
    }
}
