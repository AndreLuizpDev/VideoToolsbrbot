# VideoToolsbrbot

C# project inspired by https://github.com/GabrielRF/VideoCaptionsBot, the project aims solely for the development of programming skills and knowledge.

<p align="center">
   <img src="http://img.shields.io/static/v1?label=STATUS&message=EM%20DESENVOLVIMENTO&color=RED&style=for-the-badge" #vitrinedev/>
</p>

### Tópicos 

- [Descrição do projeto](#descrição-do-projeto)

- [Funcionalidades](#funcionalidades)

- [Ferramentas utilizadas](#ferramentas-utilizadas)

- [Configuração Inicial](#configuração-inicial)

- [Contribuições](#contribuições)

- [Meus contatos](#meus-contatos)

- [Desenvolvedores](#desenvolvedores)

### Descrição do projeto 

<p align="justify">
  Bot do Telegram em desenvolvimento para legendar e/ou traduzir vídeos utilizando Api do Openai-whisper (https://github.com/RageAgainstThePixel/OpenAI-DotNet) e Api do Telegram (https://github.com/TelegramBots/Telegram.Bot).

### Funcionalidades

:heavy_check_mark: `Legendar Vídeo:` Ao receber um vídeo o bot irá gerar uma legenda (arquivo .srt) para legendar o vídeo, esse aquivo por padrão utiliza o mesmo idioma do vídeo.

:heavy_check_mark: `Traduzir legenda:` Atualmente essa função está gerando a tradução utilizando o mesmo idioma da conta que enviou o vídeo.

:hammer: _`Selecionar idioma:` Funcionalidade em desenvolvimento._

###

### Ferramentas utilizadas

- Api Openai-whisper: https://github.com/RageAgainstThePixel/OpenAI-DotNet

- Api do Telegram (Client .Net): https://github.com/RageAgainstThePixel/OpenAI-DotNet

- FFmpeg: https://github.com/BtbN/FFmpeg-Builds/releases

###

### Configuração Inicial

Após baixar o projeto, você precisa:

#### Token das Api's:

**Api Bot Telegram:**

- Contatar o https://t.me/botfather no telegram.
- Clicar em "Criar novo bot" (e seguir todos os passos de criação)
- Por fim guardar a api fornecida (caso precise consultar depois basta acessar "Meus bots", selecionar seu bot e clicar em "Api Token".

**Api OpenAI (paga)**

- Basta acessar o https://platform.openai.com/account/api-keys e gerar o token.

#### Bibliotecas Utilizadas

- **Telegram.Bot:** Uma biblioteca para interagir com a API do Telegram.
- **OpenAI:** Uma biblioteca para interagir com a API OpenAI.
- **OpenAI.Audio:** Biblioteca para lidar com áudio na API OpenAI (Geração das legendas / Tradução).
- **Xabe.FFmpeg:** Utilizado para edição de vídeo (Incluir as legendas no vídeo).
- **System.Diagnostics:** Utilizado para diagnóstico e depuração.
- **System.Configuration:** Biblioteca para configurar valores a partir de um arquivo de configuração.
- **System.Text e System.IO:** Bibliotecas padrão para manipulação de texto e operações de arquivo.
- **Telegram.Bot.Types.ReplyMarkups:** Biblioteca para criar respostas de bot com marcações de resposta (não utilizada atualmente).

Ao rodar o bot a primeira vez será necessário instalar essas biblioteas e assim você poderá executar o bot. 🏆 

### Contribuições

Toda contribuição é bem vinda!

* Verifique as issues abertas e, se possível, sugira soluções.
* Abra novas issues.
* Envie um PR (preferencialmente vinculado a uma issue).

### Meus Contatos

<p>  
<a href="https://t.me/ZeroGC" target="blank"><img align="center" src="https://upload.wikimedia.org/wikipedia/commons/8/82/Telegram_logo.svg" alt="andreluiz26dev" height="30" width="40" /></a>
<a href="https://t.me/ZeroGC" target="blank">@ZeroGC</a>
</p>
<p>
<a href="https://linkedin.com/in/andreluiz26dev" target="blank"><img align="center" src="https://raw.githubusercontent.com/rahuldkjain/github-profile-readme-generator/master/src/images/icons/Social/linked-in-alt.svg" alt="andreluiz26dev" height="30" width="40" /></a>
<a href="https://linkedin.com/in/andreluiz26dev" target="blank">andreluiz26dev</a>
</p>

## Desenvolvedores

| [<img src="https://avatars.githubusercontent.com/u/86925300?s=96&v=4" width=115><br><sub>André Luiz</sub>](https://github.com/AndreLuizpDev) | |
| :---: | :---: 
