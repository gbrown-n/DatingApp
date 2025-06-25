using System;
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UserController(IUserRepository userRepository, IMapper mapper) : BaseAPIController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
    {
        var users = await userRepository.GetMembersAsync();

        //var usersToReturn = mapper.Map<IEnumerable<MemberDTO>>(users);  

        return Ok(users);
    }

    [HttpGet("{username}")] // /api/users/{id}
    public async Task<ActionResult<MemberDTO>> GetUser(string username)
    {
        var user = await userRepository.GetMemberAsync(username); //could return null

        if (user == null) return NotFound();

        return user; //definitely not null
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (username == null) return BadRequest("No username found in token");

        var user = await userRepository.GetUserByUsernameAsync(username);

        if (user == null) return BadRequest("Could not find user");

        mapper.Map(memberUpdateDTO, user);

        if (await userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update the user");

    }
}
