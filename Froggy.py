import asyncio

import discord
#from pytube import Playlist
#from pyyoutube import Api
import datetime
import os
import random
import googleapiclient.discovery
from urllib.parse import parse_qs, urlparse

client = discord.Client()


@client.event
async def on_ready():
    print('We have logged in as {0.user} my dudes'.format(client))
    client.loop.create_task(status_task())


@client.event
async def on_message(message):
    if message.author == client.user:
        return

    if message.content.lower().find('wednesday') != -1:
        if datetime.datetime.today().weekday() == 2:
            await message.channel.send(f'https://www.youtube.com/watch?v={random.choice(PLAYLISTITEMS)["snippet"]["resourceId"]["videoId"]}')
        else:
            await message.channel.send('Not yet my dude')

    if message.content.lower().find('ree') != -1:
        await message.channel.send('https://media1.tenor.com/images/329f4793998843d6b2eadafca89bf87c/tenor.gif')

    if message.content.lower().find('good bot') != -1:
        await message.channel.send('Thanks, you too, human')

    if message.content.lower().find('bad bot') != -1:
        await message.channel.send('You\'ve been added to the list.')


async def status_task():
    is_wednesday = False
    while True:
        if datetime.datetime.today().weekday() == 2:
            if is_wednesday is False:
                is_wednesday = True
                channel = client.get_channel(758316238744453160)
                await channel.send(f'https://www.youtube.com/watch?v={random.choice(PLAYLISTITEMS)["snippet"]["resourceId"]["videoId"]}')
        else:
            is_wednesday = False
        await asyncio.sleep(10)
        

async def populate_playlist_items():
    youtube = googleapiclient.discovery.build("youtube", "v3", developerKey = APIKEY)

    request = youtube.playlistItems().list(
        part = "snippet",
        playlistId = PLAYLIST,
        maxResults = 50
    )
    response = request.execute()

    PLAYLISTITEMS = []
    while request is not None:
        response = request.execute()
        PLAYLISTITEMS += response["items"]
        request = youtube.playlistItems().list_next(request, response)

    print(f"total: {len(PLAYLISTITEMS)}")


TOKEN = input("DC_BOT_TOKEN not set, please provide: ")
PLAYLIST = input("PLAYLIST_ID not set, please provide: ")
APIKEY = input("YOUTUBE_v3_API_KEY not set, please provide: ")
PLAYLISTITEMS = []
populate_playlist_items()


client.run(TOKEN)
