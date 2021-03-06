﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetSharp;
using TwitterDealer.Interfaces;
using TwitterDealer.Models.TwitterUserModels;

namespace TwitterDealer.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TwitterUserController : ControllerBase
	{
		private readonly IUserService _userService;
		public TwitterUserController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpGet]
		[Route("twitteruserprofile")]
		public Task<MainUserModel> GetUserInfoAsync(string screenName)
		{
			var infoResult =  _userService.GetUserInfoAsync(screenName);

			return infoResult;
		}

		[HttpGet]
		[Route("twitterusertweets")]
		public Task<IEnumerable<StatusTweet>> GetUserTweetsAsync(string screenName)
		{
			var infoResult = _userService.GetUserTweetsAsync(screenName);

			return infoResult;
		}

		[HttpGet]
		[Route("twitterusermedia")]
		public async Task<IEnumerable<UserMedia>> GetUserMediaAsync(string screenName, int mediaCount)
		{
			var media = await _userService.GetUserMediaAsync(screenName, mediaCount);

			return media;
		}
	}
}
