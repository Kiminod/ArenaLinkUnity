using System;
using System.Diagnostics.CodeAnalysis;
using Steamworks;
using UnityEngine;

namespace ArenaLink.Steam
{
    public class UserInfo
    {
        public UserInfo()
        {
            if (SteamManager.Initialized) return;
            
            throw new InvalidOperationException("Steam is not initialized!");
        }
         
        public CSteamID GetRawUserId()
        {
            return SteamUser.GetSteamID();
        }

        public string GetSteamId()
        {
            return GetRawUserId().ToString();
        }
        
        public string GetUsername()
        {
            return SteamFriends.GetPersonaName();
        }
        
        public bool GetProfilePicture([MaybeNullWhen(false)] out Texture2D avatarTexture)
        {
            var avatarInt = SteamFriends.GetLargeFriendAvatar(GetRawUserId());
            avatarTexture = null;
            
            if (avatarInt == -1) return false;

            if (!SteamUtils.GetImageSize(avatarInt, out var width, out var height)) return false;
                
            var image = new byte[width * height * 4];
            if (!SteamUtils.GetImageRGBA(avatarInt, image, (int)(4 * width * height))) return false;
                
            avatarTexture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
            avatarTexture.LoadRawTextureData(image);
            avatarTexture.Apply();
                        
            // Flip the texture vertically (Y-axis)
            for (var y = 0; y < avatarTexture.height / 2; y++)
            {
                for (var x = 0; x < avatarTexture.width; x++)
                {
                    var temp = avatarTexture.GetPixel(x, y);
                    avatarTexture.SetPixel(x, y, avatarTexture.GetPixel(x, avatarTexture.height - y - 1));
                    avatarTexture.SetPixel(x, avatarTexture.height - y - 1, temp);
                }
            }

            // Apply the changes after flipping
            avatarTexture.Apply();

            return true;
        }
    }
}