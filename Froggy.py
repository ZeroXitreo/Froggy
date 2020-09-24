import asyncio

import discord
#from pytube import Playlist
#from pyyoutube import Api
import datetime
import os

client = discord.Client()


@client.event
async def on_ready():
    print('We have logged in as {0.user}'.format(client))
    client.loop.create_task(status_task())


@client.event
async def on_message(message):
    if message.author == client.user:
        return

    if message.content.lower().find('wednesday') != -1:
        if datetime.datetime.today().weekday() == 2:
            await message.channel.send('https://www.youtube.com/watch?v=OzQ-KvxLVT0')
        else:
            await message.channel.send('Not yet ):')

    if message.content.lower().find('ree') != -1:
        await message.channel.send('https://media1.tenor.com/images/329f4793998843d6b2eadafca89bf87c/tenor.gif')

    if message.content.lower().find('good bot') != -1:
        await message.channel.send('Thanks, you too, human')

    if message.content.lower().find('bad bot') != -1:
        await message.channel.send('Fuck off, you spooky midget')


async def status_task():
    is_wednesday = False
    while True:
        if datetime.datetime.today().weekday() == 2:
            if is_wednesday is False:
                is_wednesday = True
                await client.get_channel(758316238744453160).send("https://www.youtube.com/watch?v=OzQ-KvxLVT0")
        else:
            is_wednesday = False
        await asyncio.sleep(10)
        

TOKEN = input("FROGGY_DISCORD_BOT_TOKEN not set, please input discord token: ")
client.run(TOKEN)

# playlist = Playlist('https://www.youtube.com/playlist?list=PLphs3AfjTweSmfJwgg-hQfJgA3HHmrY2i')
# print('Number of videos in playlist: %s' % len(playlist.video_urls))as