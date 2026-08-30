using AuthenticationMicroservice.Application.DTOs.Responses;
using AuthenticationMicroservice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace AuthenticationMicroservice.Application.Mappings
{
    public static class UserMappingExtensions
    {
        public static UserRegisteredResponse ToRegisteredResponse(this User user)
        {
            return new UserRegisteredResponse(
                user.Id, 
                user.Email, 
                user.UserName, 
                user.CreatedAtUtc
                );
        }

        public static UserProfileResponse ToProfileResponse(this User user)
        {
            return new UserProfileResponse(
                user.Id,
                user.Email,
                user.UserName,
                user.Role,
                user.CreatedAtUtc
                );
        }
    }
}
