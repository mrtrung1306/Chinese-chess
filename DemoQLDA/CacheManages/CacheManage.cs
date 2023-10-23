using Libs.Entity;
using Libs.Services;
using Microsoft.Extensions.Caching.Memory;

namespace DemoQLDA.CacheManages
{
    public class CacheManage
    {
        private ChessService chessService;
        private IMemoryCache memoryCache;

        public CacheManage(ChessService chessService, IMemoryCache memoryCache)
        {
            this.chessService = chessService;
            this.memoryCache = memoryCache;
        }
        public Dictionary<string, List<UserInRoom>> UserInRoom
        {
            get
            {
                Dictionary<string, List<UserInRoom>> userInRoomDic = (Dictionary<string, List<UserInRoom>>)memoryCache.Get("userInRoomCache");
                if (userInRoomDic == null)
                {
                    userInRoomDic = new Dictionary<string, List<UserInRoom>>();
                    List<UserInRoom> userInRoomList = chessService.getUserInRoomList();
                    for (int i = 0; i < userInRoomList.Count; i++)
                    {
                        if (!userInRoomDic.ContainsKey(userInRoomList[i].RoomId.ToString().ToLower()))
                        {
                            List<UserInRoom> userInRoomTemp = new List<UserInRoom>();
                            userInRoomTemp.Add(userInRoomList[i]);
                            userInRoomDic.Add(userInRoomList[i].RoomId.ToString().ToLower(), userInRoomTemp);
                        }
                        else
                        {
                            List<UserInRoom> userInRoomTemp = userInRoomDic[userInRoomList[i].RoomId.ToString().ToLower()];
                            userInRoomTemp.Add(userInRoomList[i]);
                        }
                    }
                    memoryCache.Set("userInRoomCache", userInRoomDic);
                }
                return userInRoomDic;
            }
        }
        public Dictionary<string, List<Room>> Room
        {
            get
            {
                Dictionary<string, List<Room>> userRoomDic = (Dictionary<string, List<Room>>)memoryCache.Get("roomCache");
                if (userRoomDic == null)
                {
                    userRoomDic = new Dictionary<string, List<Room>>();
                    List<Room> userInRoomList = chessService.getRoomList();
                    for (int i = 0; i < userInRoomList.Count; i++)
                    {
                        if (!userRoomDic.ContainsKey(userInRoomList[i].Id.ToString().ToLower()))
                        {
                            List<Room> userRoomTemp = new List<Room>();
                            userRoomTemp.Add(userInRoomList[i]);
                            userRoomDic.Add(userInRoomList[i].Id.ToString().ToLower(), userRoomTemp);
                        }
                        else
                        {
                            List<Room> userInRoomTemp = userRoomDic[userInRoomList[i].Id.ToString().ToLower()];
                            userInRoomTemp.Add(userInRoomList[i]);
                        }
                    }
                    memoryCache.Set("roomCache", userRoomDic);
                }
                return userRoomDic;
            }
        }
    }
}
