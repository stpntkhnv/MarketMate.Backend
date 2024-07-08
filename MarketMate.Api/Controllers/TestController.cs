using MarketMate.Application.Abstractions.HttpClients;
using MarketMate.Application.Models;
using MarketMate.Domain.Settings;
using MarketMate.Utilities.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MarketMate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IVkMethodsApiClient _vkMethodsApiClient;
        private readonly IUserContextAccessor _userContextAccessor;

        private readonly Regex _linkRegex = new(@"https:\/\/vk\.com\/wall(?<ownerId>-?\d+)_(?<postId>\d+)", RegexOptions.IgnoreCase);

        public TestController(IVkMethodsApiClient vkMethodsApiClient, IUserContextAccessor userContextAccessor, VkApiSettings vkApiSettings)
        {
            _vkMethodsApiClient = vkMethodsApiClient;
            _userContextAccessor = userContextAccessor;
            _vkMethodsApiClient.ConfigureGeneralParams(userContextAccessor.GetVkAccessToken(), vkApiSettings.ApiVersion);
        }

        [HttpPost("copy-photos")]
        public async Task<IActionResult> CopyPhotos([FromBody] CopyPhotosRequest request)
        {
            var donorPhotos = await _vkMethodsApiClient.GetPhotosAsync(request.DonorGroupId, request.DonorAlbumId);

            var testPhotos = await _vkMethodsApiClient.GetPhotosAsync(request.TestGroupId, request.TestAlbumId);

            for (var i = 0; i < testPhotos.Count; i++)
            {
                await _vkMethodsApiClient.EditPhotoAsync(request.TestGroupId, testPhotos[i].Id,
                    donorPhotos[i].Text
                    );

                await Task.Delay(500);
            }

            return Ok();
        }

        [HttpPost("copy-descriptions")]
        public async Task<IActionResult> CopyDescriptions([FromBody] CopyDescriptionsRequest request)
        {
            var photos = await _vkMethodsApiClient.GetPhotosAsync(-request.GroupId, request.AlbumId);
            var sellerLinks = photos
                .Select(x => GetSellerIdFromText(x.Text))
                .Distinct()
                .Select(GetSellerInfoAsync)
                .ToList();

            var infos = await Task.WhenAll(sellerLinks);

            return Ok(infos);
        }

        private async Task<SellerInfo> GetSellerInfoAsync(long id)
        {
            if (id < 0)
            {
                var groupInfo = await _vkMethodsApiClient.GetGroupInfoAsync(id);
                return new SellerInfo
                {
                    Description = groupInfo.Description,
                    Name = groupInfo.Name
                };
            }
            else
            {
               var userInfo = await _vkMethodsApiClient.GetUserInfoAsync(id);
                return new SellerInfo
                {
                    Name = $"{userInfo.FirstName} {userInfo.LastName}",
                    Description = userInfo.About
                };
            }
        }

        private long GetSellerIdFromText(string text)
        {
            var match = _linkRegex.Match(text);
            if (match.Success)
            {
                var ownerId = long.Parse(match.Groups["ownerId"].Value);
                var postId = long.Parse(match.Groups["postId"].Value);

                return ownerId;
            }

            return 0;
        }
    }

    public class CopyPhotosRequest
    {
        public int DonorGroupId { get; set; }
        public int DonorAlbumId { get; set; }
        public int TestGroupId { get; set; }
        public int TestAlbumId { get; set; }
        public int NumberOfPhotos { get; set; }
    }

    public class CopyDescriptionsRequest
    {
        public long GroupId { get; set; }
        public long AlbumId { get; set; }
        public int Count { get; set; }
    }
}
