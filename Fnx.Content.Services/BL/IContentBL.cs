using Fnx.Content.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fnx.Content.Services.BL
{
    public interface IContentBL
    {
        public Task<ResultData<GitRepoItem>> GetGitItems(string? searchKey, int? take = 10, int? skip = 10);

    }
}
