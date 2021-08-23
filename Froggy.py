import asyncio

import discord
#from pytube import Playlist
#from pyyoutube import Api
import datetime
import os
import random
import googleapiclient.discovery
import json
from urllib.parse import parse_qs, urlparse


client = discord.Client()
is_wednesday = False


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


async def status_task():
    while True:
        if datetime.datetime.today().weekday() == 2:
            if is_wednesday is False:
                is_wednesday = True
                channel = client.get_channel(758316238744453160)
                await channel.send(f'https://www.youtube.com/watch?v={random.choice(PLAYLISTITEMS)["snippet"]["resourceId"]["videoId"]}')
        else:
            is_wednesday = False
        await asyncio.sleep(10)
        

def populate_playlist_items():
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
    return PLAYLISTITEMS


config = json.load(open("config.json"))

TOKEN = config["discord_token"]
PLAYLIST = config["yt_playlist"]
APIKEY = config["yt_api_key"]
PLAYLISTITEMS = populate_playlist_items()

client.run(TOKEN)
