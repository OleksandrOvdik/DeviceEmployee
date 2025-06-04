# DeviceEmployee
Time for update! Entity Framework

If you wanna launch this application successfully, you need to generate appsettings.json file and put your connectionString.

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "your db connection"
  },
  
  "Jwt" : {
    "Issuer" : "your url",
    "Audience" : "your url",
    "Key" : "your key",
    "ValidInMinutes" : 10
  }
  
}

```
JSON REQUEST BODY FOR CREATING AND UPDATING(POST and PUT endpoints)
```
{
  "deviceName": "TraidingBot",
  "deviceTypeName": "PC",
  "isEnabled": true,
  "additionalProperties": "{"advantage": "trade on futures, stocks, spot, minerals", "disadvantage" : "does not know that he has to earn money, not lose it :)" }"
}

```

One man came down from the heavenly mountains, whose name is spoken with a capital letter, a man who became a legend during his lifetime, a man who created an era and became an era himself, a man I honor and respect - he taught me. In fact, I divide all my code into several projects for the following reasons:

*The code is separated, not jumbled, which makes it easy to read and understand

*Code flexibility

*And it's just much more pleasant and easier for me to work with a project that is so well structured and divided


Kostya,thanks🤝
