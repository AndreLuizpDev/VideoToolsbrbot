# VideoToolsbrbot

C# project inspired by https://github.com/GabrielRF/VideoCaptionsBot, the project aims solely for the development of programming skills and knowledge.

<p align="center">
   <img src="http://img.shields.io/static/v1?label=STATUS&message=EM%20DESENVOLVIMENTO&color=RED&style=for-the-badge" #vitrinedev/>
</p>

[![CodeFactor](https://www.codefactor.io/repository/github/andreluizpdev/videotoolsbrbot/badge)](https://www.codefactor.io/repository/github/andreluizpdev/videotoolsbrbot)

### Topics

- [Project Description](#project-description)

- [Features](#features)

- [Tools Used](#tools-used)

- [Initial Configuration](#initial-configuration)

- [Contributions](#contributions)

- [My Contacts](#my-contacts)

- [Developers](#developers)

### Project Description

<p align="justify">
  Telegram bot under development for captioning and/or translating videos using the Openai-whisper API (https://github.com/RageAgainstThePixel/OpenAI-DotNet) and the Telegram API (https://github.com/TelegramBots/Telegram.Bot).

### Features

:heavy_check_mark: `Caption Video:` When receiving a video, the bot will generate a caption (`.srt` file) for the video. By default, this file uses the same language as the video.

:heavy_check_mark: `Translate Caption:` Currently, this function is generating the translation using the same language as the account that sent the video.

:hammer: _`Select Language:` Functionality under development._

###

### Tools Used

- Openai-whisper API: https://github.com/RageAgainstThePixel/OpenAI-DotNet

- Telegram API (Client .Net): https://github.com/RageAgainstThePixel/OpenAI-DotNet

- FFmpeg: https://github.com/BtbN/FFmpeg-Builds/releases

###

### Initial Configuration

After downloading the project, you need to:

#### API Tokens:

**Telegram Bot API:**

- Contact https://t.me/botfather on Telegram.
- Click on "Create a new bot" (and follow all the creation steps).
- Finally, save the provided API token (if you need to consult it later, simply go to "My bots," select your bot, and click on "API Token").

**OpenAI API (paid)**

- Simply access https://platform.openai.com/account/api-keys and generate the token.

#### Used Libraries

- **Telegram.Bot:** A library for interacting with the Telegram API.
- **OpenAI:** A library for interacting with the OpenAI API.
- **OpenAI.Audio:** A library for handling audio in the OpenAI API (Caption generation/Translation).
- **Xabe.FFmpeg:** Used for video editing (Including subtitles in the video).
- **System.Diagnostics:** Used for diagnostics and debugging.
- **System.Configuration:** A library for configuring values from a configuration file.
- **System.Text and System.IO:** Standard libraries for text manipulation and file operations.
- **Telegram.Bot.Types.ReplyMarkups:** A library for creating bot responses with reply markups (currently not used).

When running the bot for the first time, you will need to install these libraries, and then you can run the bot. 🏆 

### Contributions

All contributions are welcome!

* Check the open issues and, if possible, suggest solutions.
* Open new issues.
* Submit a PR (preferably linked to an issue).

### My Contacts

<p>  
<a href="https://t.me/ZeroGC" target="blank"><img align="center" src="https://upload.wikimedia.org/wikipedia/commons/8/82/Telegram_logo.svg" alt="andreluiz26dev" height="30" width="40" /></a>
<a href="https://t.me/ZeroGC" target="blank">@ZeroGC</a>
</p>
<p>
<a href="https://linkedin.com/in/andreluiz26dev" target="blank"><img align="center" src="https://raw.githubusercontent.com/rahuldkjain/github-profile-readme-generator/master/src/images/icons/Social/linked-in-alt.svg" alt="andreluiz26dev" height="30" width="40" /></a>
<a href="https://linkedin.com/in/andreluiz26dev" target="blank">andreluiz26dev</a>
</p>

## Developers

| [<img src="https://avatars.githubusercontent.com/u/86925300?s=96&v=4" width=115><br><sub>André Luiz</sub>](https://github.com/AndreLuizpDev) | |
| :---: | :---:
