using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController(ILikesRepository LikesRepository) : BaseAPIController
{
    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        var sourceUserId = User.GetUserId();

        if (sourceUserId == targetUserId)
            return BadRequest("You cannot like yourself!");

        var existingLike = await LikesRepository.GetUserLike(sourceUserId, targetUserId);

        // If the like does not exist, we create a new one
        if (existingLike == null)
        {
            var newLike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId
            };

            LikesRepository.AddLike(newLike);
            return Ok();
        }

        // If the like already exists, we remove it
        else
        {
            LikesRepository.DeleteLike(existingLike);
            if (await LikesRepository.SaveChanges())
                return Ok();
        }

        //could not add or remove like
        return BadRequest("Failed to update like");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
    {
        return Ok(await LikesRepository.GetCurrentUserLikeIds(User.GetUserId()));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        likesParams.UserId = User.GetUserId();
        var users = await LikesRepository.GetUserLikes(likesParams);
        Response.AddPaginationHeader(users);
        return Ok(users);
        
    }
}
