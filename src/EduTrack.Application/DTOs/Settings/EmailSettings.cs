namespace EduTrack.Application.DTOs.Settings;

//"EmailSettings": {
//  "Server": "smtp.gmail.com",
//  "Port": 587,
//  "Username": "be127.siliconmade@gmail.com",
//  "Password": "mowp zlqv ejjj pcqm",
//}

public class EmailSettings
{
    public string Server { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}