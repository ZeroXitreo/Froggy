import asyncio

import discord
# from discord_slash import SlashCommand, SlashContext
#from pytube import Playlist
#from pyyoutube import Api
import datetime
import os
import random
import googleapiclient.discovery
import json
import os.path
from os import path
from urllib.parse import parse_qs, urlparse


client = discord.Client()
is_wednesday = False

# slash = SlashCommand(client)

# @slash.slash(name="test")
# async def test(ctx: SlashContext):
#     embed = Embed(title="Embed Test")
#     await ctx.send(embed=embed)

@client.event
async def on_ready():
    print('We have logged in as {0.user} my dudes'.format(client))
    client.loop.create_task(status_task())


@client.event
async def on_message(message):
    if message.author == client.user:
        return

    if message.content.lower().startswith(".dizone"):
        if message.channel.id not in serverchannels:
            serverchannels.append(message.channel.id)
            SaveServerChannels()
        await message.channel.send('Gotcha.')

    if message.content.lower().startswith(".notdiz"):
        if message.channel.id in serverchannels:
            serverchannels.remove(message.channel.id)
            SaveServerChannels()
        await message.channel.send('):')


async def status_task():
    is_wednesday = False
    while True:
        if datetime.datetime.today().weekday() == 2:
            if is_wednesday is False:
                is_wednesday = True
                for channelId in serverchannels:
                    channel = client.get_channel(channelId)
                    await channel.send(f'https://www.youtube.com/watch?v={random.choice(PLAYLISTITEMS)["snippet"]["resourceId"]["videoId"]}')
        else:
            is_wednesday = False
        await asyncio.sleep(60*60)
        

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

def SaveServerChannels():
    print(serverchannels)
    with open("serverchannels.json", "w") as output:
        json.dump(serverchannels, output)
    pass

config = json.load(open("config.json"))

TOKEN = config["discord_token"]
PLAYLIST = config["yt_playlist"]
APIKEY = config["yt_api_key"]
PLAYLISTITEMS = populate_playlist_items()

serverchannels = []

if path.exists("serverchannels.json"):
    serverchannels = json.load(open("serverchannels.json"))

client.run(TOKEN)
