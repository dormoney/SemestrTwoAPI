﻿using SemestrTwoAPI.Model;

namespace SemestrTwoAPI.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}