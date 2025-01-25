using System;

namespace Bluewater.Web.Users;

public class GetUserResponse(string? name)
{
    public string? Name { get; set; } = name;
}
