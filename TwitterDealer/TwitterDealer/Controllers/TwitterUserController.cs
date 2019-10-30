﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetSharp;
using TwitterDealer.Interfaces;

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
		[Route("TwitterUserProfile")]
		public TwitterUser GetUserInfoAsync(string screenName)
		{
			var infoResult =  _userService.GetUserInfo(screenName);

			return infoResult;
		}

		[HttpGet]
		[Route("TwitterUserTweets")]
		public IEnumerable<TwitterStatus> GetUserTweetsAsync(string screenName)
		{
			var infoResult = _userService.GetUserTweets(screenName);

			return infoResult;
		}

		[HttpGet]
		[Route("TwitterUserMedia")]
		public IEnumerable<TwitterMedia> GetUserMedia(string screenName, int mediaCount)
		{
			var media = _userService.GetUserMedia(screenName, mediaCount);

			return media;
		}
	}
}
