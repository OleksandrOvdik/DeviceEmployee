﻿namespace Services.Tokens;

public interface ITokenService
{
    string GenerateToken(string username, string role, int userId, int accountId);
}