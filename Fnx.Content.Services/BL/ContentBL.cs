using Fnx.Content.Models;
using Fnx.Content.Services.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fnx.Content.Services.BL
{
    public class ContentBL : IContentBL
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public ContentBL(HttpClient client, IConfiguration configuration) 
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        }
        public async Task<ResultData<GitRepoItem>> GetGitItems(string? searchKey, int? take = 100, int? skip = 0)
        {
            var url = _configuration["CommonUrls:GitAPI"];
            _client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "http://developer.github.com/v3/#user-agent-required");
            var response = await _client.GetAsync($"{url}?q={searchKey ?? " "}");
            var x = await response.ReadContentAs<ResultData<GitRepoItem>>();
            x.Items = x.Items.Skip(skip ?? 0).Take(take ?? 100).ToList();
            return x;

        }
    }
}
